using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Payment.Communication.RabbitMq;
using Payment.EventSourcing.Messages;
using Payment.Gateway.Model;
using Action = Payment.Gateway.Model.Action;

namespace Payment.Gateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly RabbitMqPublisher _rabbitMqPublisher;

        public TransactionController(RabbitMqPublisher rabbitMqPublisher)
        {
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AuthoriseInput input)
        {
            var (merchantId, creditCard, currency, amount) = input;
            var transactionId = Guid.NewGuid();
            var authorisationCommand =
                new AuthorisationCommand(transactionId, merchantId, creditCard, currency, amount);
            var commandSerialized = JsonSerializer.Serialize(authorisationCommand);
            await _rabbitMqPublisher.SendMessageAsync(commandSerialized, Queues.Authorise);
            return Accepted(transactionId);
        }

        [HttpPost("{id:guid}")]
        public async Task<IActionResult> Post([FromRoute] Guid id, [FromBody] TransactionRequest transactionRequest)
        {
            var (action, _, amount) = transactionRequest;
            switch (action)
            {
                case Action.Capture:
                    var capture = JsonSerializer.Serialize(new CaptureCommand(id, amount));
                    await _rabbitMqPublisher.SendMessageAsync(capture, Queues.Capture);
                    break;
                case Action.Refund:
                    var refund = JsonSerializer.Serialize(new RefundCommand(id, amount));
                    await _rabbitMqPublisher.SendMessageAsync(refund, Queues.Refund);
                    break;
                case Action.Void:
                    var @void = JsonSerializer.Serialize(new VoidCommand(id));
                    await _rabbitMqPublisher.SendMessageAsync(@void, Queues.Void);
                    break;
                default:
                    return BadRequest();
            }

            return Accepted();
        }
    }
}