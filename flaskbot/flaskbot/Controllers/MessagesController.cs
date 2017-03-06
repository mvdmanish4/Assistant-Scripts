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

namespace flaskbot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                Activity ReplyMessage;
                String reply = " ";
                String temp = activity.Text;
                if (activity.Text == "Hi")
                {
                    reply = "INTRO: Welcome to FINAIS - Your one stop Financial Assistant.\n\n General Query Structure:\n\n - MAIL : Mail me at <email id>\n\n - PLOT: show me plots for <company1> \n\n - COMPARISION: <company1> vs <company2> ";
                    ReplyMessage = activity.CreateReply(reply);
                    await connector.Conversations.ReplyToActivityAsync(ReplyMessage);
                    int milliseconds = 5000;
                    Thread.Sleep(milliseconds);
                    reply = "What company filings are you looking for?";
                    ReplyMessage = activity.CreateReply(reply);
                    await connector.Conversations.ReplyToActivityAsync(ReplyMessage);                    
                }else if(activity.Text == "Microsoft" || activity.Text == "Give me Microsoft filings" || activity.Text == "msft" || activity.Text == "Give me Apple filings")
                {
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
                        balanceinfo += i + "-" + Convert.ToString(rInt) + ",";
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
                        if (cardContent.Key == "Balance Sheet")
                        {
                            balanceSheetButtons.Add("Current Asset");
                            balanceSheetButtons.Add("Marketable Securities");
                            balanceSheetButtons.Add("Inventory");

                        }
                        else if (cardContent.Key == "Cash Flow")
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
                                Value = "What is " + item + "?",
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
                }else if(temp.Contains("What is")){
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
                    ReplyMessage = activity.CreateReply($"{responseFromServer}");
                    await connector.Conversations.ReplyToActivityAsync(ReplyMessage);
                }else if (temp.Contains("Compare"))
                {
                    ReplyMessage = activity.CreateReply($"check the plot");
                    ReplyMessage.Attachments = new List<Microsoft.Bot.Connector.Attachment>();
                    ReplyMessage.Attachments.Add(new Microsoft.Bot.Connector.Attachment()
                    {
                        ContentUrl = $"http://i1.wp.com/revenuesandprofits.com/wp-content/uploads/2016/01/Apple-Microsoft-Net-Profit-Margin-1995-To-2015.png",
                        ContentType = "image/png",
                        Name = "myPlot.png"
                    });
                    Thread.Sleep(5000);
                    await connector.Conversations.ReplyToActivityAsync(ReplyMessage);
                }else if (temp.Contains("Plot"))
                {
                    ReplyMessage = activity.CreateReply($"check the plot");
                    ReplyMessage.Attachments = new List<Microsoft.Bot.Connector.Attachment>();
                    ReplyMessage.Attachments.Add(new Microsoft.Bot.Connector.Attachment()
                    {
                        ContentUrl = $"http://i1.wp.com/revenuesandprofits.com/wp-content/uploads/2016/01/Microsoft-Revenues-1995-to-2015.png",
                        ContentType = "image/png",
                        Name = "myPlot.png"
                    });
                    Thread.Sleep(5000);
                    await connector.Conversations.ReplyToActivityAsync(ReplyMessage);
                }else if (temp.Contains("Mail"))
                {
                    WebRequest request = WebRequest.Create(
               "http://127.0.0.1:5000/Mail");
                    request.Credentials = CredentialCache.DefaultCredentials;
                    WebResponse resp = request.GetResponse();
                    Console.WriteLine(((HttpWebResponse)resp).StatusDescription);
                    Stream dataStream = resp.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    Console.WriteLine(responseFromServer);
                    reader.Close();
                    resp.Close();
                    ReplyMessage = activity.CreateReply($"{responseFromServer}");
                    Thread.Sleep(5000);
                    await connector.Conversations.ReplyToActivityAsync(ReplyMessage);
                }else if (temp.Contains("Summarize"))
                {
                    WebRequest request = WebRequest.Create(
              "http://127.0.0.1:5000/Summary");
                    request.Credentials = CredentialCache.DefaultCredentials;
                    WebResponse resp = request.GetResponse();
                    Console.WriteLine(((HttpWebResponse)resp).StatusDescription);
                    Stream dataStream = resp.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    List<string> tokens = new List<string>(responseFromServer.Split('^'));
                    reply = "";
                    foreach (string sub in tokens)
                    {
                        reply += " - " + sub + "\n\n";
                    }
                    Console.WriteLine(responseFromServer);
                    reader.Close();
                    resp.Close();
                    ReplyMessage = activity.CreateReply(reply);
                    Thread.Sleep(2000);
                    await connector.Conversations.ReplyToActivityAsync(ReplyMessage);
                }                
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
              
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
              
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
            }
            else if (message.Type == ActivityTypes.Typing)
            {
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}