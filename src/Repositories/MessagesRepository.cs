using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zs.Bot.Data.Models;
using Zs.Bot.Data.Repositories;

namespace Zs.Bot.Data.PostgreSQL.Repositories
{
#warning Depends on Telegram
    public sealed class MessagesRepository<TContext> : MessagesRepositoryBase<TContext>
        where TContext : DbContext
    {
        public MessagesRepository(
            IDbContextFactory<TContext> contextFactory,
            TimeSpan? criticalQueryExecutionTimeForLogging = null,
            ILogger<MessagesRepository<TContext>> logger = null)
            : base(contextFactory, criticalQueryExecutionTimeForLogging, logger)
        {
        }

        public override async Task<Message> FindByRawDataIdsAsync(int rawMessageId, long rawChatId)
        {
            return await FindBySqlAsync(
                  $"select * from bot.messages "
                + $"where cast(raw_data ->> 'MessageId' as integer) = {rawMessageId}"
                + $"  and cast(raw_data -> 'Chat' ->> 'Id' as bigint) = {rawChatId}").ConfigureAwait(false);
        }

        public override async Task<List<Message>> FindDailyMessages(int chatId)
        {
            return await FindAllBySqlAsync(
                  $"select * from bot.messages "
                + $"where chat_id = {chatId} and cast(raw_data ->> 'Date' as timestamptz) > now()::date").ConfigureAwait(false);
        }

        public override async Task<Dictionary<int, int>> FindUserIdsAndMessagesCountSinceDate(int chatId, DateTime? startDate)
        {
            var userIdsAndMessageCounts = new Dictionary<int, int>();

            if (startDate != null)
            {
                var selectChatMessagesSinceDate =
                      $"select * from bot.messages "
                    + $"where chat_id = {chatId} "
                    + $"  and cast(raw_data ->> 'Date' as timestamptz) > now()::date " // ???
                    + $"  and cast(raw_data ->> 'Date' as timestamptz) > '{startDate}'";

                var messagesSinceDate = await FindAllBySqlAsync(selectChatMessagesSinceDate).ConfigureAwait(false);

                foreach (var messageGroup in messagesSinceDate.GroupBy(m => m.UserId))
                {
                    userIdsAndMessageCounts.Add(messageGroup.Key, messageGroup.Count());
                }     
            }

            return userIdsAndMessageCounts;
        }
    }
}
