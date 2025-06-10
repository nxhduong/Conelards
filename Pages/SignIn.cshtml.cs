using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Colards;

namespace Colards.Pages;

public class SignInModel : PageModel
{
    private readonly ILogger<SignInModel> _logger;

    [BindProperty]
    public Credential SubmittedCredential { get; set; }

    public SignInModel(ILogger<SignInModel> logger)
    {
        _logger = logger;
    }

    public async Task OnPostAsync()
    {
        if (!ModelState.IsValid) return;

        using var db = new AccountContext();

        if (SubmittedCredential.Action != "Sign up"
            && !db.Users.Any(cred =>
                cred.Username == SubmittedCredential.Username
                && cred.HashPassword == TextHasher.Hash(SubmittedCredential.Password)
            )
        )
        {
            RedirectToPage("/Login");
            return;
        }

        if (SubmittedCredential.Action == "Sign up" && !db.Users.Any(cred => cred.Username == SubmittedCredential.Username))
        {
            db.Add(new AccountModel
            {
                Username = SubmittedCredential.Username,
                HashPassword = TextHasher.Hash(SubmittedCredential.Password)
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

public class Credential
{
    [Required]
    [Length(5, 25)]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Length(5, 25)]
    public string Password { get; set; }

    [Required]
    public string Action { get; set; }
}