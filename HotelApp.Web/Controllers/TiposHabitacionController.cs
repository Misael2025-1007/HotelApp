using HotelApp.BL.Dtos;
using HotelApp.BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web_.Controllers
{
    public class TiposHabitacionController : AppController
    {
        private readonly ITipoHabitacionService _svc;
        public TiposHabitacionController(ITipoHabitacionService svc) => _svc = svc;

        public async Task<IActionResult> Index()
        {
            var r = await _svc.GetAllAsync();
            if (!r.Success) TempData["Error"] = r.Message;
            return View(Lista<TipoHabitacionDto>(r));
        }

        public IActionResult Create() => View("Form", new UpdateTipoHabitacionDto());

        public async Task<IActionResult> Edit(int id)
        {
            var r = await _svc.GetByIdAsync(id);
            if (!r.Success || r.Data is not TipoHabitacionDto t)
            {
                TempData["Error"] = r.Message ?? "El tipo de habitación no existe.";
                return RedirectToAction(nameof(Index));
            }
            return View("Form", new UpdateTipoHabitacionDto
            {
                Id = t.Id, Nombre = t.Nombre, Descripcion = t.Descripcion
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(UpdateTipoHabitacionDto dto)
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
