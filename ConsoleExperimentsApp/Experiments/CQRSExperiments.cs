using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleExperimentsApp.Experiments
{
    public static class CQRSExperiments
    {
        public static async Task Run()
        {
            Console.WriteLine("=== CQRS and Event Sourcing Experiments ===\n");

            var eventStore = new EventStore();
            var commandHandler = new AccountCommandHandler(eventStore);
            var queryHandler = new AccountQueryHandler(eventStore);

            var accountId = Guid.NewGuid();

            // Execute Commands (Write Side)
            Console.WriteLine("--- Executing Commands ---");
            await commandHandler.Handle(new CreateAccountCommand(accountId, "John Doe", 1000m));
            await commandHandler.Handle(new DepositMoneyCommand(accountId, 500m));
            await commandHandler.Handle(new WithdrawMoneyCommand(accountId, 200m));
            await commandHandler.Handle(new DepositMoneyCommand(accountId, 300m));

            // Execute Queries (Read Side)
            Console.WriteLine("\n--- Executing Queries ---");
            var accountDetails = await queryHandler.Handle(new GetAccountDetailsQuery(accountId));
            Console.WriteLine($"Account Holder: {accountDetails.AccountHolder}");
            Console.WriteLine($"Current Balance: ${accountDetails.Balance}");

            var transactions = await queryHandler.Handle(new GetTransactionHistoryQuery(accountId));
            Console.WriteLine($"\nTransaction History ({transactions.Count} transactions):");
            foreach (var transaction in transactions)
            {
                Console.WriteLine($"  - {transaction.Type}: ${transaction.Amount} on {transaction.Timestamp:yyyy-MM-dd HH:mm:ss}");
            }

            // Demonstrate Event Sourcing - Replay Events
            Console.WriteLine("\n--- Event Sourcing: Replaying Events ---");
            var events = eventStore.GetEvents(accountId);
            Console.WriteLine($"Total Events Stored: {events.Count}");

            var replayedBalance = 0m;
            foreach (var @event in events)
            {
                Console.WriteLine($"  Event: {@event.GetType().Name}");
                if (@event is AccountCreatedEvent created)
                    replayedBalance = created.InitialBalance;
                else if (@event is MoneyDepositedEvent deposited)
                    replayedBalance += deposited.Amount;
                else if (@event is MoneyWithdrawnEvent withdrawn)
                    replayedBalance -= withdrawn.Amount;
            }
            Console.WriteLine($"Replayed Balance: ${replayedBalance}");

            Console.WriteLine("\n=== CQRS experiment completed ===");
        }
    }

    #region Commands (Write Side)

    public record CreateAccountCommand(Guid AccountId, string AccountHolder, decimal InitialBalance);
    public record DepositMoneyCommand(Guid AccountId, decimal Amount);
    public record WithdrawMoneyCommand(Guid AccountId, decimal Amount);

    #endregion

    #region Queries (Read Side)

    public record GetAccountDetailsQuery(Guid AccountId);
    public record GetTransactionHistoryQuery(Guid AccountId);

    #endregion

    #region Events (Event Sourcing)

    public abstract record DomainEvent(Guid AggregateId, DateTime Timestamp);

    public record AccountCreatedEvent(Guid AggregateId, string AccountHolder, decimal InitialBalance, DateTime Timestamp)
        : DomainEvent(AggregateId, Timestamp);

    public record MoneyDepositedEvent(Guid AggregateId, decimal Amount, DateTime Timestamp)
        : DomainEvent(AggregateId, Timestamp);

    public record MoneyWithdrawnEvent(Guid AggregateId, decimal Amount, DateTime Timestamp)
        : DomainEvent(AggregateId, Timestamp);

    #endregion

    #region Command Handler

    public class AccountCommandHandler
    {
        private readonly EventStore _eventStore;

        public AccountCommandHandler(EventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(CreateAccountCommand command)
        {
            var @event = new AccountCreatedEvent(
                command.AccountId,
                command.AccountHolder,
                command.InitialBalance,
                DateTime.UtcNow);

            _eventStore.SaveEvent(command.AccountId, @event);
            Console.WriteLine($"Account created for {command.AccountHolder} with initial balance ${command.InitialBalance}");
            await Task.CompletedTask;
        }

        public async Task Handle(DepositMoneyCommand command)
        {
            var account = ReconstructAccountFromEvents(command.AccountId);

            var @event = new MoneyDepositedEvent(
                command.AccountId,
                command.Amount,
                DateTime.UtcNow);

            _eventStore.SaveEvent(command.AccountId, @event);
            Console.WriteLine($"Deposited ${command.Amount} to account {command.AccountId}");
            await Task.CompletedTask;
        }

        public async Task Handle(WithdrawMoneyCommand command)
        {
            var account = ReconstructAccountFromEvents(command.AccountId);

            if (account.Balance < command.Amount)
            {
                Console.WriteLine($"Insufficient funds! Cannot withdraw ${command.Amount}");
                return;
            }

            var @event = new MoneyWithdrawnEvent(
                command.AccountId,
                command.Amount,
                DateTime.UtcNow);

            _eventStore.SaveEvent(command.AccountId, @event);
            Console.WriteLine($"Withdrew ${command.Amount} from account {command.AccountId}");
            await Task.CompletedTask;
        }

        private Account ReconstructAccountFromEvents(Guid accountId)
        {
            var events = _eventStore.GetEvents(accountId);
            var account = new Account();

            foreach (var @event in events)
            {
                account.Apply(@event);
            }

            return account;
        }
    }

    #endregion

    #region Query Handler and Read Models

    public class AccountQueryHandler
    {
        private readonly EventStore _eventStore;

        public AccountQueryHandler(EventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<AccountDetailsReadModel> Handle(GetAccountDetailsQuery query)
        {
            var events = _eventStore.GetEvents(query.AccountId);
            var readModel = new AccountDetailsReadModel
            {
                AccountId = query.AccountId
            };

            foreach (var @event in events)
            {
                if (@event is AccountCreatedEvent created)
                {
                    readModel.AccountHolder = created.AccountHolder;
                    readModel.Balance = created.InitialBalance;
                }
                else if (@event is MoneyDepositedEvent deposited)
                {
                    readModel.Balance += deposited.Amount;
                }
                else if (@event is MoneyWithdrawnEvent withdrawn)
                {
                    readModel.Balance -= withdrawn.Amount;
                }
            }

            return await Task.FromResult(readModel);
        }

        public async Task<List<TransactionReadModel>> Handle(GetTransactionHistoryQuery query)
        {
            var events = _eventStore.GetEvents(query.AccountId);
            var transactions = new List<TransactionReadModel>();

            foreach (var @event in events)
            {
                if (@event is MoneyDepositedEvent deposited)
                {
                    transactions.Add(new TransactionReadModel
                    {
                        Type = "Deposit",
                        Amount = deposited.Amount,
                        Timestamp = deposited.Timestamp
                    });
                }
                else if (@event is MoneyWithdrawnEvent withdrawn)
                {
                    transactions.Add(new TransactionReadModel
                    {
                        Type = "Withdrawal",
                        Amount = withdrawn.Amount,
                        Timestamp = withdrawn.Timestamp
                    });
                }
            }

            return await Task.FromResult(transactions);
        }
    }

    public class AccountDetailsReadModel
    {
        public Guid AccountId { get; set; }
        public string AccountHolder { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }

    public class TransactionReadModel
    {
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }

    #endregion

    #region Domain Model (Aggregate)

    public class Account
    {
        public Guid Id { get; private set; }
        public string AccountHolder { get; private set; } = string.Empty;
        public decimal Balance { get; private set; }

        public void Apply(DomainEvent @event)
        {
            switch (@event)
            {
                case AccountCreatedEvent created:
                    Id = created.AggregateId;
                    AccountHolder = created.AccountHolder;
                    Balance = created.InitialBalance;
                    break;
                case MoneyDepositedEvent deposited:
                    Balance += deposited.Amount;
                    break;
                case MoneyWithdrawnEvent withdrawn:
                    Balance -= withdrawn.Amount;
                    break;
            }
        }
    }

    #endregion

    #region Event Store

    public class EventStore
    {
        private readonly Dictionary<Guid, List<DomainEvent>> _events = new();

        public void SaveEvent(Guid aggregateId, DomainEvent @event)
        {
            if (!_events.ContainsKey(aggregateId))
            {
                _events[aggregateId] = new List<DomainEvent>();
            }

            _events[aggregateId].Add(@event);
        }

        public List<DomainEvent> GetEvents(Guid aggregateId)
        {
            return _events.ContainsKey(aggregateId) 
                ? _events[aggregateId] 
                : new List<DomainEvent>();
        }
    }

    #endregion
}
