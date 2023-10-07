using System.Net;

namespace MagicVilla_API.Models
{
    public class APIResponse
    {
        //** Estado Code **
        public HttpStatusCode statusCode { get; set; }

        public bool IsExistoso { get; set; } = true;

        public List<String> ErrorMessage { get; set; }

        //Tipo objeto
        public object Resultado { get; set; }
    }
}
