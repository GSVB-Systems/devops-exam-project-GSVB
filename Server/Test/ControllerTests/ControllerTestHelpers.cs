using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Test.ControllerTests;

internal static class ControllerTestHelpers
{
    internal static void SetUser(ControllerBase controller, string? userId = null)
    {
        ClaimsIdentity identity;

        if (string.IsNullOrWhiteSpace(userId))
        {
            identity = new ClaimsIdentity();
        }
        else
        {
            identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, userId)
            ],
            authenticationType: "TestAuth");
        }

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
    }
}

