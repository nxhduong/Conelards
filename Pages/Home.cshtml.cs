using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Conelards.Pages;

[Authorize]
public class HomePageModel : PageModel
{
    private readonly ILogger<HomePageModel> _logger;

    [BindProperty]
    RoomIdForm SubmittedRoomId { get; set; }

    public HomePageModel(ILogger<HomePageModel> logger)
    {
        _logger = logger;
    }

    public void OnPost()
    {
        var identity = User.Identity as ClaimsIdentity;

        identity!.RemoveClaim(identity.FindFirst(claim => claim.Type == "CurrentRoomId"));
        identity.AddClaim(new Claim("CurrentRoomId", SubmittedRoomId.Code));

        RedirectToPage("/Room"); //?id=" + SubmittedRoomId.Code);
    }
}

public class RoomIdForm
{
    [Required]
    [Length(9, 16)]
    public string Code { get; set; }
}