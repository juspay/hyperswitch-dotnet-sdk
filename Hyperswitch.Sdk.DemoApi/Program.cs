using Hyperswitch.Sdk;
using Hyperswitch.Sdk.Models;
using Hyperswitch.Sdk.Services;
using Hyperswitch.Sdk.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json; // Required for JsonSerializer

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure HyperswitchClient
// Configure HyperswitchClient
string secretKey = "API_KEY_HERE";
string publishableKey = "PUBLISHABLE_KEY_HERE";
string defaultProfileId = "PROFILE_ID_HERE";

builder.Services.AddSingleton<HyperswitchClient>(sp =>
    new HyperswitchClient(secretKey: secretKey, publishableKey: publishableKey, defaultProfileId: defaultProfileId)
);

builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<RefundService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<MerchantService>();

// Add logging services
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

// Helper for logging responses
void LogResponse(ILogger logger, object? responseData, string endpointName, int? statusCode = null)
{
    try
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var responseJson = JsonSerializer.Serialize(responseData, options);
        logger.LogInformation("Response from {EndpointName} (Status: {StatusCode}):\n{ResponseJson}", endpointName, statusCode ?? 200, responseJson);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error serializing response for logging from {EndpointName}.", endpointName);
    }
}

void LogErrorResponse(ILogger logger, HyperswitchApiException apiEx, string endpointName)
{
    var errorPayload = new
    {
        ErrorType = "HyperswitchApiException",
        StatusCode = apiEx.StatusCode,
        ErrorCode = apiEx.ErrorDetails?.Error?.Code,
        ErrorMessage = apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message,
        RawResponse = apiEx.ResponseContent
    };
    LogResponse(logger, errorPayload, endpointName, apiEx.StatusCode);
}

void LogGenericErrorResponse(ILogger logger, Exception ex, string endpointName)
{
    var errorPayload = new
    {
        ErrorType = ex.GetType().Name,
        ErrorMessage = ex.Message,
        StackTrace = ex.StackTrace
    };
    LogResponse(logger, errorPayload, endpointName, 500);
}


app.MapGet("/", (ILogger<Program> logger) =>
{
    const string endpointName = "GET /";
    var response = "Welcome to Hyperswitch SDK Demo API - Direct SDK Function Mapping";
    LogResponse(logger, response, endpointName);
    return response;
});

// --- Payment Service Endpoints ---
app.MapPost("/payments", async ([FromBody] PaymentIntentRequest request, PaymentService service, ILogger<Program> logger) =>
{
    const string endpointName = "POST /payments";
    try
    {
        var sdkResponse = await service.CreateAsync(request);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});

app.MapPost("/payments/{id}/confirm", async (string id, [FromBody] PaymentConfirmRequest request, PaymentService service, ILogger<Program> logger) =>
{
    const string endpointName = "POST /payments/{id}/confirm";
    try
    {
        var sdkResponse = await service.ConfirmPaymentAsync(id, request);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});

app.MapPost("/payments/{id}/capture", async (string id, [FromBody] PaymentCaptureRequest request, PaymentService service, ILogger<Program> logger) =>
{
    const string endpointName = "POST /payments/{id}/capture";
    try
    {
        var sdkResponse = await service.CapturePaymentAsync(id, request);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});

app.MapGet("/payments/{id}", async (string id, PaymentService service, ILogger<Program> logger, [FromQuery] string? clientSecret, [FromQuery] bool forceSync = false) =>
{
    const string endpointName = "GET /payments/{id}";
    try
    {
        var sdkResponse = await service.SyncPaymentStatusAsync(id, clientSecret, forceSync);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});

app.MapPost("/payments/{id}/void", async (string id, [FromBody] PaymentCancelRequest request, PaymentService service, ILogger<Program> logger) =>
{
    const string endpointName = "POST /payments/{id}/void";
    try
    {
        var sdkResponse = await service.CancelPaymentAsync(id, request);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});

app.MapPost("/payments/{id}", async (string id, [FromBody] PaymentUpdateRequest request, PaymentService service, ILogger<Program> logger) =>
{
    const string endpointName = "POST /payments/{id} (Update)";
    try
    {
        var sdkResponse = await service.UpdatePaymentAsync(id, request);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});


// --- Refund Service Endpoints ---
app.MapPost("/refunds", async ([FromBody] RefundCreateRequest request, RefundService service, ILogger<Program> logger) =>
{
    const string endpointName = "POST /refunds";
    try
    {
        var sdkResponse = await service.CreateRefundAsync(request);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});

app.MapGet("/refunds/{id}", async (string id, RefundService service, ILogger<Program> logger) =>
{
    const string endpointName = "GET /refunds/{id}";
    try
    {
        var sdkResponse = await service.RetrieveRefundAsync(id);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});

app.MapPost("/refunds/{id}", async (string id, [FromBody] RefundUpdateRequest request, RefundService service, ILogger<Program> logger) =>
{
    const string endpointName = "POST /refunds/{id} (Update)";
    try
    {
        var sdkResponse = await service.UpdateRefundAsync(id, request);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});

app.MapPost("/refunds/list", async ([FromBody] RefundListRequest request, RefundService service, ILogger<Program> logger) =>
{
    const string endpointName = "POST /refunds/list";
    try
    {
        var sdkResponse = await service.ListRefundsAsync(request);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});


// --- Customer Service Endpoints ---
app.MapPost("/customers", async ([FromBody] CustomerRequest request, CustomerService service, ILogger<Program> logger) =>
{
    const string endpointName = "POST /customers";
    try
    {
        var sdkResponse = await service.CreateCustomerAsync(request);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});

app.MapGet("/customers/{id}", async (string id, CustomerService service, ILogger<Program> logger) =>
{
    const string endpointName = "GET /customers/{id}";
    try
    {
        var sdkResponse = await service.RetrieveCustomerAsync(id);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return apiEx.StatusCode == (int)HttpStatusCode.NotFound ? Results.NotFound() : Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});

app.MapPost("/customers/{id}", async (string id, [FromBody] CustomerUpdateRequest request, CustomerService service, ILogger<Program> logger) =>
{
    const string endpointName = "POST /customers/{id} (Update)";
    try
    {
        var sdkResponse = await service.UpdateCustomerAsync(id, request);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return apiEx.StatusCode == (int)HttpStatusCode.NotFound ? Results.NotFound() : Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});

app.MapPost("/customers/list", async ([FromBody] CustomerListRequest request, CustomerService service, ILogger<Program> logger) =>
{
    const string endpointName = "POST /customers/list";
    try
    {
        var sdkResponse = await service.ListCustomersAsync(request);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});

app.MapDelete("/customers/{id}", async (string id, CustomerService service, ILogger<Program> logger) =>
{
    const string endpointName = "DELETE /customers/{id}";
    try
    {
        var sdkResponse = await service.DeleteCustomerAsync(id);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return apiEx.StatusCode == (int)HttpStatusCode.NotFound ? Results.NotFound() : Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});

app.MapGet("/customers/{id}/payment_methods", async (string id, CustomerService service, ILogger<Program> logger) =>
{
    const string endpointName = "GET /customers/{id}/payment_methods";
    try
    {
        var sdkResponse = await service.ListPaymentMethodsAsync(id);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return apiEx.StatusCode == (int)HttpStatusCode.NotFound ? Results.NotFound() : Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});


// --- Merchant Service Endpoints ---
app.MapPost("/merchant/payment_methods", async ([FromBody] MerchantPMLRequest request, MerchantService service, ILogger<Program> logger) =>
{
    const string endpointName = "POST /merchant/payment_methods";
    try
    {
        var sdkResponse = await service.ListAvailablePaymentMethodsAsync(request);
        LogResponse(logger, sdkResponse, endpointName);
        return Results.Ok(sdkResponse);
    }
    catch (HyperswitchApiException apiEx) { LogErrorResponse(logger, apiEx, endpointName); return Results.Problem(detail: apiEx.ResponseContent, statusCode: apiEx.StatusCode, title: $"HS API Error ({apiEx.ErrorDetails?.Error?.Code}): {apiEx.ErrorDetails?.Error?.Message ?? apiEx.Message}"); }
    catch (Exception ex) { LogGenericErrorResponse(logger, ex, endpointName); return Results.Problem(detail: ex.StackTrace, statusCode: 500, title: $"Error: {ex.Message}"); }
});


app.Run();

// DTOs are no longer needed as endpoints directly use SDK models.
