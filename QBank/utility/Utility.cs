using System;

namespace QBank.utility
{
    public class Utility
    {
        public static decimal GetUnsignDecimalNumber()
        {
            decimal choice;
            while (true)
            {
                try
                {
                    var strChoice = Console.ReadLine();
                    choice = Decimal.Parse(strChoice);
                    if (choice <= 0)
                    {
                        throw new FormatException();
                    }
                    else
                    {
                        break;
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Please enter a unsign number.");
                }
            }

            return choice;
        }
    }
}