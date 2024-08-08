using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var oaiModel = config["OAI_TXT_GEN_MODEL"]!;
var azModel = config["AZ_TXT_GEN_MODEL"]!;
var azKey = config["AZ_KEY"]!;
var oaiKey = config["OAI_KEY"]!;
var service = config["SERVICE"]!;
var endpoint = config["AZ_ENDPOINT"]!;

var builder = Kernel.CreateBuilder();

if (service.Equals("openai", StringComparison.OrdinalIgnoreCase))
    builder.Services.AddOpenAITextGeneration(oaiModel,oaiKey);
else
    builder.Services.AddAzureOpenAITextGeneration(azModel, endpoint, azKey);

var kernel = builder.Build();

var functionDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "Baking");
var semanticFunctions = kernel.ImportPluginFromPromptDirectory(functionDirectory);

/* request user for input */
Console.ForegroundColor = ConsoleColor.DarkBlue;
Console.Write("Enter a cake type you want to bake: ");
var cakeType = Console.ReadLine();

var functionResult = await kernel.InvokeAsync(semanticFunctions["CakeRecipe"],
    new KernelArguments {
                { "input", cakeType }
    });
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine(functionResult);
Console.WriteLine();
