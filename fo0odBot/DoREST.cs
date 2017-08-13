using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;


namespace fo0odBot
{
    public class DoREST
    {
        public static async Task<string> GetData(string query, int count = 3)
        {
            try
            {
                HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(string.Format("https://developers.zomato.com/api/v2.1/search?q={0}&count={1}", query, count.ToString()));
                GETRequest.Method = "GET";
                GETRequest.Headers.Add("user-key", "your_api_key");

                Task<WebResponse> responseTask = GETRequest.GetResponseAsync();
                WebResponse response = await responseTask;
                Encoding enc = System.Text.Encoding.GetEncoding(1252);
                StreamReader ioResponseStream = new StreamReader(response.GetResponseStream(), enc);
                string json_reponse = ioResponseStream.ReadToEnd();

                JToken token = JToken.Parse(json_reponse);
                JArray array = (JArray)token.SelectToken("restaurants");
                string response_text = "";
                foreach (JToken node in array)
                {
                    response_text = response_text + "Hotel name: " + node["restaurant"]["name"] + "\n";
                    response_text = response_text + "Hotel loc: " + node["restaurant"]["location"]["locality_verbose"] + "\n";
                    response_text = response_text + "cost for 2: " + node["restaurant"]["average_cost_for_two"] + "\n";
                    response_text = response_text + "for more: " + node["restaurant"]["url"] + "\n";
                    response_text = response_text + "\n\n\n---------------------------------";
                }
                return response_text;
            }
            catch (WebException ex)
            {
                //handle your exception here  
                throw ex;
            }
        }
    }
}