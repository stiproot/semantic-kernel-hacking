using Microsoft.SemanticKernel;

internal static class SimpleNativeFnExampleWithTwoArgs
{
    public static async Task RunAsync()
    {
        var kernel = Factories.CreateKernel();

        // Import the Math Plugin
        var mathPlugin = kernel.ImportPluginFromObject(new Plugins.MathPlugin.Math(), "MathPlugin");

        var args = new KernelArguments(
            new Dictionary<string, object?>
            {
                { "input", "12.34" },
                { "number2", "56.78" }
            }
        );

        // Make a request that runs the Sqrt function
        var result = await kernel.InvokeAsync(mathPlugin["Multiply"], args);

        Console.WriteLine(result.GetValue<double>());
    }
}
