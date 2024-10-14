using ExpensesService.Api.Data;
using ExpensesService.Api.Models;
using ExpensesService.Gateway.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesService.Gateway.Gateway
{
    public class UserRepository:IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IEnumerable<ExpenseModel>> GetUserExpensesAsync(int idUser)
        {
            IQueryable<ExpenseModel> query = _context.Expenses;
            query = query.Include(expense => expense.User);

            return await query.Where(expense => expense.User.IdUser == idUser).ToListAsync();
        }

        public async Task<UserModel?> GetUserByIdAsync(int idUser)
        {
            return await _context.Users.FindAsync(idUser);
        }

        public async Task createUserAsync(UserModel user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
