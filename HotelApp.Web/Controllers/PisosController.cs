using HotelApp.BL.Dtos;
using HotelApp.BL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelApp.Web_.Controllers
{
    public class PisosController : AppController
    {
        private readonly IPisoService _svc;
        private readonly IHotelService _hoteles;
        private readonly IBloqueService _bloques;

        public PisosController(IPisoService svc, IHotelService hoteles, IBloqueService bloques)
        {
            _svc = svc;
            _hoteles = hoteles;
            _bloques = bloques;
        }

        private async Task CargarCombosAsync()
        {
            ViewBag.Hoteles = Lista<HotelDto>(await _hoteles.GetAllAsync())
                .Where(h => h.Status)
                .Select(h => new SelectListItem(h.Nombre, h.Id.ToString()))
                .ToList();

            ViewBag.Bloques = Lista<BloqueDto>(await _bloques.GetAllAsync())
                .Where(b => b.Status)
                .Select(b => new SelectListItem($"{b.HotelNombre} - {b.Nombre}", b.Id.ToString()))
                .ToList();
        }

        public async Task<IActionResult> Index()
        {
            var r = await _svc.GetAllAsync();
            if (!r.Success) TempData["Error"] = r.Message;
            return View(Lista<PisoDto>(r));
        }

        public async Task<IActionResult> Create()
        {
            await CargarCombosAsync();
            return View("Form", new UpdatePisoDto());
        }

        public async Task<IActionResult> Edit(int id)
        {
            var r = await _svc.GetByIdAsync(id);
            if (!r.Success || r.Data is not PisoDto p)
            {
                TempData["Error"] = r.Message ?? "El piso no existe.";
                return RedirectToAction(nameof(Index));
            }
            await CargarCombosAsync();
            return View("Form", new UpdatePisoDto
            {
                Id = p.Id, HotelId = p.HotelId, BloqueId = p.BloqueId, Numero = p.Numero
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(UpdatePisoDto dto)
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
