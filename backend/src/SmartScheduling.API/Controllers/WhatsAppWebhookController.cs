using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SmartScheduling.API.Controllers;

[ApiController]
[Route("api/webhook/whatsapp")]
public class WhatsAppWebhookController(IConfiguration configuration, ILogger<WhatsAppWebhookController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult Verify(
        [FromQuery(Name = "hub.mode")] string mode,
        [FromQuery(Name = "hub.verify_token")] string token,
        [FromQuery(Name = "hub.challenge")] string challenge)
    {
        if (mode == "subscribe" && token == configuration["WhatsApp:VerifyToken"])
            return Ok(challenge);
        return Forbid();
    }

    [HttpPost]
    public IActionResult Receive([FromBody] JsonElement payload)
    {
        logger.LogInformation("Webhook recebido");
        return Ok();
    }
}
