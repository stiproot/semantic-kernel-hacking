using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Chroma;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using Kernel = Microsoft.SemanticKernel.Kernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

#pragma warning disable SKEXP0011, SKEXP0022, SKEXP0052, SKEXP0003

public static class Factories
{
    public static ISemanticTextMemory CreateWithVolatileMemoryStore()
    {
        var (useAzureOpenAI, model, azureEndpoint, apiKey, orgId) = Settings.LoadFromFile();

        var memoryBuilder = new MemoryBuilder();

        if (useAzureOpenAI)
        {
            memoryBuilder.WithAzureOpenAITextEmbeddingGeneration(
                "text-embedding-ada-002",
                azureEndpoint, 
                apiKey,
                "model-id");
        }
        else
        {
            memoryBuilder.WithOpenAITextEmbeddingGeneration("text-embedding-ada-002", apiKey);
        }

        memoryBuilder.WithMemoryStore(new VolatileMemoryStore());

        var memory = memoryBuilder.Build();

        return memory;
    }

    public static ISemanticTextMemory CreateWithChromaStore()
    {

        var memoryBuilder = new MemoryBuilder();

        var (useAzureOpenAI, model, azureEndpoint, apiKey, orgId) = Settings.LoadFromFile();
        if (useAzureOpenAI)
        {
            memoryBuilder.WithAzureOpenAITextEmbeddingGeneration("text-embedding-ada-002", azureEndpoint, apiKey, "model-id");
        }
        else
        {
            memoryBuilder.WithOpenAITextEmbeddingGeneration("text-embedding-ada-002", apiKey);
        }

        var chromaMemoryStore = new ChromaMemoryStore("http://127.0.0.1:8000");

        memoryBuilder.WithMemoryStore(chromaMemoryStore);

        var memory = memoryBuilder.Build();

        return memory;
    }

    public static Microsoft.SemanticKernel.Kernel CreateKernel()
    {
        var builder = Kernel.CreateBuilder();

        // Configure AI backend used by the kernel
        var (useAzureOpenAI, model, azureEndpoint, apiKey, orgId) = Settings.LoadFromFile();

        if (useAzureOpenAI) builder.AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);
        else builder.AddOpenAIChatCompletion(model, apiKey, orgId);

        var kernel = builder.Build();

        return kernel;
    }
}