using MicroRabbit.Transfer.Data.Context;
using MicroRabbit.Transfer.Domain.Interfaces;
using MicroRabbit.Transfer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRabbit.Transfer.Data.Repository
{
	public class TransferRepository : ITransferRepository
	{
		private readonly TransferDbContext _context;

		public TransferRepository(TransferDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(TransferLog transferLog)
		{
			await _context.TransferLogs.AddAsync(transferLog);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<TransferLog>> GetTransferLogsAsync()
		{
			return await _context.TransferLogs.ToListAsync();
		}
	}
}
