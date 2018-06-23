using System;
using System.Net.Mime;
using QBank.entity;
using QBank.model;
using QBank.utility;

namespace QBank.controller
{
    public class AccountController
    {
        private AccountModel model = new AccountModel();

        public void SignIn()
        {
            Console.WriteLine("Please enter your account information:");
            Console.WriteLine("Username: ");
            var username = Console.ReadLine();
            Console.WriteLine("Password: ");
            var password = Console.ReadLine();
            Console.WriteLine("Confirm Password: ");
            var cpassword = Console.ReadLine();
            Console.WriteLine("Identity Card: ");
            var identityCard = Console.ReadLine();
            Console.WriteLine("Full Name: ");
            var fullName = Console.ReadLine();
            Console.WriteLine("Email: ");
            var email = Console.ReadLine();
            Console.WriteLine("Phone: ");
            var phone = Console.ReadLine();
            var account = new Account(username, password, cpassword, identityCard, phone, email, fullName);
            var errors = account.CheckValid();
            if (errors.Count == 0)
            {
                model.Save(account);
                Console.WriteLine("Register success!");
                Console.ReadLine();
            }
            else
            {
                Console.Error.WriteLine("Please fix following errors and try again.");
                foreach (var messagErrorsValue in errors.Values)
                {
                    Console.Error.WriteLine(messagErrorsValue);
                }

                Console.ReadLine();
            }
        }

        public Boolean DoLogin()
        {
            Console.WriteLine("Please enter your account information to continue:");
            Console.WriteLine("Username: ");
            var username = Console.ReadLine();
            Console.WriteLine("Password: ");
            var password = Console.ReadLine();
            var account = new Account(username, password);
            var errors = account.ValidLoginInformation();
            if (errors.Count > 0)
            {
                Console.WriteLine("Invalid login information. Please fix errors below.");
                foreach (var messagErrorsValue in errors.Values)
                {
                    Console.Error.WriteLine(messagErrorsValue);
                }

                Console.ReadLine();
                return false;
            }

            account = model.GetAccountByUserName(username);
            if (account == null)
            {
                Console.WriteLine("Invalid login information. Please try again.");
                return false;
            }

            if (account.Password != Hash.GenerateSaltedSHA1(password, account.Salt))
            {
                Console.WriteLine("Invalid login information. Please try again.");
                return false;
            }

            Program.currentLoggedIn = account;
            return true;
        }

        public void Withdraw()
        {
            Console.WriteLine("Withdraw money.");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Please enter amount to withdraw: ");
            var amount = Utility.GetUnsignDecimalNumber();
            var historyTransaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                Type = Transaction.TransactionType.WITHDRAW,
                Amount = amount,
                Status = Transaction.ActiveStatus.DONE
            };
            if (model.UpdateBalance(Program.currentLoggedIn, historyTransaction))
            {
                Console.WriteLine("Transaction success!");
            }
            else
            {
                Console.WriteLine("Transaction fails, please try again!");
            }
            Program.currentLoggedIn = model.GetAccountByUserName(Program.currentLoggedIn.Username);
            Console.WriteLine("Current balance: " + Program.currentLoggedIn.Balance);
            Console.WriteLine("Press enter to continue!");
            Console.ReadLine();
            
        }

        public void Deposit()
        {
            Console.WriteLine("Deposit.");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Please enter amount to deposit: ");
            var amount = Utility.GetUnsignDecimalNumber();
            Console.WriteLine("Please enter message content: ");
            var content = Console.ReadLine();
            var historyTransaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                Type = Transaction.TransactionType.DEPOSIT,
                Amount = amount,
                Content = content,
                SenderAccountNumber = Program.currentLoggedIn.AccountNumber,
                ReceiverAccountNumber = Program.currentLoggedIn.AccountNumber,
                Status = Transaction.ActiveStatus.DONE
            };
            if (model.UpdateBalance(Program.currentLoggedIn, historyTransaction))
            {
                Console.WriteLine("Transaction success!");
            }
            else
            {
                Console.WriteLine("Transaction fails, please try again!");
            }
            Program.currentLoggedIn = model.GetAccountByUserName(Program.currentLoggedIn.Username);
            Console.WriteLine("Current balance: " + Program.currentLoggedIn.Balance);
            Console.WriteLine("Press enter to continue!");
            Console.ReadLine();
        }

        public void Transfer()
        {
            Console.WriteLine("Transfer.");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Please enter amount to transfer: ");
            var amount = Utility.GetUnsignDecimalNumber();
            Console.WriteLine("Please enter the receiver's account number you wishes to transfer ");
            Console.WriteLine("Please enter message content: ");
            var content = Console.ReadLine();
            var receiver = Console.ReadLine();
            var historyTransaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                Type = Transaction.TransactionType.TRANSFER,
                Amount = amount,
                Content = content,
                SenderAccountNumber = Program.currentLoggedIn.AccountNumber,
                ReceiverAccountNumber = receiver,
                Status = Transaction.ActiveStatus.DONE
            };
            if (model.UpdateBalance(Program.currentLoggedIn, historyTransaction))
            {
                Console.WriteLine("Transaction success!");
            }
            else
            {
                Console.WriteLine("Transaction fails, please try again!");
            }
            Program.currentLoggedIn = model.GetAccountByUserName(Program.currentLoggedIn.Username);
            Console.WriteLine("Current balance: " + Program.currentLoggedIn.Balance);
            Console.WriteLine("Press enter to continue!");
            Console.ReadLine();
        }

        public void CheckBalance()
        {
            Program.currentLoggedIn = model.GetAccountByUserName(Program.currentLoggedIn.Username);
            Console.WriteLine("Account Information");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Full name: " + Program.currentLoggedIn.FullName);
            Console.WriteLine("Account number: " + Program.currentLoggedIn.AccountNumber);
            Console.WriteLine("Balance: " + Program.currentLoggedIn.Balance);
            Console.WriteLine("Press enter to continue!");
            Console.ReadLine();
        }
    }
}