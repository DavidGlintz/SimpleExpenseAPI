using ExpensesService.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesService.Gateway.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserModel>> GetUsersAsync();
        Task<IEnumerable<ExpenseModel>> GetUserExpensesAsync(int idUser);
        Task<UserModel?> GetUserByIdAsync(int idUser);
        Task createUserAsync(UserModel user);

    }
}
