using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;

using System.Collections.Generic;
using System.Threading;

namespace fin_bot
{
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {

            string strStock = "";

            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                StateClient stateClient = activity.GetStateClient();
                BotData userData = stateClient.BotState.GetPrivateConversationData(activity.ChannelId, activity.Conversation.Id, activity.From.Id);
                Activity ReplyMessage;
                StockLuis stLuis = await LuisStockClient.ParseUserInput(activity.Text);
                string reply="";
                string request = activity.Text;
                
                if (stLuis.intents.Count() > 0)
                {
                    switch (stLuis.intents[0].intent)
                    {
                        case "GetLastStock":
                            if (!userData.GetProperty<bool>("SentStock"))
                            {
                                reply = " I do not have a previous query to look upto ";
                            }
                            else
                            {
                                string LuisEntity = userData.GetProperty<string>("StockString");
                                reply = await GetStock(LuisEntity);
                            }
                            break;

                        case "StockPrice":

                            reply = await GetStock(stLuis.entities[0].entity);                         
                            userData.SetProperty<string>("StockString", stLuis.entities[0].entity);
                            userData.SetProperty<bool>("SentStock", true);
                            break;

                        case "Summary":
                            //string ts = getTickerSymbol(request);
                            string ts;
                            if (request.Contains("microsoft"))
                            ts = "MSFT";
                            else if (request.Contains("tesla"))
                            ts = "TSLA";
                            else if (request.Contains("apple"))
                            ts = "AAPL";
                            else ts = "MSFT";
                            string summary = getSummaryString(ts);
                            List<string> tokens = new List<string>(summary.Split('^'));
                            reply = "";
                            foreach (string sub in tokens)
                            {
                                reply += " - "+sub + "\n\n";
                            }
                            //reply = await GetStock(stLuis.entities[0].entity);
                            userData.SetProperty<string>("SummaryString", summary);
                            userData.SetProperty<bool>("SentSummary", true);

                            
                            ReplyMessage = activity.CreateReply(reply);
                            Thread.Sleep(2000);
                            await connector.Conversations.ReplyToActivityAsync(ReplyMessage);
                            break;

                        case "getallvalues":
                            
                            
                            Activity replyMessage = activity.CreateReply("Balance Sheet");
                            replyMessage.Recipient = activity.From;
                            replyMessage.Type = "message";
                            replyMessage.Attachments = new List<Microsoft.Bot.Connector.Attachment>();

                            Dictionary<string, string> cardContentList = new Dictionary<string, string>();

                            List<String> balanceterms = new List<string>();
                            balanceterms.Add("Inventories");
                            balanceterms.Add("Cash and Cash Equivalence");
                            balanceterms.Add("Deferred Income Taxes");
                            balanceterms.Add("Total Current Assets");
                            balanceterms.Add("Account Payable");

                            string balanceinfo = "";

                            Random r = new Random();

                            foreach (string i in balanceterms)
                            {
                                int rInt = r.Next(900, 1500);
                                balanceinfo += i +"-"+Convert.ToString(rInt) + ",";
                            }
                            
                            cardContentList.Add("Balance Sheet", balanceinfo);

                            List<String> cashflow = new List<string>();
                            cashflow.Add("Security Lending Payable");
                            cashflow.Add("Foreign Exchange Contracts");
                            cashflow.Add("Equity Contracts");
                            cashflow.Add("Other Long Term Assets");
                            cashflow.Add("Net Sales");

                            string cashflowinfo = "";
                            foreach (string i in cashflow)
                            {
                                int rInt = r.Next(500, 2000);
                                cashflowinfo += i + "-" + Convert.ToString(rInt) + ",";
                            }

                            cardContentList.Add("Cash Flow", cashflowinfo);

                            List<String> income = new List<string>();
                            income.Add("Long term Debt");
                            income.Add("Net Income");
                            income.Add("Total Current Liabilities");
                            income.Add("Retained Earnings");
                            

                            string incomeinfo = "";
                            
                            foreach (string i in income)
                            {                             
                                int rInt = r.Next(500, 1000);
                                incomeinfo += i + "-" + Convert.ToString(rInt) + ",";
                            }

                            cardContentList.Add("Income Statement", incomeinfo);

                            foreach (KeyValuePair<string, string> cardContent in cardContentList)
                            {
                                List<CardAction> cardButtons = new List<CardAction>();
                                List<string> balanceSheetButtons = new List<string>();
                                if(cardContent.Key== "Balance Sheet")
                                {
                                    balanceSheetButtons.Add("Current Asset");
                                    balanceSheetButtons.Add("Marketable Securities");
                                    balanceSheetButtons.Add("Inventory");

                                }
                                else if(cardContent.Key== "Cash Flow")
                                {
                                    balanceSheetButtons.Add("Debt");
                                    balanceSheetButtons.Add("Operating Activity");
                                    balanceSheetButtons.Add("Investing Activity");
                                }
                                else
                                {
                                    balanceSheetButtons.Add("Total Income");
                                    balanceSheetButtons.Add("Total Equity");
                                    balanceSheetButtons.Add("Liquified Income");
                                }
                                foreach (string item in balanceSheetButtons)
                                {
                                    CardAction plButton = new CardAction()
                                    {
                                        Value = "What is "+item+"?",
                                        Type = "imBack",
                                        Title = item
                                    };
                                    cardButtons.Add(plButton);
                                }
                                
                                HeroCard plCard = new HeroCard()
                                {
                                    Title = $"{cardContent.Key}",
                                    Subtitle = $"{cardContent.Value}",                                    
                                    Buttons = cardButtons
                                };
                                Microsoft.Bot.Connector.Attachment plAttachment = plCard.ToAttachment();
                                Thread.Sleep(2000);
                                replyMessage.Attachments.Add(plAttachment);
                            }

                            replyMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            await connector.Conversations.ReplyToActivityAsync(replyMessage);

                            
                            break;

                        case "comparision":
                            
                            replyMessage = activity.CreateReply($"check the plot");
                            replyMessage.Attachments = new List<Microsoft.Bot.Connector.Attachment>();
                            replyMessage.Attachments.Add(new Microsoft.Bot.Connector.Attachment()
                            {
                                ContentUrl = $"https://scopecreep.azurewebsites.net/getPlot/myCompare.png?termname=liabilities",
                                ContentType = "image/png",
                                Name = "myPlot.png"
                            });
                            Thread.Sleep(5000);
                            await connector.Conversations.ReplyToActivityAsync(replyMessage);
                            break;
                        case "showplot":

                            replyMessage = activity.CreateReply($"check the plot");
                            replyMessage.Attachments = new List<Microsoft.Bot.Connector.Attachment>();
                            replyMessage.Attachments.Add(new Microsoft.Bot.Connector.Attachment()
                            {
                                ContentUrl = $"https://scopecreep.azurewebsites.net/getPlot/myPlot1.png",
                                ContentType = "image/png",
                                Name = "myPlot.png"
                            });
                            Thread.Sleep(5000);
                            await connector.Conversations.ReplyToActivityAsync(replyMessage);
                            break;
                        case "greeting":
                            reply = "INTRO: Welcome to FINAIS - Your one stop Financial Assistant.\n\n General Query Structure:\n\n - MAIL : Mail me at <email id>\n\n - PLOT: show me plots for <company1> \n\n - COMPARISION: <company1> vs <company2> ";
                            ReplyMessage = activity.CreateReply(reply);
                            await connector.Conversations.ReplyToActivityAsync(ReplyMessage);
                            int milliseconds = 5000;
                            Thread.Sleep(milliseconds);
                            reply = "What company filings are you looking for?";
                            ReplyMessage = activity.CreateReply(reply);
                            await connector.Conversations.ReplyToActivityAsync(ReplyMessage);
                            break;

                        case "mail":

                            string url = showMatchUrl(request);
                            if (url != string.Empty)
                            {
                                reply = queryFlaskUrl(url);
                            }
                            
                            if (!userData.GetProperty<bool>("SentSummary"))
                            {
                                reply = " I do not have a previous summary to look upto ";
                            }
                            else
                            {
                                string LuisEntity = userData.GetProperty<string>("SummaryString");

                                
                            }
                            reply = SendEmail(request);
                            ReplyMessage = activity.CreateReply(reply);
                            await connector.Conversations.ReplyToActivityAsync(ReplyMessage);
                            break;

                        case "wikiquery":
                            reply = queryWiki(request);
                            ReplyMessage = activity.CreateReply(reply);
                            await connector.Conversations.ReplyToActivityAsync(ReplyMessage);
                            break;

                        default:
                            string matchurl = showMatchUrl(request);
                            if(matchurl!=string.Empty)
                            {
                                reply = "URL matched:" + matchurl;
                            }
                            else
                            reply = "Sorry, I don't understand...NO Intents";
                            ReplyMessage = activity.CreateReply(reply);
                            await connector.Conversations.ReplyToActivityAsync(ReplyMessage);
                            break;
                    }
                }
                else
                {
                    reply = "Sorry, I don't understand...";
                }

                
            stateClient.BotState.SetPrivateConversationData(activity.ChannelId, activity.Conversation.Id, activity.From.Id, userData);
            
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private static string getSummaryString(string ts)
        {
            WebRequest request = WebRequest.Create(
              "http://scopecreep.azurewebsites.net/getSumary?ticker="+ts);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            Console.WriteLine(responseFromServer);
            reader.Close();
            response.Close();
            return responseFromServer;
        }

        private static string getPlot()
        {
            WebRequest request = WebRequest.Create(
              "http://scopecreep.azurewebsites.net/getPlot");
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            Stream dataStream = response.GetResponseStream();
            Image img = System.Drawing.Image.FromStream(dataStream);
            string path = System.IO.Path.GetTempPath() + "\\myImage.Jpeg";
            img.Save(path);

            response.Close();
            return path;
        }
        private static string getTickerSymbol(string company)
        {
            WebRequest request = WebRequest.Create(
              "http://scopecreep.azurewebsites.net/getTickerSymbol?query=" + company);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            Console.WriteLine(responseFromServer);
            reader.Close();
            response.Close();
            return responseFromServer;
        }
        private static string getAllData(string company)
        {
            WebRequest request = WebRequest.Create(
              "http://scopecreep.azurewebsites.net/getAllData?query=" + company);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            Console.WriteLine(responseFromServer);
            reader.Close();
            response.Close();
            
            return responseFromServer;
        }
        private static string queryWiki(string mssg)
        {
            WebRequest request = WebRequest.Create(
              "http://scopecreep.azurewebsites.net/getDefinition?query=" + mssg);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            Console.WriteLine(responseFromServer);
            reader.Close();
            response.Close();
            return responseFromServer;
        }
        private static string queryFlaskUrl(string url)
        {
            // Create a request for the URL. 
            //return "world";
            WebRequest request = WebRequest.Create(
              "http://scopecreep.azurewebsites.net/geturl?url=" + url);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.k
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine(responseFromServer);
            // Clean up the streams and the response.
            reader.Close();
            response.Close();

            return responseFromServer;
        }
        private static string showMatchUrl(string text)
        {

            var matches = Regex.Matches(text, @"\w+[^\s]*\w+|\w");
            string url = string.Empty;
            foreach (Match match in matches)
            {
                string word = match.Value;
                if (Uri.IsWellFormedUriString(word, System.UriKind.Absolute))
                {
                    url = word;
                }

            }
            // Console.WriteLine("URL"+url);
            return url;
        }
        private static string showMatchEmail(string text)
        {

            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
            //find items that matches with our pattern
            MatchCollection emailMatches = emailRegex.Matches(text);


            string email = string.Empty;
            foreach (Match emailMatch in emailMatches)
            {

                email = Convert.ToString(emailMatch);
            }
            return email;

        }
        private string SendEmail(string request)
        {
            string email = showMatchEmail(request);
            Console.WriteLine(email);
            string reply = string.Empty;
            try
            {

                var fromAddress = new MailAddress("scopecreep4@gmail.com", "Arpan");
                var toAddress = new MailAddress(email, "Hello");
                const string fromPassword = "admin123admin";
                const string subject = "Summary from FinAis";
                const string body = "	D&C cost of revenue increased $1.1 billion or 31% mainly due to Phone Hardware.\nD & C cost of revenue increased $5.6 billion or 44 % mainly due to Phone Hardware.\nWe report the financial performance of the acquired business in our Phone Hardware segment.\nChanges in these factors could materially impact our consolidated financial statements.\nCost of revenue increased mainly due to Phone Hardware.\nCost of revenue increased mainly due to Phone Hardware.\nEquity and other investments were $12.0 billion as of March\xc2\xa031 2015 compared to $14.6 billion as of June\xc2\xa030 2014.\nAny of these actions by customers could adversely affect our revenue.\nBelow is operating income loss by segment group.\nThese increases were offset in part by a decline in revenue from Office Commercial Windows OEM and licensing of Windows Phone operating system.";
                //const string attachmentFilename = "C:/Users/arpan_/Pictures/finance.png";
                const string attachmentFilename = null;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body

                })
                {
                    if (attachmentFilename != null)
                    {
                        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(attachmentFilename, MediaTypeNames.Application.Octet);
                        ContentDisposition disposition = attachment.ContentDisposition;
                        disposition.CreationDate = File.GetCreationTime(attachmentFilename);
                        disposition.ModificationDate = File.GetLastWriteTime(attachmentFilename);
                        disposition.ReadDate = File.GetLastAccessTime(attachmentFilename);
                        disposition.FileName = Path.GetFileName(attachmentFilename);
                        disposition.Size = new FileInfo(attachmentFilename).Length;
                        disposition.DispositionType = DispositionTypeNames.Attachment;
                        message.Attachments.Add(attachment);
                    }
                    smtp.Send(message);
                }
                reply = "MAIL Sent Successfully";
            }
            catch (Exception ex)
            {
                reply = Convert.ToString(ex);
            }
            return reply;
        }

        private async Task<string> GetStock(string strStock)
        {
            string strRet = string.Empty;
            double? dblStock = await Yahoo.GetStockPriceAsync(strStock);
            // return our reply to the user
            if (null == dblStock)
            {
                strRet = string.Format("Stock {0} doesn't appear to be valid", strStock.ToUpper());
            }
            else
            {
                strRet = string.Format("Stock: {0}, Value: {1}", strStock.ToUpper(), dblStock);
            }

            return strRet;
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