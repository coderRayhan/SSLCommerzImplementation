using Microsoft.AspNetCore.Mvc;
using SSLCommerzImplementation.Models;
using SSLCommerzImplementation.PaymentGateway;
using System.Collections.Specialized;
using System.Diagnostics;

namespace SSLCommerzImplementation.Controllers
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Checkout()
        {
            var productName = "HP Pavilion";
            var price = 85000;

            var baseUrl = Request.Scheme + "://" + Request.Host;

            NameValueCollection postData = new NameValueCollection();

            postData.Add("total_amount", $"{price}");
            postData.Add("tran_id", $"TR43214321");
            postData.Add("success_url", baseUrl + "/home/CheckoutConfirmation");
            postData.Add("fail_url", baseUrl + "/home/CheckoutFail");
            postData.Add("cancel_url", baseUrl + "/home/CheckoutCancel");
            postData.Add("version", "3.00");
            postData.Add("cus_name", "ABC XY");
            postData.Add("cus_email", "abc.xyz@mail.co");
            postData.Add("cus_add1", "Address Line On");
            postData.Add("cus_add2", "Address Line Tw");
            postData.Add("cus_city", "City Nam");
            postData.Add("cus_state", "State Nam");
            postData.Add("cus_postcode", "Post Cod");
            postData.Add("cus_country", "Countr");
            postData.Add("cus_phone", "0111111111");
            postData.Add("cus_fax", "0171111111");
            postData.Add("ship_name", "ABC XY");
            postData.Add("ship_add1", "Address Line On");
            postData.Add("ship_add2", "Address Line Tw");
            postData.Add("ship_city", "City Nam");
            postData.Add("ship_state", "State Nam");
            postData.Add("ship_postcode", "Post Cod");
            postData.Add("ship_country", "Countr");
            postData.Add("value_a", "ref00");
            postData.Add("value_b", "ref00");
            postData.Add("value_c", "ref00");
            postData.Add("value_d", "ref00");
            postData.Add("shipping_method", "NO");
            postData.Add("num_of_item", "1");
            postData.Add("product_name", $"{productName}");
            postData.Add("product_profile", "general");
            postData.Add("product_category", "Demo");

            var storeId = "xyzco67270ea750c75";
            var storePassword = "xyzco67270ea750c75@ssl";
            var isSandboxMood = true;

            SSLCommerzService ssl = new SSLCommerzService(storeId, storePassword, isSandboxMood);

            string response = ssl.InitiateTransaction(postData);
            return await Task.FromResult(Redirect(response));
        }

        public IActionResult CheckoutConfirmation()
        {
            if (!(!string.IsNullOrEmpty(Request.Form["status"]) && Request.Form["status"] == "VALID"))
            {
                ViewBag.SuccessInfo = "There some error while processing your payment. Please try againt.";
                return View();
            }
            string TrxID = Request.Form["tran_id"];

            string amount = "85000";

            string currency = "BDT";

            var storeId = "xyzco67270ea750c75";
            var storePassword = "xyzco67270ea750c75@ssl";

            SSLCommerzService ssl = new SSLCommerzService(storeId, storePassword, true);
            var response = ssl.OrderValidate(TrxID, amount, currency, Request);
            var succesInfo = $"Validation response: {response}";

            ViewBag.SuccessInfo = succesInfo;

            return View();
        }

        public IActionResult CheckoutFail()
        {
            ViewBag.FailInfo = "There some error processing your payment. Please try again";
            return View();
        }

        public IActionResult CheckoutCancel()
        {
            ViewBag.CancelInfo = "Your payment has been cancel";

            return View();
        }
    }
}
