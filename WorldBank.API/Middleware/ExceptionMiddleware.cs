using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using WorldBank.Shared.Constant;
using WorldBank.Shared.ResponseModel.CommonResponse;

namespace WorldBank.API.Middleware
{
    public static class ExceptionMiddleware
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            //ILog log = log4net.LogManager.GetLogger(typeof(ExceptionMiddleware));

            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    string pathName = "";
                    if (context.Request.Path.HasValue)
                    {
                        pathName = context.Request.Path.Value;
                    }

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        //logger.LogError($"{pathName}: {contextFeature.Error}");
                        await context.Response.WriteAsync(new ErrorResponse(Constant.ErrorCode.InternalServerError, Constant.ErrorMessage.InternalServerError).ToString());
                    }
                });
            });
        }
    }
}
