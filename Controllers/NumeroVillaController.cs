using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Models;
using MagicVilla_API.Models.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NumeroVillaController : ControllerBase
    {
        private readonly ILogger<NumeroVillaController> _logger;

        //private readonly ApplicationDbContext _dbcontext; LO REEMPLAZAMOS...

        private readonly IVillaRepositorio _villaRepo; 


        private readonly INumeroVillaRepositorio _numeroRepo; 


        
        private readonly IMapper _mapper;

        protected APIResponse _response; 




        public NumeroVillaController(ILogger<NumeroVillaController> logger,IMapper mapper,IVillaRepositorio villaRepo,INumeroVillaRepositorio numeroRepo)
        {
                _logger = logger;
               //_dbcontext = dbContext;
                _numeroRepo = numeroRepo;
                _villaRepo = villaRepo;
                _mapper = mapper;
                _response = new(); 
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetNumeroVillas()
        {
            try
            {
                _logger.LogInformation("Obtener Numeros villas");
                IEnumerable<NumeroVilla> numerovillaList = await _numeroRepo.ObtenerTodos();

                _response.Resultado = _mapper.Map<IEnumerable<NumeroVillaDto>>(numerovillaList);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {

                _response.IsExistoso = false;
                _response.ErrorMessage = new List<string>() {ex.ToString()};
            }

            return _response; 
           
        }


        [HttpGet("id:int",Name ="GetNumeroVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> GetNumeroVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogInformation("Error al traer Numero Villa con Id"+id);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsExistoso = false; 
                    return BadRequest(_response); // mala solicitud 
                                                  // nos devuelve un código estado 400. 
                }

                //var villa = VillaStore.villaList.FirstOrDefault(p => p.Id == id); 
                var numerovilla = await _numeroRepo.Obtener(x => x.VillaNo == id); //Solo un registro.

                if (numerovilla == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.IsExistoso = false; 
                    return NotFound(_response);// no lo encontro
                                      // nos devuelve un código estado 404. 
                }
                _response.Resultado = _mapper.Map<NumeroVillaDto>(numerovilla); 
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response); //mapper
            }
            catch (Exception ex)
            {
                _response.IsExistoso = false;
                _response.ErrorMessage = new List<string> { ex.ToString() }; 
             
            }
            return _response; 

        }
        //*********************************************Here.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] NumeroVillaCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _numeroRepo.Obtener(p => p.VillaNo == createDto.VillaNo) != null)
                {
                    ModelState.AddModelError("NombreExiste", "El numero de villa ya existe!");
                    return BadRequest(ModelState);
                }

                if (await _villaRepo.Obtener(v => v.Id==createDto.VillaId) == null)
                {

                    ModelState.AddModelError("ClaveForanea", "El id de la villa no existe!");
                    return BadRequest(ModelState);
                }

                if (createDto == null)
                {
                    return BadRequest(createDto);

                }

                NumeroVilla modelo = _mapper.Map<NumeroVilla>(createDto);


                modelo.FechaCreacion = DateTime.Now; 
                modelo.FechaActualizacion = DateTime.Now;
                await _numeroRepo.Crear(modelo);
                _response.Resultado = modelo; 
                _response.statusCode = HttpStatusCode.Created;   



                return CreatedAtRoute("GetNumeroVilla", new { id = modelo.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsExistoso = false; 
                _response.ErrorMessage = new List<string>() { ex.ToString() };
               
            }

            return _response; 
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNumeroVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest; 
                    return BadRequest(_response);
                }
                var numerovilla = await _numeroRepo.Obtener(p => p.VillaNo == id);
                if (numerovilla == null)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.NotFound; 
                    return NotFound(_response);
                }

                await _numeroRepo.Remover(numerovilla);

                _response.statusCode = HttpStatusCode.NoContent;
                return NoContent();
            }
            catch (Exception ex)
            {

                _response.IsExistoso = false;
                _response.ErrorMessage = new List<string>() { ex.ToString() }; 

            }

            return BadRequest(_response);   
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async  Task<IActionResult> UpdateNumeroVilla(int id, [FromBody] NumeroVillaUpdateDto villaDto)
        {
            if (villaDto == null || id!=villaDto.VillaNo)
            {
                _response.IsExistoso=false;
                _response.statusCode = HttpStatusCode.BadRequest; 
                return BadRequest(_response); 
            }

            //var villa = VillaStore.villaList.FirstOrDefault(p => p.Id == id);
            //villa.Nombre = villaDto.Nombre; 
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados; 


            if (await _villaRepo.Obtener(V=>V.Id == villaDto.VillaId)==null)
            {
                ModelState.AddModelError("ClaveForenea","El id de la villa no existe");
                return BadRequest(ModelState);

            }

            NumeroVilla modelo = _mapper.Map<NumeroVilla>(villaDto);    //**

          

            await _numeroRepo.Actualizar(modelo);
            _response.statusCode = HttpStatusCode.NoContent;

            return Ok(_response);
        }

    }
}
