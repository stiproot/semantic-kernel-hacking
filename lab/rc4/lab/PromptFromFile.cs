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

public static class PromptFromFile
{
    public static async Task RunAsync()
    {
        var kernel = Factories.CreateKernel();

        var funPluginDirectoryPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins", "FunPlugin");

        // Load the FunPlugin from the Plugins Directory
        var funPluginFunctions = kernel.ImportPluginFromPromptDirectory(funPluginDirectoryPath);

        // Construct arguments
        var arguments = new KernelArguments() { ["input"] = "time travel to dinosaur age" };

        // Run the Function called Joke
        var result = await kernel.InvokeAsync(funPluginFunctions["JokeFn"], arguments);

        // Return the result to the Notebook
        Console.WriteLine(result);
    }
}