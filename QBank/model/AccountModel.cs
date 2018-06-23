using System;
using MySql.Data.MySqlClient;
using QBank.model;
using QBank.entity;
using QBank.error;
using QBank.utility;
namespace QBank.model
{
    public class AccountModel
    {
        public Boolean Save(Account account)
        {
            Db.Instance().OpenConnection(); 
            var salt = Hash.RandomString(7);
            account.Salt = salt;
            account.Password = Hash.GenerateSaltedSHA1(account.Password, account.Salt);
            var sqlQuery = "insert into `accounts` " +
                           "(`username`, `password`, `accountNumber`, `identityCard`, `balance`, `phone`, `email`, `fullName`, `salt`, `status`) values" +
                           "(@username, @password, @accountNumber, @identityCard, @balance, @phone, @email, @fullName, @salt, @status)";
            var cmd = new MySqlCommand(sqlQuery, Db.Instance().Connection);
            cmd.Parameters.AddWithValue("@username", account.Username);
            cmd.Parameters.AddWithValue("@password", account.Password);
            cmd.Parameters.AddWithValue("@accountNumber", account.AccountNumber);
            cmd.Parameters.AddWithValue("@identityCard", account.IdentityCard);
            cmd.Parameters.AddWithValue("@balance", account.Balance);
            cmd.Parameters.AddWithValue("@phone", account.Phone);
            cmd.Parameters.AddWithValue("@email", account.Email);
            cmd.Parameters.AddWithValue("@fullName", account.FullName);
            cmd.Parameters.AddWithValue("@salt", account.Salt);
            cmd.Parameters.AddWithValue("@status", account.Status);
            var result = cmd.ExecuteNonQuery();
            Db.Instance().CloseConnection();
            return result == 1;
        }
        
        public bool UpdateBalance(Account account, Transaction historyTransaction)
        {
            Db.Instance().OpenConnection();
            var transaction = Db.Instance().Connection.BeginTransaction();

            try
            {
                var queryBalance = "select balance from `accounts` where username = @username and status = @status";
                MySqlCommand queryBalanceCommand = new MySqlCommand(queryBalance, Db.Instance().Connection);
                queryBalanceCommand.Parameters.AddWithValue("@username", account.Username);
                queryBalanceCommand.Parameters.AddWithValue("@status", account.Status);
                var balanceReader = queryBalanceCommand.ExecuteReader();
                if (!balanceReader.Read())
                {
                    throw new TransactionException("Invalid username");
                }

                var currentBalance = balanceReader.GetDecimal("balance");
                balanceReader.Close();

                if (historyTransaction.Type != Transaction.TransactionType.DEPOSIT
                    && historyTransaction.Type != Transaction.TransactionType.WITHDRAW)
                {
                    throw new TransactionException("Invalid transaction type!");
                }

                if ((historyTransaction.Type == Transaction.TransactionType.WITHDRAW || historyTransaction.Type == Transaction.TransactionType.TRANSFER) &&
                    historyTransaction.Amount > currentBalance)
                {
                    throw new TransactionException("Not enough money!");
                }

                if (historyTransaction.Type == Transaction.TransactionType.DEPOSIT)
                {
                    currentBalance += historyTransaction.Amount;
                }
                else
                {
                    currentBalance -= historyTransaction.Amount;
                }

                var updateAccountResult = 0;
                var queryUpdateAccountBalance =
                    "update `accounts` set balance = @balance where username = @username and status = 1";
                var cmdUpdateAccountBalance =
                    new MySqlCommand(queryUpdateAccountBalance, Db.Instance().Connection);
                cmdUpdateAccountBalance.Parameters.AddWithValue("@username", account.Username);
                cmdUpdateAccountBalance.Parameters.AddWithValue("@balance", currentBalance);
                updateAccountResult = cmdUpdateAccountBalance.ExecuteNonQuery();

                var insertTransactionResult = 0;
                var queryInsertTransaction = "insert into `transactions` " +
                                             "(id, type, amount, content, senderAccountNumber, receiverAccountNumber, status) " +
                                             "values (@id, @type, @amount, @content, @senderAccountNumber, @receiverAccountNumber, @status)";
                var cmdInsertTransaction =
                    new MySqlCommand(queryInsertTransaction, Db.Instance().Connection);
                cmdInsertTransaction.Parameters.AddWithValue("@id", historyTransaction.Id);
                cmdInsertTransaction.Parameters.AddWithValue("@type", historyTransaction.Type);
                cmdInsertTransaction.Parameters.AddWithValue("@amount", historyTransaction.Amount);
                cmdInsertTransaction.Parameters.AddWithValue("@content", historyTransaction.Content);
                cmdInsertTransaction.Parameters.AddWithValue("@senderAccountNumber",
                    historyTransaction.SenderAccountNumber);
                cmdInsertTransaction.Parameters.AddWithValue("@receiverAccountNumber",
                    historyTransaction.ReceiverAccountNumber);
                cmdInsertTransaction.Parameters.AddWithValue("@status", historyTransaction.Status);
                insertTransactionResult = cmdInsertTransaction.ExecuteNonQuery();

                if (updateAccountResult == 1 && insertTransactionResult == 1)
                {
                    transaction.Commit();
                    return true;
                }
            }
            catch (TransactionException e)
            {
                transaction.Rollback();
                return false;
            }

            Db.Instance().CloseConnection();
            return false;
        }

        public Boolean CheckExistUserName(string username)
        {
            return false;
        }
        
        public Account GetAccountByUserName(string username)
        {
            Db.Instance().OpenConnection();
            var queryString = "select * from  `accounts` where username = @username and status = 1";
            var cmd = new MySqlCommand(queryString, Db.Instance().Connection);
            cmd.Parameters.AddWithValue("@username", username);
            var reader = cmd.ExecuteReader();
            Account account = null;
            if (reader.Read())
            {
                var _username = reader.GetString("username");
                var password = reader.GetString("password");
                var salt = reader.GetString("salt");
                var accountNumber = reader.GetString("accountNumber");
                var identityCard = reader.GetString("identityCard");
                var balance = reader.GetDecimal("balance");
                var phone = reader.GetString("phone");
                var email = reader.GetString("email");
                var fullName = reader.GetString("fullName");
                var createdAt = reader.GetString("createdAt");
                var updatedAt = reader.GetString("updatedAt");
                var status = reader.GetInt32("status");
                account = new Account(_username, password, salt, accountNumber, identityCard, balance, phone, email,
                    fullName, createdAt, updatedAt, (Account.ActiveStatus) status);
            }

            Db.Instance().CloseConnection();
            return account;
        }

        public void GetAccountByNumber(String acccountNumber)
        {
            Db.Instance().OpenConnection();
            var queryString = "select * from 'accounts' where accountNumber = @accountNumber and status = 1";
        }

   //     public Transaction GetTransaction(String acccountNumber)
    //    {
    //        Db.Instance().OpenConnection();
    //        var queryString = "select * from  `transactions` where senderAccountNumber = @acccountNumber or receiverAccountNumber = @acccountNumber";
    //        var cmd = new MySqlCommand(queryString, Db.Instance().Connection);
    //        cmd.Parameters.AddWithValue("@acccountNumber", acccountNumber);
    //        var reader = cmd.ExecuteReader();
    //        Transaction transaction = null;
    //        if (reader.Read())
    //        {
    //            var id = reader.GetString("id");
    //            var type = reader.GetString("type");
    //            var amount = reader.GetString("amount");
    //            var content = reader.GetString("content");
    //            var senderAccountNumber = reader.GetString("senderAccountNumber");
    //            var receiverAccountNumber = reader.GetString("receiverAccountNumber");
    //            var status = reader.GetString("status");
    //            transaction = new Transaction(id, type, amount, content, senderAccountNumber, receiverAccountNumber, status);
    //        }
    //        Db.Instance().CloseConnection();
    //        return transaction;
    //   }

    }
}