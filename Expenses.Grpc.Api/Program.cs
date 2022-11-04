using Dapr.Client;
using Expenses.Grpc.Server;
using Grpc.Core;
using static Dapr.Client.Autogen.Grpc.v1.Dapr;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDaprClient();

builder.Services.AddGrpcClient<ExpenseSvc.ExpenseSvcClient>(o =>
{
    var islocalhost = builder.Configuration.GetValue("grpc:localhost", false);

    var serverAddress = "";

    if (islocalhost)
    {
        var port = "7029";
        var scheme = "https";
        var daprGRPCPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT");

        if (!string.IsNullOrEmpty(daprGRPCPort))
        {
            scheme = "http";
            port = daprGRPCPort;
        }

        serverAddress = string.Format(builder.Configuration.GetValue<string>("grpc:server"), scheme, port);
    }
    else
    {
        serverAddress = builder.Configuration.GetValue<string>("grpc:server");
    }

    o.Address = new Uri(serverAddress);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/api/expenses", async (ExpenseSvc.ExpenseSvcClient grpcClient, Dapr.Client.DaprClient daprClient, string owner) =>
{
    GetExpensesResponse? response;
    
    var request = new GetExpensesRequest { Owner = owner };

    if (builder.Configuration.GetValue("grpc:daprClientSDK", false))
    {
        app?.Logger.LogInformation("DaprClientSDK::Calling grpc server (GetExpenses) for owner: {owner}", owner);

        response = await daprClient.InvokeMethodGrpcAsync<GetExpensesRequest, GetExpensesResponse>("expenses-grpc-server", "GetExpenses", request);
    }
    else
    {
        app?.Logger.LogInformation("Calling grpc server (GetExpenses) for owner: {owner}", owner);

        response = await grpcClient.GetExpensesAsync(request, BuildMetadataHeader());
    }

    return Results.Ok(response.Expenses);

});

app.MapGet("/api/expenses/{id}", async (ExpenseSvc.ExpenseSvcClient grpcClient, Dapr.Client.DaprClient daprClient, int id) =>
{
    GetExpenseByIdResponse? response;

    var request = new GetExpenseByIdRequest { Id = id };

    if (builder.Configuration.GetValue("grpc:daprClientSDK", false))
    {
        app?.Logger.LogInformation("DaprClientSDK::Calling grpc server (GetExpenseByIdRequest) for id: {id}", id);
        response = await daprClient.InvokeMethodGrpcAsync<GetExpenseByIdRequest, GetExpenseByIdResponse>("expenses-grpc-server", "GetExpenseById", request);
    }
    else
    {
        app?.Logger.LogInformation("Calling grpc server (GetExpenseByIdRequest) for id: {id}", id);
        response = await grpcClient.GetExpenseByIdAsync(request, BuildMetadataHeader());
    }

    return Results.Ok(response.Expense);

}).WithName("GetExpenseById");

app.MapPost("/api/expenses", async (ExpenseSvc.ExpenseSvcClient grpcClient, Dapr.Client.DaprClient daprClient,ExpenseModel expenseModel) =>
{

    AddExpenseResponse? response;

    var request = new AddExpenseRequest
    {
        Provider = expenseModel.Provider,
        Amount = expenseModel.Amount,
        Category = expenseModel.Category,
        Owner = expenseModel.Owner,
        Workflowstatus = expenseModel.Workflowstatus,
        Description = expenseModel.Description
    };

    if (builder.Configuration.GetValue("grpc:daprClientSDK", false))
    {
        app?.Logger.LogInformation("DaprClientSDK::Calling grpc server (AddExpenseRequest) for provider: {provider}", expenseModel.Provider);
        response = await daprClient.InvokeMethodGrpcAsync<AddExpenseRequest, AddExpenseResponse>("expenses-grpc-server", "AddExpense", request);
    }
    else
    {
        app?.Logger.LogInformation("Calling grpc server (AddExpenseRequest) for provider: {provider}", expenseModel.Provider);
        response = await grpcClient.AddExpenseAsync(request, BuildMetadataHeader());
    }

    return Results.CreatedAtRoute("GetExpenseById", new { id = response.Expense.Id }, response.Expense);
});

Metadata? BuildMetadataHeader()
{
    //The gRPC port that the Dapr sidecar is listening on
    var daprGRPCPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT");

    Metadata? metadata = null;

    if (!string.IsNullOrEmpty(daprGRPCPort))
    {
        metadata = new Metadata();
        var serverDaprAppId = "expenses-grpc-server";
        metadata.Add("dapr-app-id", serverDaprAppId);
        app?.Logger.LogInformation("Calling gRPC server app id '{server}' using dapr sidecar on gRPC port: {daprGRPCPort}", serverDaprAppId, daprGRPCPort);
    }

    return metadata;
}

app.Run();

internal class ExpenseModel
{
    public string Provider { get; set; } = string.Empty;
    public double Amount { get; set; } = 0.0;
    public string Category { get; set; } = string.Empty;
    public string Owner  { get; set; } = string.Empty;
    public int Workflowstatus  { get; set; } 
    public string Description { get; set; } = string.Empty;
}