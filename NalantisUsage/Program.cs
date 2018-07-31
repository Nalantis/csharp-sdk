using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Nalantis.Models;
using Nalantis.Services;

namespace NalantisUsage
{
    static class Program
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

            var analysis = api.Analyse(job);
            
            Console.WriteLine("-------");
            Console.WriteLine("Highlights: ");

            foreach (var offset in analysis.Offsets)
            {
                var line = analysis.Content.Substring(offset.Start, offset.Length());
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
            Console.WriteLine("Concepts per category");

            Console.WriteLine("\t> Domains:");
            foreach (var domain in analysis.Domains)
            {
                Console.WriteLine("\t\t> " + domain);
            }
            
            foreach (KeyValuePair<string,List<string>> pair in analysis.ConceptsPerCategory)
            {
                Console.WriteLine("\t> " + pair.Key + ":");
                foreach (var concept in pair.Value)
                {
                    Console.WriteLine("\t\t> " + concept);
                }
            }

            var matches = api.Match(job);
            Console.WriteLine("Matches:");
            foreach (var document in matches.Documents)
            {
                Console.WriteLine("\t> New Document: ");
                foreach (KeyValuePair<string,List<MatchPair>> pair in document.MatchingConceptsPerCategory)
                {
                    Console.WriteLine("\t\t> " + pair.Key + ":");
                    foreach (var matchPair in pair.Value)
                    {
                        Console.WriteLine("\t\t\t> " + matchPair.JobConcept);
                        foreach (var cvConcept in matchPair.CvConcepts)
                        {
                            Console.WriteLine("\t\t\t\t> " + cvConcept);
                        }
                    }
                }
            }
        }
    }
}