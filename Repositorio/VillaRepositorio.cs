using MagicVilla_API.Models;
using MagicVilla_API.Models.Dto;
using MagicVilla_API.Repositorio.IRepositorio;

namespace MagicVilla_API.Repositorio
{
    public class VillaRepositorio : Repositorio<Villa>, IVillaRepositorio
    {
        //dbContext...

        private readonly ApplicationDbContext _db;

        public VillaRepositorio(ApplicationDbContext db):base(db)
        {
            _db = db;   

        }

        public async Task<Villa> Actualizar(Villa entidad)
        {
            entidad.FechaActualizacion = DateTime.Now;
            _db.villas.Update(entidad);
            await _db.SaveChangesAsync();
            return entidad; 
        }
    }
}
