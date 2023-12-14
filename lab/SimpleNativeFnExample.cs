using Microsoft.SemanticKernel;

internal static class SimpleNativeFnExample
{
    public static async Task RunAsync()
    {
        var kernel = Factories.CreateKernel();

        // Import the Math Plugin
        var mathPlugin = kernel.ImportPluginFromObject(new Plugins.MathPlugin.Math(), "MathPlugin");

        // Make a request that runs the Sqrt function
        var result = await kernel.InvokeAsync(mathPlugin["Sqrt"], new KernelArguments("12"));

        Console.WriteLine(result.GetValue<double>());
    }
}
