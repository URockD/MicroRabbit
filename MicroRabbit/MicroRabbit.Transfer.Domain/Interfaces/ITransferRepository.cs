using MicroRabbit.Transfer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRabbit.Transfer.Domain.Interfaces
{
	public interface ITransferRepository
	{
		Task AddAsync(TransferLog transferLog);
		Task<IEnumerable<TransferLog>> GetTransferLogsAsync();
	}
}
