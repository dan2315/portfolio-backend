using Microsoft.AspNetCore.Mvc;

[ApiController]
[RequireAdminApiKey]
[Route("/admin")]
public class AdminController : ControllerBase
{
    [HttpGet("check")]
    public ActionResult Check()
    {
        return Ok(new { message = "Admin API key is valid" });
    }
}