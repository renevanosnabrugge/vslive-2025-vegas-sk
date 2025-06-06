using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace UseSemanticKernelFromNET
{
    public class BasicsOfSK
    {
        public async Task SimplestPromptLoop(string deploymentName, string endpoint, string apiKey)
        {
            Kernel kernel = Kernel.CreateBuilder().
                AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey).Build();

            string response = string.Empty;

            while (response != "quit")
            {
                Console.WriteLine("Enter your message:");
                response = Console.ReadLine();
                Console.WriteLine(await kernel.InvokePromptAsync(response));
            }
        }

        public async Task SimplestPromptLoopUsingConfig(string deploymentName, string endpoint, string apiKey)
        {
            Kernel kernel = Kernel.CreateBuilder().
                AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey).Build();

            string response = string.Empty;
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            while (response != "quit")
            {
                Console.WriteLine("Enter your message:");
                response = Console.ReadLine();
                Console.WriteLine(await chatCompletionService.GetChatMessageContentAsync(response));
            }
        }
        public async Task AddingMessageHistory(string deploymentName, string endpoint, string apiKey)
        {
            Kernel kernel = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey).Build(); ;

 


            string response = string.Empty;

            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            ChatHistory chatHistory = new();

            while (response != "quit")
            {
                Console.WriteLine("Enter your message:");
                response = Console.ReadLine();
                chatHistory.AddUserMessage(response);

                var assistantMessage = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
                Console.WriteLine(assistantMessage);
                chatHistory.Add(assistantMessage);
            }
        }

        public async Task AddingMessageHistoryWithLogging(string deploymentName, string endpoint, string apiKey)
        {
            var  builder = Kernel.CreateBuilder();
            builder.Services.AddLogging(
                                    s => s.AddConsole().SetMinimumLevel(LogLevel.Trace));

            var kernel = builder.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey).Build(); ;

            // Create an MCPClient for the GitHub server
            await using var mcpClient = await McpClientFactory.CreateAsync(new StdioClientTransport(new()
            {
                Name = "MCPServer",
                Command = "npx",
                Arguments = ["-y", "@modelcontextprotocol/server-github"],
            }));
            
            // Retrieve the list of tools available on the GitHub server
            var tools = await mcpClient.ListToolsAsync().ConfigureAwait(false);
            foreach (var tool in tools)
            {
                Console.WriteLine($"{tool.Name}: {tool.Description}");
            }

            kernel.Plugins.AddFromFunctions("GitHub", tools.Select(aiFunction => aiFunction.AsKernelFunction()));
            OpenAIPromptExecutionSettings executionSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true })
            };
            string response = string.Empty;


            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            ChatHistory chatHistory = new();
            chatHistory.AddSystemMessage("""
            You are a helpful and structured assistant designed to gather project information from consultants in order to populate a knowledge base of consultancy projects. Your goal is to ask clear, concise, and context-aware questions to collect all the necessary information for each project.

            For each project, gather and organize the following details. Introduce the section and ask one question at a time section at a time and allow the consultant to answer before proceeding. Use conversational yet professional tone, and ensure completeness and clarity in responses. If something is unclear or missing, ask follow-up questions.

             Ask:

            * What was the **name** of this project?
            * Who was the **client**? *(Once the name is shared, look up basic public background info about the company to enrich the context.)*
            * In which **country or region** did the project take place?
            * What were the **start and end dates**?
 
            Always clarify vague answers, summarize inputs back to the consultant for confirmation, and suggest categories or tags based on responses. Once the project record is complete, confirm with the consultant if they’d like to add anything or mark it as final then create a markdown file woth all the information gathered and create a GitHub Issue in the current repository
            Start conversation with the consultant by asking the first question in this section.
            """
            );

            while (response != "quit")
            {
                Console.WriteLine("Enter your message:");
                response = Console.ReadLine();
                chatHistory.AddUserMessage(response);

                var assistantMessage = await chatCompletionService.GetChatMessageContentAsync(chatHistory,executionSettings);
                Console.WriteLine(assistantMessage);
                chatHistory.Add(assistantMessage);
            }
        }


        public async Task SimpleContentStreaming(string deploymentName, string endpoint, string apiKey)
        {
            var builder = Kernel.CreateBuilder();
 
            Kernel kernel = builder.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey).Build();

            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            ChatHistory chatHistory = new("You are an AI tourguide that ensures people have a great time when they travel.");

            chatHistory.AddAssistantMessage("Welcome to the tourguide chat. How can I help you today?");
            var message = chatHistory.Last();
            Console.WriteLine($"{message.Role}: {message.Content}");

            chatHistory.AddUserMessage("I want to get information about Orlando");
            message = chatHistory.Last();
            Console.WriteLine($"{message.Role}: {message.Content}");

            await foreach (StreamingChatMessageContent chatUpdate in chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory))
            {
                Console.Write(chatUpdate.Content);
            }
            Console.ReadLine();
        }

        public async Task ChangeOpenAISettings(string deploymentName, string endpoint, string apiKey)
        {

            var builder = Kernel.CreateBuilder();
            builder.Services.AddLogging(
                                    s => s.AddConsole().SetMinimumLevel(LogLevel.Trace));

            Kernel kernel = builder.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey).Build();


            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            KernelArguments arguments = new(new OpenAIPromptExecutionSettings 
            { MaxTokens = 500, Temperature = .1});
            Console.WriteLine("************** Temperature .1:***************");
            Console.WriteLine(await kernel.InvokePromptAsync("The sun is: ", arguments));
            Console.WriteLine("************** Temperature .1:***************");
            Console.WriteLine(await kernel.InvokePromptAsync("The sun is: ", arguments)); 
            Console.WriteLine("************** Temperature .1:***************");
            Console.WriteLine(await kernel.InvokePromptAsync("The sun is: ", arguments));

            arguments = new(new OpenAIPromptExecutionSettings 
            { MaxTokens = 500, Temperature = .9 });
            Console.WriteLine("************** Temperature .9:***************");
            Console.WriteLine(await kernel.InvokePromptAsync("The sun is: ", arguments));
            Console.WriteLine("************** Temperature .9:***************");
            Console.WriteLine(await kernel.InvokePromptAsync("The sun is: ", arguments));
            Console.WriteLine("************** Temperature .9:***************");
            Console.WriteLine(await kernel.InvokePromptAsync("The sun is: ", arguments));
            Console.WriteLine("************** Temperature .9:***************");
            Console.WriteLine(await kernel.InvokePromptAsync("The sun is: ", arguments));
        }


    }
}
