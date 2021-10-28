using System;
using Microsoft.AspNetCore.Mvc;
using Payment.EventSourcing.Messages;

namespace Payment.Gateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthoriseController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] AuthorisationCommand authorisationCommand)
        {
            throw new NotImplementedException();
        }
    }
}