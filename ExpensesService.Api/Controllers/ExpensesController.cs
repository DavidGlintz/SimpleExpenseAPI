using ExpensesService.Api.Models;
using ExpensesService.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesService.Api.Controllers
{
    [Route("api/expense")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly ServiceManager _serviceManager;

        public ExpensesController(ServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("{sortBy}")]
        public async Task<IActionResult> GetExpenses(string sortBy = "date")
        {
            try
            {
                return Ok(await _serviceManager.GetAllExpensesAsync(sortBy));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetExpenseById(int id)
        {
            try
            {
                var expense = await _serviceManager.GetExpenseByIdAsync(id);
                if (expense == null) return NotFound();
                return Ok(expense);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostExpense([FromBody] Expense expense)
        {
            try
            {
                var errorList = await _serviceManager.ValidateExpenseAsync(expense);
                if (errorList.Any())
                {
                    return BadRequest(_serviceManager.composeErrorMessage(errorList, typeof(Expense).Name));
                }

                await _serviceManager.PostExpenseAsync(expense);
                return CreatedAtAction(nameof(GetExpenseById), new { id = expense.IdExpense }, expense);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}

