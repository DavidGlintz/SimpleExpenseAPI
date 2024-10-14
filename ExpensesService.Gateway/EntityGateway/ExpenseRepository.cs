using ExpensesService.Api.Data;
using ExpensesService.Api.Models;
using ExpensesService.Gateway.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ExpensesService.Gateway.EntityGateway
{
    public class ExpenseRepository: IExpenseRepository
    {
        private readonly ApplicationDbContext _context;

        public ExpenseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExpenseModel>> GetExpensesAsync(string sortBy)
        {
            IQueryable<ExpenseModel> query = _context.Expenses;
            if (sortBy is not null)
            {
                query = sortBy.ToLower() switch
                {
                    "amount" => query.OrderBy(e => e.Amount),
                    "date" => query.OrderBy(e => e.Date),
                    _ => query
                };
            }

            query = query.Include(expense => expense.User);

            return await query.ToListAsync();
        }

        public async Task<bool> ExpenseExistsAsync(DateTime date, decimal amount)
        {
            IQueryable<ExpenseModel> query = _context.Expenses;
            query = query.Include(expense => expense.User);
            date = date.ToUniversalTime();
            return await query.AnyAsync(expense => expense.Date == date.Date && expense.Amount == amount);
        }

        public async Task<ExpenseModel?> GetExpenseByIdAsync(int IdExpense)
        {
            IQueryable<ExpenseModel> query = _context.Expenses;
            query = query.Include(expense => expense.User);

            return await query.FirstOrDefaultAsync(expense => expense.IdExpense == IdExpense);
        }


        public async Task createExpenseAsync(ExpenseModel expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }
    }
}
