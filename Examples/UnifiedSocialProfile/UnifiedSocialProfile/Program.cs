using System;
using Com.CloudRail.SI.Interfaces;
using Com.CloudRail.SI;
using Com.CloudRail.SI.Types;
using Com.CloudRail.SI.Services;
using Com.CloudRail.SI.ServiceCode.Commands.CodeRedirect;

namespace UnifiedSocialProfile
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            String serviceName = "facebook";

            IProfile service = InitService(serviceName);

            service.Login();
            String name = service.GetFullName();
            String mail = service.GetEmail();
            DateOfBirth dob = service.GetDateOfBirth();
            String dobString = dob.GetDay() + "." + dob.GetMonth() + "." + dob.GetYear();
            String id = service.GetIdentifier();

            Console.WriteLine("Logged in to " + args[0]);
            Console.WriteLine("logged in as " + name + " (email: " + mail + "), born " + dobString);
            Console.WriteLine("unique identifier (can be used for social login): " + id);

        }

        static IProfile InitService(String serviceName)
        {
            CloudRail.AppKey =  "[Your CloudRail Key]";
            int port = 8082;
            String redirectUri = "http://localhost:" + port + "/";

            IProfile service = null;

            switch (serviceName.ToLower())
            {
                case "facebook":
                    service = new Facebook(
                            new LocalReceiver(port),
                            "[Facebook Client Identifier]",
                            "[Facebook Client Secret]",
                            redirectUri,
                            "someState"
                        );
                    break;
                case "github":
                    service = new GitHub(
                            new LocalReceiver(port),
                            "[GitHub Client Identifier]",
                            "[GitHub Client Secret]",
                            redirectUri,
                            "someState"
                        );
                    break;
                case "googleplus":
                    service = new GooglePlus(
                            new LocalReceiver(port),
                            "[Google Plus Client Identifier]",
                            "[Google Plus Client Secret]",
                            redirectUri,
                            "someState"
                        );
                    break;
                case "heroku":
                    service = new Heroku(
                            new LocalReceiver(port),
                            "[Heroku Client Identifier]",
                            "[Heroku Client Secret]",
                            redirectUri,
                            "someState"
                        );
                    break;
                case "instagram":
                    service = new Instagram(
                            new LocalReceiver(port),
                            "[Instagram Client Identifier]",
                            "[Instagram Client Secret]",
                            redirectUri,
                            "someState"
                        );
                    break;
                case "linkedin":
                    service = new LinkedIn(
                            new LocalReceiver(port),
                            "[LinkedIn Client Identifier]",
                            "[LinkedIn Client Secret]",
                            redirectUri,
                            "someState"
                        );
                    break;
                case "producthunt":
                    service = new ProductHunt(
                            new LocalReceiver(port),
                            "[Product Hunt Client Identifier]",
                            "[Product Hunt Client Secret]",
                            redirectUri,
                            "someState"
                        );
                    break;
                case "slack":
                    service = new Slack(
                            new LocalReceiver(port),
                            "[Slack Client Identifier]",
                            "[Slack Client Secret]",
                            redirectUri,
                            "someState"
                        );
                    break;
                case "twitter":
                    service = new Twitter(
                            new LocalReceiver(port),
                            "[Twitter Client Identifier]",
                            "[Twitter Client Secret]",
                            redirectUri
                        );
                    break;
                case "microsoftlive":
                    service = new MicrosoftLive(
                            new LocalReceiver(port),
                            "[Windows Live Client Identifier]",
                            "[Windows Live Client Secret]",
                            redirectUri,
                            "someState"
                        );
                    break;
                case "yahoo":
                    service = new Yahoo(
                            new LocalReceiver(port),
                            "[Yahoo Client Identifier]",
                            "[Yahoo Client Secret]",
                            redirectUri,
                            "someState"
                        );
                    break;
            }

            return service;
        }
    }
}
