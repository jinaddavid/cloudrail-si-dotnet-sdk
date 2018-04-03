using System;
using System.Collections.Generic;
using System.IO;
using Com.CloudRail.SI;
using Com.CloudRail.SI.Interfaces;
using Com.CloudRail.SI.ServiceCode.Commands.CodeRedirect;
using Com.CloudRail.SI.Services;
using Com.CloudRail.SI.Types;

namespace UnifiedCloudStorage
{
    class MainClass
    {
        static ICloudStorage service;
        static String currentPath = "/";

        public static void Main(string[] args)
        {
            CloudRail.AppKey = "[Your Cloudrail Key]";

            int port = 8082;

            String serviceName = SelectService();

            Box box = new Box(
                new LocalReceiver(port),
                "[Box Client Identifier]",
                "[Box Client Secret]",
                "http://localhost:" + port + "/",
                "someState"
            );

            Dropbox dropbox = new Dropbox(
                    new LocalReceiver(port),
                    "[Dropbox Client Identifier]",
                    "[Dropbox Client Secret]",
                    "http://localhost:" + port + "/",
                    "someState"
                    );

            Egnyte egnyte = new Egnyte(
                    new LocalReceiver(port),
                    "[Your Egnyte Domain]",
                    "[Your Egnyte API Key]",
                    "[Your Egnyte Shared Secret]",
                    "http://localhost:" + port + "/",
                    "someState"
                    );

            GoogleDrive googledrive = new GoogleDrive(
                    new LocalReceiver(port),
                    "[Google Drive Client Identifier]",
                    "",
                    "http://localhost:" + port + "/",
                    "someState"
                    );

            OneDrive onedrive = new OneDrive(
                    new LocalReceiver(port),
                    "[OneDrive Client Identifier]",
                    "[OneDrive Client Secret]",
                    "http://localhost:" + port + "/",
                    "someState"
                    );

            OneDriveBusiness onedrivebusiness = new OneDriveBusiness(
                    new LocalReceiver(port),
                    "[OneDrive Business Client Identifier]",
                    "[OneDrive Business Client Secret]",
                    "http://localhost:" + port + "/",
                    "someState"
                    );

            PCloud pCloud = new PCloud(new LocalReceiver(port),
                    "[pCloud Client Identifier]",
                    "[pCloud Client Secret]",
                    "http://localhost:" + port + "/",
                    "someState");

            service = null;
            switch (serviceName)
            {
                case "1":
                    Console.WriteLine("Selected Service: Box");
                    service = box;
                    break;
                case "2":
                    Console.WriteLine("Selected Service: Dropbox");
                    service = dropbox;
                    break;
                case "3":
                    Console.WriteLine("Selected Service: Egnyte");
                    service = egnyte;
                    break;
                case "4":
                    Console.WriteLine("Selected Service: Google Drive");
                    service = googledrive;
                    break;
                case "5":
                    Console.WriteLine("Selected Service: Onedrive");
                    service = onedrive;
                    break;
                case "6":
                    Console.WriteLine("Selected Service: Onedrive for business");
                    service = onedrivebusiness;
                    break;
                case "7":
                    Console.WriteLine("Selected Service: pCloud");
                    service = pCloud;
                    break;
            }


            Login();
            ShowPath();
            GetNextUserCommand();
        }


        static String SelectService()
        {
            String s = "1";
            Console.WriteLine("Enter the Service Number (e.g. 1 is for Box) you want to use:");
            Console.WriteLine("----");
            Console.WriteLine("1.Box\n2.Dropbox\n3.Egnyte\n4.Google Drive\n5.Onedrive\n6.Onedrive for business\n7.pCloud");
            Console.WriteLine("----");
            s = Console.ReadLine();
            Console.Clear();
            return s;
        }

        static void Login()
        {
            try
            {
                service.Login();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

        }


        static void ShowPath()
        {
            try
            {
                Console.WriteLine("Showing folder " + currentPath);
                List<CloudMetaData> children = service.GetChildren(currentPath);

                foreach (CloudMetaData c in children)
                {
                    Console.WriteLine(c.GetName());
                }
                Console.WriteLine("");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void GetNextUserCommand()
        {
            try
            {
                ShowHelp();
                String cmdLine = Console.ReadLine();

                String[] stringSeparators = { " " };

                List<String> input = new List<String>(cmdLine.Split(stringSeparators, StringSplitOptions.None));
                String cmd = input[0];
                input.RemoveAt(0);

                switch (cmd)
                {
                    case "help":
                        ShowHelp();
                        break;
                    case "cd":
                        CD(String.Join(" ", input));
                        break;
                    case "download":
                        Download(String.Join(" ", input));
                        break;
                    case "upload":
                        Upload();
                        break;
                    case "exit":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Unknown command. Try entering \"help\".\n");
                        break;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            GetNextUserCommand();
        }


        static void ShowHelp()
        {
            Console.WriteLine("\nSelect Commands:");
            Console.WriteLine("----");
            Console.WriteLine("\"help\" displays this help");
            Console.WriteLine("\"cd relativePath\" opens a child folder, where relativePath is its path starting from the currently displayed folder.");
            Console.WriteLine("\"cd ..\" goes to the current folder's parent folder.");
            Console.WriteLine("\"download fileName\" downloads the respective file from the currently displayed folder.");
            Console.WriteLine("\"upload \" upload sample image file at root folder");
            Console.WriteLine("\"exit\" quits the program.");
            Console.WriteLine("----");
        }

        static void CD(String change)
        {
            if (change.Equals(".."))
            {
                List<String> path = new List<String>((currentPath.Split('/')));

                if (path.Count > 0)
                {
                    path.RemoveAt(path.Count - 1);
                }

                String newPath = String.Join("/", path);

                if (newPath.Equals(""))
                {
                    newPath = "/";
                }

                currentPath = newPath;
            }
            else
            {
                if (!currentPath.Equals("/"))
                {
                    currentPath = currentPath + "/";
                }
                currentPath = currentPath + change;
            }

            ShowPath();
        }


        static void Download(String input)
        {
            try
            {
                String pathToFile = currentPath;
                if (!pathToFile.Equals("/")) 
                { 
                    pathToFile = pathToFile + "/"; 
                }

                pathToFile = pathToFile + input;

                Stream downloadStream = service.Download(pathToFile);

                String destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, input);

                using (var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
                {
                    downloadStream.CopyTo(fileStream);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        static void Upload()
        {
            try
            {
                String pathToFile = "/cloudrail_image.jpg";

                String strPhoto = (@"../../cloudrail_image.jpg");
                FileStream fs = new FileStream(strPhoto, FileMode.Open, FileAccess.Read);
                service.Upload(pathToFile,fs,fs.Length,true);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
