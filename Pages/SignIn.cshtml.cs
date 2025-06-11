using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Conelards;
using Conelards.Database;
using Conelards.Helpers;

namespace Conelards.Pages;

public class SignInPageModel : PageModel
{
    private readonly ILogger<SignInPageModel> _logger;

    [BindProperty]
    public SignInForm SubmittedCredential { get; set; }

    public SignInPageModel(ILogger<SignInPageModel> logger)
    {
        _logger = logger;
    }

    public async Task OnPostAsync()
    {
        if (!ModelState.IsValid) return;

        using var db = new AccountDbContext();

        if (SubmittedCredential.Action != "Sign up"
            && !db.Users.Any(acc =>
                acc.Username == SubmittedCredential.Username
                && acc.HashPassword == SHA256TextHasher.Hash(SubmittedCredential.Password)
            )
        )
        {
            RedirectToPage("/Login");
            return;
        }

        if (SubmittedCredential.Action == "Sign up"
            && !db.Users.Any(cred => cred.Username == SubmittedCredential.Username))
        {
            db.Add(new AccountDbModel
            {
                Username = SubmittedCredential.Username,
                HashPassword = SHA256TextHasher.Hash(SubmittedCredential.Password)
            });

            await db.SaveChangesAsync();
        }

        await HttpContext.SignInAsync(
            AuthCookie.Name,
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    [new Claim(ClaimTypes.Name, SubmittedCredential.Username)]
                )
            )
        );

        RedirectToPage("/Home");
    }
}

public class SignInForm
{
    [Required]
    [Length(5, 25)]
    public string Username { get; set; }

    [Required]
    [Length(5, 25)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    public string Action { get; set; }
}