using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelApp.DAL.Repositories;
using HotelApp.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDalDependencies(
    this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<HotelDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IBloqueRepository, BloqueRepository>();
            services.AddScoped<IPisoRepository, PisoRepository>();
            services.AddScoped<ITipoHabitacionRepository, TipoHabitacionRepository>();
            services.AddScoped<IHabitacionRepository, HabitacionRepository>();

            return services;
        }
    }
}