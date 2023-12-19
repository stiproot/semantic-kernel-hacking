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

public static class MemoryInteraction_Links
{
    public static async Task RunAsync()
    {
        var builder = Kernel.CreateBuilder();

        // Configure AI backend used by the kernel
        var (useAzureOpenAI, model, azureEndpoint, apiKey, orgId) = Settings.LoadFromFile();

        if (useAzureOpenAI)
            builder.AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);
        else
            builder.AddOpenAIChatCompletion(model, apiKey, orgId);

        var kernel = builder.Build();

        #pragma warning disable SKEXP0011, SKEXP0022, SKEXP0052

        var memoryBuilder = new MemoryBuilder();

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

        const string memoryCollectionName = "SKGitHub";

        var githubFiles = new Dictionary<string, string>()
        {
            ["https://github.com/microsoft/semantic-kernel/blob/main/README.md"]
                = "README: Installation, getting started, and how to contribute",
            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks/02-running-prompts-from-file.ipynb"]
                = "Jupyter notebook describing how to pass prompts from a file to a semantic plugin or function",
            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks/00-getting-started.ipynb"]
                = "Jupyter notebook describing how to get started with the Semantic Kernel",
            ["https://github.com/microsoft/semantic-kernel/tree/main/samples/plugins/ChatPlugin/ChatGPT"]
                = "Sample demonstrating how to create a chat plugin interfacing with ChatGPT",
            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/Plugins/Plugins.Memory/VolatileMemoryStore.cs"]
                = "C# class that defines a volatile embedding store",
        };

        Console.WriteLine("Adding some GitHub file URLs and their descriptions to Chroma Semantic Memory.");

        var i = 0;
        foreach (var entry in githubFiles)
        {
            await memory.SaveReferenceAsync(
                collection: memoryCollectionName,
                description: entry.Value,
                text: entry.Value,
                externalId: entry.Key,
                externalSourceName: "GitHub"
            );
            Console.WriteLine($"  URL {++i} saved");
        }

        string ask = "I love Jupyter notebooks, how should I get started?";
        Console.WriteLine("===========================\n" +
                    "Query: " + ask + "\n");

        var memories = memory.SearchAsync(memoryCollectionName, ask, limit: 5, minRelevanceScore: 0.6);

        i = 0;
        await foreach (var _memory in memories)
        {
            Console.WriteLine($"Result {++i}:");
            Console.WriteLine("  URL:     : " + _memory.Metadata.Id);
            Console.WriteLine("  Title    : " + _memory.Metadata.Description);
            Console.WriteLine("  Relevance: " + _memory.Relevance);
            Console.WriteLine();
        }
    }
}