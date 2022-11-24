using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using Appnetwork2022.Models;
using System.Text;

namespace Appnetwork2022.Controllers
{       
    
    public class HomeController : Controller  
    {
        walletDetails detailForWallet;
        string rMsg = "";
        public ActionResult Index(string urlVal) {

              personalAcctDetails persDet=new personalAcctDetails();
            if (Session["logUid"] == null) return RedirectToRoute("access");// RedirectToAction("Index","Access");
            else {
                DbVar dbObj = new DbVar();                
                string pixName = "";
                try { 
                    dbObj.cmdObj = new MySqlCommand("SELECT *  FROM personalacct_tbl WHERE uid=@p1", new MySqlConnection(DbVar.conStr));
                    dbObj.cmdObj.Parameters.AddWithValue("@p1", Session["logUid"].ToString());
                    dbObj.cmdObj.Connection.Open();
                    dbObj.drObj = dbObj.cmdObj.ExecuteReader();
                    if (dbObj.drObj.Read()) { 
                        pixName = dbObj.drObj["img"].ToString();
                       // if (string.IsNullOrEmpty(dbObj.drObj["noknames"].ToString())) {      //not yet registered for Shareholders
                            persDet.sName = dbObj.drObj["sName"].ToString();
                            persDet.oName = dbObj.drObj["OName"].ToString();
                            persDet.pNum = dbObj.drObj["pNum"].ToString();
                            persDet.sex = dbObj.drObj["sex"].ToString();
                            persDet.Dob =Convert.ToDateTime(dbObj.drObj["dob"]);
                            persDet.Occupation = dbObj.drObj["occupation"].ToString();
                            persDet.Religion = dbObj.drObj["religion"].ToString();
                            persDet.Addrs = dbObj.drObj["addrs"].ToString();
                            persDet.Country = dbObj.drObj["country"].ToString();
                            persDet.State = dbObj.drObj["state"].ToString();
                            persDet.City = dbObj.drObj["city"].ToString();
                            persDet.referralCode = dbObj.drObj["referralCode"].ToString();
                            persDet.eMail = dbObj.drObj["email"].ToString();
                            persDet.acctNumber = dbObj.drObj["acctNumber"].ToString();
                            persDet.acctName = dbObj.drObj["acctName"].ToString();
                            persDet.bankName = dbObj.drObj["bankName"].ToString();
                            persDet.pixName= dbObj.drObj["img"].ToString();
                            persDet.isShareholder = new utilityFunct().safeReadAllType<bool>(dbObj.drObj["isShareholder"]);     //Convert.ToBoolean( dbObj.drObj["isShareholder"]);// new utilityFunct().safeReadAllType<bool>(dbObj.drObj["isShareholder"]);//!=DBNull.Value && (bool)dbObj.drObj["isShareholder"];
                           // ViewBag.pHoda = persDet;
                        //}else ViewBag.pHoda = null;
                    }
                } catch (Exception) {
                    persDet.pixName = "Avatar.png";
                } finally {
                    if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                    dbObj = null;
                }
                ViewBag.pHoda = persDet;
                if (urlVal != null) ViewBag.msg = new utilityFunct().codeViewMsg(urlVal, false); else ViewBag.msg = null;
                return View();
            }         
            
        }

        public ActionResult Menuroute(string urlVal) {
            if (Session["logUid"] == null) return RedirectToRoute("access");// RedirectToAction("Index","Access");
            else if(!string.IsNullOrEmpty(urlVal)){
                if ( urlVal.Equals("UChat", StringComparison.OrdinalIgnoreCase)) {
                    Session["curChat"] = urlVal;
                    return RedirectToAction("Chat", "Chat");// RedirectToAction("Chat", "Chat", new { urlVal = urlVal });
                }    
            }
            //RedirectToAction("Chat", "Chat", new { urlVal = urlVal });
            //return View("~/Views/Chat/Index",)
            return View();
        }

        public ActionResult Logout() {
            Session.Clear();
            Session.Abandon();           
            return RedirectToAction("index", "Access");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult shareholder(personalAcctDetails pShareHolda) {
            decimal walletBallance = new shareholderfunctions().getWalletBallance(Session["logUid"].ToString());
            if (walletBallance > allConstant.shareHolderSubFee) { //check if the User has enough ballance 
                DbVar dbObj = new DbVar();
                try {   //update User accout
                    dbObj.cmdObj = new MySqlCommand("UPDATE personalacct_tbl SET noknames=@noknames,nokemail=@nokemail,nokcountry=@nokcountry,nokphone=@nokphone,nokaddress=@nokaddress,isShareholder=@isShareholder WHERE uid=@uid", new MySqlConnection(DbVar.conStr));
                    dbObj.cmdObj.Parameters.AddWithValue("@noknames", pShareHolda.nokNames);
                    dbObj.cmdObj.Parameters.AddWithValue("@nokcountry", pShareHolda.nokCountry);
                    dbObj.cmdObj.Parameters.AddWithValue("@nokaddress", pShareHolda.nokAddress);
                    dbObj.cmdObj.Parameters.AddWithValue("@nokphone", pShareHolda.nokPhone);
                    dbObj.cmdObj.Parameters.AddWithValue("@isShareholder", true);
                    dbObj.cmdObj.Parameters.AddWithValue("@nokemail", pShareHolda.nokEmail);
                    dbObj.cmdObj.Parameters.AddWithValue("@shareholderRegdate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    dbObj.cmdObj.Parameters.AddWithValue("@uid", Session["logUid"].ToString());
                    dbObj.cmdObj.Connection.Open();
                    if (dbObj.cmdObj.ExecuteNonQuery() > 0) { //then register the User as legimate shareholder
                        dbObj.cmdObj.Dispose();
                        dbObj.cmdObj = new MySqlCommand("INSERT INTO shareholders_tbl(rowId,uid,InvestAmount,InvestDate)VALUES(@rowId,@uid,@InvestAmount,@InvestDate)", new MySqlConnection(DbVar.conStr));
                        dbObj.cmdObj.Parameters.AddWithValue("@rowId", new utilityFunct().GenUIN());
                        dbObj.cmdObj.Parameters.AddWithValue("@uid", Session["logUid"].ToString());
                        dbObj.cmdObj.Parameters.AddWithValue("@InvestAmount", allConstant.shareHolderSubFee.ToString());
                        dbObj.cmdObj.Parameters.AddWithValue("@InvestDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        dbObj.cmdObj.Connection.Open();
                        if (dbObj.cmdObj.ExecuteNonQuery() > 0) {   //report the deduction to the User wallet account
                            detailForWallet = new walletDetails {
                                rowId = new utilityFunct().GenUIN(),
                                uid = Session["logUid"].ToString(),
                                PayAmount = allConstant.shareHolderSubFee,
                                Paydate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                isCredit = false,
                                payOutSource = AmountSource.forSubscription,
                                Paydetails = ("for shareholder's subscription"),
                                isConfirmed = true,
                                isWalletToWallet = false
                            };
                            if (new shareholderfunctions().pushAmntIntoWallet(detailForWallet)) {
                                new shareholderfunctions().prorateSharesForRefral(Session["logUid"].ToString(), allConstant.shareHolderSubFee, 2);
                                rMsg = "Congratulation! You have been successfully registered as a Shareholder on Appnetwork Limited";
                            } else rMsg = "Critical internal event trigered! Please report to the Admin quicky with this code to stop system from deleting your account with Appnetwork: AVe001";
                        }else rMsg = "Unable to register your Shareholders Account! please report the Admin with this code: RSHe03";
                    } else rMsg = "Unable to update your record! Please report the Admin with this code: RSHe01";
                } catch (Exception) {
                    rMsg = "This proccess was terminated for vital reasons! Please screenshot this code for report to Admin: RSHe02";
                } finally {
                    if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                    dbObj = null;
                }
            } else rMsg = "You don't have enuough ballance in your wallet to subcribe for Shareholder";
            //ViewBag.msg = new utilityFunct().codeViewMsg("Congratulation! You have been successfully registered as a Shareholder on Appnetwork Limited", true);
            return  RedirectToAction("Index", new { urlVal = new utilityFunct().codeViewMsg(rMsg, true) });
        }

        //[ChildActionOnly]
        //public ActionResult buildPayment()
        //{
        //    DbVar dbObj = new DbVar();
        //    List<string> bPay = new List<string>();
        //    try {
        //        dbObj.cmdObj = new MySqlCommand("SELECT OName,email  FROM personalacct_tbl WHERE uid=@p1", new MySqlConnection(DbVar.conStr));
        //        dbObj.cmdObj.Parameters.AddWithValue("@p1", Session["logUid"] .ToString());
        //        dbObj.cmdObj.Connection.Open();
        //        dbObj.drObj = dbObj.cmdObj.ExecuteReader();
        //        if (dbObj.drObj.Read()) {                    
        //            bPay.Add(Convert.ToBase64String(Encoding.ASCII.GetBytes(new utilityFunct().genUIN(5) + "-" + Session["logUid"].ToString() )));
        //            bPay.Add(dbObj.drObj.GetString("OName"));
        //            bPay.Add(dbObj.drObj.GetString("email"));                    
        //         } 

        //    } catch (Exception) {

        //    } finally {
        //        if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
        //        dbObj = null;
        //    }
        //     return PartialView(bPay);
        //}
    }
}