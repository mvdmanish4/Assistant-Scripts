using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;

using System.Collections.Generic;
using System.Threading;


namespace AlaAssi
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
             
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                Activity reply;
                string StockRateString;
                // calculate something for us to return
                //int length = (activity.Text ?? string.Empty).Length;
                //<!----------------TEXT ANALYTICS CODE BEGINS--------------------------->
                const string apiKey = "dd297215302b4bca86ddab0761c2fbbe";
                const string queryUri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";

                var client = new HttpClient
                {
                    DefaultRequestHeaders = {
                {"Ocp-Apim-Subscription-Key", apiKey},
                {"Accept", "application/json"}
                     }
                };
                var sentimentInput = new BatchInput
                 {
                    documents = new List<DocumentInput> {
                         new DocumentInput {
                    id = 1,
                    text = activity.Text,
                      }
                   }
                };
                var json = JsonConvert.SerializeObject(sentimentInput);
                var sentimentPost = await client.PostAsync(queryUri, new StringContent(json, Encoding.UTF8, "application/json"));
                var sentimentRawResponse = await sentimentPost.Content.ReadAsStringAsync();
                var sentimentJsonResponse = JsonConvert.DeserializeObject<BatchResult>(sentimentRawResponse);
                var sentimentScore = sentimentJsonResponse?.documents?.FirstOrDefault()?.score ?? 0;
                if (sentimentScore < 0.3)
                {
                    //StockRateString = $"I'm sorry to hear that..";
                    reply = activity.CreateReply($"I'm sorry to hear that..Contact the customer care executive");
                    reply.Recipient = activity.From;
                    reply.Type = "message";
                    reply.Attachments = new List<Microsoft.Bot.Connector.Attachment>();

                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = "",
                        Type = "openUrl",
                        Title = "Call Customer service"
                    };
                    cardButtons.Add(plButton);
                    HeroCard plCard = new HeroCard()
                    {
                        Subtitle = "Place a Call",
                        Buttons = cardButtons
                    };
                    Microsoft.Bot.Connector.Attachment plAttachment = plCard.ToAttachment();
                    reply.Attachments.Add(plAttachment);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                //<!----------------TEXT ANALYTICS CODE ENDS----------------------------->
                StockLuis StLUIS = await GetEntityFromLUIS(activity.Text);
                if (StLUIS.intents.Count() > 0)
                {
                    switch (StLUIS.intents[0].intent)
                    {
                        case "StockPrice":
                            StockRateString = await GetStock(StLUIS.entities[0].entity);
                            reply = activity.CreateReply(StockRateString);
                            await connector.Conversations.ReplyToActivityAsync(reply);
                            break;
                        case "wordMeaning":
                            WebRequest request = WebRequest.Create(
                "http://127.0.0.1:5000/Definition?Def=" + activity.Text);
                            request.Credentials = CredentialCache.DefaultCredentials;
                            WebResponse resp = request.GetResponse();
                            Console.WriteLine(((HttpWebResponse)resp).StatusDescription);
                            Stream dataStream = resp.GetResponseStream();
                            StreamReader reader = new StreamReader(dataStream);
                            string responseFromServer = reader.ReadToEnd();
                            Console.WriteLine(responseFromServer);
                            reader.Close();
                            resp.Close();
                            StockRateString = String.Copy(responseFromServer);
                            reply = activity.CreateReply(StockRateString);
                            await connector.Conversations.ReplyToActivityAsync(reply);
                            break;
                        case "greetings":
                            StockRateString = "INTRO: Welcome to ALAAIS - Your one stop Financial Assistant.\n\n General Query Structure:\n\n - MAIL : Mail me at <email id>\n\n - PLOT: show me plots for <company1> \n\n - COMPARISION: <company1> vs <company2> ";
                            reply = activity.CreateReply(StockRateString);
                            await connector.Conversations.ReplyToActivityAsync(reply);
                            int milliseconds = 5000;
                            Thread.Sleep(milliseconds);
                            StockRateString = "What are you looking for?";
                            reply = activity.CreateReply(StockRateString);
                            await connector.Conversations.ReplyToActivityAsync(reply);
                            break;                                                                      
                        default:
                            StockRateString = "Sorry, I am not getting you...";
                            break;
                    }
                }
                else
                {
                    StockRateString = "Sorry, I am not getting you...";
                }
                // return our reply to the user                
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<string> GetStock(string StockSymbol)
        {
            double? dblStockValue = await Yahoo.GetStockRateAsync(StockSymbol);
            if (dblStockValue == null)
            {
                return string.Format("This \"{0}\" is not an valid stock symbol", StockSymbol);
            }
            else
            {
                return string.Format("Stock Price of {0} is {1}", StockSymbol, dblStockValue);
            }
        }

        private static async Task<StockLuis> GetEntityFromLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            StockLuis Data = new StockLuis();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/f0a93ae3-fdd5-4d17-824b-f8aef414c15f?subscription-key=2635884f0d0041c4ab453562efed7460&verbose=true&spellCheck=true&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<StockLuis>(JsonDataResponse);
                }
            }
            return Data;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}