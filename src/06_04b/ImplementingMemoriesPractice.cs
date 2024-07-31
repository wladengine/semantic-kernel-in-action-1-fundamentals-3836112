using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace _06_04b;

public class ImplementingMemoriesPractice
{
    private const string MemoryCollectionName = "aboutMe";

    public static async Task Execute()
    {
        var modelDeploymentName = "Gpt4v32k";
        var embeddingModelDeploymentName = "text-embedding-ada-002";
        var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZUREOPENAI_ENDPOINT");
        var azureOpenAIApiKey = Environment.GetEnvironmentVariable("AZUREOPENAI_APIKEY");

        #pragma warning disable SKEXP0001
        #pragma warning disable SKEXP0010
        #pragma warning disable SKEXP0050
        
        var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                modelDeploymentName,
                azureOpenAIEndpoint,
                azureOpenAIApiKey,
                modelId: "gpt-4-32k")
            .Build();

        IMemoryStore memoryStore = new VolatileMemoryStore();

        ISemanticTextMemory textMemory = new MemoryBuilder()
            .WithAzureOpenAITextEmbeddingGeneration(
                    embeddingModelDeploymentName,
                    azureOpenAIEndpoint,
                    azureOpenAIApiKey)
                .WithMemoryStore(memoryStore)
                .Build();

        // Creating and Adding the memory plugin to the kernel
        var memoryPlugin = kernel.ImportPluginFromObject(new TextMemoryPlugin(textMemory));

        // Adding some memories
        await kernel.InvokeAsync(memoryPlugin["Save"], new()
        {
            [TextMemoryPlugin.InputParam] = "I live in Zurich. ",
            [TextMemoryPlugin.CollectionParam] = MemoryCollectionName,
            [TextMemoryPlugin.KeyParam] = "info5",
        });

        await kernel.InvokeAsync(memoryPlugin["Save"], new()
        {
            [TextMemoryPlugin.InputParam] = "I love learning, AI, XR and complex challenges. ",
            [TextMemoryPlugin.CollectionParam] = MemoryCollectionName,
            [TextMemoryPlugin.KeyParam] = "info6",
        });

        // Recalling memories
        await Ask("Where do I live?", kernel, memoryPlugin);
        await Ask("What do I love?", kernel, memoryPlugin);

        Console.ReadLine();
    }

    private static async Task Ask(string ask, Kernel kernel, KernelPlugin memoryPlugin)
    {
        Console.WriteLine($"Ask: {ask}");

        var result = await kernel.InvokeAsync(memoryPlugin["Recall"], new()
        {
            [TextMemoryPlugin.InputParam] = ask,
            [TextMemoryPlugin.CollectionParam] = MemoryCollectionName,
            [TextMemoryPlugin.LimitParam] = "2",
            [TextMemoryPlugin.RelevanceParam] = "0.79",
        });

        Console.WriteLine($"Answer: {result.GetValue<string>()}");
    }
}