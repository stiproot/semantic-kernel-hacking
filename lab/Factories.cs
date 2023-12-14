using Microsoft.SemanticKernel;

internal static class Factories
{
    public static Microsoft.SemanticKernel.Kernel CreateKernel()
    {
        return 
            new KernelBuilder()
                .AddAzureOpenAIChatCompletion(
                    Secrets.Gpt_4_32k,
                    Secrets.Gpt_4_32k,
                    Secrets.Endpoint,
                    Secrets.ApiKey
                )
                .Build();
    }
}
