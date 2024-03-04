using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AutoresAPI.Utilidades {
    public class SwaggerVersion : IControllerModelConvention {
        public void Apply(ControllerModel controller) {
            var nameSpace = controller.ControllerType.Namespace;
            var version = nameSpace.Split(".").Last().ToLower();

            controller.ApiExplorer.GroupName = version;
        }
    }
}
