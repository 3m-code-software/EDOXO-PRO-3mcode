using System.Security.Claims;
using EdoxoPro.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EdoxoPro.Api.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class PermissionFilter : Attribute, IAuthorizationFilter
{
    private readonly string _permission;

    public PermissionFilter(string permission)
    {
        _permission = permission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user.Identity is not { IsAuthenticated: true })
        {
            context.Result = new UnauthorizedObjectResult(ApiResponse<object>.Fail("Unauthorized"));
            return;
        }

        var permissionsClaim = user.FindFirstValue("permissions");
        if (permissionsClaim == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var permissions = permissionsClaim.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (!permissions.Contains(_permission))
        {
            context.Result = new ForbidResult();
        }
    }
}
