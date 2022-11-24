using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Appnetwork2022.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Transports;
using Newtonsoft.Json;

namespace Appnetwork2022 {

    
    
    //class userTracker {
    //    public string chatType { get; set; }
    //    public string userRefral { get; set; }
    //    public string lastChatId { get; set; }
    //}

     public class ChatHub : Hub {
                
        public override Task OnConnected() {
            // Add your own code here.
            // For example: in a chat application, record the association between
            // the current connection ID and user name, and mark the user as online.
            // After the code in this method completes, the client is informed that
            // the connection is established; for example, in a JavaScript client,
            // the start().done callback is executed.
            //string userName = Clients.Caller.userName;    
            var uRef = new utilityFunct().codeViewMsg(Context.QueryString["uRef"], false).Split('`')[0];
            string connId = Context.ConnectionId;
            new ChatFunctions().RegisterChatTrack("combined_chat_tracker_tbl",uRef,connId);
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled) {
            // Add your own code here.
            // For example: in a chat application, mark the user as offline, 
            // delete the association between the current connection id and user name.
            string uRef = new utilityFunct().codeViewMsg(Context.QueryString["uRef"], false).Split('`')[0];
            var discDetails = new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byReference, uRef);
            if (discDetails != null) {
                var olD = new onlineUsersDetails { name = discDetails.sName + " " + discDetails.oName, reference = discDetails.referenceCode };
                if (new ChatFunctions().DoDisconnection(Context.ConnectionId, "combined_chat_tracker_tbl"))
                    Clients.Others.notifyDisconnetion(JsonConvert.SerializeObject(olD), stopCalled);
            }
            return base.OnDisconnected(stopCalled);
        }
        public override Task OnReconnected() {
            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.
            return base.OnReconnected();
        }

        public async Task VerifyConnection() {
            onlineUsersDetails oUd; List<onlineUsersDetails> onlineUsers = new List<onlineUsersDetails>();
            string uRef = new utilityFunct().codeViewMsg(Context.QueryString["uRef"], false).Split('`')[0];
            if (new ChatFunctions().VeriUserTrackCon("combined_chat_tracker_tbl", uRef,Context.ConnectionId)){
                personalAcctDetails pd = new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byReference, uRef);
                onlineUsers = new ChatFunctions().LoadOnlineUsers("combined_chat_tracker_tbl", 20);
                if(pd != null){
                    oUd = new onlineUsersDetails { pix = pd.pixName, name = pd.sName + " " + pd.oName, reference = uRef };
                    //oUd.pix = pd.pixName; oUd.name = pd.sName + " " + pd.oName; oUd.reference = uRef;
                    await Clients.Others.notifyConnection(JsonConvert.SerializeObject(oUd)); 
                }
                if (onlineUsers != null) await Clients.Caller.loadOnLineUser(JsonConvert.SerializeObject(onlineUsers));
            } 
        }
        public async Task<string> PostMsg(string msg,string tMsgId) {
            string[] eData= new utilityFunct().codeViewMsg(Context.QueryString["uRef"], false).Split('`');
            string inMsg = msg.Trim(); uint postId; personalAcctDetails pData; chatDetails msgComp; string retVal = string.Empty;
            if (!string.IsNullOrEmpty(msg)) {
                if(eData[1].Equals("UChat",StringComparison.OrdinalIgnoreCase)) {
                    postId = new ChatFunctions().saveChatHistory(eData[0], "univ_chat_history_tbl", inMsg);
                    if(postId != uint.MaxValue) {
                        pData = new utilityFunct().FetchUserByUidorRefraOrRefren(FetchType.byReference, eData[0]);
                        msgComp.senderPix = pData.pixName; msgComp.senderName = pData.oName + " " + pData.sName;
                        msgComp.senderCountry = pData.Country; msgComp.fromOwner = pData.referenceCode == eData[0];
                        msgComp.postContent = inMsg; msgComp.postDateTime = DateTime.UtcNow; msgComp.posterReference = eData[0];
                        await Clients.Others.reportNewMsg(JsonConvert.SerializeObject(msgComp));
                        retVal= tMsgId;
                        //await Clients.Caller.sentMsgReport(tMsgId); 
                    }
                }
            }
            return retVal;
        }

        public async Task<bool> LoadMissedMsg() {
            uint startFrom;List<chatDetails> allMissedChat; string jsonMissedChat; bool retVal = false;
            string[] eData = new utilityFunct().codeViewMsg(Context.QueryString["uRef"], false).Split('`');            
            if (eData[1].Equals("UChat", StringComparison.OrdinalIgnoreCase)) {
                 startFrom = new ChatFunctions().trackLastChat(eData[0], "univLastRead");
                if (startFrom != uint.MaxValue) {
                    allMissedChat = new ChatFunctions().getMissedChat(eData[0], startFrom, "univ_chat_history_tbl");
                    if (allMissedChat.Count > 0) {
                        jsonMissedChat = JsonConvert.SerializeObject(allMissedChat);
                        new ChatFunctions().UpdatelastRead(eData[0], (uint)allMissedChat.Count+startFrom, "combined_chat_tracker_tbl", "univLastRead");
                        await Clients.Caller.reportMissedMsg(jsonMissedChat);
                        retVal = true;
                    }
                }
            }
            return retVal;
        }

        public int GetOnline() {
            return GlobalHost.DependencyResolver.Resolve<ITransportHeartbeat>().GetConnections().Count;
        }
        public void Send(string name, string message) {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
            
        }
    }
}