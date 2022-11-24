using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Appnetwork2022.Controllers
{
    public class FluterwaveRedirectController : Controller
    {
        // GET: FluterwaveRedirect
      
        public ActionResult handleRedirect(string status, string tx_ref)
        {
            string inreq = Request.RawUrl;
            if (status == "successful") {
                string tID = Request.QueryString["transaction_id"];
                var VerifyResponse = new Payment("FLWSECK-00dcc6eeb2ea770d34fa5704947b9453-X").VerifyTransaction(tID);
                string vf = VerifyResponse.Result;
                if (string.IsNullOrEmpty(VerifyResponse.ToString())) ViewBag.resp = "Nothing Response";
                else ViewBag.resp = vf.ToString();
            } else ViewBag.resp = "Not successful";
            return View();
        }
    }
}
