using Microsoft.SemanticKernel;

namespace _03_03b;

class PromptFunctions
{
    private static string someText = "Effective prompt design is essential to achieving desired outcomes with LLM AI models. Prompt engineering, also known as prompt design, is an emerging field that requires creativity and attention to detail. It involves selecting the right words, phrases, symbols, and formats that guide the model in generating high-quality and relevant texts.\r\n\r\nIf you've already experimented with ChatGPT, you can see how the model's behavior changes dramatically based on the inputs you provide. For example, the following prompts produce very different outputs:";

    public static async Task Execute()
    {
        var modelDeploymentName = "Gpt4v32k";
        var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZUREOPENAI_ENDPOINT");
        var azureOpenAIApiKey = Environment.GetEnvironmentVariable("AZUREOPENAI_APIKEY");

        var builder = Kernel.CreateBuilder();

        builder.Services.AddAzureOpenAIChatCompletion(
            deploymentName: modelDeploymentName,
            endpoint: azureOpenAIEndpoint,
            apiKey: azureOpenAIApiKey,
            modelId: "gpt-4-32k"
        );

        var kernel = builder.Build();

        //await CreateAndExecutePrompt(kernel);

        await ImportPluginFromFolderAndExecute(kernel);
    }

    private static async Task CreateAndExecutePrompt(Kernel kernel)
    {
        var prompt = "What is the meaning of the life?";
        var kernelFunction = kernel.CreateFunctionFromPrompt(prompt);

        var result = await kernelFunction.InvokeAsync(kernel);

        System.Console.WriteLine(result);
    }

    private static async Task ImportPluginFromFolderAndExecute(Kernel kernel)
    {
        /*
        var dir = Path.Combine(Directory.GetCurrentDirectory(), "plugins", "SummarizePlugin");
        var plugin = kernel.ImportPluginFromPromptDirectory(dir);

        var summarizeResult = await kernel.InvokeAsync(
            "SummarizePlugin", 
            "Summarize", 
            new ()
            {
                { "input", someText }
            });

        System.Console.WriteLine(summarizeResult);

        */

        // Import a plugin from a prompt directory
        var SummarizePluginDirectory = Path.Combine(
            System.IO.Directory.GetCurrentDirectory(),
            "plugins",
            "SummarizePlugin");
        kernel.ImportPluginFromPromptDirectory(SummarizePluginDirectory);

        var summarizeResult =
            await kernel.InvokeAsync(
            "SummarizePlugin",
            "Summarize",
            new() {
                { "input", someText }
            });

        Console.WriteLine($"Result:  {summarizeResult}");
    }
}