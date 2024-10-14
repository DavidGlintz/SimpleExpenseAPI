using ExpensesService.Abstraction.Mapper;
using ExpensesService.Api.Models;
using ExpensesService.Gateway.Interfaces;

namespace ExpensesService.Service.Services
{
    public class ServiceManager
    {

        private readonly IExpenseRepository _expenseRepository;
        private readonly IUserRepository _userRepository;

        public ServiceManager(IExpenseRepository expenseRepository, IUserRepository userRepository)
        {
            _expenseRepository = expenseRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Expense>> GetAllExpensesAsync(string sortBy = "")
        {
            return (await _expenseRepository.GetExpensesAsync(sortBy)).Select(x => x.ToDto());
        }

        public async Task<bool> ExpenseExistsAsync(DateTime date, decimal amount)
        {
            return await _expenseRepository.ExpenseExistsAsync(date, amount);
        }

        public async Task<Expense?> GetExpenseByIdAsync(int IdExpense)
        {
            return (await _expenseRepository.GetExpenseByIdAsync(IdExpense))?.ToDto();
        }

        public async Task PostExpenseAsync(Expense expense)
        {
            var validationErrors = await ValidateExpenseAsync(expense);

            await _expenseRepository.createExpenseAsync(expense.ToModel());
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return (await _userRepository.GetUsersAsync()).Select(x => x.ToDto());
        }

        public async Task<IEnumerable<Expense>> GetUserExpensesAsync(int idUser)
        {
            return (await _userRepository.GetUserExpensesAsync(idUser)).Select(x => x.ToDto());
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return (await _userRepository.GetUserByIdAsync(id))?.ToDto();
        }

        public async Task PostUserAsync(User user)
        {
            await _userRepository.createUserAsync(user.ToModel());
        }

        public async Task<List<string>> ValidateExpenseAsync(Expense expense)
        {
            var errors = new List<string>();

            if (expense.Date.ToUniversalTime() > DateTime.UtcNow)
            {
                errors.Add($"An expense cannot have a date in the future\nPosted expense date : {expense.Date}");
            }

            if (expense.Date.ToUniversalTime() < DateTime.Today.AddMonths(-3))
            {
                errors.Add($"An expense cannot be dated more than 3 months ago\nPosted expense date : {expense.Date}");
            }

            if (string.IsNullOrEmpty(expense.Comment))
            {
                errors.Add($"The comment is mandatory");
            }

            if (await ExpenseExistsAsync(expense.Date, expense.Amount))
            {
                errors.Add($"A user cannot declare the same expense twice (same date and amount)\nPosted Date: {expense.Date}\nPosted amount: {expense.Amount}");
            }

            var user = await GetUserByIdAsync(expense.IdUser);
            if (user.Currency != expense.Currency)
            {
                errors.Add($"The currency of the expense must match the user’s currency\nExpense currency: {expense.Currency}\nUser currency: {user.Currency}");
            }

            return errors;
        }

        public string composeErrorMessage(IEnumerable<string> errorMessages, string typeName) 
        {
            var errorMessage = $"Posted {typeName} is not valid:\n";
            foreach (var error in errorMessages)
            {
                errorMessage += "- " + error + "\n";
            }
            return errorMessage;
        }

    }
}
