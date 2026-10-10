using Azure.Core.Pipeline;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ShahrChap.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.Security
{

    public class PermissionCheckerAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly int[] _permissionIds;

        public PermissionCheckerAttribute(params int[] permissionIds)
        {
            _permissionIds = permissionIds;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated != true)
            {
                context.Result = new RedirectResult("/Login");
                return;
            }

            var permissionService = context.HttpContext.RequestServices
                .GetRequiredService<IPermissionService>();

            string username = context.HttpContext.User.Identity.Name!;

            bool hasPermission = _permissionIds.Any(permissionId =>
                permissionService.CheckPermission(permissionId, username));

            if (!hasPermission)
            {
                if (_permissionIds.Contains(1))
                {
                    context.Result = new RedirectResult("/AccessDenied");
                }
                else
                {
                    context.Result = new RedirectResult("/Admin/AdminAccessDenied");
                }
            }
        }
    }

}
