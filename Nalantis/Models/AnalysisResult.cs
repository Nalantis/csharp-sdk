using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Nalantis.Models
{
    public class AnalysisResult
    {
        public Dictionary<string, List<string>> ConceptsPerCategory { get; }
        public List<string> Domains { get; }
        public List<Offset> Offsets { get; }
        public readonly string Content;
      

        public AnalysisResult(JObject analysisResult)
        {
            ConceptsPerCategory = new Dictionary<string, List<string>>();
            Domains = new List<string>();
            Offsets = new List<Offset>();
            Content = analysisResult["content"].Value<string>();

            foreach (var cat in analysisResult["category"])
            {
                var category = cat["name"].Value<string>();
                if (!Standards.Categories.Contains(category)) continue;

                ConceptsPerCategory[category] = new List<string>();
                
                foreach (var concept in cat["concept"])
                {
                    var n = concept["name"].Value<string>();
                    ConceptsPerCategory[category].Add(n);
                }
            }

            foreach (var sm in analysisResult["semanticMatches"])
            {
                var offsets = sm["offsets"];
                var begin = offsets["begin"].Value<int>();
                var end = offsets["end"].Value<int>();
                
                var offset = new Offset(begin, end);
                 Offsets.Add(offset);
            }

            foreach (var domain in analysisResult["domains"])
            {
                Domains.Add(domain.Value<string>());
            }
            
        }

        public IEnumerable<string> Concepts()
        {
            var flattenedList = ConceptsPerCategory.SelectMany(x => x.Value);

            return flattenedList;
        }
    }

    public class Offset
    {
        public int Start { get; }
        public int End { get; }

        public Offset(int start, int end)
        {
            this.Start = start;
            this.End = end;
        }
    }
}