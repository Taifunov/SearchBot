using Microsoft.AspNetCore.Mvc;
using SearchBot.Telegram.Api.Services;
using Telegram.Bot.Types;

namespace SearchBot.Telegram.Api.Controllers;

public class UpdateController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromServices] IHandleUpdateService handleUpdateService,
                                          [FromBody] Update update)
    {
        await handleUpdateService.HandleUpdate(update);
        return Ok();
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}
