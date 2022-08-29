using Azure;
using Azure.AI.Language.QuestionAnswering;
using Bot_QnA;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

class Program
{
    static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();

        /*Uri endpointBOT = new Uri("https://langresurs.cognitiveservices.azure.com/");
        AzureKeyCredential credential = new AzureKeyCredential("d1edd582ceb743af8fc7b77eb6cbd812");
        string projectName = "Labb1AIKurs";
        string deploymentName = "production";

        string question = "I'm tired";
        

        QuestionAnsweringClient client = new QuestionAnsweringClient(endpointBOT, credential);
        QuestionAnsweringProject project = new QuestionAnsweringProject(projectName, deploymentName);

        Response<AnswersResult> response = client.GetAnswers(question, project);

        foreach (KnowledgeBaseAnswer answer in response.Value.Answers)
        {
            Console.WriteLine($"Q:{question}");
            Console.WriteLine($"A:{answer.Answer}");
            Console.WriteLine($"({answer.Confidence})");
        }*/
    }
    public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging((logging) =>
                    {
                        logging.AddDebug();
                        logging.AddConsole();
                    });
                    webBuilder.UseStartup<Startup>();
                });
}