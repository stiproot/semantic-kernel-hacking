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

public static class MemoryWithChroma_Text
{
    public static async Task RunAsync()
    {
        var builder = Kernel.CreateBuilder();

        // Configure AI backend used by the kernel
        var (useAzureOpenAI, model, azureEndpoint, apiKey, orgId) = Settings.LoadFromFile();

        if (useAzureOpenAI) builder.AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);
        else builder.AddOpenAIChatCompletion(model, apiKey, orgId);

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

        const string MemoryCollectionName = "aboutMe";

        // await memory.SaveInformationAsync(MemoryCollectionName, id: "info1", text: "My name is Andrea");
        // await memory.SaveInformationAsync(MemoryCollectionName, id: "info2", text: "I currently work as a tourist operator");
        // await memory.SaveInformationAsync(MemoryCollectionName, id: "info3", text: "I currently live in Seattle and have been living there since 2005");
        // await memory.SaveInformationAsync(MemoryCollectionName, id: "info4", text: "I visited France and Italy five times since 2015");
        // await memory.SaveInformationAsync(MemoryCollectionName, id: "info5", text: "My family is from New York");


        var questions = new[]
        {
            "what is my name?",
            "where do I live?",
            "where is my family from?",
            "where have I travelled?",
            "what do I do for work?",
        };

        foreach (var q in questions)
        {
            var response = await memory.SearchAsync(MemoryCollectionName, q, limit: 1, minRelevanceScore: 0.5).FirstOrDefaultAsync();
            Console.WriteLine(q + " " + response?.Metadata.Text);
        }

        #pragma warning disable SKEXP0052

        // TextMemoryPlugin provides the "recall" function
        kernel.ImportPluginFromObject(new TextMemoryPlugin(memory));

        const string skPrompt = @"
            ChatBot can have a conversation with you about any topic.
            It can give explicit instructions or say 'I don't know' if it does not have an answer.

            Information about me, from previous conversations:
            - {{$fact1}} {{recall $fact1}}
            - {{$fact2}} {{recall $fact2}}
            - {{$fact3}} {{recall $fact3}}
            - {{$fact4}} {{recall $fact4}}
            - {{$fact5}} {{recall $fact5}}

            Chat:
            {{$history}}
            User: {{$userInput}}
            ChatBot: ";

        var chatFunction = kernel.CreateFunctionFromPrompt(skPrompt, new OpenAIPromptExecutionSettings { MaxTokens = 200, Temperature = 0.8 });

        #pragma warning disable SKEXP0052

        var arguments = new KernelArguments();

        arguments["fact1"] = "what is my name?";
        arguments["fact2"] = "where do I live?";
        arguments["fact3"] = "where is my family from?";
        arguments["fact4"] = "where have I travelled?";
        arguments["fact5"] = "what do I do for work?";

        arguments[TextMemoryPlugin.CollectionParam] = MemoryCollectionName;
        arguments[TextMemoryPlugin.LimitParam] = "2";
        arguments[TextMemoryPlugin.RelevanceParam] = "0.8";

        var history = "";
        arguments["history"] = history;
        Func<string, Task> Chat = async (string input) => {
            // Save new message in the kernel arguments
            arguments["userInput"] = input;

            // Process the user message and get an answer
            var answer = await chatFunction.InvokeAsync(kernel, arguments);

            // Append the new interaction to the chat history
            var result = $"\nUser: {input}\nChatBot: {answer}\n";

            history += result;
            arguments["history"] = history;
            
            // Show the bot response
            Console.WriteLine(result);
        };

// await Chat("Hello, I think we've met before, remember? my name is...");
// await Chat("I want to plan a trip and visit my family. Do you know where that is?");
// await Chat("Great! What are some fun things to do there?");

    }

}
