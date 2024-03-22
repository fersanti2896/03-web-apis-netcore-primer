using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers {
    public class CustomBaseController : ControllerBase {
        protected string obtenerUsuarioId() {
            var usuarioClaim = HttpContext.User.Claims.Where(x => x.Type == "id").FirstOrDefault();
            var usuarioId = usuarioClaim.Value;

            return usuarioId;
        }
    }
}
