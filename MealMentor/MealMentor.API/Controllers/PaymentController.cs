using System.Security.Claims;
using MealMentor.API.Services.PayOSService;
using MealMentor.Core.DTOs.Payment;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;

namespace MealMentor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PayOS _payOS;
        private readonly IPayOSService _payOSService;

        public PaymentController(PayOS payOS, IPayOSService payOSService)
        {
            _payOS = payOS;
            _payOSService = payOSService;
        }

        [HttpPost("create-link")]
        public async Task<IActionResult> CreatePaymentLink()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized("Invalid userId");
            }
            try
            {
                var paymentLink = await _payOSService.CreatePaymentLink(userId);
                return Ok(new Response(0, "success", paymentLink));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder([FromRoute] int orderId)
        {
            try
            {
                PaymentLinkInformation paymentLinkInformation = await _payOS.getPaymentLinkInformation(orderId);
                return Ok(new Response(0, "Ok", paymentLinkInformation));
            }
            catch (System.Exception exception)
            {

                Console.WriteLine(exception);
                return Ok(new Response(-1, "fail", null));
            }

        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> CancelOrder([FromRoute] int orderId)
        {
            try
            {
                PaymentLinkInformation paymentLinkInformation = await _payOS.cancelPaymentLink(orderId);
                return Ok(new Response(0, "Ok", paymentLinkInformation));
            }
            catch (System.Exception exception)
            {

                Console.WriteLine(exception);
                return Ok(new Response(-1, "fail", null));
            }
        }

        [HttpPost("payos_transfer_handler")]
        public async Task<IActionResult> payOSTransferHandler(WebhookType body)
        {
            try
            {
                WebhookData data = _payOS.verifyPaymentWebhookData(body);


                if (data.code == "00")
                {
                    var result = await _payOSService.ProcessPayment(data.orderCode);
                    return Ok(new Response(0, "Ok", result));
                }
                return Ok(new Response(0, "Ok", null));
            }
            catch (Exception e)
            {

                return Ok(new Response(-1, "fail", null));
            }
        }
    }
}
