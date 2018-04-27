using System;
using System.Net;
using System.IO;
using Com.CloudRail.SI;
using Com.CloudRail.SI.Interfaces;
using Com.CloudRail.SI.Services;
using Com.CloudRail.SI.Types;
using System.Collections.Generic;

namespace UnifiedMessaging
{
    class MainClass
    {
        static IMessaging service;

        public static void Main(string[] args)
        {
            CloudRail.AppKey = "[Your Cloudrail Key]";

            String serviceName = SelectService();

            FacebookMessenger facebookMessenger = new FacebookMessenger(null, "[Bot Token]");

            Telegram telegram = new Telegram(null, "[Bot Token]", "[Webhook URL]");

            Line line = new Line(null, "[Bot Token]");

            Viber viber = new Viber(null, "[Bot Token]", "[Webhook URL]", "[Bot Name]");

            SlackBot slackBot = new SlackBot(null, "[Bot Token]");

            service = null;
            switch (serviceName)
            {
                case "1":
                    Console.WriteLine("Selected Service: FacebookMessenger");
                    service = facebookMessenger;
                    break;
                case "2":
                    Console.WriteLine("Selected Service: Telegram");
                    service = telegram;
                    break;
                case "3":
                    Console.WriteLine("Selected Service: Line");
                    service = line;
                    break;
                case "4":
                    Console.WriteLine("Selected Service: Viber");
                    service = viber;
                    break;
                case "5":
                    Console.WriteLine("Selected Service: SlackBot");
                    service = slackBot;
                    break;
                default:
                    Console.WriteLine("Selected Service: FacebookMessenger");
                    service = facebookMessenger;
                    break;
            }

            RunHttpServer();
        }

        static String SelectService()
        {
            String s = "1";
            Console.WriteLine("Enter the Service Number (e.g. 2 is for Telegram) you want to use:");
            Console.WriteLine("----");
            Console.WriteLine("1.Facebook Messenger\n2.Telegram\n3.Line\n4.Viber\n5.SlackBot");
            Console.WriteLine("----");
            s = Console.ReadLine();
            Console.Clear();
            return s;
        }


        public static void RunHttpServer()
        {
            var listener = new HttpListener();
            String host = "http://localhost:8082/";
            listener.Prefixes.Add(host);
            Console.WriteLine("Listening..");
            listener.Start();

            while (true)
            {
                try
                {
                    var context = listener.GetContext(); 
                    var response = context.Response;
                    var request = context.Request;
                    response.StatusCode = 200;
                    response.SendChunked = true;

                    Stream body = request.InputStream;


                    if (body == null)
                    {
                        return;
                    }
                    else
                    {
                        List<Message> messages = service.ParseReceivedMessages(body);

                        foreach (Message message in messages)
                        {
                            String chat = message.GetChatId();
                            String messageText = message.GetMessageText().ToLower();

                            if (message.GetAttachments().Count > 0)
                            {
                                service.SendMessage(chat, "You send a media message / an attachment");
                            }
                            else if (messageText.Equals("send a photo"))
                            {
                                service.SendImage(chat, "here's an image", "https://raw.githubusercontent.com/mediaelement/mediaelement-files/master/big_buck_bunny.jpg", null, null, null);
                            }
                            else if (messageText.Equals("send a video"))
                            {
                                service.SendVideo(chat, "here's a video", "https://raw.githubusercontent.com/mediaelement/mediaelement-files/master/big_buck_bunny.mp4", null, null, 1055736);
                            }
                            else if (messageText.Equals("send a file"))
                            {
                                service.SendFile(chat, "here's a general file", "http://unec.edu.az/application/uploads/2014/12/pdf-sample.pdf", null, null, "cloudrailFile.pdf", 7945);
                            }
                            else if (messageText.Equals("give me a choice"))
                            {
                                List<MessageButton> buttons1 = new List<MessageButton>
                                {
                                    new MessageButton("Yay!", "postback", "https://raw.githubusercontent.com/mediaelement/mediaelement-files/master/big_buck_bunny.jpg", "Yay!"),
                                    new MessageButton("That's cool!", "postback", "https://raw.githubusercontent.com/mediaelement/mediaelement-files/master/big_buck_bunny.jpg", "That's cool!")
                                };

                                List<MessageButton> buttons2 = new List<MessageButton>
                                {
                                    new MessageButton("Nay!", "postback", "https://raw.githubusercontent.com/mediaelement/mediaelement-files/master/big_buck_bunny.jpg", "Nay!"),
                                    new MessageButton("Why would I do that?", "postback", "https://raw.githubusercontent.com/mediaelement/mediaelement-files/master/big_buck_bunny.jpg", "Why would I do that?")
                                };

                                List<MessageItem> mItems = new List<MessageItem>
                                {
                                    new MessageItem("CloudRail", "Implement all services with one common code", "https://raw.githubusercontent.com/mediaelement/mediaelement-files/master/big_buck_bunny.jpg", buttons1),
                                    new MessageItem("REST APIs", "Bother with a different API for every service", "https://raw.githubusercontent.com/mediaelement/mediaelement-files/master/big_buck_bunny.jpg", buttons2)
                                };

                                service.SendCarousel(chat, mItems);
                            }
                            else if (messageText != null)
                            {
                                service.SendMessage(chat, "You send a new message at time " + message.GetSendAt() + " with ID " + message.GetMessageId() + " and content \"" + messageText + "\"");
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
