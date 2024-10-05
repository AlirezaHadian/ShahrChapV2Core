using Azure.Core.Pipeline;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
        private IPermissionService _permissionService;
        private int _permissionId = 0;
        public PermissionCheckerAttribute(int permissionId)
        {
            _permissionId = permissionId;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _permissionService = (IPermissionService)context.HttpContext.RequestServices.GetService(typeof(IPermissionService));
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                string username = context.HttpContext.User.Identity.Name;
                if (!_permissionService.CheckPermission(_permissionId, username))
                {
                    if (_permissionId == 1) //تلاش کاربر عادی برای ورود به پنل ادمین
                    {
                        context.Result = new RedirectResult("/AccessDenied");
                    }
                    else
                    {
                        context.Result = new RedirectResult("/Admin/AdminAccessDenied");
                    }
                }
            }
            else
            {
                context.Result = new RedirectResult("/Login");
            }
        }
    }
}
