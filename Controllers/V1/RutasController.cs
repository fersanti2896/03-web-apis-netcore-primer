using AutoresAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AutoresAPI.Controllers.V1 {
    [ApiController]
    [Route("api/v1")]
    public class RutasController : ControllerBase {
        [HttpGet(Name = "ObtenerRoot")]
        public ActionResult<IEnumerable<DatoHATEOAS>> obtenerRoot() {
            var datos = new List<DatoHATEOAS>();
            datos.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerRoot", new { }), descripcion: "self", metodo: "GET"));
            datos.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerAutores", new { }), descripcion: "Listado de Autores.", metodo: "GET"));
            datos.Add(new DatoHATEOAS(enlace: Url.Link("ObtieneAutor", new { Id = 1 }), descripcion: "Obtiene un autor por su id.", metodo: "GET"));
            datos.Add(new DatoHATEOAS(enlace: Url.Link("CrearAutor", new { }), descripcion: "Crear un autor.", metodo: "POST"));

            return datos; 
        }
    }
}
