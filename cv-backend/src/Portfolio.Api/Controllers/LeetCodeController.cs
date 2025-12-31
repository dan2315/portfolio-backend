using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Models;

[ApiController]
[Route("/leetcode")]
public class LeetCodeController : ControllerBase
{
    private const string LeetCodeUsername = "dan2315"; 
    private readonly ILeetCodeService _leetCodeService;

    public LeetCodeController(ILeetCodeService leetCodeService)
    {
        _leetCodeService = leetCodeService;
    }

    [HttpGet("profile")]
    [TrackActivity]
    public async Task<ActionResult<LeetCodeProfile>> GetProfile()
    {
        var profile = await _leetCodeService.GetProfileAsync(LeetCodeUsername);
        return Ok(profile);
    }

    [HttpGet("languages")]
    public async Task<ActionResult<LeetCodeLanguages>> GetLanguages()
    {
        var langs = await _leetCodeService.GetLanguagesAsync(LeetCodeUsername);
        return Ok(langs);
    }

    [HttpGet("submissions")]
    public async Task<ActionResult<LeetCodeSubmissions>> GetSubmissions([FromQuery] int limit = 8)
    {
        var submissions = await _leetCodeService.GetSubmissionsAsync(LeetCodeUsername, limit);
        return Ok(submissions);
    }

    [HttpGet("activity")]
    public async Task<ActionResult<LeetCodeActivity>> GetActivity([FromQuery] int? year)
    {
        var activity = await _leetCodeService.GetActivityAsync(LeetCodeUsername, year);
        return Ok(activity);
    }
}
