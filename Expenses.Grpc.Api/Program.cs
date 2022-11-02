using Expenses.Grpc.Server;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Xml.Linq;
using static Grpc.Core.Metadata;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddGrpcClient<ExpenseSvc.ExpenseSvcClient>(o =>
{
    var islocalhost = builder.Configuration.GetValue("grpc:localhost", false);

    var serverAddress = "";

    if (islocalhost)
    {
        var port = "7029";
        var scheme = "https";
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

app.MapGet("/api/expenses", async (ExpenseSvc.ExpenseSvcClient grpcClient, string owner) =>
{
    
    app?.Logger.LogInformation("Calling grpc server (GetExpensesAsync) for owner: {owner}", owner);

    var request = new GetExpensesRequest { Owner = owner };

    var response = await grpcClient.GetExpensesAsync(request);

    return Results.Ok(response.Expenses);

});

app.MapGet("/api/expenses/{id}", async (ExpenseSvc.ExpenseSvcClient grpcClient,int id) =>
{
    app?.Logger.LogInformation("Calling grpc server (GetExpenseByIdRequest) for id: {id}", id);

    var request = new GetExpenseByIdRequest { Id = id };

    var response = await grpcClient.GetExpenseByIdAsync(request);

    return Results.Ok(response.Expense);

}).WithName("GetExpenseById");

app.MapPost("/api/expenses", async (ExpenseSvc.ExpenseSvcClient grpcClient, ExpenseModel expenseModel) =>
{
    app?.Logger.LogInformation("Calling grpc server (AddExpenseRequest) for provider: {provider}", expenseModel.Provider);
   
    var request = new AddExpenseRequest { Provider = expenseModel.Provider, 
                                        Amount = expenseModel.Amount, 
                                        Category = expenseModel.Category, 
                                        Owner = expenseModel.Owner, 
                                        Workflowstatus = expenseModel.Workflowstatus, 
                                        Description = expenseModel.Description };

    var response = await grpcClient.AddExpenseAsync(request);

    return Results.CreatedAtRoute("GetExpenseById", new { id = response.Expense.Id }, response.Expense);
});

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