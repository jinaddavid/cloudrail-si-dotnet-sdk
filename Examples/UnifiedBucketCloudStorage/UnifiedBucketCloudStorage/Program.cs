using System;
using System.Collections.Generic;
using System.IO;
using Com.CloudRail.SI;
using Com.CloudRail.SI.Interfaces;
using Com.CloudRail.SI.Services;
using Com.CloudRail.SI.Types;

namespace UnifiedBucketCloudStorage
{
    class MainClass
    {
        static IBusinessCloudStorage service;
        static Bucket currentBucket;

        public static void Main(string[] args)
        {
            CloudRail.AppKey = "[Your CloudRail Key]";

            String serviceName = SelectService();

            AmazonS3 amazons3 = new AmazonS3(
                null,
                "[Your S3 Access Key ID]",
                "[Your S3 Secret Access Key]",
                "[Your AWS region]"
            );

            Backblaze backblaze = new Backblaze(
                null,
                "[Your Backblaze Account ID]",
                "[Your Backblaze App Key]"
            );

            GoogleCloudPlatform googlecloudplatform = new GoogleCloudPlatform(
                null,
                "[Your Google Client Email]",
                "[Your Google Private Key]",
                "[Your Google Project ID]"
            );

            MicrosoftAzure microsoftazure = new MicrosoftAzure(
                null,
                "[Your Azure Account Name]",
                "[Your Azure Access Key]"
            );

            Rackspace rackspace = new Rackspace(
                null,
                "[Your Rackspace User Name]",
                "[Your Rackspace API Key]",
                "[Your Rackspace Region]"
            );

            switch (serviceName)
            {
                case "1": service = amazons3; break;
                case "2": service = backblaze; break;
                case "3": service = googlecloudplatform; break;
                case "4": service = microsoftazure; break;
                case "5": service = rackspace; break;
            }

            ShowBuckets();
            GetNextUserCommand();
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
                    case "buckets":
                        ShowBuckets();
                        break;
                    case "show":
                        ShowBucket(String.Join(" ", input));
                        break;
                    case "download":
                        Download(String.Join(" ", input));
                        break;
                    case "exit":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Unknown command. Try entering \"help\".\n");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            GetNextUserCommand();
        }



        static String SelectService()
        {
            String s = "1";
            Console.WriteLine("Enter the Service Number (e.g. 4 is for Microsoft Azure) you want to use:");
            Console.WriteLine("----");
            Console.WriteLine("1.Amazon S3\n2.Backblaze\n3.Google Cloud Platform\n4.Microsoft Azure\n5.Rackspace");
            Console.WriteLine("----");
            s = Console.ReadLine();
            Console.Clear();
            return s;
        }


        static void ShowHelp()
        {
            Console.WriteLine("Possible commands:");
            Console.WriteLine("\"help\" displays this help");
            Console.WriteLine("\"buckets\" shows a list of all Buckets.");
            Console.WriteLine("\"show bucketname\" shows the Bucket with the specified name.");
            Console.WriteLine("\"download fileName\" downloads the respective file from the currently displayed Bucket.");
            Console.WriteLine("\"exit\" quits the program.");
        }

        static void ShowBuckets()
        {
            Console.WriteLine("List of Buckets:");
            List<Bucket> buckets = service.ListBuckets();
            foreach (Bucket b in buckets)
            {
                Console.WriteLine(b.GetName());
            }
            Console.WriteLine("");
        }

        static void ShowBucket(String bucketName)
        {
            List<Bucket> buckets = service.ListBuckets();
            Bucket bucket = null;
            foreach (Bucket b in buckets)
            {
                if (b.GetName().Equals(bucketName))
                {
                    bucket = b;
                    currentBucket = b;
                }
            }
            List<BusinessFileMetaData> fileMetaDatas = service.ListFiles(bucket);
            foreach (BusinessFileMetaData fileMetaData in fileMetaDatas)
            {
                Console.WriteLine(fileMetaData.GetFileName());
            }
            Console.WriteLine("");
        }

        static void Download(String fileName)
        {
            service.DownloadFile(fileName, currentBucket);

            try
            {
                Stream downloadStream = service.DownloadFile(fileName, currentBucket);

                String destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                using (var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
                {
                    downloadStream.CopyTo(fileStream);
                }

                Console.WriteLine("File " + fileName + " from Bucket " + currentBucket.GetName() + " downloaded.\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
