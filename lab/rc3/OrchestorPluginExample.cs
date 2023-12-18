using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;

internal static class OrchestratorPluginExample
{
    public static async Task RunAsync()
    {
        var kernel = Factories.CreateKernel();

        var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins", "OrchestratorPlugin");

        // Import the OrchestratorPlugin from the plugins directory.
        var orchestratorPlugin = kernel
            .ImportPluginFromPromptDirectory(pluginsDirectory, "GetIntent");
        #pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        kernel.ImportPluginFromObject(new ConversationSummaryPlugin(), "ConversationSummaryPlugin");
        #pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        var args = new KernelArguments(
            new Dictionary<string, object?>
            {
                {
                    "input", "Yes"
                },
                {
                    "history", 
                    @"Bot: How can I help you?
                    User: What's the weather like today?
                    Bot: Where are you located?
                    User: I'm in Seattle.
                    Bot: It's 70 degrees and sunny in Seattle today.
                    User: Thanks! I'll wear shorts.
                    Bot: You're welcome.
                    User: Could you remind me what I have on my calendar today?
                    Bot: You have a meeting with your team at 2:00 PM.
                    User: Oh right! My team just hit a major milestone; I should send them an email to congratulate them.
                    Bot: Would you like to write one for you?"
                },
                {
                    "options", "SendEmail, ReadEmail, SendMeeting, RsvpToMeeting, SendChat"
                }
            }
        );

        // Get the GetIntent function from the OrchestratorPlugin and run it
        var result = await kernel.InvokeAsync(
            orchestratorPlugin["GetIntent"],
            args
        );

        Console.WriteLine(result);
    }
}