// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EmptyBot .NET Template version v4.17.1

using Azure;
using Azure.AI.Language.QuestionAnswering;
using Azure.AI.TextAnalytics;
using Kurs_AI_Laboration_1_NPL_Frågetjänster;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace QnA_Bot
{
    public class QnA : ActivityHandler
    {

        protected string defaultWelcome = "Hello, Bonjour, Hola, Hej";
        Uri endpointBOT;
        AzureKeyCredential credential;
        string projectName = "Labb1AIKurs";
        string deploymentName = "production";
        
        public QnA()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            string langKey = configuration["LanguageEndpointKey"];
            string langEndpoint = configuration["LanguageEndpointHostName"];

            credential = new AzureKeyCredential(langKey);
            endpointBOT = new Uri(langEndpoint);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            string inputMessage = turnContext.Activity.Text;
            string inputLanguage = await Kurs_AI_Laboration_1_NPL_Frågetjänster.Program.GetLanguage(inputMessage);

            if (inputLanguage != "en")
            {
                string translatedMessage = await Kurs_AI_Laboration_1_NPL_Frågetjänster.Program.TranslateToEnglish(inputMessage, inputLanguage);
                Console.WriteLine($"Translation: {translatedMessage}");
                string responseMessage = "";

                QuestionAnsweringClient client = new QuestionAnsweringClient(endpointBOT, credential);
                QuestionAnsweringProject project = new QuestionAnsweringProject(projectName, deploymentName);

                Response<AnswersResult> response = client.GetAnswers(translatedMessage, project);

                foreach (KnowledgeBaseAnswer answer in response.Value.Answers)
                {
                    responseMessage = answer.Answer;
                    responseMessage = await Kurs_AI_Laboration_1_NPL_Frågetjänster.Program.TranslateFromEnglish(answer.Answer, inputLanguage);
                }
                await turnContext.SendActivityAsync(MessageFactory.Text(responseMessage, responseMessage), cancellationToken);
            }
            else
            {
                string responseMessage = "";

                QuestionAnsweringClient client = new QuestionAnsweringClient(endpointBOT, credential);
                QuestionAnsweringProject project = new QuestionAnsweringProject(projectName, deploymentName);

                Response<AnswersResult> response = client.GetAnswers(inputMessage, project);

                foreach (KnowledgeBaseAnswer answer in response.Value.Answers)
                {
                    responseMessage = answer.Answer;
                }
                await turnContext.SendActivityAsync(MessageFactory.Text(responseMessage, responseMessage), cancellationToken);
            }
            
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(defaultWelcome), cancellationToken);
                }
            }
        }
        
    }
}
