using System;
using System.Collections.Generic;
using Com.CloudRail.SI;
using Com.CloudRail.SI.Interfaces;
using Com.CloudRail.SI.Services;
using Com.CloudRail.SI.ServiceCode.Commands.CodeRedirect;
using Com.CloudRail.SI.Types;
using System.IO;

namespace UnifiedEmailSending
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            CloudRail.AppKey = "[Your CloudRail Key]";

            int port = 8082;

            MailJet mailJet = new MailJet(null, "[Mailjet API Key]", "[Mailjet API Private Key]");
            SendGrid sendGrid = new SendGrid(null, "[SendGrid API Key]");
            GMail gmail = new GMail(new LocalReceiver(port), "[Client ID]", "", "http://localhost:" + port + "/auth", "state");

            List<String> recipients = new List<String>
            {
                "[recipient address]"
            };

            String sender = "[sender address]";
         

            String strPhoto = (@"../../cloudrail_image.jpg");
            FileStream fs = new FileStream(strPhoto, FileMode.Open, FileAccess.Read);

            Attachment attachment = new Attachment(fs, "image/jpg", "cloudrail_image.jpg");

            List<Attachment> attachments = new List<Attachment>
            {
                attachment
            };


            List<IEmail> services = new List<IEmail>
            {
                mailJet,
                sendGrid,
                gmail
            };

            foreach (IEmail service in services)
            {
                try
                {
                    service.SendEmail(sender, 
                                      "CloudRailTest", 
                                      recipients, 
                                      "This is cool from Cloudrail", 
                                      "Hi there,\\n\\nGo ahead and try it yourself!!", 
                                      "<strong>Hi there,<br/><br/>Go ahead and try it yourself!</strong><br/>Sent with " + service, 
                                      null, //cc Addresses
                                      null, //bcc Addresses
                                      attachments);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
