// using System.Text.Json;
// using System.Web;
// using Azure;
// using Azure.AI.OpenAI;
// using Newtonsoft.Json.Linq;


// var cred = new AzureKeyCredential(apiKey);
// var client = new OpenAIClient(new Uri(endpoint), cred);

// var userMessage = new ChatMessage(ChatRole.User, @"I'd like to provide with comprehensive feedback on the recent technical interview for the Software Engineer role for Mr.X. Overall, the technical knowledge and problem-solving skills were evident, but the the engagement during the interview is low.
//                                     Mr.X displayed a solid grasp of fundamental concepts, particularly in areas such as data structures and algorithms. Mr.X's ability to break down complex problems into manageable steps and systematically approach solutions was impressive. Additionally, Mr.X's coding skills were commendable. Mr.X code was well-structured and readable, and Mr.X articulated his thought process clearly during the coding exercises.
//                                     While the code was correct, there was room for optimization in terms of time or space complexity. Paying closer attention to edge cases and potential corner scenarios is crucial, as they can impact the completeness of a solution. In terms of communication, consider providing a bit more context before diving into code to enhance clarity. Mr.X did not ask claryfying questions to confirm the understanding.
//                                     He missed to use descriptive variable names and adding comments, especially for complex logic, so the code readability was low. Candidate needs further evaluation to decide as a culture fit as per the company culture or not.
//                                     ");

// var chatCompletionsOptions = new ChatCompletionsOptions()
// {
//     Messages = { userMessage },
//     MaxTokens = 4000,
//     Functions = { GetInterviewInsights() },
//     Temperature = 0.4f,
//     DeploymentName = Gpt_4_32k
// };

// NullableResponse<ChatCompletions> response = await client.GetChatCompletionsAsync(chatCompletionsOptions, new CancellationToken());

// if (!response.HasValue)
// {
//     Console.WriteLine("Error: " + response);
// }
// else
// {
//     // Read and print the response content
//     if (response.Value?.Choices is not null && response.Value?.Choices[0]?.FinishReason == "function_call")
//     {
//         var finalContent = response.Value.Choices[0].Message.FunctionCall.Arguments;
//         FormatResponseData(finalContent);
//     }
// }

// static FunctionDefinition GetInterviewInsights()
// {
//     return new FunctionDefinition()
//     {
//         Name = "interview_insights",
//         Description = "Get the data for Interview Insights",
//         Parameters = BinaryData.FromObjectAsJson(
//         new
//         {
//             Type = "object",
//             Properties = new
//             {
//                 Name = new
//                 {
//                     Type = "string",
//                     Description = "Name indicates the name of the candidate, appearing for interview.",
//                 },
//                 Role = new
//                 {
//                     Type = "string",
//                     Description = "Role here means the position the candidate is interviewed for.",
//                 },
//                 ActionableFeedback = new
//                 {
//                     Type = "array",
//                     Items = new
//                     {
//                         Type = "string"
//                     },
//                     Description = "Actionable here means feedback that candidate can work on to improve their skills and be better in future.",
//                 },
//                 Highlights = new
//                 {
//                     Type = "array",
//                     Items = new
//                     {
//                         Type = "object",
//                         Properties = new
//                         {
//                             Trait = new
//                             {
//                                 Type = "string",
//                                 Description = @"Highlights will include good traits of the candidate's interview like the strengths of the the candidate or positive remarks in the interview
//                                         It must be very short and precise.Summarize it and extract 2-3 words."
//                             },
//                             Weight = new
//                             {
//                                 Type = "string",
//                                 Description = @"Weight of the highlights will be calculated depending on the sentiment.
//                                              It MUST be within scale of 1-5. 1 being on nuetral. 5 being extremely positive",
//                             }
//                         }
//                     }
//                 },
//                 Lowlights = new
//                 {
//                     Type = "array",
//                     Items = new
//                     {
//                         Type = "object",
//                         Properties = new
//                         {
//                             Trait = new
//                             {
//                                 Type = "string",
//                                 Description = @"Lowlights will include bad traits of the candidate's interview like the weakness of the the candidate or negative remarks in the interview or the constructive feedback.
//                                         It must be short and precise Summarize it and extract 2-3 words."
//                             },
//                             Weight = new
//                             {
//                                 Type = "string",
//                                 Description = @"Weight of the Lowlights will be calculated depending on the sentiment.
//                                               It MUST be within scale of 1-5. 1 being on nuetral side. 5 being extremely negative",
//                             }
//                         }
//                     }
//                 }
//             },
//             Required = new[]
//             {
//             "name", "role"
//             },
//         },
//         new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
//     };
// }

// static void FormatResponseData(string response)
// {
//     // Parse the JSON string into a JObject
//     JObject jsonObject = JObject.Parse(response);

//     // Extract the "role"
//     string role = (string)jsonObject["role"]!;

//     // Extract the "CandidateName"
//     string name = (string)jsonObject["name"]!;

//     // Extract the "actionableFeedback" array
//     JArray? actionableFeedbackArray = jsonObject["actionableFeedback"] as JArray;
//     List<string>? actionableFeedbackList = actionableFeedbackArray?.ToObject<List<string>>();

//     // Extract the "Highlights" array
//     JArray? highlightsArray = jsonObject["highlights"] as JArray;
//     List<TraitProperties>? highlightsList = highlightsArray?.ToObject<List<TraitProperties>>();

//     // Extract the "lowLights" array
//     JArray? lowlightsArray = jsonObject["lowlights"] as JArray;
//     List<TraitProperties>? lowLightsList = lowlightsArray?.ToObject<List<TraitProperties>>();

//     // Print the extracted values

//     Console.ResetColor();
//     Console.ForegroundColor = ConsoleColor.Blue;
//     Console.WriteLine($"Generating interview insights for Candidate : {name}");
//     Console.WriteLine();

//     Console.ResetColor();
//     Console.ForegroundColor = ConsoleColor.Magenta;
//     Console.WriteLine($"Role:{role}");
//     Console.WriteLine();

//     if (actionableFeedbackList!.Any())
//     {
//         Console.ResetColor();
//         Console.ForegroundColor = ConsoleColor.DarkYellow;
//         Console.WriteLine("Actionable Feedback:");
//         foreach (var feedback in actionableFeedbackList!)
//         {
//             Console.WriteLine("- " + feedback);
//         }
//         Console.WriteLine();
//     }
//     if (highlightsList!.Any())
//     {
//         Console.ResetColor();
//         Console.ForegroundColor = ConsoleColor.Green;
//         Console.WriteLine("Highlights:");
//         foreach (TraitProperties traits in highlightsList!)
//         {
//             Console.Write("- Trait: " + traits.Trait);
//             Console.WriteLine("  [Weight: " + traits.Weight + " ]");
//         }
//         Console.WriteLine();
//     }
//     if (lowLightsList!.Any())
//     {
//         Console.ResetColor();
//         Console.ForegroundColor = ConsoleColor.Red;
//         Console.WriteLine("LowLights:");
//         foreach (TraitProperties traits in lowLightsList!)
//         {
//             Console.Write("- Trait: " + traits.Trait);
//             Console.WriteLine("  [Weight: " + traits.Weight + " ]");
//         }
//         Console.WriteLine();
//     }
// }

// public class TraitProperties
// {
//     public string Trait { get; set; } = default!;
//     public string Weight { get; set; } = default!;
// }
