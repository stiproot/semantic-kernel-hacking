using Microsoft.SemanticKernel.Connectors.Memory.Chroma;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Text;
// using Microsoft.SemanticKernel;

internal static class ChromaConnector
{
    public static async Task RunAsync()
    {
        const string endpoint = "http://localhost:8000";

        #pragma warning disable SKEXP0022 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        ChromaMemoryStore memoryStore = new(endpoint);

        // Kernel kernel = new KernelBuilder()
        //     // .WithLogger(logger)
        //     .WithOpenAITextEmbeddingGenerationService("text-embedding-ada-002", "OPENAI_API_KEY")
        //     .WithMemoryStorage(memoryStore)
        //     //.WithChromaMemoryStore(endpoint) // This method offers an alternative approach to registering Chroma memory store.
        //     .Build();

        await memoryStore.CreateCollectionAsync("simon_says_things");

        // var memoryRecord = MemoryRecord.

        // memoryStore.UpsertAsync

        #pragma warning restore SKEXP0022 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
}
