using System.Text;
using Nalantis.Services;
using Xunit;

namespace NalantisTest.UnitTests.Services
{
    public class ApiTest 
    {
        private readonly string _serviceUrl;
        private readonly string _username;
        private readonly string _password;

        public ApiTest()
        {
            DotNetEnv.Env.Load();

            _serviceUrl = System.Environment.GetEnvironmentVariable("TAUTONA_SERVICE_URL");
            _username = System.Environment.GetEnvironmentVariable("TAUTONA_USERNAME");
            _password = System.Environment.GetEnvironmentVariable("TAUTONA_PASSWORD");
        }

        [Fact]
        public void TestAnalyse()
        {
            var api = new Api(_serviceUrl, _username, _password);

            var job = Encoding.ASCII.GetBytes("we are looking for a java developer who speaks french and mandarin");
            
            var result = api.Analyse(job);

            Assert.Contains("Programmers", result.Concepts());
            Assert.Contains("Java", result.Concepts());
            Assert.Contains("French", result.Concepts());
            Assert.Contains("Chinese", result.Concepts());
            
            Assert.Contains("Domain_ICT", result.Domains);
            
        }

        [Fact]
        public void TestMatching()
        {
            var api = new Api(_serviceUrl, _username, _password);

            var job = Encoding.ASCII.GetBytes("we are looking for a java developer who speaks russian.");

            var result = api.Match(job);
            
            Assert.NotEmpty(result.Documents);
        }
    }
}