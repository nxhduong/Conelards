using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Colards.Pages;

public class HomeModel : PageModel
{
    private readonly ILogger<HomeModel> _logger;

    [BindProperty]
    RoomId Room { get; set; }

    public HomeModel(ILogger<HomeModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        //
    }

    public void OnPost()
    {
        
    }
}

public class RoomId
{
    [Required]
    public string Code { get; set; }
}