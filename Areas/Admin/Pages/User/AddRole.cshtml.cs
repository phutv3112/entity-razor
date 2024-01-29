// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using razorweb.models;

namespace App.Admin.USer
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public string[] Roles { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SelectList allRoles { set; get; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"Unable to load user.");
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }
            Roles = (await _userManager.GetRolesAsync(user)).ToArray();
            List<string> rolesName = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(rolesName);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"Unable to load user.");
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }


            var OldRoles = (await _userManager.GetRolesAsync(user)).ToArray();
            var deleteRoles = OldRoles.Where(r => !Roles.Contains(r));
            var addRoles = Roles.Where(r => !OldRoles.Contains(r));

            List<string> rolesName = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(rolesName);

            var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRoles);
            if (!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(e =>
                {
                    ModelState.AddModelError(string.Empty, e.Description);
                });
                return Page();
            }
            var resultAdd = await _userManager.AddToRolesAsync(user, addRoles);
            if (!resultAdd.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(e =>
                {
                    ModelState.AddModelError(string.Empty, e.Description);
                });
                return Page();
            }
            StatusMessage = $"Update Role for {user.UserName} successfully!";
            return RedirectToPage("./Index");
        }
    }
}
