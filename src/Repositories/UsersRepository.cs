using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Zs.Bot.Data.Models;
using Zs.Bot.Data.Repositories;

namespace Zs.Bot.Data.PostgreSQL.Repositories
{
    public sealed class UsersRepository<TContext> : UsersRepositoryBase<TContext>
        where TContext : DbContext
    {
        public UsersRepository(
            IDbContextFactory<TContext> contextFactory,
            TimeSpan? criticalQueryExecutionTimeForLogging = null,
            ILogger<UsersRepository<TContext>> logger = null)
            : base(contextFactory, criticalQueryExecutionTimeForLogging, logger)
        {
        }

        public override async Task<User> FindByRawDataIdAsync(long rawId)
        {
            return await FindBySqlAsync($"select * from bot.users where cast(raw_data ->> 'Id' as bigint) = {rawId}").ConfigureAwait(false);
        }
    }
}
