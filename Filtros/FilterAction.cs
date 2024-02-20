using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoresAPI.Filtros
{
    public class FilterAction : IActionFilter
    {
        private readonly ILogger<FilterAction> logger;

        public FilterAction(ILogger<FilterAction> logger)
        {
            this.logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("Antes de ejecutar la acción");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("Después de ejecutar la acción");
        }        
    }
}
