using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text;
using Appnetwork2022.Models;


namespace Appnetwork2022 {
    
    public class DbVar {
         public const string conStr = "server=localhost; userid=root; database=appnetworkdb; password=Dammy; pooling=false;port=3306";// "Server=MYSQL5046.site4now.net;Database=db_a894f4_andb;Uid=a894f4_andb;Pwd=andb1234";// "server=localhost; userid=root; database=db_a894f4_andb; password=Dammy; pooling=false;port=3306";
        //public const string conStr ="Server=MYSQL5046.site4now.net;Database=db_a894f4_andb;Uid=a894f4_andb;Pwd=andb1234";// "server=localhost; userid=root; database=db_a894f4_andb; password=Dammy; pooling=false;port=3306";
        public string cmdStr;
        public MySqlCommand cmdObj;
        public MySqlDataReader drObj;
    }

    public struct FetchType {
        public const byte byReference = 0;
        public const byte byReferral = 1;
        public const byte byUid = 2;
    }
    public struct chatTypes {
        public const string Universal = "Universal";

    }

   


    public class ChatFunctions{
        
        public bool DoDisconnection(string conId,string trackingTable) {
            DbVar dbObj = new DbVar(); bool retBool = false; //dbObj.cmdObj.Connection = new MySqlConnection(DbVar.conStr); 
            try {
                uint totChatCount = new utilityFunct().CountRecords("univ_chat_history_tbl", "postId");                
                //dbObj.cmdObj.CommandText = "UPDATE " + trackingTable + " SET univLastRead=@univLastRead,univChatOnlineState=FALSE WHERE ConnectionId=@ConnectionId";
                 dbObj.cmdObj = new MySqlCommand("UPDATE " + trackingTable + " SET univLastRead=@univLastRead,univChatOnlineState=FALSE WHERE ConnectionId=@ConnectionId", new MySqlConnection(DbVar.conStr));
                dbObj.cmdObj.Parameters.AddWithValue("@univLastRead", totChatCount);
                dbObj.cmdObj.Parameters.AddWithValue("@ConnectionId", conId);
                dbObj.cmdObj.Connection.Open();
                if (dbObj.cmdObj.ExecuteNonQuery() > 0) retBool = true;
                    //{
                //    dbObj.cmdObj.CommandText= "SELECT userReference FROM" + trackingTable + "WHERE ConnectionId=@ConnectionId"
                //}
            } catch (Exception) {
            } finally {
                if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                dbObj = null;
            }
            return retBool;
        }
        public List<onlineUsersDetails> LoadOnlineUsers( string dTable, uint dLimit) {
            DbVar dbObj = new DbVar(); List<onlineUsersDetails> allOnlineUsers= new List<onlineUsersDetails>();
            onlineUsersDetails oLineUser; List<onlineUsersDetails> allOnlineUsersTemp = new List<onlineUsersDetails>();
            try {
                dbObj.cmdObj = new MySqlCommand("SELECT* FROM " + dTable + " WHERE univChatOnlineState=TRUE ORDER BY trackId ASC LIMIT " + dLimit, new MySqlConnection(DbVar.conStr));
                dbObj.cmdObj.Connection.Open();
                dbObj.drObj = dbObj.cmdObj.ExecuteReader();
                while (dbObj.drObj.Read()) {
                    oLineUser = new onlineUsersDetails { 
                        reference = new utilityFunct().safeReadAllType<string>(dbObj.drObj["userReference"]),
                        conId = new utilityFunct().safeReadAllType<string>(dbObj.drObj["ConnectionId"])
                    };
                    allOnlineUsers.Add(oLineUser);
                }
                allOnlineUsersTemp.AddRange(allOnlineUsers);
               // allOnlineUsers.ForEach(onlineUsersDetails => { allOnlineUsersTemp.Add(onlineUsersDetails);});
                foreach (var olUser in allOnlineUsers) {
                    dbObj.cmdObj.Dispose();
                    dbObj.cmdObj = new MySqlCommand("SELECT* FROM personalacct_tbl WHERE referenceCode = @referenceCode", new MySqlConnection(DbVar.conStr));
                    dbObj.cmdObj.Parameters.AddWithValue("@referenceCode",olUser.reference);
                    dbObj.cmdObj.Connection.Open();
                    dbObj.drObj = dbObj.cmdObj.ExecuteReader();
                    while (dbObj.drObj.Read()) {
                        int tempIndex = allOnlineUsersTemp.FindIndex(x => x.reference == new utilityFunct().safeReadAllType<string>(dbObj.drObj["referenceCode"]));
                        if (tempIndex > -1) {
                            oLineUser = allOnlineUsersTemp[tempIndex];
                            oLineUser.name = new utilityFunct().safeReadAllType<string>(dbObj.drObj["OName"]) + " " + new utilityFunct().safeReadAllType<string>(dbObj.drObj["sName"]);
                            oLineUser.pix = new utilityFunct().safeReadAllType<string>(dbObj.drObj["img"]);
                            allOnlineUsersTemp[tempIndex]=oLineUser;
                        }
                    }
                }
            } catch (Exception) {
            } finally {
                if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                dbObj = null;
            }
            return allOnlineUsersTemp;
        }


        public void UpdatelastRead(string userReference,uint dval, string trackingTable,string colToUpdate) {
            DbVar dbObj = new DbVar();
            try {
                dbObj.cmdObj = new MySqlCommand("UPDATE "+ trackingTable + " SET " + colToUpdate + "=@dval WHERE userReference=@userReference", new MySqlConnection(DbVar.conStr));
                dbObj.cmdObj.Parameters.AddWithValue("@userReference", userReference);
                dbObj.cmdObj.Parameters.AddWithValue("@dval", dval);
                dbObj.cmdObj.Connection.Open();
                dbObj.cmdObj.ExecuteNonQuery();
            } catch (Exception) {
            } finally {
                if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                dbObj = null;
            }
        }
        public bool VeriUserTrackCon(string trackingTable, string userReference, String conId) {
            DbVar dbObj = new DbVar();
            bool retBool = false;
            try{
                dbObj.cmdObj = new MySqlCommand("SELECT* FROM " + trackingTable + " WHERE userReference = @userReference AND ConnectionId=@ConnectionId", new MySqlConnection(DbVar.conStr));
                dbObj.cmdObj.Parameters.AddWithValue("@userReference", userReference); 
                dbObj.cmdObj.Parameters.AddWithValue("@ConnectionId", conId);
                dbObj.cmdObj.Connection.Open();
                dbObj.drObj = dbObj.cmdObj.ExecuteReader();
                if (dbObj.drObj.Read()) retBool=true;
            } catch (Exception) { 
            } finally {
                if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                dbObj = null;
            }
            return retBool;
        }

        public bool RegisterChatTrack(string trackingTable,string userReference, String conId) {
            DbVar dbObj = new DbVar();
            uint totIndex; bool retBool=false;
            try {
                dbObj.cmdObj = new MySqlCommand("SELECT* FROM " + trackingTable + " WHERE userReference = @userReference", new MySqlConnection(DbVar.conStr));
                dbObj.cmdObj.Parameters.AddWithValue("@userReference", userReference);
                dbObj.cmdObj.Connection.Open();
                dbObj.drObj = dbObj.cmdObj.ExecuteReader(); 
                if (dbObj.drObj.Read()) {
                    dbObj.cmdObj.Dispose();
                    dbObj.cmdObj = new MySqlCommand("UPDATE combined_chat_tracker_tbl SET univChatLastDateTime=@univChatLastDateTime,univChatOnlineState=TRUE,ConnectionId=@ConnectionId WHERE userReference=@userReference", new MySqlConnection(DbVar.conStr));
                    dbObj.cmdObj.Parameters.AddWithValue("@univChatLastDateTime",DateTime.UtcNow);
                    dbObj.cmdObj.Parameters.AddWithValue("@ConnectionId",conId);
                    dbObj.cmdObj.Parameters.AddWithValue("@userReference", userReference);
                    dbObj.cmdObj.Connection.Open();
                    dbObj.cmdObj.ExecuteNonQuery();
                    retBool = true;
                } else {
                    dbObj.cmdObj.Dispose();
                    totIndex = new utilityFunct().CountRecords("combined_chat_tracker_tbl", "trackId");
                    if (totIndex != uint.MaxValue) {
                        dbObj.cmdObj = new MySqlCommand("INSERT INTO combined_chat_tracker_tbl(trackId,userReference,univChatLastDateTime,univChatOnlineState,ConnectionId)VALUES(@trackId,@userReference,@univChatLastDateTime,@univChatOnlineState,@ConnectionId)", new MySqlConnection(DbVar.conStr));
                        dbObj.cmdObj.Parameters.AddWithValue("@trackId", ++totIndex);
                        dbObj.cmdObj.Parameters.AddWithValue("@userReference", userReference);
                        dbObj.cmdObj.Parameters.AddWithValue("@univChatLastDateTime", DateTime.UtcNow);
                        dbObj.cmdObj.Parameters.AddWithValue("@univChatOnlineState", true);
                        dbObj.cmdObj.Parameters.AddWithValue("@ConnectionId", conId);
                        dbObj.cmdObj.Connection.Open();
                        dbObj.cmdObj.ExecuteNonQuery();
                        retBool = true;
                    }
                }
                  
            } catch { } finally {
                if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                dbObj = null;
            }
            return retBool;
        }


            public List<chatDetails> getMissedChat(string OnwerReference, uint trackStart, string chatTable) {
            chatDetails chatDetails;
            List<chatDetails> allChat = new List<chatDetails>();
            List<chatDetails> allChatTemp= new List<chatDetails>();
            DbVar dbObj = new DbVar();
            var itr = 0;
            try {
                dbObj.cmdObj = new MySqlCommand("SELECT* FROM " + chatTable + " WHERE postId>@trackStart ORDER BY postId ASC", new MySqlConnection(DbVar.conStr));
                dbObj.cmdObj.Parameters.AddWithValue("@trackStart", trackStart);
                dbObj.cmdObj.Connection.Open();
                dbObj.drObj = dbObj.cmdObj.ExecuteReader();
                while (dbObj.drObj.Read()) {
                    chatDetails= new chatDetails();
                    chatDetails.postContent= new utilityFunct().safeReadAllType<string>(dbObj.drObj["postContent"]);
                    chatDetails.postDateTime= new utilityFunct().safeReadAllType<DateTime>(dbObj.drObj["postDateTime"]);
                    chatDetails.posterReference = new utilityFunct().safeReadAllType<string>(dbObj.drObj["posterReference"]);
                    chatDetails.fromOwner = chatDetails.posterReference == OnwerReference;
                    allChat.Add(chatDetails);
                }
               allChat.ForEach(chatDetail => {allChatTemp.Add(chatDetail);});
                //allChatTemp = allChat;                
                foreach (var dChat in allChat){ 
                    dbObj.cmdObj.Dispose();
                    dbObj.cmdObj = new MySqlCommand("SELECT referenceCode,img,sName,OName,country FROM personalacct_tbl WHERE referenceCode = @referenceCode", new MySqlConnection(DbVar.conStr));
                    dbObj.cmdObj.Parameters.AddWithValue("@referenceCode", dChat.posterReference); 
                    dbObj.cmdObj.Connection.Open();
                    dbObj.drObj = dbObj.cmdObj.ExecuteReader();
                    if (dbObj.drObj.Read()) {
                        if(dChat.posterReference== new utilityFunct().safeReadAllType<string>(dbObj.drObj["referenceCode"])) {
                            // int tempIndex = allChat.FindIndex(x => x.posterReference == new utilityFunct().safeReadAllType<string>(dbObj.drObj["referenceCode"]));
                            // if (tempIndex > -1) {
                            chatDetails = dChat;// allChat[tempIndex];// allChat.First(x => x.posterRefral == new utilityFunct().safeReadAllType<string>(dbObj.drObj["referralCode"]));
                            chatDetails.senderName = new utilityFunct().safeReadAllType<string>(dbObj.drObj["sName"]) + " " + new utilityFunct().safeReadAllType<string>(dbObj.drObj["OName"]);
                            chatDetails.senderPix = new utilityFunct().safeReadAllType<string>(dbObj.drObj["img"]);
                            chatDetails.senderCountry = new utilityFunct().safeReadAllType<string>(dbObj.drObj["country"]);
                            allChatTemp[itr] = chatDetails;//.Add(chatDetails);// [tempIndex]=chatDetails; 
                        }
                    }
                    itr++;
                }
            } catch (Exception e) {
            } finally {
                if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                dbObj = null;
            }
            return allChatTemp;
        }
    
        public uint trackLastChat(string trackReference,string fromColumn) {
            DbVar dbObj = new DbVar();
            uint trackNum = uint.MaxValue;
            try {
                dbObj.cmdObj = new MySqlCommand("SELECT " + fromColumn + " FROM combined_chat_tracker_tbl WHERE userReference=@userReference", new MySqlConnection(DbVar.conStr));
                dbObj.cmdObj.Parameters.AddWithValue("@userReference", trackReference);
                dbObj.cmdObj.Connection.Open();
                dbObj.drObj = dbObj.cmdObj.ExecuteReader();
                if (dbObj.drObj.Read()) trackNum =new utilityFunct().safeReadAllType<uint>(dbObj.drObj[fromColumn]);             
            } catch (Exception) {
            } finally {
                if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                dbObj = null;
            }
            return trackNum;
        }
    
        public uint saveChatHistory(string posterReference,string chatTable, string postContent) {
            DbVar dbObj = new DbVar();
            uint totIndex=uint.MaxValue;
            try {
                totIndex = new utilityFunct().CountRecords("univ_chat_history_tbl", "postId");
                if (totIndex != uint.MaxValue) {
                    dbObj.cmdObj = new MySqlCommand("INSERT INTO "+ chatTable + " (postId,posterReference, postDateTime,postContent)VALUES(@postId,@posterReference, @postDateTime,@postContent)", new MySqlConnection(DbVar.conStr));
                    dbObj.cmdObj.Parameters.AddWithValue("@posterReference", posterReference);
                    dbObj.cmdObj.Parameters.AddWithValue("@postDateTime", DateTime.UtcNow); 
                    dbObj.cmdObj.Parameters.AddWithValue("@postContent", postContent);
                    dbObj.cmdObj.Parameters.AddWithValue("@postId", ++totIndex);
                    dbObj.cmdObj.Connection.Open();
                    dbObj.cmdObj.ExecuteNonQuery();
                }               
            }catch (Exception) {
            } finally {
                if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                dbObj = null;
            }
            return totIndex;
        }
     }


   public class shareholderfunctions {
        walletDetails detailForWallet;
        public ushort prorateSharesForRefral(string ownerUid, decimal pAmnt, ushort tillGeneration) {
            ushort itr = 0;
            try {
                string fDat = new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byUid, ownerUid)?.referralCode ?? default(string);// 30% for direct referral 
                if (!string.IsNullOrEmpty(fDat)) {
                    fDat =new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byReference, fDat)?.Uid ?? default(string);     //get the Id for wallet saving
                    detailForWallet = new walletDetails {
                        rowId = new utilityFunct().GenUIN(),
                        uid = fDat,
                        PayAmount =new utilityFunct().GetpercentOf(allConstant.shareHolderSubFee, 30),
                        Paydate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        isCredit = true,
                        payOutSource = AmountSource.fromBonus,
                        Paydetails = "30% referral bonus",
                        isConfirmed = true,
                        isWalletToWallet = true
                    };
                    if (!string.IsNullOrEmpty(fDat)) pushAmntIntoWallet(detailForWallet);
                    else return itr;
                } else return (1);
                // Last fDat contain Uid to fetch the next referral
                for (itr = 1; itr < (tillGeneration - 1); itr++) {
                    fDat = new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byUid, fDat)?.referralCode ?? default(string);
                    if (!string.IsNullOrEmpty(fDat)) {
                        fDat =new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byReference, fDat)?.Uid ?? default(String);
                        detailForWallet = new walletDetails {
                            rowId = new utilityFunct().GenUIN(),
                            uid = fDat,
                            PayAmount =new utilityFunct().GetpercentOf(allConstant.shareHolderSubFee, 1),
                            Paydate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            isCredit = true,
                            payOutSource = AmountSource.fromBonus,
                            Paydetails = "1% referral bonus",
                            isConfirmed = true,
                            isWalletToWallet = true
                        };
                        if (!string.IsNullOrEmpty(fDat)) pushAmntIntoWallet(detailForWallet);
                        else break;
                    } else break;
                }
            } catch (Exception) { }
            return itr;
        }


        public bool DoWalletConfirm(bool confirmIt, string rowID) {
            bool itDone = false;
            DbVar dbObj = new DbVar();
            try {
                dbObj.cmdObj = new MySqlCommand("UPDATE wallethistory_tbl SET isConfirmed=@isConfirmed WHERE rowId=@rowId", new MySqlConnection(DbVar.conStr));
                if (confirmIt) dbObj.cmdObj.Parameters.AddWithValue("@isConfirmed", true);
                else dbObj.cmdObj.Parameters.AddWithValue("@isConfirmed", false);
                dbObj.cmdObj.Parameters.AddWithValue("@rowId", rowID);
                dbObj.cmdObj.Connection.Open();
                dbObj.cmdObj.ExecuteNonQuery();
                itDone = true;
            } catch (Exception) {
                itDone = false;
            } finally {
                if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                dbObj = null;
            }
            return itDone;
        }//string uid,string rowId,decimal PayAmount,string Paydate,bool isCredit,string payOutSource,string Paydetails,bool isConfirmed,bool isWalletToWallet
        public bool pushAmntIntoWallet(walletDetails walDtail) {   //bool.TrueString
            bool itDone = false;
            DbVar dbObj = new DbVar();
            try {
                ////MySqlCommand setcmd = new MySqlCommand("SET character_set_results=utf8", new MySqlConnection(DbVar.conStr));
                ////setcmd.Connection.Open();
                ////int n = setcmd.ExecuteNonQuery();
                ////setcmd.Dispose();
                dbObj.cmdObj = new MySqlCommand("INSERT INTO wallethistory_tbl(uid,PayAmount,Paydate,isWalletToWallet,isCredit,Paydetails,rowId,isConfirmed,payOutSource)VALUES(@uid,@PayAmount,@Paydate,@isWalletToWallet,@isCredit,@Paydetails,@rowId,@isConfirmed,@payOutSource)", new MySqlConnection(DbVar.conStr));
                dbObj.cmdObj.Parameters.AddWithValue("@uid", walDtail.uid);
                dbObj.cmdObj.Parameters.AddWithValue("@rowId", walDtail.rowId);
                dbObj.cmdObj.Parameters.AddWithValue("@PayAmount", walDtail.PayAmount);
                dbObj.cmdObj.Parameters.AddWithValue("@Paydate", walDtail.Paydate);
                dbObj.cmdObj.Parameters.AddWithValue("@isCredit", walDtail.isCredit);
                dbObj.cmdObj.Parameters.AddWithValue("@payOutSource", walDtail.payOutSource);
                dbObj.cmdObj.Parameters.AddWithValue("@Paydetails", walDtail.Paydetails);
                dbObj.cmdObj.Parameters.AddWithValue("@isConfirmed", walDtail.isConfirmed);
                dbObj.cmdObj.Parameters.AddWithValue("@isWalletToWallet", walDtail.isWalletToWallet);
                dbObj.cmdObj.Connection.Open();
                dbObj.cmdObj.ExecuteNonQuery();
                itDone = true;
            } catch (Exception) {
                itDone = false;
            } finally {
                if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                dbObj = null;
            }
            return itDone;
        }

        public decimal getWalletBallance(string uid) {
            DbVar dbObj = new DbVar();
            decimal creVal = 0, debVal = 0;
            decimal totCalc = 0;
            try {
                dbObj.cmdObj = new MySqlCommand("SELECT SUM(PayAmount) AS paySum  FROM wallethistory_tbl WHERE isCredit=TRUE AND uid=@dUid AND isConfirmed=TRUE", new MySqlConnection(DbVar.conStr));
                dbObj.cmdObj.Parameters.AddWithValue("@dUid", uid);
                dbObj.cmdObj.Connection.Open();
                dbObj.drObj = dbObj.cmdObj.ExecuteReader();
                if (dbObj.drObj.Read()) {
                    creVal =new utilityFunct().safeReadAllType<decimal>(dbObj.drObj["paySum"]); //dbObj.drObj["paySum"] //dbObj.drObj.get<decimal>(dbObj.drObj.GetOrdinal("paySum")); //  (dbObj.drObj.GetDecimal("paySum"). DBNull.Value) ? Decimal.Zero: dbObj.drObj.GetDecimal("paySum")
                    dbObj.cmdObj.Dispose();
                    if (creVal > 0) {
                        dbObj.cmdObj = new MySqlCommand("SELECT SUM(PayAmount) AS debSum  FROM wallethistory_tbl WHERE isCredit=FALSE AND uid=@uid", new MySqlConnection(DbVar.conStr));
                        dbObj.cmdObj.Parameters.AddWithValue("@uid", uid);
                        dbObj.cmdObj.Connection.Open();
                        dbObj.drObj = dbObj.cmdObj.ExecuteReader();
                        if (dbObj.drObj.Read()) {
                            debVal =new utilityFunct().safeReadAllType<decimal>(dbObj.drObj["debSum"]);// dbObj.drObj.GetDecimal("debSum");
                            totCalc = creVal - debVal;
                        } else totCalc = creVal;
                    }
                }
            } catch (Exception) {
                totCalc = 0;
            } finally {
                if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                dbObj = null;
            }
            return totCalc;
        }

    }

    public class utilityFunct{
        
       public enum what2Do { saveIt = 1, updateIt = 2, deleteIt = 3 }

        public string codeViewMsg(string dMsg, bool encodeIt) {
            if (encodeIt)return Convert.ToBase64String(Encoding.ASCII.GetBytes(dMsg));
            else return Encoding.UTF8.GetString(Convert.FromBase64String(dMsg));
        }

        public uint CountRecords(string dTbl,string refCol) {
            DbVar dbObj = new DbVar();
            uint rowC =0;
            try {
                dbObj.cmdObj = new MySqlCommand("SELECT " + refCol+ " FROM " + dTbl + " ORDER BY "+ refCol + " DESC LIMIT 1", new MySqlConnection(DbVar.conStr));
                dbObj.cmdObj.Connection.Open();
                dbObj.drObj = dbObj.cmdObj.ExecuteReader();
                if (dbObj.drObj.Read())rowC = new utilityFunct().safeReadAllType<uint>(dbObj.drObj[refCol]);
            } catch (Exception) {
                rowC = uint.MaxValue;
            } finally {
                if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                dbObj = null;
            }
            return rowC; 
        }        
        public decimal GetpercentOf(decimal dAmnt, ushort dPecentage) { return ((dPecentage / 100m) * dAmnt);}
        public string GenUIN(ushort dLen = 8){
            var uin = new System.Text.StringBuilder();
            var rnd = new Random();
            string[] assortedNumbAlpha = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,KL,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',');
            for (int i = 0; i < dLen; i++) uin.Append(assortedNumbAlpha[rnd.Next(35)]);
            return uin.ToString();
        }

        public bool doPostedFile( what2Do doCmd, string dName, string loc, HttpPostedFileBase inpFile = null) {
                try {
                    string locFolder = HttpContext.Current.Server.MapPath("~/" + loc + "/");
                   // if (doCmd == what2Do.saveIt && inpFile != null) inpFile.SaveAs(locFolder + dName);
                    if (((doCmd == what2Do.saveIt) || (doCmd == what2Do.updateIt)) && inpFile != null) {
                        if (File.Exists(locFolder + dName)) File.Delete(locFolder + dName);
                        inpFile.SaveAs(locFolder + dName);
                        
                    } else if (doCmd == what2Do.deleteIt) File.Delete(locFolder + dName);
                } catch (Exception) {return false;}                
                return true; 
        }
        

        public personalAcctDetails FetchUserByUidorRefraOrRefren(byte byWhat, string refralOrUidOrRefren) {
            DbVar dbObj = new DbVar();
            personalAcctDetails persDet=null;
                //string dPara = "";
                try {
                    dbObj.cmdObj = new MySqlCommand("",new MySqlConnection(DbVar.conStr));
                    if (byWhat==FetchType.byUid) dbObj.cmdObj.CommandText = "SELECT * FROM personalacct_tbl WHERE uid = @p1";
                    else if(byWhat==FetchType.byReference) dbObj.cmdObj.CommandText = "SELECT * FROM personalacct_tbl WHERE referenceCode = @p1";
                    else if(byWhat==FetchType.byReferral) dbObj.cmdObj.CommandText = "SELECT * FROM personalacct_tbl WHERE referralCode = @p1";
                dbObj.cmdObj.Parameters.AddWithValue("@p1", refralOrUidOrRefren);
                    dbObj.cmdObj.Connection.Open();
                    dbObj.drObj = dbObj.cmdObj.ExecuteReader();
                    if (dbObj.drObj.Read()) {
                        persDet= new personalAcctDetails();
                        persDet.sName = dbObj.drObj["sName"].ToString();
                        persDet.oName = dbObj.drObj["OName"].ToString();
                        persDet.pNum = dbObj.drObj["pNum"].ToString();
                        persDet.sex = dbObj.drObj["sex"].ToString();
                        persDet.Dob = Convert.ToDateTime( dbObj.drObj["dob"].ToString());
                        persDet.Occupation = dbObj.drObj["occupation"].ToString();
                        persDet.Religion = dbObj.drObj["religion"].ToString();
                        persDet.Addrs = dbObj.drObj["addrs"].ToString();
                        persDet.Country = dbObj.drObj["country"].ToString();
                        persDet.State = dbObj.drObj["state"].ToString();
                        persDet.City = dbObj.drObj["city"].ToString();  // dbObj.drObj["city"]?.ToString()??default(string);
                        persDet.referralCode = dbObj.drObj["referralCode"].ToString();
                        persDet.eMail = dbObj.drObj["email"].ToString();
                        persDet.nokNames = dbObj.drObj["noknames"].ToString();
                        persDet.pixName= dbObj.drObj["img"].ToString();
                        persDet.Uid = dbObj.drObj["uid"].ToString();
                        persDet.bankName = dbObj.drObj["bankName"].ToString();
                        persDet.acctNumber = dbObj.drObj["acctNumber"].ToString();
                        persDet.referenceCode = dbObj.drObj["referenceCode"].ToString();
                }               
            } catch (Exception) {
                persDet=null;
            } finally {
                if (dbObj.cmdObj != null && dbObj.cmdObj.Connection != null) dbObj.cmdObj.Connection.Close();
                dbObj = null;
            }
            return persDet;
        }
        
        public  T safeReadAllType <T>(object readerVal) {
            if ((readerVal == DBNull.Value) || ( readerVal == null)) return default(T);
            return (T)readerVal;
        }

       
    } 
}
    

