using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Models;
using MagicVilla_API.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;

        private readonly ApplicationDbContext _dbcontext;
        
        private readonly IMapper _mapper; 




        public VillaController(ILogger<VillaController> logger, ApplicationDbContext dbContext,IMapper mapper)
        {
                _logger = logger;   
                _dbcontext = dbContext;
                _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            _logger.LogInformation("Obtener las villas");
            IEnumerable<Villa> villaList = await _dbcontext.villas.ToListAsync(); 

            return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList)); //**
        }


        [HttpGet("id:int",Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest(); // mala solicitud 
                                    // nos devuelve un código estado 400. 
            }
            
            //var villa = VillaStore.villaList.FirstOrDefault(p => p.Id == id); 
            var villa = await _dbcontext.villas.FirstOrDefaultAsync(x => x.Id == id); //Solo un registro.

            if (villa == null)
            {
                return NotFound();// no lo encontro
                                  // nos devuelve un código estado 404. 
            }

            return Ok(_mapper.Map<VillaDto>(villa)); //mapper

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            if (await _dbcontext.villas.FirstOrDefaultAsync(p=>p.Nombre.ToLower() == createDto.Nombre.ToLower())!=null)
            {
                ModelState.AddModelError("NombreExiste","La villa con ese nombre ya existe!");
                return BadRequest(ModelState);
            }


            if (createDto == null)
            {
                return BadRequest(createDto);

            }

            Villa modelo = _mapper.Map<Villa>(createDto); //**

          

           await _dbcontext.villas.AddAsync(modelo);
           await _dbcontext.SaveChangesAsync(); 


            return CreatedAtRoute("GetVilla", new {id=modelo.Id},modelo); 
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id==0) 
            {
                return BadRequest(); 
            }
            var villa = await _dbcontext.villas.FirstOrDefaultAsync(p => p.Id == id);
            if (villa == null)
            {
                return NotFound(); 
            }

            _dbcontext.villas.Remove(villa); // no es un método asincrono.
            await _dbcontext.SaveChangesAsync(); 

            return NoContent(); 
        }


     
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async  Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto villaDto)
        {
            if (villaDto == null || id!=villaDto.Id)
            {
                return BadRequest(); 
            }

            //var villa = VillaStore.villaList.FirstOrDefault(p => p.Id == id);
            //villa.Nombre = villaDto.Nombre; 
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados; 

            Villa modelo = _mapper.Map<Villa>(villaDto);    //**

          

            _dbcontext.villas.Update(modelo);
            await _dbcontext.SaveChangesAsync(); 

            return NoContent();
        }


        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbcontext.villas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa); 

            if(villa==null) return BadRequest();


            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  

            }
            Villa modelo = _mapper.Map<Villa>(villaDto); 

            
           _dbcontext.villas.Update(modelo);
           await _dbcontext.SaveChangesAsync(); 

            return NoContent();
        }



    }

}
