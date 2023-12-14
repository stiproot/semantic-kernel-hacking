using System.ComponentModel;
using System.Text.Json.Nodes;
using Microsoft.SemanticKernel;

namespace Plugins.OrchestratorPlugin;

public class Orchestrator
{
    private readonly Microsoft.SemanticKernel.Kernel _kernel;

    public Orchestrator(Microsoft.SemanticKernel.Kernel kernel)
    {
        _kernel = kernel;
    }

    [KernelFunction]
    public async Task<string> RouteRequestAsync(
        [Description("The user request")] string input
    )
    {
        // Save the original user request
        string request = input;

        // Retrieve the intent from the user request
        KernelFunction getIntent = this._kernel.Plugins.GetFunction(pluginName:"OrchestratorPlugin", functionName:"GetIntent");
        var getIntentVariables = new KernelArguments
        {
            ["input"] = input,
            ["options"] = "Sqrt, Multiply"
        };

        FunctionResult intentResult = await this._kernel.InvokeAsync(getIntent, getIntentVariables);

        string intent = intentResult.GetValue<string>()!.Trim();

        // Retrieve the numbers from the user request
        var getNumbers = this._kernel.Plugins.GetFunction("OrchestratorPlugin", "GetNumbers");
        string numbersJson = (await this._kernel.InvokeAsync(getNumbers, new KernelArguments(request))).GetValue<string>()!;
        JsonObject numbers = JsonObject.Parse(numbersJson)!.AsObject();

        // Call the appropriate function
        FunctionResult result;
        switch (intent)
        {
            case "Sqrt":
                // Call the Sqrt function with the first number
                var sqrt = _kernel.Plugins.GetFunction("MathPlugin", "Sqrt");
                result = await _kernel.InvokeAsync(sqrt, new KernelArguments(numbers["number1"]!.ToString()));
                return result.GetValue<string>()!.ToString();
            case "Multiply":
                // Call the Multiply function with both numbers
                var multiply = _kernel.Plugins.GetFunction("MathPlugin", "Multiply");
                var multiplyVariables = new KernelArguments
                {
                    ["input"] = numbers["number1"]!.ToString(),
                    ["number2"] = numbers["number2"]!.ToString()
                };
                result = await _kernel.InvokeAsync(multiply, multiplyVariables);
                return result.GetValue<double>()!.ToString();
            default:
                return "I'm sorry, I don't understand.";
        }
    }
}