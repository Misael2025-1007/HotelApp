using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelApp.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.DAL.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }

        public DbSet<Hotel> Hoteles { get; set; }
        public DbSet<Bloque> Bloques { get; set; }
        public DbSet<Piso> Pisos { get; set; }
        public DbSet<TipoHabitacion> TiposHabitacion { get; set; }
        public DbSet<Habitacion> Habitaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hotel>(e =>
            {
                e.Property(h => h.Nombre).IsRequired().HasMaxLength(150);
                e.Property(h => h.Direccion).IsRequired().HasMaxLength(250);
                e.Property(h => h.Telefono).HasMaxLength(20);
            });

            modelBuilder.Entity<Bloque>(e =>
            {
                e.Property(b => b.Nombre).IsRequired().HasMaxLength(100);
                e.Property(b => b.Descripcion).HasMaxLength(250);
                e.HasOne(b => b.Hotel)
                 .WithMany(h => h.Bloques)
                 .HasForeignKey(b => b.HotelId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Piso>(e =>
            {
                e.HasOne(p => p.Hotel)
                 .WithMany(h => h.Pisos)
                 .HasForeignKey(p => p.HotelId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(p => p.Bloque)
                 .WithMany(b => b.Pisos)
                 .HasForeignKey(p => p.BloqueId)
                 .OnDelete(DeleteBehavior.Restrict);

                // RN03: no pueden existir dos pisos activos con el mismo número en un hotel
                e.HasIndex(p => new { p.HotelId, p.Numero })
                 .IsUnique()
                 .HasFilter("[Status] = 1");
            });

            modelBuilder.Entity<TipoHabitacion>(e =>
            {
                e.Property(t => t.Nombre).IsRequired().HasMaxLength(100);
                e.Property(t => t.Descripcion).HasMaxLength(250);
            });

            modelBuilder.Entity<Habitacion>(e =>
            {
                e.Property(h => h.Numero).IsRequired().HasMaxLength(10);
                e.Property(h => h.Tarifa).HasPrecision(10, 2);
                e.HasOne(h => h.Piso)
                 .WithMany(p => p.Habitaciones)
                 .HasForeignKey(h => h.PisoId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(h => h.TipoHabitacion)
                 .WithMany(t => t.Habitaciones)
                 .HasForeignKey(h => h.TipoHabitacionId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}