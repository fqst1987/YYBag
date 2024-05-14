using Microsoft.AspNetCore.Mvc;
using YYBagProgram.Service;
using static Google.Apis.Requests.BatchRequest;

namespace YYBagProgram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ECPayService _payService;
        private readonly IConfiguration _config;

        public PaymentController(ECPayService payService, IConfiguration config)
        {
            _payService = payService;
            _config = config;
        }
    }
}
