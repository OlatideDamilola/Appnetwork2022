using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Appnetwork2022.Models{

  public  struct onlineUsersDetails {
        public string pix;
        public string name;
        public string reference;
        public string conId;
    }
    public struct chatDetails {
        public string senderPix;
        public string senderName;
        public string postContent;
        public bool fromOwner;
        public DateTime postDateTime;
        public string posterReference;
        public string senderCountry;
    }
  // public struct bankStruct { public string code; public string name; }
    public struct walletDetails {
        public string rowId;
        public string uid;
        public decimal PayAmount;
        public string Paydate;
        public bool isCredit;
        public string payOutSource;
        public string Paydetails ;
        public bool isConfirmed ;
        public bool isWalletToWallet;
    }

    public class chatHistory {
        public List<chatDetails> chatDetailList = new List<chatDetails>();
    }

    public class allConstant {
        public const decimal shareHolderSubFee= 100;
        public const decimal ANWdollarRate = 600;
        public const int leastTransaction = 600;
        public const string baseReferenceCode = "APPNETWK";
    }
    public class AmountSource {
        public const string fromFunding = "fund";
        public const string fromBonus = "bonus";
        public const string forTransfer = "transfer";
        public const string forSubscription = "subscript";
        public const string forCashout = "cashout";
    }
    public class personalAcctDetails{      
        public HttpPostedFileBase fotoFile { get; set; }
        public string Uid { get; set; }
        public string referralCode { get; set; }        
        public string referenceCode { get; set; }
        public string sName{ get; set; }
        public string oName{ get; set; }
        public string pNum { get; set; }
        public string eMail { get; set; }
        public string Gender { get; set; }
        public DateTime Dob { get; set; }
        public string Occupation { get; set; }
        public string Religion { get; set; }
        public string Addrs { get; set; }
        public string Country { get; set; }
        public string Pword { get; set; }
        public string sex { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string nokNames { get; set; }
        public string nokCountry { get; set; }
        public string nokAddress { get; set; }
        public string nokPhone { get; set; }
        public string nokEmail { get; set; }
        public string pixName { get; set; }
        public string acctNumber { get; set; }
        public string acctName { get; set; }
        public string bankName { get; set; }
        public bool isShareholder { get; set; } = false;
        public DateTime shareholderRegdate { get; set; }
        public DateTime regDate { get; set; }
    }

   

    //public class personalAcctTemplate { 
    //    public personalAcctDetails personalAcctInst=new personalAcctDetails();
    //}
}