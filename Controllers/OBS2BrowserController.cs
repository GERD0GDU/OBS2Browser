using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace OBS2Browser;

[Route("[controller]")]
[ApiController]
public class OBS2BrowserController : ControllerBase
{
    private readonly IHubContext<WSHub> _hubContext;

    public OBS2BrowserController(IHubContext<WSHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpGet("/")]
    [Produces("application/html")]
    public IActionResult Get()
    {
        using StreamReader sr = new(@".\wwwroot\index.html");
        HttpContext.Response.Headers.Location = "/";
        return new ContentResult()
        {
            ContentType = "text/html",
            Content = sr.ReadToEnd(),
            StatusCode = StatusCodes.Status200OK
        };
    }

    [HttpPost("/whip")]
    [Produces("application/sdp")]
    public async Task<IActionResult> Post()
    {
        string offer = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        if (string.IsNullOrEmpty(offer)) {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        Console.WriteLine("[offer]");
        Console.WriteLine(offer);

        WSHub.ResetEvent();
        await _hubContext.Clients.All.SendAsync("ReceiveOfferMessage", offer);
        string? answer = await WSHub.GetAnswerAsync();

        if (string.IsNullOrEmpty(answer)) {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        Console.WriteLine("[answer]");
        Console.WriteLine(answer);

        HttpContext.Response.Headers.Location = "/";
        return new ContentResult()
        {
            ContentType = "application/sdp",
            Content = answer,
            StatusCode = StatusCodes.Status201Created
        };
    }
}