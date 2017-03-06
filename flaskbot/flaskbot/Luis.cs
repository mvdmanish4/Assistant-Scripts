using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace flaskbot
{
    public class Luis
    {
        public static async Task<StockLuis> ParseUserInput(string strInput)
        {
            string strRet = string.Empty;
            string strEscaped = Uri.EscapeDataString(strInput);

            using (var client = new HttpClient())
            {
                //string uri = "https://api.projectoxford.ai/luis/v1/application?id=f99baba0-bb58-4cc0-89b6-75deab127a0f&subscription-key=7b120604af1543d89349a3e24d594af6&q=" + strEscaped;
                string uri = "https://api.projectoxford.ai/luis/v1/application?id=e8121af3-11fb-4214-a34d-dfc6955acb18&subscription-key=5d2c4a0fae4a4e858e146aad7c7366d3&q=" + strEscaped;

                HttpResponseMessage msg = await client.GetAsync(uri);

                if (msg.IsSuccessStatusCode)
                {
                    var jsonResponse = await msg.Content.ReadAsStringAsync();
                    var _Data = JsonConvert.DeserializeObject<StockLuis>(jsonResponse);
                    return _Data;
                }
            }
            return null;
        }
    }
    public class StockLuis
    {
        public string query { get; set; }
        public lIntent[] intents { get; set; }
        public lEntity[] entities { get; set; }
    }

    public class lIntent
    {
        public string intent { get; set; }
        public float score { get; set; }
    }

    public class lEntity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public float score { get; set; }
    }
}