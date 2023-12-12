using System.Net;

namespace TorunLive.WebApi.Authentication
{
    public class ApiKeyMiddleware(RequestDelegate next, IApiKeyValidation apiKeyValidation)
    {
        private readonly RequestDelegate _next = next;
        private readonly IApiKeyValidation _apiKeyValidation = apiKeyValidation;

        public async Task InvokeAsync(HttpContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Request.Headers[Constants.ApiKeyHeaderName]))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
            string? userApiKey = context.Request.Headers[Constants.ApiKeyHeaderName];
            if (!_apiKeyValidation.IsValid(userApiKey!))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
            await _next(context);
        }
    }
}
