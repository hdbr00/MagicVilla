using MagicVilla_API.Models;

namespace MagicVilla_API.Repositorio.IRepositorio
{
    public interface INumeroVillaRepositorio : IRepositorio<NumeroVilla>
    {

        //Definimos método de actualización. 
        Task<NumeroVilla> Actualizar(NumeroVilla entidad);



    }
}
