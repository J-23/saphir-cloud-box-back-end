using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SaphirCloudBox.Host.Infractructure;
using SaphirCloudBox.Services.Contracts.Exceptions;
using SaphirCloudBox.Services.Contracts.Services;

namespace SaphirCloudBox.Host.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogService _logService;

        public ExceptionMiddleware(RequestDelegate next, ILogService logService)
        {
            _next = next;
            _logService = logService;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await _logService.Add(Enums.LogType.Error, ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            var message = string.Empty;

            if (exception is FoundSameObjectException)
            {
                message = ResponseMessage.SAME_OBJECT.ToString();
            }
            else if (exception is NotFoundException)
            {
                message = ResponseMessage.NOT_FOUND.ToString();
            }
            else if (exception is ExistDependencyException)
            {
                message = ResponseMessage.EXIST_DEPENDENCY_OBJECTS.ToString();
            }
            else if (exception is NotFoundDependencyObjectException)
            {
                message = ResponseMessage.NOT_FOUND_DEPENDENCY_OBJECT.ToString();
            }
            else if (exception is RoleManagerException)
            {
                message = ResponseMessage.ERROR.ToString();
            }
            else if (exception is UserManagerException)
            {
                message = ResponseMessage.ERROR.ToString();
            }
            else if (exception is UnavailableOperationException)
            {
                message = ResponseMessage.NO_ACCESS.ToString();
            }
            else if (exception is AppUnauthorizedAccessException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                var ex = exception as AppUnauthorizedAccessException;

                if (ex.UnautorizedType == UnautorizedType.NOT_FOUND)
                {
                    if (ex.ObjectType == ObjectType.USER)
                    {
                        message = ResponseMessage.NOT_FOUND_USER.ToString();
                    }
                    else if (ex.ObjectType == ObjectType.ROLE)
                    {
                        message = ResponseMessage.NOT_FOUND_ROLE.ToString();
                    }
                }
                else if (ex.UnautorizedType == UnautorizedType.SAME_OBJECT)
                {
                    if (ex.ObjectType == ObjectType.USER)
                    {
                        message = ResponseMessage.SAME_USER.ToString();
                    }
                    else if (ex.ObjectType == ObjectType.ROLE)
                    {
                        message = ResponseMessage.SAME_ROLE.ToString();
                    }
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return context.Response.WriteAsync(message);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
