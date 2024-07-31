using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Experimental.Agents;

namespace _05_03b;

#pragma warning disable SKEXP0101 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
public class AgentCraftingPractice
{
    // Track agents for clean-up
    readonly List<IAgent> _agents = new();

    IAgentThread? _agentsThread = null;

    public async Task Execute()
    {
        var openAIFunctionEnabledModelId = "gpt-4-turbo-preview";
        //var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_APIKEY");
        var builder = Kernel.CreateBuilder();
        // builder.Services.AddOpenAIChatCompletion(
        //         openAIFunctionEnabledModelId,
        //         openAIApiKey);

        var modelDeploymentName = "Gpt4v32k";
        var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZUREOPENAI_ENDPOINT");
        var azureOpenAIApiKey = Environment.GetEnvironmentVariable("AZUREOPENAI_APIKEY");

        builder.Services.AddAzureOpenAIChatCompletion(
            modelDeploymentName,
            azureOpenAIEndpoint,
            azureOpenAIApiKey,
            modelId: "gpt-4-32k"
        );
        var kernel = builder.Build();

        // create agent in code
        var codeAgent = await new AgentBuilder()
            .WithAzureOpenAIChatCompletion(azureOpenAIEndpoint, "gpt-4-32k", azureOpenAIApiKey)
            .WithInstructions("Repeat the user message in the voice of a pirate " +
            "and then end with parrot sounds.")
            .WithName("CodeParrot")
            .WithDescription("A fun chat bot that repeats the user message in the" +
            " voice of a pirate.")
            .BuildAsync();
        _agents.Add(codeAgent);

        // Create agent from file
        var pathToPlugin = Path.Combine(Directory.GetCurrentDirectory(), "Agents", "ParrotAgent.yaml");
        string agentDefinition = File.ReadAllText(pathToPlugin);
        var fileAgent = await new AgentBuilder()
                .WithAzureOpenAIChatCompletion(azureOpenAIEndpoint, "gpt-4-32k", azureOpenAIApiKey)
                .FromTemplatePath(pathToPlugin)
                .BuildAsync();
        _agents.Add(fileAgent);

        try
        {
            // Invoke agent plugin.
            var response =
                    await fileAgent.AsPlugin().InvokeAsync(
                            "Practice makes perfect.",
                            new KernelArguments { { "count", 2 } }
                    );

            // Display result.
            Console.WriteLine(response ?? $"No response from agent: {fileAgent.Id}");
        }
        finally
        {
            // Clean-up (storage costs $)
            await CleanUpAsync();
            await fileAgent.DeleteAsync();
            await codeAgent.DeleteAsync();
        }
    }

    private async Task CleanUpAsync()
    {
        if (_agentsThread != null)
        {
            await _agentsThread.DeleteAsync();
            _agentsThread = null;
        }

        if (_agents.Any())
        {
            await Task.WhenAll(_agents.Select(agent => agent.DeleteAsync()));
            _agents.Clear();
        }
    }
}