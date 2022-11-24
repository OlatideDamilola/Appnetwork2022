using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;



namespace Appnetwork2022 {
    //Note: Don't change the property names to avoid errors 
    public class TransactionResult {
        public bool status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    //Note: Don't change the property names to avoid errors 
    public class Data {
        [JsonProperty("authorization_url")]
        public string authorization_url { get; set; }

        [JsonProperty("access_code")]
        public string access_code { get; set; }

        [JsonProperty("reference")]
        public string reference { get; set; }
    }


    public class Payment {
        private readonly string _flutterwaveSecretKey;
        /// <summary>
        /// The secret key is needed for authentication and verification purposes. 
        /// Note: It is best to store the secret key in the web.config file.
        /// </summary>
        /// <param name="secretKey"></param>
        public Payment(string secretKey) {
            _flutterwaveSecretKey = secretKey;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //Don't for get to add this. 
        }
        /// <summary>
        /// Initialize a transaction. Check out documentation on https://developers.paystack.co/docs/initialize-a-transaction
        /// </summary>
        /// <param name="amountInKobo">For eg, 5000000 is 5000 that is "5000*100"</param>
        /// <param name="email">The customer email</param>
        /// <param name="metadata">Additional data can be included. </param>
        /// <param name="resource">The optional api endpoint which is already specified</param>
        /// <returns></returns>
        public async Task<TransactionResult> InitializeTransaction(int amountInKobo, string email, string metadata = "",
            string resource = "https://api.paystack.co/transaction/initialize") {
            var client = new HttpClient { BaseAddress = new Uri(resource) };

            client.DefaultRequestHeaders.Add("Authorization", _flutterwaveSecretKey);
            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            var body = new Dictionary<string, string>
            {

                {"amount", amountInKobo.ToString()},
                {"email", email},
                {"metadata", metadata}
            };
            var content = new FormUrlEncodedContent(body);
            var response = await client.PostAsync(resource, content);
            if (response.IsSuccessStatusCode) {
                //TransactionResult responseData = await response.Content.ReadAsAsync<TransactionResult>();
                var responseAsString = await response.Content.ReadAsStringAsync();
                TransactionResult responseData = JsonConvert.DeserializeObject<TransactionResult>(responseAsString);
                return responseData;
            }
            return null;
        }

        /// <summary>
        /// Verify a transaction
        /// </summary>
        /// <param name="transactionID">The reference code returned from the transaction initialization result </param>
        /// <param name="resource">The optional api endpoint which is already specified</param>
        /// <returns></returns>
        public async Task<string> VerifyTransaction(string transactionID, string resource = "https://api.flutterwave.com/v3/transactions/") {
            var client = new HttpClient { BaseAddress = new Uri(resource) };

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
           client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", _flutterwaveSecretKey));

           

            var response = await client.GetAsync(transactionID + "/verify");

            if (response.IsSuccessStatusCode) {
                string responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
            return null;

        }

    }




}