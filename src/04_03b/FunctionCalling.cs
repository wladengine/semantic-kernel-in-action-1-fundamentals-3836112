using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace _04_03b;

public class FunctionCalling
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
        var kernel = builder.Build();

        KernelFunction kernelFunctionRespondAsScientific =
                KernelFunctionFactory.CreateFromPrompt(
                        "Respond to the user question as if you were a Scientific. Respond to it as you were him, showing your personality",
                        functionName: "RespondAsScientific",
                        description: "Responds to a question as a Scientific.");

        KernelFunction kernelFunctionRespondAsPoliceman =
                KernelFunctionFactory.CreateFromPrompt(
                        "Respond to the user question as if you were a Policeman. Respond to it as you were him, showing your personality, humor and level of intelligence.",
                        functionName: "RespondAsPoliceman",
                        description: "Responds to a question as a Policeman.");

        KernelPlugin roleOpinionsPlugin =
                KernelPluginFactory.CreateFromFunctions(
                        "RoleTalk",
                        "Responds to questions or statements assuming different roles.",
                        new[] {
                            kernelFunctionRespondAsScientific,
                            kernelFunctionRespondAsPoliceman
                        });
        kernel.Plugins.Add(roleOpinionsPlugin);
        kernel.Plugins.AddFromType<WhatDateIsIt>();

        string userPrompt = "I just woke up and found myself in the middle of nowhere, " +
                "do you know what date is it? and what would a policeman and a scientist do in my place?" +
                "Please provide me the date using the WhatDateIsIt plugin and the Date function, and then " +
                "the responses from the policeman and the scientist, on this order. " +
                "For this two responses, use the RoleTalk plugin and the RespondAsPoliceman and RespondAsScientific functions.";

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new(){
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        };

        var result = await kernel.InvokePromptAsync(userPrompt, new(openAIPromptExecutionSettings));

        System.Console.WriteLine(result);
    }
}