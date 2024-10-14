using ExpensesService.Api.Models;

namespace ExpensesService.Abstraction.Mapper
{
    public static class GatewayMapper
    {
        public static ExpenseModel ToModel(this Expense model) => new()
        {
            Amount = model.Amount,
            Currency = model.Currency,
            Date = model.Date, 
            IdExpense = model.IdExpense,
            User = model.User?.ToModel(),
            IdUser = model.IdUser,
            Type = model.Type,
            Comment = model.Comment 
        };

        public static Expense ToDto(this ExpenseModel dto) => new()
        {
            Amount = dto.Amount,
            Currency = dto.Currency,
            Date = dto.Date,
            IdExpense = dto.IdExpense,
            User = dto.User?.ToDto(),
            IdUser = dto.IdUser,
            Type = dto.Type,
            Comment = dto.Comment
        };

        public static UserModel ToModel(this User dto) => new()
        {
            Currency = dto.Currency,
            IdUser = dto.IdUser,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
        };

        public static User ToDto(this UserModel model) => new()
        {
            Currency = model.Currency,
            IdUser = model.IdUser,
            FirstName = model.FirstName,
            LastName = model.LastName,
        };

    }
}
