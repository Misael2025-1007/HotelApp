using HotelApp.BL.Base;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web_.Controllers
{
    public abstract class AppController : Controller
    {
        protected static List<T> Lista<T>(OperationResult r)
            => r.Success && r.Data is List<T> lista ? lista : new List<T>();

        protected void Notificar(OperationResult r)
        {
            if (r.Success) TempData["Ok"] = r.Message;
            else TempData["Error"] = r.Message;
        }
    }
}
