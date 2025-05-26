using Microsoft.AspNetCore.Http;
using StarWarsAPI.Application.Exceptions;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;


namespace StarWarsAPI.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            if (exception is NotFoundException)
            {
                code = HttpStatusCode.NotFound;
            }
            else if (exception is InvalidCredentialsException)
            {
                code = HttpStatusCode.Unauthorized;
            }
            else if (exception is ArgumentException)
            {
                code = HttpStatusCode.BadRequest;
            }
            else if (exception is EmailAlreadyExistsException)
            {
                code = HttpStatusCode.BadRequest;
            }

            var response = new
            {
                error = exception.Message
            };

            result = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }

    }
}



