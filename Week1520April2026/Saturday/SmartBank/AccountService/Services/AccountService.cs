using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AccountService.Data;
using AccountService.DTOs;
using AccountService.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AccountService.Services
{
  public class AccountService : IAccountService
  {
    private readonly AccountDbContext _accountDbContext;
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountService(AccountDbContext accountDbContext, IHttpClientFactory httpClientFactory)
    {
      _accountDbContext = accountDbContext;
      _httpClientFactory = httpClientFactory;
    }

    public async Task<Account> OpenAccountAsync(OpenAccountDto openAccountDto)
    {
      if (openAccountDto.AccountType != "Savings" && openAccountDto.AccountType != "Current")
      {
        throw new ArgumentException("AccountType must be Savings or Current");
      }

      var accountNo = await GenerateUniqueAccountNoAsync();

      var account = new Account
      {
        AccountNo = accountNo,
        AccountType = openAccountDto.AccountType,
        CustomerId = openAccountDto.CustomerId,
        Balance = 0,
        Status = "Active",
        CreatedAt = DateTime.UtcNow
      };

      _accountDbContext.Accounts.Add(account);
      await _accountDbContext.SaveChangesAsync();

      await SendAuditLogAsync(
        "OpenAccount",
        account.AccountNo,
        $"Account opened with type {account.AccountType}");

      return account;
    }

    public async Task<bool> DepositAsync(DepositDto depositDto)
    {
      var account = await _accountDbContext.Accounts
        .FirstOrDefaultAsync(a => a.AccountNo == depositDto.AccountNo);

      if (account == null || account.Status != "Active")
      {
        return false;
      }

      account.Balance += depositDto.Amount;

      var transaction = new Transaction
      {
        AccountNo = depositDto.AccountNo,
        Type = "Credit",
        Amount = depositDto.Amount,
        Description = depositDto.Description,
        CreatedAt = DateTime.UtcNow
      };

      _accountDbContext.Transactions.Add(transaction);
      await _accountDbContext.SaveChangesAsync();

      await SendAuditLogAsync(
        "Deposit",
        account.AccountNo,
        $"Amount {depositDto.Amount} credited");

      return true;
    }

    public async Task<bool> WithdrawAsync(WithdrawDto withdrawDto)
    {
      var account = await _accountDbContext.Accounts
        .FirstOrDefaultAsync(a => a.AccountNo == withdrawDto.AccountNo);

      if (account == null || account.Status != "Active")
      {
        return false;
      }

      if (account.Balance < withdrawDto.Amount)
      {
        throw new InvalidOperationException("Insufficient balance");
      }

      account.Balance -= withdrawDto.Amount;

      var transaction = new Transaction
      {
        AccountNo = withdrawDto.AccountNo,
        Type = "Debit",
        Amount = withdrawDto.Amount,
        Description = withdrawDto.Description,
        CreatedAt = DateTime.UtcNow
      };

      _accountDbContext.Transactions.Add(transaction);
      await _accountDbContext.SaveChangesAsync();

      await SendAuditLogAsync(
        "Withdraw",
        account.AccountNo,
        $"Amount {withdrawDto.Amount} debited");

      return true;
    }

    public async Task<object?> GetBalanceAsync(string accountNo)
    {
      var account = await _accountDbContext.Accounts
        .AsNoTracking()
        .FirstOrDefaultAsync(a => a.AccountNo == accountNo);

      if (account == null)
      {
        return null;
      }

      return new
      {
        account.AccountNo,
        account.Balance,
        account.Status
      };
    }

    public async Task<List<Transaction>> GetMiniStatementAsync(string accountNo)
    {
      return await _accountDbContext.Transactions
        .AsNoTracking()
        .Where(t => t.AccountNo == accountNo)
        .OrderByDescending(t => t.CreatedAt)
        .Take(5)
        .ToListAsync();
    }

    public async Task<bool> CloseAccountAsync(string accountNo)
    {
      var account = await _accountDbContext.Accounts
        .FirstOrDefaultAsync(a => a.AccountNo == accountNo);

      if (account == null)
      {
        return false;
      }

      account.Status = "Closed";
      await _accountDbContext.SaveChangesAsync();

      await SendAuditLogAsync(
        "CloseAccount",
        account.AccountNo,
        "Account status changed to Closed");

      return true;
    }

    public async Task<object?> GetInternalBalanceAsync(string accountNo)
    {
      return await GetBalanceAsync(accountNo);
    }

    public async Task<bool> InternalDebitAsync(WithdrawDto withdrawDto)
    {
      return await WithdrawAsync(withdrawDto);
    }

    public async Task<bool> InternalCreditAsync(DepositDto depositDto)
    {
      return await DepositAsync(depositDto);
    }

    private async Task<string> GenerateUniqueAccountNoAsync()
    {
      var random = new Random();
      string accountNo;

      do
      {
        accountNo = string.Concat(Enumerable.Range(0, 12).Select(_ => random.Next(0, 10).ToString()));
      }
      while (await _accountDbContext.Accounts.AnyAsync(a => a.AccountNo == accountNo));

      return accountNo;
    }

    private async Task SendAuditLogAsync(string action, string accountNo, string description)
    {
      try
      {
        var httpClient = _httpClientFactory.CreateClient("AuditService");

        var payload = new
        {
          Service = "AccountService",
          Action = action,
          AccountNo = accountNo,
          Description = description,
          CreatedAt = DateTime.UtcNow
        };

        await httpClient.PostAsJsonAsync("/api/audit/log", payload);
      }
      catch (Exception exception)
      {
        Log.Warning(exception, "AuditService is unavailable. Action: {Action}, AccountNo: {AccountNo}", action, accountNo);
      }
    }
  }
}
