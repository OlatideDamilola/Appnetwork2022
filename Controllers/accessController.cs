using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using Appnetwork2022.Models;
using System.Text;
using System.IO;


namespace Appnetwork2022.Controllers {
  
    public class accessController : Controller
    {
        //public static readonly string[] allCountries = { "Value1", "Value2", "Value3" };
        string dMsg = "";
        string genStr = "";
        private string veriAccess(personalAcctDetails acctDet) {
            DbVar dbObj = new DbVar();
            string foundUID = null;
            try {
                dbObj.cmdObj = new MySqlCommand("SELECT uid,pword  FROM personalacct_tbl WHERE pNum=@p1 AND pword=@p2", new MySqlConnection(DbVar.conStr));
                dbObj.cmdObj.Parameters.AddWithValue("@p1", acctDet.pNum);
                dbObj.cmdObj.Parameters.AddWithValue("@p2", acctDet.Pword);
                dbObj.cmdObj.Connection.Open();
                dbObj.drObj = dbObj.cmdObj.ExecuteReader();
                if (dbObj.drObj.Read()&& dbObj.drObj["pword"].ToString().Equals(acctDet.Pword)) {                   
                    foundUID = dbObj.drObj["uid"].ToString(); 
                } else {
                    dMsg = "Your login credentials is invalid";
                    foundUID = null;
                }
            } catch (HttpRequestValidationException) {
                dMsg = "Invalid Input";
                foundUID = null;
            } catch (Exception e) {
                dMsg = "Unable to sign you in,Please report to the Admin with this code:va01";
                foundUID = null;
            } finally {
                if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                dbObj = null;
            }
            return foundUID;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(personalAcctDetails pLogin) {
            genStr = veriAccess(pLogin);
            if (genStr == null) return RedirectToAction("Index", new { urlVal = new utilityFunct().codeViewMsg(dMsg,true)});
            else {
                Session["LogUid"] = genStr;
                return RedirectToRoute("home");  // RedirectToAction("Index", "Home");
               }
        }

        public ActionResult Invite(string urlVal) {
            if (!string.IsNullOrWhiteSpace(urlVal)) {
                ViewBag.inVrefCod = urlVal.Trim();
                return View("Index");
            }else return View("Index");
        }


        public ActionResult DirAuth(string urlVal) {
            personalAcctDetails pAcct=new personalAcctDetails();
            pAcct.pNum = urlVal.Split('`')[0];
            pAcct.Pword = urlVal.Split('`')[1];
            genStr = veriAccess(pAcct);
            if (genStr==null)return RedirectToAction("index");
            else {
                Session["LogUid"] = genStr;
                return RedirectToAction("Index", "Home");
            } 
        }

        public ActionResult Redirector(string urlVal)
        {
            
            return View();
        }
        
        // GET: access
        public ActionResult Index(string urlVal){
          if(urlVal !=null) ViewBag.msg =new utilityFunct().codeViewMsg(urlVal,false);

            //List<bankStruct> AllBanks = new List<bankStruct>();

            
            //PayStack.Net.PayStackApi PayApi=new PayStack.Net.PayStackApi("sk_test_4d565d7c4e00fc223282a2d828b793d4736e6768");
            //var resp= PayApi.Miscellaneous.ListBanks();
            //foreach (var b in resp.Data) AllBanks.Add(new bankStruct { code = b.Code, name = b.Name });
            //ViewBag.Banks=AllBanks;
                return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePersonalAcct(personalAcctDetails pAcct)
        {
                  DbVar dbObj = new DbVar();
             try {
                    dbObj.cmdObj = new MySqlCommand("SELECT * FROM personalacct_tbl WHERE pNum=@p1 OR pword=@p2", new MySqlConnection(DbVar.conStr));
                    dbObj.cmdObj.Parameters.AddWithValue("@p1", pAcct.pNum);
                    dbObj.cmdObj.Parameters.AddWithValue("@p2", pAcct.Pword);
                    dbObj.cmdObj.Connection.Open();
                    dbObj.drObj = dbObj.cmdObj.ExecuteReader();
               
                    if (dbObj.drObj.Read()){
                        if (dbObj.drObj["pNum"] != null) dMsg = "This phone number: " + dbObj.drObj["pNum"].ToString() + " is already in use";
                    }else{
                        dbObj.cmdObj.Connection.Close();
                        dbObj=null;
                        dbObj = new DbVar();
                        dbObj.cmdObj = new MySqlCommand("INSERT INTO personalacct_tbl(rowid,uid, pword, img, referralCode, sName, oName, pNum, email, sex, dob, occupation, religion, addrs,city,state,country,referenceCode,acctNumber,acctName,bankName)VALUES(@rowid,@uid, @pword, @img, @referralCode, @sName, @oName, @pNum, @email, @sex,@dob, @occupation, @religion, @addrs,@state,@city, @country,@referenceCode,@acctNumber,@acctName,@bankName)", new MySqlConnection(DbVar.conStr));
                        dbObj.cmdObj.Parameters.AddWithValue("@rowid", new utilityFunct().GenUIN());
                        dbObj.cmdObj.Parameters.AddWithValue("@uid", new utilityFunct().GenUIN());
                        dbObj.cmdObj.Parameters.AddWithValue("@pword", pAcct.Pword);

                        if(pAcct.fotoFile!=null &&  pAcct.fotoFile.ContentLength > 0) {
                            string fN = new utilityFunct().GenUIN() + pAcct.fotoFile.FileName.Remove(0, pAcct.fotoFile.FileName.LastIndexOf("."));
                            if (new utilityFunct().doPostedFile(utilityFunct.what2Do.saveIt, fN, "Images", pAcct.fotoFile)) 
                                dbObj.cmdObj.Parameters.AddWithValue("@img", fN);
                            else dbObj.cmdObj.Parameters.AddWithValue("@img", "avatar.png");
                        } else dbObj.cmdObj.Parameters.AddWithValue("@img", "avatar.png");
                   
                        dbObj.cmdObj.Parameters.AddWithValue("@referralCode", string.IsNullOrEmpty(pAcct.referralCode)? allConstant.baseReferenceCode: pAcct.referralCode.ToUpper());
                        dbObj.cmdObj.Parameters.AddWithValue("@sName", pAcct.sName);
                        dbObj.cmdObj.Parameters.AddWithValue("@oName", pAcct.oName);
                        dbObj.cmdObj.Parameters.AddWithValue("@pNum", pAcct.pNum);
                        dbObj.cmdObj.Parameters.AddWithValue("@email", pAcct.eMail);
                        dbObj.cmdObj.Parameters.AddWithValue("@sex", pAcct.sex);
                        dbObj.cmdObj.Parameters.AddWithValue("@dob", pAcct.Dob);
                        dbObj.cmdObj.Parameters.AddWithValue("@occupation", pAcct.Occupation);
                        dbObj.cmdObj.Parameters.AddWithValue("@religion", pAcct.Religion);
                        dbObj.cmdObj.Parameters.AddWithValue("@addrs", pAcct.Addrs);
                        dbObj.cmdObj.Parameters.AddWithValue("@state", pAcct.State);
                        dbObj.cmdObj.Parameters.AddWithValue("@city", pAcct.City);
                        dbObj.cmdObj.Parameters.AddWithValue("@country", pAcct.Country); 
                        dbObj.cmdObj.Parameters.AddWithValue("@referenceCode", new utilityFunct().GenUIN());
                        dbObj.cmdObj.Parameters.AddWithValue("@acctNumber", pAcct.acctNumber);
                        dbObj.cmdObj.Parameters.AddWithValue("@acctName", pAcct.acctName);
                        dbObj.cmdObj.Parameters.AddWithValue("@bankName", pAcct.bankName);
                        dbObj.cmdObj.Parameters.AddWithValue("@regDate",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        dbObj.cmdObj.Connection.Open();
                        dbObj.drObj = dbObj.cmdObj.ExecuteReader();
                        dMsg = "Your registration is successful! You can now sign-in with your Username and Password";
                }

             } catch (HttpRequestValidationException) {
                dMsg = "Invalid Input";
             } catch (Exception){
                    dMsg = "Internal Error; Please try again later";
             }finally{
                 if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                 dbObj = null;
              }
            return RedirectToAction("Index", new {urlVal =new utilityFunct().codeViewMsg(dMsg,true)});
        }

        //public string[] ReadContent(string urlVal) {
        //    StreamReader file = new StreamReader(Server.MapPath("~/Content/assets/allBanks.html"));
        //    string dContent = file.ReadToEnd();

        //    return dContent.Split('|');
        //}


    }
}