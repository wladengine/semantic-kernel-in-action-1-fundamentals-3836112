using Microsoft.SemanticKernel;

namespace _03_05b;

public class NativeFunctions
{
    public static async Task Execute()
    {
        var modelDeploymentName = "Gpt4v32k";
        var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZUREOPENAI_ENDPOINT");
        var azureOpenAIApiKey = Environment.GetEnvironmentVariable("AZUREOPENAI_APIKEY");
        
        var builder = Kernel.CreateBuilder();
        builder.Services.AddAzureOpenAIChatCompletion(
            modelDeploymentName,
            azureOpenAIEndpoint,
            azureOpenAIApiKey,
            modelId: "gpt-4-32k"
        );

        builder.Plugins.AddFromType<MyMathPlugin>();
           
        var kernel = builder.Build();

        var num = 81;

        var result = await kernel.InvokeAsync("MyMathPlugin", "Sqrt", new KernelArguments() { { "number", num }});

        System.Console.WriteLine(result);
    }
}