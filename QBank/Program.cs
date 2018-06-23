using System;
using QBank.view;
using QBank.entity;
namespace QBank
{
    class Program
    {
        public static Account currentLoggedIn;
        static void Main(string[] args)
        {
            View.GenerateMenu();
        }
    }
}