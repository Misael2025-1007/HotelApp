using HotelApp.BL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.BL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBlDependencies(this IServiceCollection services)
        {
            services.AddScoped<IHotelService, HotelService>();
            services.AddScoped<ITipoHabitacionService, TipoHabitacionService>();
            services.AddScoped<IBloqueService, BloqueService>();
            services.AddScoped<IPisoService, PisoService>();
            services.AddScoped<IHabitacionService, HabitacionService>();
            return services;
        }
    }
}
