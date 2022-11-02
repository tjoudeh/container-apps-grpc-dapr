namespace Expenses.Grpc.Server.Services
{
    public class ExpensesRepo : IExpensesRepo
    {
        private static List<ExpenseModel> _expensesList = new List<ExpenseModel>();

        private void GenerateRandomExpenses()
        {

            if (_expensesList.Count > 0)
            {
                return;
            }

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

        public ExpensesRepo()
        {
            GenerateRandomExpenses();
        }

        public ExpenseModel AddExpense(ExpenseModel expense)
        {
            expense.Id = _expensesList.Max(e => e.Id) + 1;
            _expensesList.Add(expense);
            return expense;
        }

        public ExpenseModel? GetExpenseById(int id)
        {
            return _expensesList.SingleOrDefault(e => e.Id == id);
        }

        public List<ExpenseModel> GetExpensesByOwner(string owner)
        {
            var expensesList = _expensesList.Where(t => t.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase)).OrderByDescending(o => o.Id).ToList();

            return expensesList;
        }
    }
}
