using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Models;
using MicroRabbit.Banking.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicroRabbit.Banking.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class BankingController : ControllerBase
	{
		private readonly ILogger<BankingController> _logger;
		private readonly IAccountService _accountService;
		public BankingController(ILogger<BankingController> logger, IAccountService accountService)
		{
			_logger = logger;
			_accountService = accountService;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var result = await _accountService.GetAccountsAsync();
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] AccountTransfer accountTransfer)
		{
			await _accountService.TransferAsync(accountTransfer);
			return Ok(accountTransfer);
		}
	}
}
