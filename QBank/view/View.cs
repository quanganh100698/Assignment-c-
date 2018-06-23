using System;
using System.IO;
using System.Runtime.InteropServices;
using QBank.entity;
using QBank.utility;
using QBank.controller;
namespace QBank.view
{
    public class View
    {
        private static AccountController controller = new AccountController();
        
        public static void GenerateMenu()
        {
            while (true)
            {
                if (Program.currentLoggedIn == null)
                {
                    NotLogMenu();
                }
                else
                {
                    LogMenu();
                }
            }
        }

        private static void NotLogMenu()
        {
            while (true)
            {
                Console.WriteLine("--------------------Q's Bank--------------------");
                Console.WriteLine("Welcome to Q's Bank, your trusted partner.");
                Console.WriteLine("Select 1 option below to continue.");
                Console.WriteLine("1. Login to existed account.");
                Console.WriteLine("2. Sign in to new accounct.");
                Console.WriteLine("3. Exit.");
                Console.WriteLine("Please enter your choice");
                int choice = Convert.ToInt32(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        if (controller.DoLogin())
                        {
                            Console.WriteLine("Login successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Login unsuccessfully.");
                        }

                        break;
                    case 2:
                        controller.SignIn();
                        break;
                    case 3:
                        Console.WriteLine("Goodbye.");
                        Environment.Exit(1);
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                        
                }

                if (Program.currentLoggedIn != null)
                {
                    break;
                }
            }
        }

        private static void LogMenu()
        {
            while (true)
            {
                Console.WriteLine("--------------------Q's Bank--------------------");
                Console.WriteLine("Hello " + Program.currentLoggedIn.FullName);
                Console.WriteLine("Please selection one option below to continue:");
                Console.WriteLine("1. Check your balance.");
                Console.WriteLine("2. Withdraw money.");
                Console.WriteLine("3. Deposit money.");
                Console.WriteLine("4. Transfer money.");
                Console.WriteLine("5. Exit.");
                Console.WriteLine("Please enter your choice");
                int choice = Convert.ToInt32(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        controller.CheckBalance();
                        break;
                    case 2:
                        controller.Withdraw();
                        break;
                    case 3:
                        controller.Deposit();
                        break;
                    case 4:
                        controller.Transfer();
                        break;
                    case 5:
                        Console.WriteLine("Goodbye.");
                        Environment.Exit(1);
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}