using ExpensesService.Api.Models;
using Moq;
using ExpensesService.Service.Services;
using ExpensesService.Gateway.Interfaces;
using Shouldly;
using ExpensesService.Abstraction.Mapper;

namespace ExpensesService.Test
{
    [TestFixture]
    public class ServiceLayerTest
    {
        private Mock<IExpenseRepository> _expenseRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private ServiceManager _serviceManager;

        [SetUp]
        public void Setup()
        {
            _expenseRepositoryMock = new Mock<IExpenseRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _serviceManager = new ServiceManager(_expenseRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Test]
        public async Task PostExpensesValidation_ExpenseCannotHaveDateInTheFuture_SuccesAsync()
        {
            // Arrange
            var user = new User { IdUser = 1, FirstName = "Anthony", LastName = "Stark", Currency = "U.S. dollar" };
            var expense = new Expense { IdExpense = 1, Amount = 2, Date = DateTime.Today.AddDays(-1), Comment = "Comment", User = user, IdUser = user.IdUser, Currency = user.Currency, Type = "Hotel" };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(user.ToModel);

            // Act & Assert
            var errorMessages = (await _serviceManager.ValidateExpenseAsync(expense)).ToList();
            errorMessages.ShouldNotBeNull();
            errorMessages.Count().ShouldBe(0);
        }

        [Test]
        public async Task PostExpensesValidation_ExpenseCannotHaveDateInTheFuture_FailAsync()
        {
            // Arrange
            var user = new User { IdUser = 1, FirstName = "Anthony", LastName = "Stark", Currency = "U.S. dollar" };
            var expense = new Expense { IdExpense = 1, Amount = 2, Date = DateTime.Today.AddDays(3), Comment = "Comment", User = user, IdUser = user.IdUser, Currency = user.Currency, Type = "Hotel" };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(user.ToModel);

            // Act & Assert
            var errorMessages = (await _serviceManager.ValidateExpenseAsync(expense)).ToList();
            errorMessages.ShouldNotBeNull();
            errorMessages.Count().ShouldBe(1);
            errorMessages[0].ShouldBe($"An expense cannot have a date in the future\nPosted expense date : {DateTime.Today.AddDays(3)}");
        }

        [Test]
        public async Task PostExpensesValidation_ExpenseCannotBeDatedMoreThan3MonthsAgo_SuccesAsync()
        {
            // Arrange
            var user = new User { IdUser = 1, FirstName = "Anthony", LastName = "Stark", Currency = "U.S. dollar" };
            var expense = new Expense { IdExpense = 1, Amount = 2, Date = DateTime.Now.AddMonths(-3), Comment = "Comment", User = user, IdUser = user.IdUser, Currency = user.Currency, Type = "Hotel" };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(user.ToModel);

            // Act & Assert
            var errorMessages = (await _serviceManager.ValidateExpenseAsync(expense)).ToList();
            errorMessages.ShouldNotBeNull();
            errorMessages.Count().ShouldBe(0);
        }

        [Test]
        public async Task PostExpensesValidation_ExpenseCannotBeDatedMoreThan3MonthsAgo_FailAsync()
        {
            // Arrange
            var user = new User { IdUser = 1, FirstName = "Anthony", LastName = "Stark", Currency = "U.S. dollar" };
            var expense = new Expense { IdExpense = 1, Amount = 2, Date = DateTime.Today.AddMonths(-3), Comment = "Comment", User = user, IdUser = user.IdUser, Currency = user.Currency, Type = "Hotel" };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(user.ToModel);

            // Act & Assert
            var errorMessages = (await _serviceManager.ValidateExpenseAsync(expense)).ToList();
            errorMessages.ShouldNotBeNull();
            errorMessages.Count().ShouldBe(1);
            errorMessages[0].ShouldBe($"An expense cannot be dated more than 3 months ago\nPosted expense date : {DateTime.Today.AddMonths(-3)}");
        }

        [Test]
        public async Task PostExpensesValidation_CommentIsMandatory_SuccesAsync()
        {
            // Arrange
            var user = new User { IdUser = 1, FirstName = "Anthony", LastName = "Stark", Currency = "U.S. dollar" };
            var expense = new Expense { IdExpense = 1, Amount = 2, Date = DateTime.Today, Comment = "Comment", User = user, IdUser = user.IdUser, Currency = user.Currency, Type = "Hotel" };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(user.ToModel);

            // Act & Assert
            var errorMessages = (await _serviceManager.ValidateExpenseAsync(expense)).ToList();
            errorMessages.ShouldNotBeNull();
            errorMessages.Count().ShouldBe(0);
        }

        [Test]
        public async Task PostExpensesValidation_CommentIsMandatory_FailAsync()
        {
            // Arrange
            var user = new User { IdUser = 1, FirstName = "Anthony", LastName = "Stark", Currency = "U.S. dollar" };
            var expense = new Expense { IdExpense = 1, Amount = 2, Date = DateTime.Today, User = user, IdUser = user.IdUser, Currency = user.Currency, Type = "Hotel" };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(user.ToModel);

            // Act & Assert
            var errorMessages = (await _serviceManager.ValidateExpenseAsync(expense)).ToList();
            errorMessages.ShouldNotBeNull();
            errorMessages.Count().ShouldBe(1);
            errorMessages[0].ShouldBe($"The comment is mandatory");
        }

        [Test]
        public async Task PostExpensesValidation_UserCannotDeclareTheSameExpenseTwice_SuccesAsync()
        {
            // Arrange
            var user = new User { IdUser = 1, FirstName = "Anthony", LastName = "Stark", Currency = "U.S. dollar" };
            var expense = new Expense { IdExpense = 1, Amount = 2, Date = DateTime.Today, Comment = "Comment", User = user, IdUser = user.IdUser, Currency = user.Currency, Type = "Hotel" };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(user.ToModel);
            _expenseRepositoryMock.Setup(repo => repo.ExpenseExistsAsync(DateTime.Today, 1)).ReturnsAsync(false);

            // Act & Assert
            var errorMessages = (await _serviceManager.ValidateExpenseAsync(expense)).ToList();
            errorMessages.ShouldNotBeNull();
            errorMessages.Count().ShouldBe(0);
        }

        [Test]
        public async Task PostExpensesValidation_UserCannotDeclareTheSameExpenseTwice_FailAsync()
        {
            // Arrange
            var user = new User { IdUser = 1, FirstName = "Anthony", LastName = "Stark", Currency = "U.S. dollar" };
            var expense = new Expense { IdExpense = 1, Amount = 2, Date = DateTime.Today, Comment = "Comment", User = user, IdUser = user.IdUser, Currency = user.Currency, Type = "Hotel" };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(user.ToModel);
            _expenseRepositoryMock.Setup(repo => repo.ExpenseExistsAsync(DateTime.Today, 2)).ReturnsAsync(true);

            // Act & Assert
            var errorMessages = (await _serviceManager.ValidateExpenseAsync(expense)).ToList();
            errorMessages.ShouldNotBeNull();
            errorMessages.Count().ShouldBe(1);
            errorMessages[0].ShouldBe($"A user cannot declare the same expense twice (same date and amount)\nPosted Date: 04/07/2024 00:00:00\nPosted amount: 2");
        }

        [Test]
        public async Task PostExpensesValidation_TheCurrencyOfTheExpenseMustMatchTheUsersCurrency_SuccesAsync()
        {
            // Arrange
            var user = new User { IdUser = 1, FirstName = "Anthony", LastName = "Stark", Currency = "U.S. dollar" };
            var expense = new Expense { IdExpense = 1, Amount = 2, Date = DateTime.Today, Comment = "Comment", User = user, IdUser = user.IdUser, Currency = user.Currency, Type = "Hotel" };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(user.ToModel);

            // Act & Assert
            var errorMessages = (await _serviceManager.ValidateExpenseAsync(expense)).ToList();
            errorMessages.ShouldNotBeNull();
            errorMessages.Count().ShouldBe(0);
        }

        [Test]
        public async Task PostExpensesValidation_TheCurrencyOfTheExpenseMustMatchTheUsersCurrency_FailAsync()
        {
            // Arrange
            var user = new User { IdUser = 1, FirstName = "Anthony", LastName = "Stark", Currency = "euro" };
            var expense = new Expense { IdExpense = 1, Amount = 2, Date = DateTime.Today, Comment = "Comment", User = user, IdUser = user.IdUser, Currency = "U.S. dollar", Type = "Hotel" };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(user.ToModel);

            // Act & Assert
            var errorMessages = (await _serviceManager.ValidateExpenseAsync(expense)).ToList();
            errorMessages.ShouldNotBeNull();
            errorMessages.Count().ShouldBe(1);
            errorMessages[0].ShouldBe($"The currency of the expense must match the user’s currency\nExpense currency: U.S. dollar\nUser currency: euro");
        }
    }
}