using MagicVilla_API.Models;

namespace MagicVilla_API.Repositorio.IRepositorio
{
    public interface IVillaRepositorio : IRepositorio<Villa>
    {

        //Definimos método de actualización. 
        Task<Villa> Actualizar(Villa entidad);

    }
}
