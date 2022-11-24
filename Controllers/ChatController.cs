using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Appnetwork2022.Models;
namespace Appnetwork2022.Controllers
{
    public class ChatController : Controller{
        // GET: Chat
        public ActionResult Index(string urlVal) {
            if (Session["logUid"] == null) return RedirectToRoute("access");
            else {
                if (urlVal.StartsWith("Universal")) {
                    chatHistory cHistory = new chatHistory();
                    string trackRefralCode = new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byUid, Session["logUid"].ToString())?.referralCode ?? null;
                    if (!string.IsNullOrEmpty(trackRefralCode)) {
                        //int chatTracknum = new chatFunctions().trackLastChat(trackRefralCode, chatTypes.Universal);
                        //if (chatTracknum > 0) cHistory.chatDetailList = new chatFunctions().getMissedChat(Session["logUid"].ToString(), chatTracknum, chatTypes.Universal);
                    }
                    ViewBag.chatUsers = cHistory.chatDetailList;    // to resolve this later
                }
                //Request.QueryString.Remove(urlVal);
                return View();
            }
        }   

        public ActionResult Chat() {
            if (Session["logUid"] == null) return RedirectToRoute("access");
            else if (Session["curChat"] == null) return RedirectToRoute("home");
            else {
                personalAcctDetails pDetail = new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byUid, Session["logUid"].ToString());
                ViewBag.cDat = pDetail;
                ViewBag.refchatData = new utilityFunct().codeViewMsg(pDetail.referenceCode + "`" + Session["curChat"].ToString(), true);
                return View();
            }
          
        }
    }
}