using Dapr.AppCallback.Autogen.Grpc.v1;
using Dapr.Client.Autogen.Grpc.v1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Expenses.Grpc.Server.Services
{
    public class ExpenseServiceAppCallBack : AppCallback.AppCallbackBase
    {

        private readonly ILogger<ExpenseServiceAppCallBack> _logger;
        private readonly IExpensesRepo _expensesRepo;
        
        public ExpenseServiceAppCallBack(IExpensesRepo expensesRepo, ILogger<ExpenseServiceAppCallBack> logger)
        {
            _expensesRepo = expensesRepo;
            _logger = logger;
        }

        public override Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
        {
            var response = new InvokeResponse();

            switch (request.Method)
            {
                case "GetExpenses":

                    var getExpensesRequestInput = request.Data.Unpack<GetExpensesRequest>();
                    var getExpensesResponseOutput = new GetExpensesResponse();

                    _logger.LogInformation("Getting expenses for owner: {owner}", getExpensesRequestInput.Owner);

                    var filteredResults = _expensesRepo.GetExpensesByOwner(getExpensesRequestInput.Owner);
                    getExpensesResponseOutput.Expenses.AddRange(filteredResults);

                    response.Data = Any.Pack(getExpensesResponseOutput);
                    break;

                case "GetExpenseById":
                   
                    var getExpenseByIdRequestInput = request.Data.Unpack<GetExpenseByIdRequest>();
                    var getExpenseByIdResponseOutput = new GetExpenseByIdResponse();

                    _logger.LogInformation("Getting expense by id: {id}", getExpenseByIdRequestInput.Id);

                    var expense = _expensesRepo.GetExpenseById(getExpenseByIdRequestInput.Id);
                    getExpenseByIdResponseOutput.Expense = expense;

                    response.Data = Any.Pack(getExpenseByIdResponseOutput);
                    break;

                case "AddExpense":

                    var addExpenseRequestInput = request.Data.Unpack<AddExpenseRequest>();
                    var addExpenseResponseOutput = new AddExpenseResponse();

                    _logger.LogInformation("Adding expense for provider {provider} for owner: {owner}", addExpenseRequestInput.Provider, addExpenseRequestInput.Owner);

                    var expenseModel = new ExpenseModel()
                    {
                        Owner = addExpenseRequestInput.Owner,
                        Amount = addExpenseRequestInput.Amount,
                        Category = addExpenseRequestInput.Category,
                        Provider = addExpenseRequestInput.Provider,
                        Workflowstatus = addExpenseRequestInput.Workflowstatus,
                        Description = addExpenseRequestInput.Description
                    };

                    _expensesRepo.AddExpense(expenseModel);
                    addExpenseResponseOutput.Expense = expenseModel;

                    response.Data = Any.Pack(addExpenseResponseOutput);
                    break;
               
                default:
                    break;
            }

            return Task.FromResult(response);
        }
    }
}
