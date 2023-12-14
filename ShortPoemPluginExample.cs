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

        var kernelWithConfiguration = new KernelBuilder()
            // .WithLoggerFactory(loggerFactory)
            .AddAzureOpenAIChatCompletion(
                Gpt_4_32k,
                Gpt_4_32k,  // The name of your deployment (e.g., "gpt-35-turbo")
                endpoint,        // The endpoint of your Azure OpenAI service
                apiKey           // The API key of your Azure OpenAI service
            )
            .Build();

        var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins", "WritePlugin");
        var writerPlugin = kernelWithConfiguration
            .ImportPluginFromPromptDirectory(pluginsDirectory, "ShortPoem");

        var arg = new KernelArguments("Hello World");

        var result = await kernelWithConfiguration.InvokeAsync(writerPlugin["ShortPoem"], arg);
        Console.WriteLine(result);
    }
}
