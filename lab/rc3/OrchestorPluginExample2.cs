using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using Plugins.OrchestratorPlugin;

internal static class OrchestratorPluginExample2
{
    public static async Task RunAsync()
    {
        var kernel = Factories.CreateKernel();

        var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins");

        // Import the semantic functions
        // kernel.ImportPluginFromPromptDirectory(pluginsDirectory, "OrchestratorPlugin");

        // Import the native functions
        kernel.ImportPluginFromObject(new Plugins.MathPlugin.Math(), "MathPlugin");
        kernel.ImportPluginFromPromptDirectory(Path.Combine(pluginsDirectory, "OrchestratorPlugin"), "OrchestratorPlugin");

        var orchestratorPlugin = kernel.ImportPluginFromObject(new Orchestrator(kernel), "NativeOrchestratorPlugin");

        #pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        kernel.ImportPluginFromObject(new ConversationSummaryPlugin(), "ConversationSummaryPlugin");
        #pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        // Make a request that runs the Sqrt function
        var result1 = await kernel.InvokeAsync(orchestratorPlugin["RouteRequest"], new KernelArguments("What is the square root of 634?"));
        Console.WriteLine(result1);

        // Make a request that runs the Multiply function
        var result2 = await kernel.InvokeAsync(orchestratorPlugin["RouteRequest"], new KernelArguments("What is 12.34 times 56.78?"));
        Console.WriteLine(result2);
    }
}