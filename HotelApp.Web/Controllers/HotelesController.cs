using HotelApp.BL.Dtos;
using HotelApp.BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web_.Controllers
{
    public class HotelesController : AppController
    {
        private readonly IHotelService _svc;
        public HotelesController(IHotelService svc) => _svc = svc;

        public async Task<IActionResult> Index()
        {
            var r = await _svc.GetAllAsync();
            if (!r.Success) TempData["Error"] = r.Message;
            return View(Lista<HotelDto>(r));
        }

        public IActionResult Create() => View("Form", new UpdateHotelDto());

        public async Task<IActionResult> Edit(int id)
        {
            var r = await _svc.GetByIdAsync(id);
            if (!r.Success || r.Data is not HotelDto h)
            {
                TempData["Error"] = r.Message ?? "El hotel no existe.";
                return RedirectToAction(nameof(Index));
            }
            return View("Form", new UpdateHotelDto
            {
                Id = h.Id, Nombre = h.Nombre, Direccion = h.Direccion, Telefono = h.Telefono
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(UpdateHotelDto dto)
        {
            if (!ModelState.IsValid) return View("Form", dto);

            var r = dto.Id == 0 ? await _svc.SaveAsync(dto) : await _svc.UpdateAsync(dto);
            if (!r.Success)
            {
                ModelState.AddModelError(string.Empty, r.Message ?? "No se pudo guardar.");
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
