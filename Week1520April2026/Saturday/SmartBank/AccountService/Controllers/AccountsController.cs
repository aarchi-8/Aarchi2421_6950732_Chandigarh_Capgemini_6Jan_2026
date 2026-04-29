using System;
using System.Threading.Tasks;
using AccountService.DTOs;
using AccountService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers
{
  [ApiController]
  [Route("api/accounts")]
  public class AccountsController : ControllerBase
  {
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
      _accountService = accountService;
    }

    [HttpPost("open")]
    [Authorize]
    public async Task<IActionResult> Open([FromBody] OpenAccountDto openAccountDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        var account = await _accountService.OpenAccountAsync(openAccountDto);
        return Ok(account);
      }
      catch (ArgumentException exception)
      {
        return BadRequest(new { Message = exception.Message });
      }
    }

    [HttpPost("deposit")]
    [Authorize]
    public async Task<IActionResult> Deposit([FromBody] DepositDto depositDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var result = await _accountService.DepositAsync(depositDto);
      if (!result)
      {
        return BadRequest(new { Message = "Account not found or inactive" });
      }

      return Ok(new { Message = "Deposit successful" });
    }

    [HttpPost("withdraw")]
    [Authorize]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawDto withdrawDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        var result = await _accountService.WithdrawAsync(withdrawDto);
        if (!result)
        {
          return BadRequest(new { Message = "Account not found or inactive" });
        }

        return Ok(new { Message = "Withdrawal successful" });
      }
      catch (InvalidOperationException)
      {
        return BadRequest(new { Message = "Insufficient balance" });
      }
    }

    [HttpGet("balance/{accountNo}")]
    [Authorize]
    public async Task<IActionResult> GetBalance(string accountNo)
    {
      var balance = await _accountService.GetBalanceAsync(accountNo);
      if (balance == null)
      {
        return NotFound(new { Message = "Account not found" });
      }

      return Ok(balance);
    }

    [HttpGet("mini-statement/{accountNo}")]
    [Authorize]
    public async Task<IActionResult> GetMiniStatement(string accountNo)
    {
      var miniStatement = await _accountService.GetMiniStatementAsync(accountNo);
      return Ok(miniStatement);
    }

    [HttpDelete("close/{accountNo}")]
    [Authorize]
    public async Task<IActionResult> Close(string accountNo)
    {
      var result = await _accountService.CloseAccountAsync(accountNo);
      if (!result)
      {
        return NotFound(new { Message = "Account not found" });
      }

      return Ok(new { Message = "Account closed successfully" });
    }

    [HttpGet("internal/balance/{accountNo}")]
    [Authorize]
    public async Task<IActionResult> GetInternalBalance(string accountNo)
    {
      var balance = await _accountService.GetInternalBalanceAsync(accountNo);
      if (balance == null)
      {
        return NotFound(new { Message = "Account not found" });
      }

      return Ok(balance);
    }

    [HttpPost("internal/debit")]
    [Authorize]
    public async Task<IActionResult> InternalDebit([FromBody] WithdrawDto withdrawDto)
    {
      // This endpoint is also used by LoanService for EMI auto-debit
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        var result = await _accountService.InternalDebitAsync(withdrawDto);
        if (!result)
        {
          return BadRequest(new { Message = "Account not found or inactive" });
        }

        return Ok(new { Message = "Internal debit successful" });
      }
      catch (InvalidOperationException)
      {
        return BadRequest(new { Message = "Insufficient balance" });
      }
    }

    [HttpPost("internal/credit")]
    [Authorize]
    public async Task<IActionResult> InternalCredit([FromBody] DepositDto depositDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var result = await _accountService.InternalCreditAsync(depositDto);
      if (!result)
      {
        return BadRequest(new { Message = "Account not found or inactive" });
      }

      return Ok(new { Message = "Internal credit successful" });
    }
  }
}
