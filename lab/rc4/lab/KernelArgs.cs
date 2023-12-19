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

public static class KernelArgs
{
    public static async Task RunAsync()
    {
        var kernel = Factories.CreateKernel();

        const string skPrompt = @"
        ChatBot can have a conversation with you about any topic.
        It can give explicit instructions or say 'I don't know' if it does not have an answer.

        {{$history}}
        User: {{$userInput}}
        ChatBot:";

        var executionSettings = new OpenAIPromptExecutionSettings 
        {
            MaxTokens = 2000,
            Temperature = 0.7,
            TopP = 0.5
        };

        var chatFunction = kernel.CreateFunctionFromPrompt(skPrompt, executionSettings);

        var history = "";
        var arguments = new KernelArguments()
        {
            ["history"] = history
        };

        var userInput = "Hi, I'm looking for book suggestions";
        arguments["userInput"] = userInput;

        var bot_answer = await chatFunction.InvokeAsync(kernel, arguments);

        history += $"\nUser: {userInput}\nAI: {bot_answer}\n";
        arguments["history"] = history;

        Console.WriteLine(history);

        Func<string, Task> Chat = async (string input) => {
            // Save new message in the arguments
            arguments["userInput"] = input;

            // Process the user message and get an answer
            var answer = await chatFunction.InvokeAsync(kernel, arguments);

            // Append the new interaction to the chat history
            var result = $"\nUser: {input}\nAI: {answer}\n";
            history += result;

            arguments["history"] = history;
            
            // Show the response
            Console.WriteLine(result);
        };

        await Chat("I would like a non-fiction book suggestion about Greece history. Please only list one book.");
        await Chat("that sounds interesting, what are some of the topics I will learn about?");
        await Chat("Which topic from the ones you listed do you think most people find interesting?");
        await Chat("could you list some more books I could read about the topic(s) you mentioned?");

        Console.WriteLine(history);
    }
}