using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

internal static class Factories
{
    public static Microsoft.SemanticKernel.Kernel CreateKernel()
    {
        var kernel = new KernelBuilder()
            .AddAzureOpenAIChatCompletion(
                Gpt_4_32k,
                Gpt_4_32k,  // The name of your deployment (e.g., "gpt-35-turbo")
                endpoint,        // The endpoint of your Azure OpenAI service
                apiKey           // The API key of your Azure OpenAI service
            )
            .Build();

        return kernel;
    }
}
