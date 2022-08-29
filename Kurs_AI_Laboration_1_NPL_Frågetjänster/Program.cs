using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Azure;
using Azure.AI.TextAnalytics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.AI.Language.QuestionAnswering;

namespace Kurs_AI_Laboration_1_NPL_Frågetjänster
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string cogSvcKey = configuration["CognitiveServiceKey"];
                string cogSvcRegion = configuration["CognitiveServiceRegion"];
                string cogSvcEndpoint = configuration["CognitiveServicesEndpoint"];

                // Set console encoding to unicode
                Console.InputEncoding = Encoding.Unicode;
                Console.OutputEncoding = Encoding.Unicode;

                //NPL Client
                AzureKeyCredential credentials = new AzureKeyCredential(cogSvcKey);
                Uri endpointNPL = new Uri(cogSvcEndpoint);
                TextAnalyticsClient CogClient = new TextAnalyticsClient(endpointNPL, credentials);

                string text = "Jag heter Malin och jag bor i Sverige";

                // Detect the language
                string language = await GetLanguage(text);
                Console.WriteLine("Language: " + language);

                // Translate if not already English
                if (language != "en")
                {
                    string translatedText = await TranslateToEnglish(text, language);
                    Console.WriteLine("\nTranslation:\n" + translatedText);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public static async Task<string> GetLanguage(string text)
        {
            // Get config settings from AppSettings
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            string cogSvcKey = configuration["CognitiveServiceKey"];
            string cogSvcRegion = configuration["CognitiveServiceRegion"];
            string translatorEndpoint = configuration["TranslatorEndpoint"];

            // Default language is English
            string language = "en";

            // Use the Translator detect function
            object[] body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    // Build the request
                    string path = "/detect?api-version=3.0";
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(translatorEndpoint + path);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", cogSvcKey);
                    request.Headers.Add("Ocp-Apim-Subscription-Region", cogSvcRegion);

                    // Send the request and get response
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                    // Read response as a string
                    string responseContent = await response.Content.ReadAsStringAsync();
                    
                    // Parse JSON array and get language
                    var jsonResponse = JArray.Parse(responseContent);
                    language = jsonResponse[0]["language"].ToString();
                }
            }
            // return the language
            return language;
        }
        public static async Task<string> TranslateToEnglish(string text, string sourceLanguage)
        {
            // Get config settings from AppSettings
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            string cogSvcKey = configuration["CognitiveServiceKey"];
            string cogSvcRegion = configuration["CognitiveServiceRegion"];
            string translatorEndpoint = configuration["TranslatorEndpoint"];

            string translation = "";

            // Use the Translator translate function
            object[] body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    // Build the request
                    string path = "/translate?api-version=3.0&from=" + sourceLanguage + "&to=en";
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(translatorEndpoint + path);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", cogSvcKey);
                    request.Headers.Add("Ocp-Apim-Subscription-Region", cogSvcRegion);

                    // Send the request and get response
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                    // Read response as a string
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Parse JSON array and get translation
                    JArray jsonResponse = JArray.Parse(responseContent);
                    translation = jsonResponse[0]["translations"][0]["text"].ToString();
                }
            }
            // Return the translation
            return translation;

        }
        public static async Task<string> TranslateFromEnglish(string text, string sourceLanguage)
        {
            // Get config settings from AppSettings
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            string cogSvcKey = configuration["CognitiveServiceKey"];
            string cogSvcRegion = configuration["CognitiveServiceRegion"];
            string translatorEndpoint = configuration["TranslatorEndpoint"];

            string translation = "";

            // Use the Translator translate function
            object[] body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    // Build the request
                    string path = "/translate?api-version=3.0&from=en&to=" + sourceLanguage;
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(translatorEndpoint + path);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", cogSvcKey);
                    request.Headers.Add("Ocp-Apim-Subscription-Region", cogSvcRegion);

                    // Send the request and get response
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    // Read response as a string
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Parse JSON array and get translation
                    JArray jsonResponse = JArray.Parse(responseContent);
                    translation = jsonResponse[0]["translations"][0]["text"].ToString();
                }
            }
            Console.WriteLine($"Answer in English: {text}");
            // Return the translation
            return translation;

        }
    }
}




