using Microsoft.SemanticKernel;

namespace _02_05b;

class TryingOutTheKernel
{
    public static async Task Execute()
    {
        var modelDeploymentName = "Gpt4v32k";
        var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZUREOPENAI_ENDPOINT");
        var azureOpenAIKey = Environment.GetEnvironmentVariable("AZUREOPENAI_APIKEY");

        var builder = Kernel.CreateBuilder();

        builder.Services.AddAzureOpenAIChatCompletion(
            deploymentName: modelDeploymentName,
            endpoint: azureOpenAIEndpoint,
            apiKey: azureOpenAIKey);

            
        var kernel = builder.Build();

        var topic = "The Semantic Kernel SDK has been born and is out to the world on December 19th, now all .NET developers are AI developers...";
        var prompt = $"Generate a very short funny poem about the given event. Be creative and be funny. Make the words rhyme together. Let your imagination run wild. Event:{topic}";

        var result = await kernel.InvokePromptAsync(prompt);

        System.Console.WriteLine(result);
    }
}