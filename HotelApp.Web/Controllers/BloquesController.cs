using HotelApp.BL.Dtos;
using HotelApp.BL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelApp.Web_.Controllers
{
    public class BloquesController : AppController
    {
        private readonly IBloqueService _svc;
        private readonly IHotelService _hoteles;

        public BloquesController(IBloqueService svc, IHotelService hoteles)
        {
            _svc = svc;
            _hoteles = hoteles;
        }

        private async Task CargarCombosAsync()
        {
            ViewBag.Hoteles = Lista<HotelDto>(await _hoteles.GetAllAsync())
                .Where(h => h.Status)
                .Select(h => new SelectListItem(h.Nombre, h.Id.ToString()))
                .ToList();
        }

        public async Task<IActionResult> Index()
        {
            var r = await _svc.GetAllAsync();
            if (!r.Success) TempData["Error"] = r.Message;
            return View(Lista<BloqueDto>(r));
        }

        public async Task<IActionResult> Create()
        {
            await CargarCombosAsync();
            return View("Form", new UpdateBloqueDto());
        }

        public async Task<IActionResult> Edit(int id)
        {
            var r = await _svc.GetByIdAsync(id);
            if (!r.Success || r.Data is not BloqueDto b)
            {
                TempData["Error"] = r.Message ?? "El bloque no existe.";
                return RedirectToAction(nameof(Index));
            }
            await CargarCombosAsync();
            return View("Form", new UpdateBloqueDto
            {
                Id = b.Id, HotelId = b.HotelId, Nombre = b.Nombre, Descripcion = b.Descripcion
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(UpdateBloqueDto dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombosAsync();
                return View("Form", dto);
            }

            var r = dto.Id == 0 ? await _svc.SaveAsync(dto) : await _svc.UpdateAsync(dto);
            if (!r.Success)
            {
                ModelState.AddModelError(string.Empty, r.Message ?? "No se pudo guardar.");
                await CargarCombosAsync();
                return View("Form", dto);
            }
            Notificar(r);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            Notificar(await _svc.RemoveAsync(new RemoveDto { Id = id }));
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            Notificar(await _svc.ActivateAsync(new RemoveDto { Id = id }));
            return RedirectToAction(nameof(Index));
        }
    }
}
