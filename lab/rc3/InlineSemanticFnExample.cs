using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI;

internal static class InlineSemanticFnExample
{
    public static async Task RunAsync()
    {
        string prompt = @"Bot: How can I help you?
        User: {{$input}}

        ---------------------------------------------

        The intent of the user in 5 words or less: ";

        PromptExecutionSettings requestSettings = new()
        {
            ExtensionData = {
                        {"MaxTokens", 500},
                        {"Temperature", 0.0},
                        {"TopP", 0.0},
                        {"PresencePenalty", 0.0},
                        {"FrequencyPenalty", 0.0}
                    }
        };

        var kernel = Factories.CreateKernel();

        var getIntentFunction = kernel.CreateFunctionFromPrompt(prompt, requestSettings, "GetIntent");

        var result = await kernel.InvokeAsync(getIntentFunction, new KernelArguments("I want to send an email to the marketing team celebraing their recent milestone."));

        Console.WriteLine(result);
    }
}