using Microsoft.Extensions.Configuration;
using UseSemanticKernelFromNET;
using Microsoft.Extensions.Configuration.UserSecrets;
using UseSemanticKernelFromNET.Plugins;
using System.Configuration;

var builder = new ConfigurationBuilder();
builder.SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddUserSecrets<Program>();

IConfiguration config = builder.Build();

string deploymentName = config.GetSection("OpenAI").GetValue<string>("Model") ?? throw new ArgumentException("OpenAI Model not set");
string endpoint = config.GetSection("OpenAI").GetValue<string>("EndPoint")?? throw new ArgumentException("OpenAI EndPoint not set");
string apiKey = config.GetSection("OpenAI").GetValue<string>("ApiKey") ?? throw new ArgumentException("OpenAIKey not set");

//1. Basics of SK
//await new BasicsOfSK().SimplestPromptLoop(deploymentName,endpoint,apiKey);
//await new BasicsOfSK().SimplestPromptLoopUsingConfig(deploymentName, endpoint, apiKey);
await new BasicsOfSK().AddingMessageHistory(deploymentName,endpoint,apiKey);
//await new BasicsOfSK().AddingMessageHistoryWithLogging(deploymentName, endpoint, apiKey);

//2. Configuration
//await new BasicsOfSK().ChangeOpenAISettings(deploymentName,endpoint,apiKey);

// Streaming content
//await new BasicsOfSK().SimpleContentStreaming(deploymentName, endpoint, apiKey);


//3. Function Calling
//await new UsingPlugins().GetDaysUntilChristmas(deploymentName,endpoint,apiKey);
//await new UsingPlugins().ChatWithMultiplePlugins(deploymentName,endpoint,apiKey);
//await new UsingPlugins().ChatWithPluginsAndConsentFilter(deploymentName,endpoint,apiKey);


//string dal_e_endpoint = config.GetSection("OpenAI").GetValue<string>("DalE-EndPoint") ?? throw new ArgumentException("OpenAI EndPoint not set");
//string dal_e_apiKey = config.GetSection("OpenAI").GetValue<string>("DalE-ApiKey") ?? throw new ArgumentException("OpenAIKey not set");
//4. Image Generation
//await new ImageGeneration().GenerateBasicImage("dall-e-3", dal_e_endpoint, dal_e_apiKey );


//5. multi modal
//await new MultiModal(config).IntepretAnImageAndProvideSuggestions(deploymentName, endpoint, apiKey);

//string memory_key = config.GetSection("SM").GetValue<string>("key") ?? throw new ArgumentException("Semeantic Memory Key not set");
//string memory_ip = config.GetSection("SM").GetValue<string>("ip") ?? throw new ArgumentException("Semeantic Memory http location not set");

//6. Memory (RAG)
//await new UsingMemory().ChatWithMemory(deploymentName, endpoint, apiKey);

























//new FindDomainsBasedOnCompany().UpdateExcellSheet();
//await new UsingPlugins().ChatWithHotelPlugin(deploymentName,endpoint,apiKey, config);
//await new UsingPlugins().ChatWithSearchPlugin(deploymentName,endpoint,apiKey);
//await new UsingPlugins().ChatWithNavigatorPlugins(deploymentName,endpoint,apiKey, config);
//await new FindDomainsBasedOnCompany().FindAndVerifyDomains(deploymentName, endpoint, apiKey);
//await new UsingMemory().ChatWithMemoryExample(deploymentName, endpoint, apiKey);    
//await new UsingMemory().ChatWithWebMemory(memory_ip, memory_key);
//await new UsingMemory().IngestXebiaNavigatorIntoMemory(memory_ip, memory_key);
//await new UsingMemory().ChatWithXebiaNavigator(deploymentName, endpoint, apiKey,memory_ip, memory_key);
//await new UsingMemory().ListIndexes(memory_ip, memory_key);