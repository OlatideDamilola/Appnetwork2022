using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Appnetwork2022.Models;
using MySql.Data.MySqlClient;
using PayStack.Net;

namespace Appnetwork2022.Controllers
{
    public class DashboardController : Controller {

        personalAcctDetails logUserDet;
        private static PayStackApi _api = new PayStackApi("sk_test_4d565d7c4e00fc223282a2d828b793d4736e6768");
        string rMsg = string.Empty;
        walletDetails detailForWallet;
        string GetTransferCode(string v) {
            string r = string.Empty; string[] j = v.Split('_');
            for (int i = 0; i < j.Length; i++) {
                if (j[i].Contains("TRF")) {
                    r = j[i + 1].Substring(0, j[i + 1].IndexOf('"'));
                    r = "TRF_" + r;
                    break;
                }
            }
            return (r);
        }


        //private void doIt(decimal vq) {
            
        //        try {
        //            decimal coFund = vq; // decimal.Parse(coFIpt);
        //            decimal wallBal = 0M;
        //            personalAcctDetails casherDetails = new utilityFunct().FetchUserByUidorReferral(true, Session["logUid"].ToString());
        //            if (coFund >= allConstant.leastTransaction) {
        //                wallBal = new shareholderfunctions().getWalletBallance(Session["logUid"].ToString());
        //                if ((wallBal * allConstant.ANWdollarRate) >= coFund) {
        //                    var recipResponse = _api.Transfers.Recipients.Create(casherDetails.oName, casherDetails.acctNumber, casherDetails.bankName, "NGN", "Cash-out", "nuban");
        //                    if (recipResponse.Status) {
        //                        var transResponse = _api.Transfers.InitiateTransfer((int)((coFund - new utilityFunct().GetpercentOf(coFund, 5)) * 100), recipResponse.Data.RecipientCode, "balance", "NGN", "Cash-out");
        //                        if (transResponse.Status) {
        //                            string transferCode = GetTransferCode(transResponse.RawJson);
        //                            if (!string.IsNullOrEmpty(transferCode)) {
        //                                detailForWallet = new walletDetails {
        //                                    isConfirmed = false,
        //                                    isCredit = false,
        //                                    isWalletToWallet = false,
        //                                    payOutSource = AmountSource.forCashout,
        //                                    PayAmount = (coFund / allConstant.ANWdollarRate),
        //                                    Paydate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //                                    Paydetails = "Pending",
        //                                    rowId = transferCode,
        //                                    uid = Session["logUid"].ToString()
        //                                };
        //                                if (new shareholderfunctions().pushAmntIntoWallet(detailForWallet)) {
        //                                    var veriResponse = _api.Transfers.FetchTransfer(transferCode);
        //                                    if (veriResponse.Status) {
        //                                        if (veriResponse.Data.Status.Contains("success")) {
        //                                            if (new shareholderfunctions().DoWalletConfirm(true, transferCode)) rMsg = "Your Cash-out process was completed and wallet record updated.";
        //                                            else rMsg = "Your Cash-out process was completed but unable to update your wallet record! Please screenshot this code for report to Admin: " + transferCode;
        //                                        } else rMsg = "Your Cash-out process was completed with a " + veriResponse.Data.Status + "status.";
        //                                    } else rMsg = "Unable to verify your Cash-out to bank transfer! The system will keep trying.";
        //                                } else rMsg = "Unable to record your Cash-out process! Please screenshot this code for report to Admin: " + transferCode;
        //                            } else rMsg = "Unable to process your Cash-out! Please screenshot this code for report to Admin: COe2";
        //                        } else rMsg = "Unable to process your Cash-out! Please screenshot this code for report to Admin: COe1";
        //                    } else rMsg = "Your account details are invalid! Please check your Bank Account details in your profile";
        //                } else rMsg = "You can not proceed with this Cash-out! You don't have enough ballance in your wallet";
        //            } else rMsg = "You can not cashout an amount that is less than #600";
        //        } catch { rMsg = "This proccess was terminated for vital reasons! Please screenshot this code for report to Admin: COe3"; }
          

        //}


        //private List<walletDetails> GetChashoutStatus(bool forSingle,string transFaCode,string dUid =null) {            
        //    List<walletDetails> rList = new List<walletDetails>();
        //    try {
        //        DbVar dbObj = new DbVar();
        //        dbObj.cmdObj = new MySqlCommand("SELECT* FROM wallethistory_tbl WHERE uid=@uid AND isConfirmed=FALSE AND payOutSource=@payOutSource", new MySqlConnection(DbVar.conStr));
        //        dbObj.cmdObj.Parameters.AddWithValue("@uid", dUid);
        //        dbObj.cmdObj.Parameters.AddWithValue("@payOutSource", AmountSource.forCashout);
        //        dbObj.cmdObj.Connection.Open();
        //        dbObj.drObj = dbObj.cmdObj.ExecuteReader();
        //        while (dbObj.drObj.Read()) {
        //            rList.Add(new walletDetails {
        //                PayAmount = new utilityFunct().safeReadAllType<decimal>(dbObj.drObj["PayAmount"]),
        //                Paydate = new utilityFunct().safeReadAllType<string>(dbObj.drObj["Paydate"]),
        //                rowId = new utilityFunct().safeReadAllType<string>(dbObj.drObj["rowId"])
        //            });
        //        }
        //        if (rList.Count > 0) {
        //            foreach (walletDetails rL in rList) {
        //                var fTransResp = _api.Transfers.FetchTransfer(rL.rowId);
        //                if (fTransResp.Status && fTransResp.Data!=null) {
        //                    if (fTransResp.Data.Status.Contains("success")) {
        //                        //remove this from wallet
        //                    }rL.Paydetails = fTransResp.Data.Status;
        //                }

        //            }

        //        }
        //        } catch { }
        //    return rList;
        //}
        // personalAcctTemplate pAcctTemplate=new personalAcctTemplate();
        private TransactionInitializeResponse InitializePaymentRequest(int nairaAmnt, string eMail, string refCod, string Payer, string payDetail, string veriCallBack) {
            var request = new TransactionInitializeRequest {
                AmountInKobo = nairaAmnt * 100,  // 900000,
                Email = eMail,
                Reference = refCod, // or your custom reference
                CallbackUrl = "http://appnetworkltd.com/dashboard/" + "VeriFundCallBack"              
            };
            request.CustomFields.Add(CustomField.From("Name", "name", Payer));  // Add customer fields          
            request.MetadataObject["DataKey"] = payDetail;  // Add other metadata
            var response = _api.Transactions.Initialize(request);
            return response;
        }
        public ActionResult Index() {
            if (Session["logUid"] == null) return RedirectToRoute("access");
            logUserDet = new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byUid, Session["logUid"].ToString());
            ViewBag.vData = logUserDet;
            //else { }
            //logUserDet = new utilityFunct().FetchUserById(Session["logUid"].ToString());
            return View();
        }

     


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashoutFund(decimal coFund) {
            if (Session["logUid"] == null) return RedirectToRoute("access");
            else {
                try {  
                    // coFund =Convert.ToInt32(form["coFIpt"]); // decimal.Parse(coFIpt);
                    decimal wallBal = 0M;
                    personalAcctDetails casherDetails = new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byUid, Session["logUid"].ToString());
                    if (coFund >= allConstant.leastTransaction) {
                        wallBal = new shareholderfunctions().getWalletBallance(Session["logUid"].ToString());
                        if ((wallBal * allConstant.ANWdollarRate) >= coFund) {
                            var recipResponse = _api.Transfers.Recipients.Create(casherDetails.oName, casherDetails.acctNumber, casherDetails.bankName, "NGN", "Cash-out", "nuban");
                            if (recipResponse.Status) {
                                var transResponse = _api.Transfers.InitiateTransfer((int)((coFund - new utilityFunct().GetpercentOf(coFund, 5)) * 100), recipResponse.Data.RecipientCode, "balance", "NGN", "Cash-out");
                                if (transResponse.Status) {
                                    string transferCode = GetTransferCode(transResponse.RawJson);
                                    if (!string.IsNullOrEmpty(transferCode)) {
                                        detailForWallet = new walletDetails {
                                            isConfirmed = true,
                                            isCredit = false,
                                            isWalletToWallet = false,
                                            payOutSource = AmountSource.forCashout,
                                            PayAmount = (coFund / allConstant.ANWdollarRate),
                                            Paydate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                            Paydetails = "Pending",
                                            rowId = transferCode,
                                            uid = Session["logUid"].ToString()
                                        };
                                        if (new shareholderfunctions().pushAmntIntoWallet(detailForWallet)) {
                                            rMsg = "Your Cash-out process was completed and wallet record updated.";
                                            //var veriResponse = _api.Transfers.FetchTransfer(transferCode);
                                            //if (veriResponse.Status) {
                                            //    if (veriResponse.Data.Status.Contains("success")) {
                                            //        if (new utilityFunct().DoWalletConfirm(true, transferCode)) rMsg = "Your Cash-out process was completed and wallet record updated.";
                                            //        else rMsg = "Your Cash-out process was completed but unable to update your wallet record! Please screenshot this code for report to Admin: " + transferCode;
                                            //    } else rMsg = "Your Cash-out process was completed with a " + veriResponse.Data.Status + "status.";
                                            //} else rMsg = "Unable to verify your Cash-out to bank transfer! The system will keep trying.";
                                        } else rMsg = "Unable to record your Cash-out process! Please screenshot this code for report to Admin: " + transferCode;
                                    } else rMsg = "Unable to process your Cash-out! Please screenshot this code for report to Admin: COe2";
                                } else rMsg = "Unable to process your Cash-out! Please screenshot this code for report to Admin: COe1";
                            } else rMsg = "Your account details are invalid! Please check your Bank Account details in your profile";
                        } else rMsg = "You can not proceed with this Cash-out! You don't have enough ballance in your wallet";
                    } else rMsg = "You can not cashout an amount that is less than #600";
                } catch { rMsg = "This proccess was terminated for vital reasons! Please screenshot this code for report to Admin: COe3"; }
            }
            return RedirectToAction("Wallet", new { urlVal = new utilityFunct().codeViewMsg(rMsg, true) });
        }


        public ActionResult Wallet(string urlVal) {
            if (Session["logUid"] == null) return RedirectToRoute("access");
            else {
                logUserDet = new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byUid, Session["logUid"].ToString());
                ViewBag.vData = logUserDet;
                ViewBag.walletBallence = new shareholderfunctions().getWalletBallance(Session["logUid"].ToString());
                if (!string.IsNullOrEmpty(urlVal)) ViewBag.inMsg = new utilityFunct().codeViewMsg(urlVal, false);
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WWTransfer(string wwRefrenceCode, decimal wwAmnt) {

            if (Session["logUid"] == null) return RedirectToRoute("access");
            else {
                if (!string.IsNullOrEmpty(wwRefrenceCode)) {
                    try {
                        personalAcctDetails pDtails = new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byReference, wwRefrenceCode);
                        if (pDtails != null) {
                            decimal walBal = new shareholderfunctions().getWalletBallance(Session["logUid"].ToString());
                            if ((walBal * allConstant.ANWdollarRate) >= wwAmnt) {
                                walletDetails wD = new walletDetails {
                                    isConfirmed = true,
                                    isCredit = true,
                                    isWalletToWallet = true,
                                    PayAmount = wwAmnt / allConstant.ANWdollarRate,
                                    Paydate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Paydetails = "Transfer from " + Session["logUid"].ToString() + " User to your Wallet.",
                                    payOutSource = AmountSource.forTransfer,
                                    rowId = new utilityFunct().GenUIN(),
                                    uid = pDtails.Uid
                                };
                                if (new shareholderfunctions().pushAmntIntoWallet(wD)){
                                    wD = new walletDetails {
                                        isConfirmed = true,
                                        isCredit = false,
                                        isWalletToWallet = true,
                                        PayAmount = wwAmnt / allConstant.ANWdollarRate,
                                        Paydate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        Paydetails = "Transfer from your wallet to " + wwRefrenceCode + " User.",
                                        payOutSource = AmountSource.forTransfer,
                                        uid = Session["logUid"].ToString(),
                                        rowId = new utilityFunct().GenUIN()
                                    };
                                    if (new shareholderfunctions().pushAmntIntoWallet(wD) ){
                                        rMsg = "Your Transfer to " + wwRefrenceCode + " User's Wallet was successfull!";
                                    }else rMsg = "Unable to your Transfer Records! Please screenshot this code for report to Admin: WWTe02-";
                                } else rMsg = "Unable to update Transfer Records for your Benefiary! Please screenshot this code for report to Admin: WWTe01"; 
                            } else rMsg = "You do not have suffient ballance in your wallet!";
                        } else rMsg = "The Referal code you entered does not exist! Please chack and retry.";
                         } catch { rMsg = "This proccess was terminated for vital reasons! Please screenshot this code for report to Admin: WWTe03"; }
                }
                return RedirectToAction("Wallet", new { urlVal = new utilityFunct().codeViewMsg(rMsg, true) });   
            }
        }
    
    

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Fundwallet(int iptFW) {
            if (Session["logUid"] == null) return RedirectToRoute("access");
            else {
                rMsg = String.Empty;
                try { 
                    personalAcctDetails fMover = new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byUid, Session["logUid"].ToString());                   

                    if (fMover != null && (iptFW >= allConstant.leastTransaction)) {
                        string retRefCod = new utilityFunct().codeViewMsg(fMover.Uid + "." + new utilityFunct().GenUIN() + "." + iptFW, true);
                        var tResponse = InitializePaymentRequest(iptFW, fMover.eMail, retRefCod, fMover.sName + " " + fMover.oName, "Bank to wallet fund", "VeriFund");
                        if (tResponse != null && tResponse.Status) {
                            detailForWallet = new walletDetails {
                                rowId = retRefCod,
                                uid = Session["logUid"].ToString(),
                                PayAmount = (iptFW / allConstant.ANWdollarRate),
                                Paydate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                isCredit = true,
                                payOutSource = AmountSource.fromFunding,
                                Paydetails = "Bank to wallet fund",
                                isConfirmed = false,
                                isWalletToWallet = false
                            };
                            if (new shareholderfunctions().pushAmntIntoWallet(detailForWallet)) {
                                Response.Redirect(tResponse.Data.AuthorizationUrl, true);
                            } else rMsg = "Unable to record your transfer! Please report to the Admin with this Code:" + tResponse.Data.AccessCode.ToUpper();
                        } else if (tResponse.Message.Contains("email")) rMsg = "Your transfer is not successfull! Ensure you have a valid Profile Email Address.";
                        else rMsg = "Your transfer is not successfull!, Please contact the Admin.";
                    }else rMsg = "Unable to fetch your Records! Please report to the Admin";
                } catch { rMsg = "This proccess was terminated for vital reasons! Please screenshot this code for report to Admin: FWe1"; }

                return RedirectToAction("Wallet", new { urlVal = new utilityFunct().codeViewMsg(rMsg, true) });   //TODO: catch this in wallet method

            }
        }

        public ActionResult VeriFundCallBack(string reference){
            try {
                rMsg=String.Empty;
                if (!string.IsNullOrEmpty(reference)) {
                    string dRef=new utilityFunct().codeViewMsg(reference,false);
                    var response = _api.Transactions.Verify(reference);    //(dRef.Split('-')[1]);  //REM:UID-refcod-Amnt
                    if (response!= null) {
                        if (response.Status && (response.Data.Amount/100).ToString()==dRef.Split('.')[2]) {
                            if (new shareholderfunctions().DoWalletConfirm(true, reference)){
                                rMsg = "You have successfully tranfered a fund into your wallet!";
                            }else rMsg = "Unabel to register your transfer! Please screenshot this code for report to Admin:" + reference;
                        }else rMsg = "Your transfer is not successfull!, Please contact the Admin.";
                    } else rMsg = "Your transfer is not successfull!, Please contact the Admin.";                    
                }
            } catch { rMsg = "This proccess was terminated for vital reasons! Please screenshot this code for report to Admin: VFe1"; }
            return RedirectToAction("Wallet", new { urlVal = new utilityFunct().codeViewMsg(rMsg, true) });   //TODO: catch this in wallet method
        }

        public ActionResult Advert() {
            if (Session["logUid"] == null) return RedirectToRoute("access");
            else {
                logUserDet = new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byUid, Session["logUid"].ToString());
                ViewBag.vData = logUserDet;
                ViewBag.walletBallence = new shareholderfunctions().getWalletBallance(Session["logUid"].ToString());
                //if (!string.IsNullOrEmpty(urlVal)) ViewBag.inMsg = new utilityFunct().codeViewMsg(urlVal, false);
            }
            return View();
        }
        

    }
}