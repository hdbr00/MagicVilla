﻿using AutoMapper;
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
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;

        //private readonly ApplicationDbContext _dbcontext; LO REEMPLAZAMOS...

        private readonly IVillaRepositorio _villaRepo; 
        
        private readonly IMapper _mapper;

        protected APIResponse _response; 




        public VillaController(ILogger<VillaController> logger, ApplicationDbContext dbContext,IMapper mapper,IVillaRepositorio villaRepo)
        {
                _logger = logger;   
                //_dbcontext = dbContext;
                _villaRepo = villaRepo;
                _mapper = mapper;
                
                _response = new(); 
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                _logger.LogInformation("Obtener las villas");
                IEnumerable<Villa> villaList = await _villaRepo.ObtenerTodos();

                _response.Resultado = _mapper.Map<IEnumerable<VillaDto>>(villaList);
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


        [HttpGet("id:int",Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsExistoso = false; 
                    return BadRequest(_response); // mala solicitud 
                                                  // nos devuelve un código estado 400. 
                }

                //var villa = VillaStore.villaList.FirstOrDefault(p => p.Id == id); 
                var villa = await _villaRepo.Obtener(x => x.Id == id); //Solo un registro.

                if (villa == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.IsExistoso = false; 
                    return NotFound(_response);// no lo encontro
                                      // nos devuelve un código estado 404. 
                }
                _response.Resultado = _mapper.Map<VillaDto>(villa); 
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
        public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] VillaCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _villaRepo.Obtener(p => p.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe!");
                    return BadRequest(ModelState);
                }


                if (createDto == null)
                {
                    return BadRequest(createDto);

                }

                Villa modelo = _mapper.Map<Villa>(createDto); //**


                modelo.Fecha = DateTime.Now; 
                modelo.FechaActualizacion = DateTime.Now;
                await _villaRepo.Crear(modelo);
                _response.Resultado = modelo; 
                _response.statusCode = HttpStatusCode.Created;   



                return CreatedAtRoute("GetVilla", new { id = modelo.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsExistoso = false; 
                _response.ErrorMessage = new List<string>() { ex.ToString() };
               
            }

            return _response; 
        }
        //*********************************************************************
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest; 
                    return BadRequest(_response);
                }
                var villa = await _villaRepo.Obtener(p => p.Id == id);
                if (villa == null)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.NotFound; 
                    return NotFound(_response);
                }

                await _villaRepo.Remover(villa);

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
        public async  Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto villaDto)
        {
            if (villaDto == null || id!=villaDto.Id)
            {
                _response.IsExistoso=false;
                _response.statusCode = HttpStatusCode.BadRequest; 
                return BadRequest(_response); 
            }

            //var villa = VillaStore.villaList.FirstOrDefault(p => p.Id == id);
            //villa.Nombre = villaDto.Nombre; 
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados; 

            Villa modelo = _mapper.Map<Villa>(villaDto);    //**

          

            await _villaRepo.Actualizar(modelo);
            _response.statusCode = HttpStatusCode.NoContent;

            return Ok(_response);
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

            var villa = await _villaRepo.Obtener(p => p.Id == id,tracked:false);

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa); 

            if(villa==null) return BadRequest();


            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  

            }
            Villa modelo = _mapper.Map<Villa>(villaDto); 

            
           await _villaRepo.Actualizar(modelo);
           _response.statusCode = HttpStatusCode.NoContent; 

            return Ok(_response);
        }
    }
}
