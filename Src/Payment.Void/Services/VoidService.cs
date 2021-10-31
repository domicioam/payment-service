using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Payment.Capture.Repository;

namespace Payment.Void.Services
{
    public class VoidService
    {
        private readonly TransactionRepository _transactionRepository;
        private readonly ILogger<VoidService> _logger;
    }
}
