using Grpc.Core;

namespace Expenses.Grpc.Server.Services
{
    public class ExpenseService : ExpenseSvc.ExpenseSvcBase
    {

        private readonly ILogger<ExpenseService> _logger;
        private static List<ExpenseModel> _expensesList = new List<ExpenseModel>();

        private void GenerateRandomExpenses()
        {
         
            if (_expensesList.Count > 0)
            {
                return;
            }

            _logger.LogInformation("Generating Random Expenses");

            _expensesList.Add(new ExpenseModel
            {
                Id = 1,
                Provider = "Golds Gym",
                Amount = 290,
                Category = "Fitness Activity",
                Owner = "tjoudeh@mail.com",
                Workflowstatus = 1,
                Description = ""
            });

            _expensesList.Add(new ExpenseModel
            {
                Id = 2,
                Provider = "Adidas",
                Amount = 100,
                Category = "Athletic Shoes",
                Owner = "tjoudeh@mail.com",
                Workflowstatus = 1,
                Description = ""
            });

            _expensesList.Add(new ExpenseModel
            {
                Id = 3,
                Provider = "FreeMind",
                Amount = 25,
                Category = "Yoga Class",
                Owner = "xyz@yahoo.com",
                Workflowstatus = 2,
                Description = ""
            });
        }

        public ExpenseService(ILogger<ExpenseService> logger)
        {
            _logger = logger;
            _logger.LogInformation("Invoking Constructor");
            GenerateRandomExpenses();
        }

        public override Task<GetExpensesResponse> GetExpenses(GetExpensesRequest request, ServerCallContext context)
        {

            _logger.LogInformation("Getting expenses for owner: {owner}", request.Owner);

            var response = new GetExpensesResponse();

            var filteredResults = _expensesList.Where(
                                    f => f.Owner.Equals(request.Owner, StringComparison.CurrentCultureIgnoreCase));

            response.Expenses.AddRange(filteredResults);

            return Task.FromResult(response);
        }

        public override Task<AddExpenseResponse> AddExpense(AddExpenseRequest request, ServerCallContext context)
        {

            _logger.LogInformation("Adding expense for provider {provider} for owner: {owner}", request.Provider, request.Owner);

            var response = new AddExpenseResponse();

            var id = _expensesList.Max(e => e.Id) + 1;
            
            var expenseModel = new ExpenseModel()
            {
                Id = id,
                Owner = request.Owner,
                Amount = request.Amount,
                Category = request.Category,
                Provider = request.Provider,
                Workflowstatus = request.Workflowstatus,
                Description = request.Description
            };

            _expensesList.Add(expenseModel);

            response.Expense = expenseModel;

            return Task.FromResult(response);
        }

        public override Task<GetExpenseByIdResponse> GetExpenseById(GetExpenseByIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Getting expense by id: {id}", request.Id);

            var response = new GetExpenseByIdResponse();

            var expense = _expensesList.SingleOrDefault(e => e.Id == request.Id);

            response.Expense = expense;

            return Task.FromResult(response);

        }

    }

}
 