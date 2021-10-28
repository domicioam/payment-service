using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Payment.Communication.RabbitMq;
using Payment.EventSourcing.Messages;

namespace Payment.Gateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthoriseController : ControllerBase
    {
        private readonly RabbitMqPublisher _rabbitMqPublisher;

        public AuthoriseController(RabbitMqPublisher rabbitMqPublisher)
        {
            _rabbitMqPublisher = rabbitMqPublisher;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AuthorisationCommand authorisationCommand)
        {
            string commandSerialized = JsonSerializer.Serialize(authorisationCommand);
            await _rabbitMqPublisher.SendMessageAsync(commandSerialized, "authorise");
            return Ok();
        }
    }
}