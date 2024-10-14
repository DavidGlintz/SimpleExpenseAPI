using ExpensesService.Api.Models;
using Moq;
using ExpensesService.Service.Services;
using ExpensesService.Gateway.Interfaces;
using Shouldly;
using ExpensesService.Abstraction.Mapper;
using ExpensesService.Api.Data;
using Microsoft.EntityFrameworkCore;
using ExpensesService.Gateway.EntityGateway;
using ExpensesService.Gateway.Gateway;

namespace ExpensesService.Test
{
    [TestFixture]
    public class GatewayLayerTest
    {
        private DbContextOptions<ApplicationDbContext> _contextOptions;

        [SetUp]
        public void Setup()
        {
            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [TearDown]
        public void TearDown()
        {
            using (var context = new ApplicationDbContext(_contextOptions))
            {
                context.Database.EnsureDeleted();
            }
        }

        [Test]
        public async Task GetExpensesAsync_ListingTheExpensesForAGivenUser()
        {
            // Arrange
            using (var context = new ApplicationDbContext(_contextOptions))
            {
                context.Users.AddRange(
                    new UserModel { IdUser = 1, FirstName = "Anthony", LastName = "Stark", Currency = "U.S. dollar" },
                    new UserModel { IdUser = 2, FirstName = "Natasha", LastName = "Romanova", Currency = "Russian ruble" }
                );
                context.Expenses.AddRange(
                    new ExpenseModel { IdExpense = 1, Amount = new decimal(2.01), Date = DateTime.Today.AddDays(-3), Comment = "Comment", IdUser = 1, Currency = "U.S. dollar", Type = "Hotel" },
                    new ExpenseModel { IdExpense = 2, Amount = new decimal(1.23), Date = DateTime.Today.AddDays(-1), Comment = "Comment", IdUser = 2, Currency = "Russian ruble", Type = "Hotel" },
                    new ExpenseModel { IdExpense = 3, Amount = new decimal(4.19), Date = DateTime.Today.AddDays(-2), Comment = "Comment", IdUser = 1, Currency = "U.S. dollar", Type = "Hotel" }
                );
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(_contextOptions))
            {
                var userRepository = new UserRepository(context);

                // Act
                var result = (await userRepository.GetUserExpensesAsync(1)).ToList();

                // Assert
                result.ShouldNotBeNull();
                result.Count().ShouldBe(2);

                result[0].IdExpense.ShouldBe(1);
                result[1].IdExpense.ShouldBe(3);
            }
        }

        [Test]
        public async Task GetExpensesAsync_SortingExpensesByAmount()
        {
            // Arrange
            using (var context = new ApplicationDbContext(_contextOptions))
            {
                context.Users.AddRange(
                    new UserModel { IdUser = 1, FirstName = "Anthony", LastName = "Stark", Currency = "U.S. dollar" },
                    new UserModel { IdUser = 2, FirstName = "Natasha", LastName = "Romanova", Currency = "Russian ruble" }
                );
                context.Expenses.AddRange(
                    new ExpenseModel { IdExpense = 1, Amount = new decimal(2.01), Date = DateTime.Today.AddDays(-3), Comment = "Comment", IdUser = 1, Currency = "U.S. dollar", Type = "Hotel" },
                    new ExpenseModel { IdExpense = 2, Amount = new decimal(1.23), Date = DateTime.Today.AddDays(-1), Comment = "Comment", IdUser = 2, Currency = "Russian ruble", Type = "Hotel" },
                    new ExpenseModel { IdExpense = 3, Amount = new decimal(4.19), Date = DateTime.Today.AddDays(-2), Comment = "Comment", IdUser = 1, Currency = "U.S. dollar", Type = "Hotel" }
                );
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(_contextOptions))
            {
                var expenseRepository = new ExpenseRepository(context);

                // Act
                var result = (await expenseRepository.GetExpensesAsync("amount")).ToList();

                // Assert
                result.ShouldNotBeNull();
                result.Count().ShouldBe(3);

                result[0].IdExpense.ShouldBe(2);
                result[0].Amount.ToString().ShouldBe("1,23");

                result[1].IdExpense.ShouldBe(1);
                result[1].Amount.ToString().ShouldBe("2,01");

                result[2].IdExpense.ShouldBe(3);
                result[2].Amount.ToString().ShouldBe("4,19");
            }
        }

        [Test]
        public async Task GetExpensesAsync_SortingExpensesByDate()
        {
            // Arrange
            using (var context = new ApplicationDbContext(_contextOptions))
            {
                context.Users.AddRange(
                    new UserModel { IdUser = 1, FirstName = "Anthony", LastName = "Stark", Currency = "U.S. dollar" },
                    new UserModel { IdUser = 2, FirstName = "Natasha", LastName = "Romanova", Currency = "Russian ruble" }
                );
                context.Expenses.AddRange(
                    new ExpenseModel { IdExpense = 1, Amount = new decimal(2.01), Date = DateTime.Today.AddDays(-3), Comment = "Comment", IdUser = 1, Currency = "U.S. dollar", Type = "Hotel" },
                    new ExpenseModel { IdExpense = 2, Amount = new decimal(1.23), Date = DateTime.Today.AddDays(-1), Comment = "Comment", IdUser = 2, Currency = "Russian ruble", Type = "Hotel" },
                    new ExpenseModel { IdExpense = 3, Amount = new decimal(4.19), Date = DateTime.Today.AddDays(-2), Comment = "Comment", IdUser = 1, Currency = "U.S. dollar", Type = "Hotel" }
                );
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(_contextOptions))
            {
                var expenseRepository = new ExpenseRepository(context);

                // Act
                var result = (await expenseRepository.GetExpensesAsync("date")).ToList();

                // Assert
                result.ShouldNotBeNull();
                result.Count().ShouldBe(3);

                result[0].IdExpense.ShouldBe(1);
                result[0].Date.ToString().ShouldBe(DateTime.Today.AddDays(-3).ToString());

                result[1].IdExpense.ShouldBe(3);
                result[1].Date.ToString().ShouldBe(DateTime.Today.AddDays(-2).ToString());

                result[2].IdExpense.ShouldBe(2);
                result[2].Date.ToString().ShouldBe(DateTime.Today.AddDays(-1).ToString());

            }
        }

        [Test]
        public async Task GetExpensesAsync_DisplayingAllThePropertiesOfTheExpense()
        {
            // Arrange
            using (var context = new ApplicationDbContext(_contextOptions))
            {
                context.Users.AddRange(
                    new UserModel { IdUser = 1, FirstName = "Anthony", LastName = "Stark", Currency = "U.S. dollar" },
                    new UserModel { IdUser = 2, FirstName = "Natasha", LastName = "Romanova", Currency = "Russian ruble" }
                );
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(_contextOptions))
            {
                var userRepository = new UserRepository(context);

                // Act
                var result = (await userRepository.GetUsersAsync()).ToList();

                // Assert
                result.ShouldNotBeNull();
                result.Count().ShouldBe(2);

                result[0].FullName.ShouldBe("Anthony Stark");
                result[1].FullName.ShouldBe("Natasha Romanova");
            }
        }
    }
}