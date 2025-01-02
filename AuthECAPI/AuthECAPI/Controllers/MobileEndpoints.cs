using AuthECAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthECAPI.Controllers
{
    public static class MobileEndpoints
    {
        public static IEndpointRouteBuilder MapMobileEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/MobileUserProfile", GetMobileUserProfile);
            app.MapPost("/UpdateMobileUserProfile", UpdateMobileUserProfile);
            return app;
        }

        [Authorize]
        private static async Task<IResult> GetMobileUserProfile(
            ClaimsPrincipal user,
            UserManager<AppUser> userManager)
        {
            string userID = user.Claims.First(x => x.Type == "userID").Value;
            var userDetails = await userManager.FindByIdAsync(userID);
            return Results.Ok(
                new
                {
                    Email = userDetails?.Email,
                    FullName = userDetails?.FullName,
                    Gender = userDetails?.Gender,
                    DOB = userDetails?.DOB,
                    LibraryID = userDetails?.LibraryID
                });
        }

        [Authorize]
        private static async Task<IResult> UpdateMobileUserProfile(
            ClaimsPrincipal user,
            UserManager<AppUser> userManager,
            [FromBody] AppUser updatedUser)
        {
            string userID = user.Claims.First(x => x.Type == "userID").Value;
            var userDetails = await userManager.FindByIdAsync(userID);

            if (userDetails != null)
            {
                userDetails.FullName = updatedUser.FullName;
                userDetails.Gender = updatedUser.Gender;
                userDetails.DOB = updatedUser.DOB;
                userDetails.LibraryID = updatedUser.LibraryID;

                var result = await userManager.UpdateAsync(userDetails);

                if (result.Succeeded)
                    return Results.Ok(result);
                else
                    return Results.BadRequest(result);
            }
            else
            {
                return Results.NotFound(new { message = "User not found." });
            }
        }
    }
}
