using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Layer.ExceptionLog;

namespace WebAPI.Layer.Authorization
{
    public class APIKeyAuthorization : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
                var apiKey = configuration.GetValue<string>("AppConfig:API_Key");

                if (!context.HttpContext.Request.Headers.TryGetValue("api_key", out var extractedApiKey) ||
                    !apiKey.Equals(extractedApiKey))
                {
                    //context.Result = new UnauthorizedResult();
                    context.Result = new ObjectResult(new
                    {
                        message = "Authorization Failed",
                        responseCode = 403,
                        details = "Unauthorized client, Please check you API-Key"
                    })
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                    return;
                }

                await next();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);

                //context.Result = new ObjectResult(new { Message = "An error occurred while authorizing the request." })
                //{
                //    StatusCode = StatusCodes.Status403Forbidden
                //};
                context.Result = new ObjectResult(new
                {
                    message = "Authorization Failed: API-Key missing",
                    responseCode = 403,
                    details = "Error Message: " + ex?.Message
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };

                return;
            }
        }
    }
}
