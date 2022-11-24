using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Appnetwork2022.Models;


namespace Appnetwork2022.Controllers{
    
    public class AdvertController : Controller{
        personalAcctDetails logUserDet;
        // GET: Advert

        public bool ThumbnailCallback() {
            return true;
        }

        private void GetThumbnail() {
            Image.GetThumbnailImageAbort callback =
                new Image.GetThumbnailImageAbort(ThumbnailCallback);
            Image image = new Bitmap(@"c:\FakePhoto.jpg");
            Image pThumbnail = image.GetThumbnailImage(100, 100, callback, new IntPtr());
            //image.Save();
            //e.Graphics.DrawImage(
            //   pThumbnail,
            //   10,
            //   10,
            //   pThumbnail.Width,
            //   pThumbnail.Height);
        }




        public ActionResult Index(){
            if (Session["logUid"] == null) return RedirectToRoute("access");
            else {
                logUserDet = new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byUid, Session["logUid"].ToString());
                ViewBag.pHoda = logUserDet;
                ViewBag.walletBallence = new shareholderfunctions().getWalletBallance(Session["logUid"].ToString());
                //if (!string.IsNullOrEmpty(urlVal)) ViewBag.inMsg = new utilityFunct().codeViewMsg(urlVal, false);
            }
            return View();
        }
    }
}