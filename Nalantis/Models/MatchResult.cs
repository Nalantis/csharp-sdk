using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Nalantis.Models
{
    public class MatchResult
    {
        public readonly List<Document> Documents;
        
        
        public MatchResult(JObject json)
        {
            Documents = new List<Document>();

            foreach (var document in json["documents"])
            {
                var doc = new Document(document);
                Documents.Add(doc);
            }
        }
    }

    public class Document
    {
        public readonly Dictionary<string, List<MatchPair>> MatchingConceptsPerCategory;

        public Document(JToken json)
        {
            MatchingConceptsPerCategory = new Dictionary<string, List<MatchPair>>();

            foreach (var category in json["matchdetailsXML"]["category"])
            {
                var cat = category["name"].Value<string>();
                if (!Standards.Categories.Contains(cat)) continue;
                
                MatchingConceptsPerCategory[cat] = new List<MatchPair>();

                foreach (var cm in category["conceptMatch"])
                {
                    var profileConceptName = cm["profileConcept"]["conceptName"].Value<string>();
                    
                    var pair = new MatchPair(profileConceptName);
                    
                    foreach (var cvConcept in cm["cvConcept"])
                    {
                        pair.AddCvConcept(cvConcept["conceptName"].Value<string>());
                    }
                }

            }
        }
    }

    public class MatchPair
    {
        private readonly List<string> _cvConcepts;
        private readonly string _jobConcept;

        public string JobConcept => _jobConcept;
        public List<string> CvConcepts => _cvConcepts;

        public MatchPair(string jobConcept)
        {
            _jobConcept = jobConcept;
            _cvConcepts = new List<string>();
        }

        public void AddCvConcept(string concept)
        {
            _cvConcepts.Add(concept);
        }

    }
}