using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.DotNet.Interactive.AIUtilities;

internal static class OpenAIFunctionsExample
{
    public static async Task RunAsync()
    {
        var cred = new AzureKeyCredential(Secrets.ApiKey);
        var client = new OpenAIClient(new Uri(Secrets.Endpoint), cred);

        var findFunction = GptFunction.Create("find", async (string currentLocation) =>
        {
            Console.WriteLine($"{nameof(currentLocation)}: {currentLocation}");

            // Emulate http call...
            await Task.Delay(1000);

            // Assume this response came from a web api...
            return "A wonderful place to eat in Seattle is the Space Needle.";

        }, enumsAsString: true);

        var response = await ProvideReccomendations($@"
                I am hungry in Seattle, actually Capitol Hill.
                At the moment I would appreaciate something local and cheap.
                Maybe a pub? Don't know what is the best to go for. I am open to any idea, What do you suggest?");

        Console.WriteLine(response);


        async Task<string> ProvideReccomendations(string userQuestion)
        {
            var response = await client.GetChatCompletionsAsync(new ChatCompletionsOptions
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, $@"
                            You are a sophisticated AI assistant, a specialist in user intent detection and interpretation.
                            Your task is to perceive and respond to the user's needs, even when they're expressed in an indirect or direct manner.
                            You excel in recognizing subtle cues: for example, if a user states they are 'hungry', you should assume they are seeking nearby dining options such as a restaurant or a cafe. If they indicate feeling 'tired', 'weary', or mention a long journey, interpret this as a request for accommodation options like hotels or guest houses. However, remember to navigate the fine line of interpretation and assumption: if a user's intent is unclear or can be interpreted in multiple ways, do not hesitate to politely ask for additional clarification. Use only values from the nums in the functions."),
                    new ChatMessage(ChatRole.User, userQuestion)
                },
                Functions = new[] { CreateFunctionDefinition(findFunction) },
                DeploymentName = Secrets.Gpt_4_32k,
            });

            foreach (var choice in response.Value.Choices) Console.WriteLine($"{choice.Index} - {choice.Message.Content}");

            var functionCall = response.Value.Choices[0].Message.FunctionCall;

            var results = await (Task<string>)findFunction.Execute(functionCall.Arguments)!;

            return results;
        }

        FunctionDefinition CreateFunctionDefinition(GptFunction function)
        {
            var functionDefinition = new FunctionDefinition(function.Name);
            var json = JsonDocument.Parse(function.JsonSignature.ToString()).RootElement;
            functionDefinition.Parameters = BinaryData.FromString(json.GetProperty("parameters").ToString());
            return functionDefinition;
        }
    }
}
