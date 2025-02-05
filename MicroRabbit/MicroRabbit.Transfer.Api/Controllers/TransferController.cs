using MicroRabbit.Transfer.Application.Interfaces;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;

namespace MicroRabbit.Transfer.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TransferController : ControllerBase
	{
		private readonly ILogger<TransferController> _logger;
		private readonly ITransferService _transferService;

		public TransferController(ITransferService transferService,  ILogger<TransferController> logger)
		{
			_transferService = transferService;
			_logger = logger;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var transferLogs = await _transferService.GetTransferLogsAsync();
			return Ok(transferLogs);
		}
	}
}
