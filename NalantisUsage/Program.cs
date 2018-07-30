using System;
using System.IO;
using System.Text;
using Nalantis.Services;

namespace NalantisUsage
{
    class Program
    {
        static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var serviceUrl = System.Environment.GetEnvironmentVariable("TAUTONA_SERVICE_URL");
            var username = System.Environment.GetEnvironmentVariable("TAUTONA_USERNAME");
            var password = System.Environment.GetEnvironmentVariable("TAUTONA_PASSWORD");

            var api = new Api(serviceUrl, username, password);

            var job = File.ReadAllBytes("job.txt");
            
            Console.WriteLine("Uploading your job");

            var analysis= api.Analyse(job);
            
            Console.WriteLine("-------");
            Console.WriteLine("Highlights: ");

            foreach (var offset in analysis.Offsets)
            {
                var line = analysis.Content.Substring(offset.Start, offset.End - offset.Start + 1);
                Console.Write("\t> ");
                Console.WriteLine(line);
            }
            
            Console.WriteLine("-------");
            Console.WriteLine("Detected Concepts:");
            foreach (var concept in analysis.Concepts())
            {
                Console.Write("\t> ");
                Console.WriteLine(concept);
            }
            
            Console.WriteLine("-------");
        }
    }
}