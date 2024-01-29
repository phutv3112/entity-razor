using System.ComponentModel.DataAnnotations;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Bcpg;
using razorweb.models;

namespace App.Admin.Role
{
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, BlogContext context) : base(roleManager, context)
        {
        }
        public class InputModel
        {
            [Display(Name = "Role Name")]
            [StringLength(255), MinLength(3)]
            public string Name { get; set; }
        }
        [BindProperty]
        public InputModel Input { set; get; }
        public IdentityRole role { set; get; }
        public async Task<IActionResult> OnGetAsync(string roleid)
        {
            if (roleid == null) return NotFound("Role not found");
            role = await _roleManager.FindByIdAsync(roleid);
            if (role != null)
            {
                Input = new InputModel()
                {
                    Name = role.Name
                };
                return Page();
            }
            return NotFound("Role not found");
        }
        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid == null) return NotFound("Role not found");
            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null) return NotFound("Role not found");

            if (!ModelState.IsValid)
            {
                return Page();
            }
            role.Name = Input.Name;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                StatusMessage = $"Updated role {Input.Name}";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(e =>
                {
                    ModelState.AddModelError(string.Empty, e.Description);
                });
            }
            return Page();
        }
    }
}
