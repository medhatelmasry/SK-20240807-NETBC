using Microsoft.SemanticKernel;
using System.Text;
using Microsoft.SemanticKernel.ChatCompletion;

// Initialize the Semantic kernel
var builder = Kernel.CreateBuilder();

// We use Semantic Kernel OpenAI API
builder.Services.AddOpenAIChatCompletion(
        modelId: "phi3",
        apiKey: null,
        endpoint: new Uri("http://localhost:11434"));

var kernel = builder.Build();

// Create a new chat
var ai = kernel.GetRequiredService<IChatCompletionService>();
ChatHistory chat = new("You are an AI assistant that helps people find information.");
StringBuilder stringBuilder = new();

// User question & answer loop
while (true)
{
    Console.Write("Question: ");
    chat.AddUserMessage(Console.ReadLine()!);

    stringBuilder.Clear();

    // Get the AI response streamed back to the console
    await foreach (var message in ai.GetStreamingChatMessageContentsAsync(chat, kernel: kernel))
    {
        Console.Write(message);
        stringBuilder.Append(message.Content);
    }
    Console.WriteLine();
    chat.AddAssistantMessage(stringBuilder.ToString());

    Console.WriteLine();
}
