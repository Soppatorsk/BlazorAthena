using AthenaResturantWebAPI.Data.AppUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISalesService _salesService;
    private readonly UserManager<ApplicationUser> _userManager;

    public SalesController(ISalesService salesService, UserManager<ApplicationUser> userManager)
    {
        _salesService = salesService;
        _userManager = userManager;
    }



    [HttpGet("history")]
    public async Task<IActionResult> GetSalesHistory([FromQuery] string timeFrame)
    {
        // Ensure timeFrame is valid and handle accordingly

        // Get the current user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(); // User not authenticated
        }

        // Check if the user is in the "Manager" role
        if (!await _userManager.IsInRoleAsync(user, "Manager"))
        {
            return Forbid(); // User does not have the required role
        }

        // Fetch sales history data based on the selected time frame
        var salesHistory = _salesService.GetSalesHistory(timeFrame);

        return Ok(salesHistory);
    }
}
