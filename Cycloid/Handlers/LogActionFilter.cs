using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Routing;
using Cycloid.Logger;

namespace Cycloid.Handlers
{
    public class LogActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Log("OnActionExecuting", actionContext);
            base.OnActionExecuting(actionContext);
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            Log("OnActionExecuted", actionExecutedContext.ActionContext);
        }

        private void Log(string methodName, HttpActionContext context)
        {
            var controllerName = context.ControllerContext.ControllerDescriptor.ControllerName;
            var actionName = context.ActionDescriptor.ActionName;
            var code = context.Response?.StatusCode;

            var message = code == null
            ? $"{methodName} controller:{controllerName} action:{actionName}"
            : $"{methodName} controller:{controllerName} action:{actionName} statusCode: {code}";

            Debug.WriteLine(message, "Action Filter Log");
            LogEngine.DefaultLogger.WriteToLog(LogLevels.Info, message, "Action Filter Log");
        }
    }
}
