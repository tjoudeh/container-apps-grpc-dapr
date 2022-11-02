namespace Expenses.Grpc.Server.Services
{
    public interface IExpensesRepo
    {
        List<ExpenseModel> GetExpensesByOwner(string owner);
        ExpenseModel? GetExpenseById(int id);
        ExpenseModel AddExpense(ExpenseModel expense);
    }
}
