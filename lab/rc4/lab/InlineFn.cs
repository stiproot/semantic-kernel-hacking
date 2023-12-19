using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Chroma;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using Kernel = Microsoft.SemanticKernel.Kernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

#pragma warning disable SKEXP0011, SKEXP0022, SKEXP0052, SKEXP0003

public static class InlineFn
{
    public static async Task RunAsync()
    {
        var kernel = Factories.CreateKernel();

        string skPrompt = """
        {{$input}}

        Summarize the content above.
        """;

        var executionSettings = new OpenAIPromptExecutionSettings 
        {
            MaxTokens = 2000,
            Temperature = 0.2,
            TopP = 0.5
        };

        var promptTemplateConfig = new PromptTemplateConfig(skPrompt);

        var promptTemplateFactory = new KernelPromptTemplateFactory();
        var promptTemplate = promptTemplateFactory.Create(promptTemplateConfig);

        var renderedPrompt = await promptTemplate.RenderAsync(kernel);

        Console.WriteLine("Rendered prompt:");
        Console.WriteLine(renderedPrompt);

        var summaryFunction = kernel.CreateFunctionFromPrompt(skPrompt, executionSettings);

        var input = """
            Demo (ancient Greek poet)
            From Wikipedia, the free encyclopedia
            Demo or Damo (Greek: Δεμώ, Δαμώ; fl. c. AD 200) was a Greek woman of the Roman period, known for a single epigram, engraved upon the Colossus of Memnon, which bears her name. She speaks of herself therein as a lyric poetess dedicated to the Muses, but nothing is known of her life.[1]
            Identity
            Demo was evidently Greek, as her name, a traditional epithet of Demeter, signifies. The name was relatively common in the Hellenistic world, in Egypt and elsewhere, and she cannot be further identified. The date of her visit to the Colossus of Memnon cannot be established with certainty, but internal evidence on the left leg suggests her poem was inscribed there at some point in or after AD 196.[2]
            Epigram
            There are a number of graffiti inscriptions on the Colossus of Memnon. Following three epigrams by Julia Balbilla, a fourth epigram, in elegiac couplets, entitled and presumably authored by "Demo" or "Damo" (the Greek inscription is difficult to read), is a dedication to the Muses.[2] The poem is traditionally published with the works of Balbilla, though the internal evidence suggests a different author.[1]
            In the poem, Demo explains that Memnon has shown her special respect. In return, Demo offers the gift for poetry, as a gift to the hero. At the end of this epigram, she addresses Memnon, highlighting his divine status by recalling his strength and holiness.[2]
            Demo, like Julia Balbilla, writes in the artificial and poetic Aeolic dialect. The language indicates she was knowledgeable in Homeric poetry—'bearing a pleasant gift', for example, alludes to the use of that phrase throughout the Iliad and Odyssey.[a][2] 
            """;

        var summaryResult = await kernel.InvokeAsync(summaryFunction, new() { ["input"] = input });

        Console.WriteLine(summaryResult);

        skPrompt = @"
            {{$input}}

            Give me the TLDR in 5 words.
            ";

        var textToSummarize = @"
            1) A robot may not injure a human being or, through inaction,
            allow a human being to come to harm.

            2) A robot must obey orders given it by human beings except where
            such orders would conflict with the First Law.

            3) A robot must protect its own existence as long as such protection
            does not conflict with the First or Second Law.
        ";

        var result = await kernel.InvokePromptAsync(skPrompt, new() { ["input"] = textToSummarize });

        Console.WriteLine(result);
    }
}