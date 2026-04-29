using System.Collections.Generic;
using System.Threading.Tasks;
using AccountService.DTOs;
using AccountService.Models;

namespace AccountService.Services
{
  public interface IAccountService
  {
    Task<Account> OpenAccountAsync(OpenAccountDto openAccountDto);
    Task<bool> DepositAsync(DepositDto depositDto);
    Task<bool> WithdrawAsync(WithdrawDto withdrawDto);
    Task<object?> GetBalanceAsync(string accountNo);
    Task<List<Transaction>> GetMiniStatementAsync(string accountNo);
    Task<bool> CloseAccountAsync(string accountNo);

    Task<object?> GetInternalBalanceAsync(string accountNo);
    Task<bool> InternalDebitAsync(WithdrawDto withdrawDto);
    Task<bool> InternalCreditAsync(DepositDto depositDto);
  }
}
