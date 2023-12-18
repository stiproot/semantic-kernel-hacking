using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using System.ComponentModel;
using Microsoft.Extensions.Logging.Debug;
using DocumentFormat.OpenXml.Wordprocessing;

internal static class SimplePluginExample
{
    public sealed class UtcPlugin
    {
        [KernelFunction]
        [Description("Get the current datetime in UTC")]
        public DateTime Now() => DateTime.UtcNow;
    }

    public static async Task RunAsync()
    {
        var kernel = new KernelBuilder().Build();

        #pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        var utcPlugin = new UtcPlugin();
        var timePlugin = new TimePlugin();

        var utc = kernel.ImportPluginFromObject(utcPlugin);

        #pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        var result = await kernel.InvokeAsync(utc["Now"]);

        Console.WriteLine(result);
    }
}