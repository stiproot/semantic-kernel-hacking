using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

internal static class ShortPoemPluginExample
{
    public static async Task RunAsync()
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(0)
                .AddDebug();
        });

        var kernelWithConfiguration = Factories.CreateKernel(); 

        var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins", "WritePlugin");
        var writerPlugin = kernelWithConfiguration
            .ImportPluginFromPromptDirectory(pluginsDirectory, "ShortPoem");

        var arg = new KernelArguments("Hello World");

        var result = await kernelWithConfiguration.InvokeAsync(writerPlugin["ShortPoem"], arg);
        Console.WriteLine(result);
    }
}
