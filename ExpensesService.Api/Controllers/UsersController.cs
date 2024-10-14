using Microsoft.AspNetCore.Mvc;
using ExpensesService.Api.Models;
using ExpensesService.Service.Services;

namespace UsersService.Api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ServiceManager _serviceManager;

        public UsersController(ServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            try
            {
                return Ok(await _serviceManager.GetAllUsersAsync());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        [HttpGet("{id}/expenses")]
        public async Task<ActionResult<IEnumerable<ExpenseModel>>> GetUserExpenses(int id)
        {
            try
            {
                return Ok(await _serviceManager.GetUserExpensesAsync(id));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUserById(int id)
        {
            try
            {
                return Ok(await _serviceManager.GetUserByIdAsync(id));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserModel>> PostExpense(User user)
        {
            try
            {
                await _serviceManager.PostUserAsync(user);
                return CreatedAtAction(nameof(GetUserById), new { id = user.IdUser }, user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}

