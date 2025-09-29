using System;
using Microsoft.AspNetCore.Mvc;
using PublisherAPI.Models;
using PublisherAPI.Services;

namespace PublisherAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RabbitMQController : ControllerBase
    {
        private readonly IRabbitMQService<ClassMessageRMQ> _rabbitMQService;

        public RabbitMQController(IRabbitMQService<ClassMessageRMQ> rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        [HttpPost("send")]
        public IActionResult PostMessage([FromBody] ClassMessageRMQ message)
        {
            _rabbitMQService.sendMessage(message);
            return Ok(new { Status = "Message sent to RabbitMQ", Message = message });
        }

        [HttpPost("sendWithConfirmation")]
        public IActionResult PostMessageWithConfirmation([FromBody] ClassMessageRMQ message)
        {
            bool confirmed = _rabbitMQService.SendMessageWithConfirmation(message);

            if (confirmed)
                return Ok(new { success = true, info = "Mensaje confirmado por RabbitMQ" });
            else
                return StatusCode(500, new { success = false, error = "RabbitMQ no confirmó el mensaje" });
        }
    }
}