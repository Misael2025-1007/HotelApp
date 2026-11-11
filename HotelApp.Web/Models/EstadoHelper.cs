using HotelApp.DAL.Models.Enums;

namespace HotelApp.Web_.Models
{
    public static class EstadoHelper
    {
        public static string Texto(EstadoHabitacion e) => e switch
        {
            EstadoHabitacion.Available => "Disponible",
            EstadoHabitacion.Reserved => "Reservada",
            EstadoHabitacion.Occupied => "Ocupada",
            EstadoHabitacion.Cleaning => "En limpieza",
            EstadoHabitacion.Maintenance => "Mantenimiento",
            EstadoHabitacion.OutOfService => "Fuera de servicio",
            _ => e.ToString()
        };

        public static string Clase(EstadoHabitacion e) => e switch
        {
            EstadoHabitacion.Available => "bg-success",
            EstadoHabitacion.Reserved => "bg-info text-dark",
            EstadoHabitacion.Occupied => "bg-primary",
            EstadoHabitacion.Cleaning => "bg-warning text-dark",
            EstadoHabitacion.Maintenance => "bg-warning text-dark",
            EstadoHabitacion.OutOfService => "bg-danger",
            _ => "bg-secondary"
        };
    }
}
