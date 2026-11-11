using HotelApp.BL.Dtos;
using HotelApp.BL.Services;
using HotelApp.DAL.Models.Enums;
using HotelApp.Web_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelApp.Web_.Controllers
{
    public class HabitacionesController : AppController
    {
        private readonly IHabitacionService _svc;
        private readonly IPisoService _pisos;
        private readonly ITipoHabitacionService _tipos;

        public HabitacionesController(IHabitacionService svc, IPisoService pisos, ITipoHabitacionService tipos)
        {
            _svc = svc;
            _pisos = pisos;
            _tipos = tipos;
        }

        private async Task CargarCombosAsync()
        {
            ViewBag.Pisos = Lista<PisoDto>(await _pisos.GetAllAsync())
                .Where(p => p.Status)
                .Select(p => new SelectListItem($"{p.HotelNombre} - Piso {p.Numero}", p.Id.ToString()))
                .ToList();

            ViewBag.Tipos = Lista<TipoHabitacionDto>(await _tipos.GetAllAsync())
                .Where(t => t.Status)
                .Select(t => new SelectListItem(t.Nombre, t.Id.ToString()))
                .ToList();

            ViewBag.Estados = Enum.GetValues<EstadoHabitacion>()
                .Select(e => new SelectListItem(EstadoHelper.Texto(e), ((int)e).ToString()))
                .ToList();
        }

        public async Task<IActionResult> Index()
        {
            var r = await _svc.GetAllAsync();
            if (!r.Success) TempData["Error"] = r.Message;
            return View(Lista<HabitacionDto>(r));
        }

        public async Task<IActionResult> Create()
        {
            await CargarCombosAsync();
            return View("Form", new UpdateHabitacionDto { Capacidad = 2 });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var r = await _svc.GetByIdAsync(id);
            if (!r.Success || r.Data is not HabitacionDto h)
            {
                TempData["Error"] = r.Message ?? "La habitación no existe.";
                return RedirectToAction(nameof(Index));
            }
            await CargarCombosAsync();
            return View("Form", new UpdateHabitacionDto
            {
                Id = h.Id, PisoId = h.PisoId, TipoHabitacionId = h.TipoHabitacionId,
                Numero = h.Numero, Capacidad = h.Capacidad, Tarifa = h.Tarifa, Estado = h.Estado
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(UpdateHabitacionDto dto)
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
