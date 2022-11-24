using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Appnetwork2022.Models;
using System.Threading.Tasks;

namespace Appnetwork2022 {

   // class userTracker {
   //     public string chatType { get; set; }
   //     public string userRefral { get; set; }
   //     public string lastChatId { get; set; }
   // }
   
   //public class chatPost {
   //     public string postContent { get; set; }
   //     public string posterRefral { get; set; }
   //     public string chatType { get; set; } = chatTypes.Universal;
   // }


    public class UnivHub : Hub {
        public override Task OnConnected() {
            // Add your own code here.
            // For example: in a chat application, record the association between
            // the current connection ID and user name, and mark the user as online.
            // After the code in this method completes, the client is informed that
            // the connection is established; for example, in a JavaScript client,
            // the start().done callback is executed.
            //string userName = Clients.Caller.userName;
            var version = Context.QueryString["UID"];
            var ii = Context.ConnectionId;
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled) {
            // Add your own code here.
            // For example: in a chat application, mark the user as offline, 
            // delete the association between the current connection id and user name.
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected() {
            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.
            return base.OnReconnected();
        }


        public void postMsg(string name, string message) {

            Clients.All.addNewMessageToPage(message);

        }

        public void GetMissedMsg() {

        }

        //public void GetChatHistory(string inRefral) { 
        //    personalAcctDetails acctDetails=new utilityFunct().FetchUserByUidorReferral(false,inRefral);

        //}
        public void Send(string name, string message) {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);

        }
    }
}