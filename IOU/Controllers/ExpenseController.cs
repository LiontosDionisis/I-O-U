using System.Security.Claims;
using Api.IOU.DTOs;
using Api.IOU.Exceptions;
using Api.IOU.Models;
using Api.IOU.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.IOU.Controllers;

[ApiController]
[Route("api/expenses")]
[Authorize]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly ILogger<ExpenseController> _logger;

    public ExpenseController(IExpenseService expenseService, ILogger<ExpenseController> logger)
    {
        _expenseService = expenseService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateExpense([FromBody] AddExpenseDTO dto)
    {
        try
        {
            var expense = await _expenseService.CreateExpenseAsync(dto);
            return Ok(expense);
        }
        catch (SessionNotFoundException e)
        {
            return BadRequest("Session does not exist");
        }
        catch (InvalidOperationException e)
        {
            return BadRequest("Something went wrong");
        }
        catch (Exception e)
        {
            return StatusCode(500, new { message = "Server error. Please try again later" });
        }
    }

    [HttpGet("session/{sessionId:int}")]
    public async Task<IActionResult> GetExpensesBySession(int sessionId)
    {
        try
        {
            var expenses = await _expenseService.GetExpensesBySessionIdAsync(sessionId);
            return Ok(expenses);
        }
        catch (SessionNotFoundException e)
        {
            return NotFound(new { message = "Session does not exist" });
        }
    }


    [HttpPut("{expenseId:int}/settle")]
    public async Task<IActionResult> SettleSplit(int expenseId)
    {
        var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        try
        {
            var split = await _expenseService.SettleExpenseSplitAsync(expenseId, userId);

            var result = new ExpenseSplitDTO
            {
                Id = split.Id,
                UserId = split.UserId,
                Amount = split.Amount,
                Status = SplitStatus.Settled
            };

            return Ok(result);
        }
        catch (ExpenseNotFoundException e)
        {
            return NotFound(new { message = "Expense was not found" });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpDelete("{expenseId:int}")]
    public async Task<IActionResult> DeleteExpense(int expenseId)
    {
        var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        try
        {
            await _expenseService.DeleteExpenseAsync(expenseId, userId);
            return Ok(new { message = "Expense was deleted" });
        }
        catch (ExpenseNotFoundException e)
        {
            return NotFound("Expense as not found");
        }
        catch (UnauthorizedAccessException e)
        {
            return Forbid(e.Message);
        }
    }
}