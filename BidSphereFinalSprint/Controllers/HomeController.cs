using BidSphereProject.Models;
using BidSphereProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace BidSphereProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult ContactAjax([FromForm] ContactFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please fill in all required fields correctly." });
            }

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("bidsphereproject@gmail.com"); // authenticated sender
                mail.To.Add("usmanntariq1100@gmail.com"); // receiver
                mail.ReplyToList.Add(new MailAddress(model.Email)); // user reply-to
                mail.Subject = $"Contact Us: {model.Subject}";
                mail.Body =
                    $"First Name: {model.FirstName}\n" +
                    $"Last Name: {model.LastName}\n" +
                    $"Email: {model.Email}\n\n" +
                    $"Message: {model.Message}";

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("bidsphereproject@gmail.com", "bixw mvug vzzx ammc");
                smtp.EnableSsl = true;
                smtp.Send(mail);

                return Json(new { success = true, message = "Your message has been sent successfully!" });
            }
            catch
            {
                return Json(new { success = false, message = "Something went wrong. Please try again." });
            }
        }
        
        public IActionResult Profile()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
