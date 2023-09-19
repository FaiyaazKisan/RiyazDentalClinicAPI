using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using RiyazDentalClinicAPI.Models;
using System.Net;

namespace RiyazDentalClinicAPI.Controllers
{
    [Route("api/EmailController")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private ApiResponse _response;
        public EmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _response = new ApiResponse();
        }
        [HttpPost]
        public async Task<IActionResult> PostEmail(string htmlMessage)
        {
           await _emailSender.SendEmailAsync("fmkisan@gmail.com", "Enquiry Form RDC", htmlMessage);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }
    }
}
