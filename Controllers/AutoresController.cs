using AutoresAPI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AutoresAPI.Controllers {
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase {
        [HttpGet]
        public ActionResult<List<Autor>> Get() { 
            return new List<Autor>() { 
                new Autor() { Id = 1, Nombre = "Gabriel", Apellido = "García Marquez" },
                new Autor() { Id = 2, Nombre = "Julio", Apellido = "Cortázar" }
            };
        }
    }
}
