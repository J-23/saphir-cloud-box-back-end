using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SaphirCloudBox.Host.Helpers;
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
        private readonly INotificationService _notificationService;

        public ExceptionMiddleware(RequestDelegate next, 
            ILogService logService,
            INotificationService notificationService)
        {
            _next = next;

            _logService = logService;
            _notificationService = notificationService;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            var message = string.Empty;
            Enums.LogType logType = Enums.LogType.Error;
            if (exception is FoundSameObjectException)
            {
                logType = Enums.LogType.SameObject;
                message = ResponseMessage.SAME_OBJECT.ToString();
            }
            else if (exception is NotFoundException)
            {
                logType = Enums.LogType.NotFound;
                message = ResponseMessage.NOT_FOUND.ToString();
            }
            else if (exception is ExistDependencyException)
            {
                logType = Enums.LogType.Error;
                message = ResponseMessage.EXIST_DEPENDENCY_OBJECTS.ToString();
            }
            else if (exception is NotFoundDependencyObjectException)
            {
                logType = Enums.LogType.NotFound;
                message = ResponseMessage.NOT_FOUND_DEPENDENCY_OBJECT.ToString();
            }
            else if (exception is RoleManagerException)
            {
                logType = Enums.LogType.Error;
                message = ResponseMessage.ERROR.ToString();
            }
            else if (exception is UserManagerException)
            {
                logType = Enums.LogType.Error;
                message = ResponseMessage.ERROR.ToString();
            }
            else if (exception is UnavailableOperationException)
            {
                logType = Enums.LogType.NoAccess;
                message = ResponseMessage.NO_ACCESS.ToString();
            }
            else if (exception is AppUnauthorizedAccessException)
            {
                logType = Enums.LogType.Error;
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
                logType = Enums.LogType.Error;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            _logService.Add(logType, exception.Message);
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
