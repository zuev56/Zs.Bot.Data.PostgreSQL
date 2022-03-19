using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;
using System.Data.Common;
using System.Reflection;
using System.Text;
using Zs.Bot.Data.Models;
using Zs.Common.Extensions;

namespace Zs.Bot.Data.PostgreSQL
{
    public sealed class PostgreSqlBotContext : BotContext<PostgreSqlBotContext>
    {
        public PostgreSqlBotContext()
            : base()
        {
        }

        public PostgreSqlBotContext(DbContextOptions<PostgreSqlBotContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();

            ConfigureEntities(modelBuilder);
            SeedData(modelBuilder);

            // TODO: ADD SCRIPTS FROM /SQL HERE
        }

        public static void ConfigureEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>(b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int")
                    .HasColumnName("id")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                b.Property<string>("ChatTypeId")
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnType("character varying(10)")
                    .HasColumnName("chat_type_id");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)")
                    .HasColumnName("name");

                b.Property<string>("Description")
                    .HasMaxLength(100)
                    .HasColumnType("character varying(100)")
                    .HasColumnName("description");

                b.Property<string>("RawData")
                    .IsRequired()
                    .HasColumnType("json")
                    .HasColumnName("raw_data");

                b.Property<string>("RawDataHash")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)")
                    .HasColumnName("raw_data_hash");

                b.Property<string>("RawDataHistory")
                    .HasColumnType("json")
                    .HasColumnName("raw_data_history");

                b.Property<DateTime>("InsertDate")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("insert_date")
                    .HasDefaultValueSql("now()");

                b.Property<DateTime>("UpdateDate")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("update_date")
                    .HasDefaultValueSql("now()");

                b.HasKey("Id");

                b.HasIndex("ChatTypeId");

                b.ToTable("chats", "bot");
            });

            modelBuilder.Entity<ChatType>(b =>
            {
                b.Property<string>("Id")
                    .HasMaxLength(10)
                    .HasColumnType("character varying(10)")
                    .HasColumnName("id");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnType("character varying(10)")
                    .HasColumnName("name");

                b.HasKey("Id");

                b.ToTable("chat_types", "bot");
            });

            modelBuilder.Entity<Command>(b =>
            {
                b.Property<string>("Id")
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)")
                    .HasColumnName("id");

                b.Property<string>("Group")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)")
                    .HasColumnName("group");

                b.Property<string>("Description")
                    .HasMaxLength(100)
                    .HasColumnType("character varying(100)")
                    .HasColumnName("description");

                b.Property<string>("DefaultArgs")
                    .HasMaxLength(100)
                    .HasColumnType("character varying(100)")
                    .HasColumnName("default_args");

                b.Property<string>("Script")
                    .IsRequired()
                    .HasMaxLength(5000)
                    .HasColumnType("character varying(5000)")
                    .HasColumnName("script");

                b.HasKey("Id");

                b.ToTable("commands", "bot");
            });

            modelBuilder.Entity<Message>(b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int")
                    .HasColumnName("id")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                b.Property<int>("ChatId")
                    .HasColumnType("integer")
                    .HasColumnName("chat_id");

                b.Property<int>("UserId")
                    .HasColumnType("integer")
                    .HasColumnName("user_id");

                b.Property<string>("MessageTypeId")
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnType("character varying(3)")
                    .HasColumnName("message_type_id");

                b.Property<string>("MessengerId")
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnType("character varying(2)")
                    .HasColumnName("messenger_id");

                b.Property<int?>("ReplyToMessageId")
                    .HasColumnType("integer")
                    .HasColumnName("reply_to_message_id");

                b.Property<string>("Text")
                    .HasMaxLength(100)
                    .HasColumnType("character varying(100)")
                    .HasColumnName("text");

                b.Property<bool>("IsSucceed")
                    .HasColumnType("bool")
                    .HasColumnName("is_succeed");

                b.Property<bool>("IsDeleted")
                    .HasColumnType("bool")
                    .HasColumnName("is_deleted");

                b.Property<string>("RawData")
                    .IsRequired()
                    .HasColumnType("json")
                    .HasColumnName("raw_data");

                b.Property<string>("RawDataHash")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)")
                    .HasColumnName("raw_data_hash");

                b.Property<string>("RawDataHistory")
                    .HasColumnType("json")
                    .HasColumnName("raw_data_history");

                b.Property<string>("FailDescription")
                    .HasColumnType("json")
                    .HasColumnName("fail_description");

                b.Property<int>("FailsCount")
                    .HasColumnType("integer")
                    .HasColumnName("fails_count");

                b.Property<DateTime>("InsertDate")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("insert_date")
                    .HasDefaultValueSql("now()");

                b.Property<DateTime>("UpdateDate")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("update_date")
                    .HasDefaultValueSql("now()");

                b.HasKey("Id");

                b.HasIndex("ChatId");

                b.HasIndex("MessageTypeId");

                b.HasIndex("MessengerId");

                b.HasIndex("ReplyToMessageId");

                b.HasIndex("UserId");

                b.ToTable("messages", "bot");
            });

            modelBuilder.Entity<MessageType>(b =>
            {
                b.Property<string>("Id")
                    .HasMaxLength(3)
                    .HasColumnType("character varying(3)")
                    .HasColumnName("id");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)")
                    .HasColumnName("name");

                b.HasKey("Id");

                b.ToTable("message_types", "bot");
            });

            modelBuilder.Entity<MessengerInfo>(b =>
            {
                b.Property<string>("Id")
                    .HasMaxLength(2)
                    .HasColumnType("character varying(2)")
                    .HasColumnName("id");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnType("character varying(20)")
                    .HasColumnName("name");

                b.HasKey("Id");

                b.ToTable("messengers", "bot");
            });

            modelBuilder.Entity<User>(b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int")
                    .HasColumnName("id")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                b.Property<string>("UserRoleId")
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnType("character varying(10)")
                    .HasColumnName("user_role_id");

                b.Property<string>("Name")
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)")
                    .HasColumnName("name");

                b.Property<string>("FullName")
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)")
                    .HasColumnName("full_name");

                b.Property<bool>("IsBot")
                    .HasColumnType("bool")
                    .HasColumnName("is_bot");

                b.Property<string>("RawData")
                    .IsRequired()
                    .HasColumnType("json")
                    .HasColumnName("raw_data");

                b.Property<string>("RawDataHash")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)")
                    .HasColumnName("raw_data_hash");

                b.Property<string>("RawDataHistory")
                    .HasColumnType("json")
                    .HasColumnName("raw_data_history");

                b.Property<DateTime>("InsertDate")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("insert_date")
                    .HasDefaultValueSql("now()");

                b.Property<DateTime>("UpdateDate")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("update_date")
                    .HasDefaultValueSql("now()");

                b.HasKey("Id");

                b.HasIndex("UserRoleId");

                b.ToTable("users", "bot");
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.Property<string>("Id")
                    .HasMaxLength(10)
                    .HasColumnType("character varying(10)")
                    .HasColumnName("id");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)")
                    .HasColumnName("name");

                b.Property<string>("Permissions")
                    .IsRequired()
                    .HasColumnType("json")
                    .HasColumnName("permissions");

                b.HasKey("Id");

                b.ToTable("user_roles", "bot");
            });
        }

        public static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessengerInfo>().HasData(new[]
            {
                new MessengerInfo() { Id = "TG", Name = "Telegram" },
                new MessengerInfo() { Id = "VK", Name = "Вконтакте" },
                new MessengerInfo() { Id = "SK", Name = "Skype" },
                new MessengerInfo() { Id = "FB", Name = "Facebook" },
                new MessengerInfo() { Id = "DC", Name = "Discord" }
            });

            modelBuilder.Entity<ChatType>().HasData(new[]
            {
                new ChatType() { Id = "CHANNEL", Name = "Channel" },
                new ChatType() { Id = "GROUP", Name = "Group" },
                new ChatType() { Id = "PRIVATE", Name = "Private" },
                new ChatType() { Id = "UNDEFINED", Name = "Undefined" }
            });

            modelBuilder.Entity<Chat>().HasData(new[]
            {
                new Chat() { Id = -1, Name = "IntegrationTestChat", Description = "IntegrationTestChat", ChatTypeId = "PRIVATE", RawData = "{ \"test\": \"test\" }", RawDataHash = "-1063294487", InsertDate = DateTime.UtcNow },
                new Chat() { Id = 1, Name = "zuev56", ChatTypeId = "PRIVATE", RawData = "{ \"Id\": 210281448 }", RawDataHash = "-1063294487", InsertDate = DateTime.UtcNow }
            });

            modelBuilder.Entity<UserRole>().HasData(new[]
            {
                new UserRole() { Id = "OWNER", Name = "Owner", Permissions = "[ \"All\" ]" },
                new UserRole() { Id = "ADMIN", Name = "Administrator", Permissions = "[ \"adminCmdGroup\", \"moderatorCmdGroup\", \"userCmdGroup\" ]" },
                new UserRole() { Id = "MODERATOR", Name = "Moderator", Permissions = "[ \"moderatorCmdGroup\", \"userCmdGroup\" ]" },
                new UserRole() { Id = "USER", Name = "User", Permissions = "[ \"userCmdGroup\" ]" }
            });

            modelBuilder.Entity<User>().HasData(new[]
            {
                new User() { Id = -10, Name = "Unknown", FullName = "for exported message reading", UserRoleId = "USER", IsBot = false, RawData = "{ \"test\": \"test\" }", RawDataHash = "-1063294487", InsertDate = DateTime.UtcNow },
                new User() { Id = -1, Name = "IntegrationTestUser", FullName = "IntegrationTest", UserRoleId = "USER", IsBot = false, RawData = "{ \"test\": \"test\" }", RawDataHash = "-1063294487", InsertDate = DateTime.UtcNow },
                new User() { Id = 1, Name = "zuev56", FullName = "Сергей Зуев", UserRoleId = "OWNER", IsBot = false, RawData = "{ \"Id\": 210281448 }", RawDataHash = "-1063294487", InsertDate = DateTime.UtcNow }
            });

            modelBuilder.Entity<MessageType>().HasData(new[]
            {
                new MessageType() { Id = "UKN", Name = "Unknown" },
                new MessageType() { Id = "TXT", Name = "Text" },
                new MessageType() { Id = "PHT", Name = "Photo" },
                new MessageType() { Id = "AUD", Name = "Audio" },
                new MessageType() { Id = "VID", Name = "Video" },
                new MessageType() { Id = "VOI", Name = "Voice" },
                new MessageType() { Id = "DOC", Name = "Document" },
                new MessageType() { Id = "STK", Name = "Sticker" },
                new MessageType() { Id = "LOC", Name = "Location" },
                new MessageType() { Id = "CNT", Name = "Contact" },
                new MessageType() { Id = "SRV", Name = "Service message" },
                new MessageType() { Id = "OTH", Name = "Other" }
            });

            modelBuilder.Entity<Command>().HasData(new[]
            {
                new Command() { Id = "/Test".ToLowerInvariant(), Script = "SELECT 'Test'", Description = "Тестовый запрос к боту. Возвращает ''Test''", Group = "moderatorCmdGroup" },
                new Command() { Id = "/NullTest".ToLowerInvariant(), Script = "SELECT null", Description = "Тестовый запрос к боту. Возвращает NULL", Group = "moderatorCmdGroup" },
                new Command() { Id = "/Help".ToLowerInvariant(), Script = "SELECT bot.sf_cmd_get_help({0})", DefaultArgs = "<UserRoleId>", Description = "Получение справки по доступным функциям", Group = "userCmdGroup" },
                new Command() { Id = "/SqlQuery".ToLowerInvariant(), Script = "select (with userQuery as ({0}) select json_agg(q) from userQuery q)", DefaultArgs = "select 'Pass your query as a parameter in double quotes'", Description = "SQL-запрос", Group = "adminCmdGroup" }
            });
        }

        public static string GetOtherSqlScripts(string configPath)
        {
            var configuration = new ConfigurationBuilder()
                   .AddJsonFile(System.IO.Path.GetFullPath(configPath))
                   .Build();

            var connectionStringBuilder = new DbConnectionStringBuilder()
            {
                ConnectionString = configuration.GetSecretValue("ConnectionStrings:Default")
            };
            var dbName = connectionStringBuilder["Database"] as string;

            var resources = new[]
            {
                "Priveleges.sql",
                "StoredFunctions.sql",
                "SequencesUpdate.sql",
                "Triggers.sql"
            };

            var sb = new StringBuilder();
            foreach (var resourceName in resources)
            {
                var sqlScript = Assembly.GetExecutingAssembly().ReadResource(resourceName);
                sb.Append(sqlScript + Environment.NewLine);
            }

            if (!string.IsNullOrWhiteSpace(dbName))
                sb.Replace("DefaultDbName", dbName);

            return sb.ToString();
        }
    }
}
