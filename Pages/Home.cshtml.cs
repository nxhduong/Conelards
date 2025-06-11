using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Conelards.Pages;

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
        RedirectToPage("/Room?id=" + SubmittedRoomId.Code);
    }
}

public class RoomIdForm
{
    [Required]
    [Length(9, 16)]
    public string Code { get; set; }
}