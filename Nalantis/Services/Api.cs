using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nalantis.Models;
using Newtonsoft.Json.Linq;

namespace Nalantis.Services 
{
    public class Api 
    {
        private readonly string _serviceUrl;

        private static readonly HttpClient Client = new HttpClient();

        public Api(string serviceUrl, string username, string password) 
        {
            _serviceUrl = serviceUrl;

            Client.DefaultRequestHeaders.Add("Accept", "application/json");
            
            var encoded = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encoded);

        }

        public AnalysisResult Analyse(byte[] file, string type = "PROFILE")
        {
            using (var analysis = DoAnalysis(file, type))
            {
                var result = analysis.Result;
                var json = JObject.Parse(result);
                
                return new AnalysisResult(json);
            }
        }

        public MatchResult Match(byte[] file, string type = "PROFILE")
        {
            using (var matchLocation = DoMatchCall(file, type))
            {
                var location = matchLocation.Result.Replace("http", "https");
                using (var matches = DoRetrieveMatches(location))
                {
                    var result = matches.Result;
                    var json = JObject.Parse(result);
                    
                    return new MatchResult(json);
                }
            }
        }

        private async Task<string> DoAnalysis(byte[] file, string type = "PROFILE")
        {
            var analysisUrl = _serviceUrl + "/v1/analysis";
            
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StreamContent(new MemoryStream(file)), "file", type +  ".txt");
                content.Add(new StringContent(type), "docType");
                
                using(var message = await Client.PostAsync(analysisUrl, content))
                {
                    var input = await message.Content.ReadAsStringAsync();

                    return input;
                }
            }
        }

        private async Task<string> DoMatchCall(byte[] file, string type = "PROFILE")
        {
            var cvsearchUrl = _serviceUrl + "/cvsearch";

            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StreamContent(new MemoryStream(file)), "file", type + ".txt");

                using (var message = await Client.PostAsync(cvsearchUrl, content))
                {
                    var location = message.Headers.Location.ToString();

                    return location;
                }
            }
        }

        private async Task<string> DoRetrieveMatches(string location)
        {
            var result = await Client.GetStringAsync(location);
            
            return result;
        }
    }
}