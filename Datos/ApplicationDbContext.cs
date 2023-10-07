﻿using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Models.Dto
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
                
        }
        public DbSet<Villa> villas { get; set; }
        public DbSet<NumeroVilla> NumeroVillas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData(
                new Villa()
                {
                    Id = 1,
                    Nombre = "Villa Real",
                    Detalle = "Detalle de la villa...",
                    ImagenUrl = "",
                    Ocupantes = 5,
                    MetrosCuadrados = 50,
                    Tarifa = 200,
                    Amenidad = "",
                    Fecha = DateTime.Now,
                    FechaActualizacion = DateTime.Now,
                },

                  new Villa()
                  {
                      Id = 2,
                      Nombre = "Premium vista a la Piscina",
                      Detalle = "Detalle de la villa...",
                      ImagenUrl = "",
                      Ocupantes = 4,
                      MetrosCuadrados = 50,
                      Tarifa = 150,
                      Amenidad = "",
                      Fecha = DateTime.Now,
                      FechaActualizacion = DateTime.Now,
                  }
           );
        }
    }
}
