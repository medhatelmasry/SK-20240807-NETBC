using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Planning.Handlebars;
using PlannerWithSK.Plugins;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

// Get configuration settings from appsettings.json
string az_endpoint = config["AZ_ENDPOINT"]!;
string az_apiKey = config["AZ_KEY"]!;
string az_gptDeployment = config["AZ_CHAT_COMP_MODEL"]!;
string service = config["SERVICE"]!;
string oai_apiKey = config["OAI_KEY"]!;
string oai_gptDeployment = config["OAI_CHAT_COMP_MODEL"]!;

// Create builder
var builder = Kernel.CreateBuilder();

// Configure SK with the AI services 
if (service.Equals("openai", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddOpenAIChatCompletion(oai_gptDeployment, oai_apiKey);
}
else
{
    builder.Services.AddAzureOpenAIChatCompletion(az_gptDeployment, az_endpoint, az_apiKey);
}

// Create the kernel using the provided settings
var kernel = builder.Build();

// Load sematic & native plugin
var cityDetectionPluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "CityDetectionPlugin");
var writerPluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "WriterPlugin");

var detectCityNameFunction = kernel.ImportPluginFromPromptDirectory(cityDetectionPluginPath);
var summaryPlugin = kernel.ImportPluginFromPromptDirectory(writerPluginPath);
var weatherPlugin = kernel.ImportPluginFromType<WeatherService>;
var cityNamePlugin = kernel.ImportPluginFromType<CityInfo>();
var timePlugin = kernel.ImportPluginFromType<TimePlugin>();

// Construct the arguments
var arguments = new KernelArguments();

// Enable auto function calling
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

// Create planner
var planner = new HandlebarsPlanner();

// Start chat
while (true)
{
    // Start with a basic chat
    Console.ForegroundColor = ConsoleColor.DarkBlue;
    Console.Write("User > ");
    string? readUserInput = Console.ReadLine();

    Func<string, Task> Chat = async (string input) =>
    {
        arguments["input"] = input;
        var originalPlan = await planner.CreatePlanAsync(kernel, input);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Original plan: \n");
        Console.WriteLine(originalPlan);

        // Execute the plan
        var originalPlanResult = await originalPlan.InvokeAsync(kernel, new KernelArguments(openAIPromptExecutionSettings));
        Console.WriteLine("Plan results: \n");
        Console.WriteLine(originalPlanResult.ToString());
    };

    await Chat(readUserInput!);
}
