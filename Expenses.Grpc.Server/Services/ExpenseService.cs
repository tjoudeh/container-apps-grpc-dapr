using Grpc.Core;
using System.Collections;

namespace Expenses.Grpc.Server.Services
{
    public class ExpenseService : ExpenseSvc.ExpenseSvcBase
    {

        private readonly ILogger<ExpenseService> _logger;
        private readonly IExpensesRepo _expensesRepo;

        public ExpenseService(IExpensesRepo expensesRepo, ILogger<ExpenseService> logger)
        {
            _logger = logger;
            _expensesRepo = expensesRepo;
            _logger.LogInformation("Invoking Constructor");
            var daprGRPCPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT");

            if (!string.IsNullOrEmpty(daprGRPCPort))
            {
                _logger.LogInformation("Instantiating gRPC server using dapr sidecar on gRPC port: {daprGRPCPort}", daprGRPCPort);
            }

        }

        public override Task<GetExpensesResponse> GetExpenses(GetExpensesRequest request, ServerCallContext context)
        {

            _logger.LogInformation("Getting expenses for owner: {owner}", request.Owner);

            var response = new GetExpensesResponse();

            var filteredResults =  _expensesRepo.GetExpensesByOwner(request.Owner);

            response.Expenses.AddRange(filteredResults);

            return Task.FromResult(response);
        }

        public override Task<AddExpenseResponse> AddExpense(AddExpenseRequest request, ServerCallContext context)
        {

            _logger.LogInformation("Adding expense for provider {provider} for owner: {owner}", request.Provider, request.Owner);

            var response = new AddExpenseResponse();

            var expenseModel = new ExpenseModel()
            {
                Owner = request.Owner,
                Amount = request.Amount,
                Category = request.Category,
                Provider = request.Provider,
                Workflowstatus = request.Workflowstatus,
                Description = request.Description
            };

            _expensesRepo.AddExpense(expenseModel);

            response.Expense = expenseModel;

            return Task.FromResult(response);
        }

        public override Task<GetExpenseByIdResponse> GetExpenseById(GetExpenseByIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Getting expense by id: {id}", request.Id);

            var response = new GetExpenseByIdResponse();

            var expense = _expensesRepo.GetExpenseById(request.Id);

            response.Expense = expense;

            return Task.FromResult(response);

        }

    }

}
