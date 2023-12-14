﻿using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Models;

public class ExecuteFunctionEndpoint
{
    private static readonly JsonSerializerOptions s_jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly IKernel _kernel;

    public ExecuteFunctionEndpoint(IKernel kernel)
    {
        this._kernel = kernel;
    }

    [Function("ExecuteFunction")]
    [OpenApiOperation(operationId: "ExecuteFunction", tags: new[] { "ExecuteFunction" }, Description = "Execute the specified semantic function. Provide skill and function names, plus any variables the function requires.")]
    [OpenApiParameter(name: "skillName", Description = "Name of the skill e.g., 'FunSkill'", Required = true)]
    [OpenApiParameter(name: "functionName", Description = "Name of the function e.g., 'Excuses'", Required = true)]
    [OpenApiRequestBody("application/json", typeof(ExecuteFunctionRequest), Description = "Variables to use when executing the specified function.", Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ExecuteFunctionResponse), Description = "Returns the response from the AI.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorResponse), Description = "Returned if the request body is invalid.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(ErrorResponse), Description = "Returned if the semantic function could not be found.")]
    public async Task<HttpResponseData> ExecuteFunctionAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "skills/{skillName}/functions/{functionName}")]
        HttpRequestData requestData,
        FunctionContext executionContext, string skillName, string functionName)
    {
#pragma warning disable CA1062
        var functionRequest = await JsonSerializer.DeserializeAsync<ExecuteFunctionRequest>(requestData.Body, s_jsonOptions).ConfigureAwait(false);
#pragma warning disable CA1062
        if (functionRequest == null)
        {
            return await CreateResponseAsync(requestData, HttpStatusCode.BadRequest, new ErrorResponse() { Message = $"Invalid request body {functionRequest}" }).ConfigureAwait(false);
        }

        // note: using skills from the repo
        var skillsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "skills");

        var skillDirectory = Path.Combine(skillsDirectory, skillName);
        if (!System.IO.Directory.Exists(skillDirectory))
        {
            return await CreateResponseAsync(requestData, HttpStatusCode.NotFound, new ErrorResponse() { Message = $"Unable to find {skillName}" }).ConfigureAwait(false);
        }

        var skill = this._kernel.ImportSemanticSkillFromDirectory(skillsDirectory, skillName);
        if (!skill.ContainsKey(functionName))
        {
            return await CreateResponseAsync(requestData, HttpStatusCode.NotFound, new ErrorResponse() { Message = $"Unable to find {skillName}.{functionName}" }).ConfigureAwait(false);
        }

        var function = skill[functionName];

        var context = new ContextVariables();
        foreach (var v in functionRequest.Variables)
        {
            context.Set(v.Key, v.Value);
        }

        var result = await this._kernel.RunAsync(context, function).ConfigureAwait(false);

        return await CreateResponseAsync(requestData, HttpStatusCode.OK, new ExecuteFunctionResponse() { Response = result.ToString() }).ConfigureAwait(false);
    }

    private static async Task<HttpResponseData> CreateResponseAsync(HttpRequestData requestData, HttpStatusCode statusCode, object responseBody)
    {
        var responseData = requestData.CreateResponse(statusCode);
        await responseData.WriteAsJsonAsync(responseBody).ConfigureAwait(false);
        return responseData;
    }
}
