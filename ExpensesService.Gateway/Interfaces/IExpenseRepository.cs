using ExpensesService.Api.Models;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesService.Gateway.Interfaces
{
    public interface IExpenseRepository
    {
        Task<IEnumerable<ExpenseModel>> GetExpensesAsync(string sortBy);
        Task<bool> ExpenseExistsAsync(DateTime date, decimal amount);
        Task<ExpenseModel?> GetExpenseByIdAsync(int IdExpense);
        Task createExpenseAsync(ExpenseModel expense);

    }
}
