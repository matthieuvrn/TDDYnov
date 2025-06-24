namespace Bank.UnitTest;

public class BankAccountTest
{
    #region Constructor Tests

    [Fact]
    public void Constructor_ValidAccountNumber_CreatesAccount()
    {
        // Arrange & Act
        var account = new BankAccount("123456");

        // Assert
        Assert.Equal("123456", account.AccountNumber);
        Assert.Equal(0, account.GetBalance());
        Assert.Empty(account.TransactionHistory);
    }

    [Fact]
    public void Constructor_ValidAccountNumberWithInitialBalance_CreatesAccountWithBalance()
    {
        // Arrange & Act
        var account = new BankAccount("123456", 100m);

        // Assert
        Assert.Equal("123456", account.AccountNumber);
        Assert.Equal(100m, account.GetBalance());
        Assert.Single(account.TransactionHistory);
        Assert.Contains("Dépôt initial: +100", account.TransactionHistory[0]);
    }

    [Fact]
    public void Constructor_NullAccountNumber_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new BankAccount(null));
        Assert.Contains("Account number cannot be null or empty", exception.Message);
    }

    [Fact]
    public void Constructor_EmptyAccountNumber_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new BankAccount(""));
        Assert.Contains("Account number cannot be null or empty", exception.Message);
    }

    [Fact]
    public void Constructor_WhitespaceAccountNumber_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new BankAccount("   "));
        Assert.Contains("Account number cannot be null or empty", exception.Message);
    }

    [Fact]
    public void Constructor_NegativeInitialBalance_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new BankAccount("123456", -50m));
        Assert.Contains("Initial balance cannot be negative", exception.Message);
    }

    [Fact]
    public void Constructor_ZeroInitialBalance_CreatesAccountSuccessfully()
    {
        // Arrange & Act
        var account = new BankAccount("123456", 0m);

        // Assert
        Assert.Equal(0m, account.GetBalance());
        Assert.Empty(account.TransactionHistory);
    }

    #endregion

    #region Deposit Tests

    [Fact]
    public void Deposit_PositiveAmount_IncreasesBalance()
    {
        // Arrange
        var account = new BankAccount("123456", 100m);

        // Act
        account.Deposit(50m);

        // Assert
        Assert.Equal(150m, account.GetBalance());
        Assert.Equal(2, account.TransactionHistory.Count);
        Assert.Contains("Dépôt: +50", account.TransactionHistory.Last());
    }

    [Fact]
    public void Deposit_MultipleDeposits_AccumulatesBalance()
    {
        // Arrange
        var account = new BankAccount("123456");

        // Act
        account.Deposit(50m);
        account.Deposit(30m);
        account.Deposit(20m);

        // Assert
        Assert.Equal(100m, account.GetBalance());
        Assert.Equal(3, account.TransactionHistory.Count);
    }

    [Fact]
    public void Deposit_NegativeAmount_ThrowsArgumentException()
    {
        // Arrange
        var account = new BankAccount("123456", 100m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => account.Deposit(-50m));
        Assert.Contains("Deposit amount must be positive", exception.Message);
        Assert.Equal(100m, account.GetBalance()); 
    }

    [Fact]
    public void Deposit_ZeroAmount_ThrowsArgumentException()
    {
        // Arrange
        var account = new BankAccount("123456", 100m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => account.Deposit(0m));
        Assert.Contains("Deposit amount must be positive", exception.Message);
        Assert.Equal(100m, account.GetBalance()); 
    }

    #endregion

    #region Withdraw Tests

    [Fact]
    public void Withdraw_ValidAmount_DecreasesBalance()
    {
        // Arrange
        var account = new BankAccount("123456", 100m);

        // Act
        account.Withdraw(30m);

        // Assert
        Assert.Equal(70m, account.GetBalance());
        Assert.Equal(2, account.TransactionHistory.Count);
        Assert.Contains("Retrait: -30", account.TransactionHistory.Last());
    }

    [Fact]
    public void Withdraw_ExactBalance_EmptiesAccount()
    {
        // Arrange
        var account = new BankAccount("123456", 50m);

        // Act
        account.Withdraw(50m);

        // Assert
        Assert.Equal(0m, account.GetBalance());
        Assert.Equal(2, account.TransactionHistory.Count);
    }

    [Fact]
    public void Withdraw_AmountExceedsBalance_ThrowsInvalidOperationException()
    {
        // Arrange
        var account = new BankAccount("123456", 50m);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => account.Withdraw(100m));
        Assert.Contains("Insufficient funds for withdrawal", exception.Message);
        Assert.Equal(50m, account.GetBalance()); 
    }

    [Fact]
    public void Withdraw_NegativeAmount_ThrowsArgumentException()
    {
        // Arrange
        var account = new BankAccount("123456", 100m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => account.Withdraw(-20m));
        Assert.Contains("Withdrawal amount must be positive", exception.Message);
        Assert.Equal(100m, account.GetBalance()); 
    }

    [Fact]
    public void Withdraw_ZeroAmount_ThrowsArgumentException()
    {
        // Arrange
        var account = new BankAccount("123456", 100m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => account.Withdraw(0m));
        Assert.Contains("Withdrawal amount must be positive", exception.Message);
        Assert.Equal(100m, account.GetBalance()); 
    }

    [Fact]
    public void Withdraw_FromEmptyAccount_ThrowsInvalidOperationException()
    {
        // Arrange
        var account = new BankAccount("123456", 0m);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => account.Withdraw(10m));
        Assert.Contains("Insufficient funds for withdrawal", exception.Message);
    }

    #endregion

    #region Transfer Tests

    [Fact]
    public void Transfer_ValidAmount_TransfersSuccessfully()
    {
        // Arrange
        var sourceAccount = new BankAccount("123456", 100m);
        var destinationAccount = new BankAccount("789012", 50m);

        // Act
        sourceAccount.Transfer(destinationAccount, 30m);

        // Assert
        Assert.Equal(70m, sourceAccount.GetBalance());
        Assert.Equal(80m, destinationAccount.GetBalance());
        Assert.Contains("Transfert vers 789012: -30", sourceAccount.TransactionHistory.Last());
        Assert.Contains("Transfert reçu de 123456: +30", destinationAccount.TransactionHistory.Last());
    }

    [Fact]
    public void Transfer_ExactBalance_TransfersAllFunds()
    {
        // Arrange
        var sourceAccount = new BankAccount("123456", 50m);
        var destinationAccount = new BankAccount("789012", 0m);

        // Act
        sourceAccount.Transfer(destinationAccount, 50m);

        // Assert
        Assert.Equal(0m, sourceAccount.GetBalance());
        Assert.Equal(50m, destinationAccount.GetBalance());
    }

    [Fact]
    public void Transfer_NullDestinationAccount_ThrowsArgumentNullException()
    {
        // Arrange
        var sourceAccount = new BankAccount("123456", 100m);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => sourceAccount.Transfer(null, 50m));
        Assert.Equal(100m, sourceAccount.GetBalance()); 
    }

    [Fact]
    public void Transfer_SameAccount_ThrowsArgumentException()
    {
        // Arrange
        var account = new BankAccount("123456", 100m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => account.Transfer(account, 50m));
        Assert.Contains("Cannot transfer to the same account", exception.Message);
        Assert.Equal(100m, account.GetBalance()); 
    }

    [Fact]
    public void Transfer_NegativeAmount_ThrowsArgumentException()
    {
        // Arrange
        var sourceAccount = new BankAccount("123456", 100m);
        var destinationAccount = new BankAccount("789012", 50m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => sourceAccount.Transfer(destinationAccount, -20m));
        Assert.Contains("Transfer amount must be positive", exception.Message);
        Assert.Equal(100m, sourceAccount.GetBalance()); 
        Assert.Equal(50m, destinationAccount.GetBalance());
    }

    [Fact]
    public void Transfer_ZeroAmount_ThrowsArgumentException()
    {
        // Arrange
        var sourceAccount = new BankAccount("123456", 100m);
        var destinationAccount = new BankAccount("789012", 50m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => sourceAccount.Transfer(destinationAccount, 0m));
        Assert.Contains("Transfer amount must be positive", exception.Message);
        Assert.Equal(100m, sourceAccount.GetBalance()); 
        Assert.Equal(50m, destinationAccount.GetBalance());
    }

    [Fact]
    public void Transfer_InsufficientFunds_ThrowsInvalidOperationException()
    {
        // Arrange
        var sourceAccount = new BankAccount("123456", 50m);
        var destinationAccount = new BankAccount("789012", 0m);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sourceAccount.Transfer(destinationAccount, 100m));
        Assert.Contains("Insufficient funds for transfer", exception.Message);
        Assert.Equal(50m, sourceAccount.GetBalance()); 
        Assert.Equal(0m, destinationAccount.GetBalance());
    }

    [Fact]
    public void Transfer_FromEmptyAccount_ThrowsInvalidOperationException()
    {
        // Arrange
        var sourceAccount = new BankAccount("123456", 0m);
        var destinationAccount = new BankAccount("789012", 50m);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sourceAccount.Transfer(destinationAccount, 10m));
        Assert.Contains("Insufficient funds for transfer", exception.Message);
    }

    #endregion

}