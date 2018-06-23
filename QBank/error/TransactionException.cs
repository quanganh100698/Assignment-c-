using System;

namespace QBank.error
{
    public class TransactionException: Exception
    {
        public TransactionException(string message) : base(message)
        {
        }
    }
}