using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Cycloid.Logger;

namespace Cycloid.Handlers
{
    public class LogExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            Log("OnException", actionExecutedContext);
        }


        private void Log(string methodName, HttpActionExecutedContext actionContext)
        {
            var controllerName = actionContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var actionName = actionContext.ActionContext.ActionDescriptor.ActionName;
            var exceptionMessage = actionContext.Exception.Message;

            var message = $"{methodName} controller:{controllerName} action:{actionName} exception message:{exceptionMessage}";

            Debug.WriteLine(message, "Exception Filter Log");
            LogEngine.DefaultLogger.WriteToLog(LogLevels.Error, message, "Exception Filter Log");
        }
    }
}
