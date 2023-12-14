using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.TextEmbedding;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.Services;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI.OpenAI;
using Irony.Parsing;
using Microsoft.DeepDev;
using System.Runtime.CompilerServices;

internal static class Kernel
{
    public static async Task RunAsync()
    {
        var openAiConfig = new OpenAIConfig();

        var azureOpenAITextConfig = new AzureOpenAIConfig();
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build()
            .Bind("KernelMemory:Services:AzureOpenAIText", azureOpenAITextConfig);

        var memory = new KernelMemoryBuilder()
            .WithAzureOpenAITextGeneration(azureOpenAITextConfig)
            .WithOpenAITextEmbeddingGeneration(openAiConfig)
            .Build();

    }
}