using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Asn1.X509;
using razorweb.models;

namespace App.Security.Requirements
{
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AppAuthorizationHandler> _logger;
        public AppAuthorizationHandler(UserManager<AppUser> userManager, ILogger<AppAuthorizationHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var requirements = context.PendingRequirements.ToList();
            foreach (var requirement in requirements)
            {
                if (requirement is GenZRequirement)
                {
                    if (IsGenZ(context.User, (GenZRequirement)requirement))
                    {
                        context.Succeed(requirement);
                    }
                }
                if (requirement is ArticleRequirement)
                {
                    if (CanUpdateArticle(context.User, context.Resource, (ArticleRequirement)requirement))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
            return Task.CompletedTask;
        }

        private bool CanUpdateArticle(ClaimsPrincipal user, object? resource, ArticleRequirement requirement)
        {
            if (user.IsInRole("Admin"))
            {
                _logger.LogInformation("Admin can update articles");
                return true;
            }
            var article = resource as Article;
            var createdDate = article.Created;
            var date = new DateTime(2024, 1, 6);
            if (createdDate < date)
            {
                _logger.LogInformation("Overtime to update");
                return false;
            }
            return true;
        }

        private bool IsGenZ(ClaimsPrincipal user, GenZRequirement requirement)
        {
            var appUserTask = _userManager.GetUserAsync(user);
            Task.WaitAll(appUserTask);
            var appUser = appUserTask.Result;

            if (appUser.DateOfBirth == null)
            {
                _logger.LogInformation("Khong thoa man GenZ");
                return false;
            }
            int year = appUser.DateOfBirth.Value.Year;
            var result = year >= requirement.FromYear && year <= requirement.ToYear;
            if (result)
            {
                _logger.LogInformation("Thoa man thoa man GenZ");
                return true;
            }
            else
            {
                _logger.LogInformation("Khong 2 thoa man GenZ");
                return false;
            }
        }
    }
}