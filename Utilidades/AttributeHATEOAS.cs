using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoresAPI.Utilidades {
    public class AttributeHATEOAS : ResultFilterAttribute {
        protected bool incluirHateoas(ResultExecutingContext context) { 
            var result = context.Result as ObjectResult;

            if (!respSuccess(result)) { 
                return false;
            }

            var header = context.HttpContext.Request.Headers["incluirHateoas"];

            if (header.Count == 0) { 
                return false;
            }

            var valor = header[0];

            if(!valor.Equals("Y", StringComparison.InvariantCultureIgnoreCase)) {
                return false;
            }

            return true;
        }

        private bool respSuccess(ObjectResult result) {
            if (result is null || result.Value == null) { 
                return false;
            }

            if (result.StatusCode.HasValue && !result.StatusCode.Value.ToString().StartsWith("2")) {
                return false;
            }

            return true;
        }
    }
}
