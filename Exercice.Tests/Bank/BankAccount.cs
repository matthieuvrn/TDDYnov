namespace Bank;

public class BankAccount : IBankAccount
{
    private decimal Balance { get; set; }
    public string AccountNumber { get; set; }
    public List<string> TransactionHistory { get; private set; }

    public decimal GetBalance() => Balance;

    public BankAccount(string accountNumber, decimal initialBalance = decimal.Zero)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number cannot be null or empty");

        if (initialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative");

        AccountNumber = accountNumber;
        Balance = initialBalance;
        TransactionHistory = new List<string>();

        if (initialBalance > 0)
        {
            TransactionHistory.Add($"Dépôt initial: +{initialBalance:C}");
        }
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be positive");

        Balance += amount;
        TransactionHistory.Add($"Dépôt: +{amount:C}");
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive");

        if (amount > Balance)
            throw new InvalidOperationException("Insufficient funds for withdrawal");

        Balance -= amount;
        TransactionHistory.Add($"Retrait: -{amount:C}");
    }

    public void Transfer(BankAccount destinationAccount, decimal amount)
    {
        ArgumentNullException.ThrowIfNull(destinationAccount);

        if (destinationAccount == this)
            throw new ArgumentException("Cannot transfer to the same account");

        if (amount <= 0)
            throw new ArgumentException("Transfer amount must be positive");

        if (amount > Balance)
            throw new InvalidOperationException("Insufficient funds for transfer");

        Balance -= amount;
        destinationAccount.Balance += amount;
        TransactionHistory.Add($"Transfert vers {destinationAccount.AccountNumber}: -{amount:C}");
        destinationAccount.TransactionHistory.Add($"Transfert reçu de {AccountNumber}: +{amount:C}");
    }
}