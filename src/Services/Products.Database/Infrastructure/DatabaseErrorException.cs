using System;

namespace Products.Database.Infrastructure
{
    [Serializable]
    internal class DatabaseErrorException : Exception
    {
        public DatabaseErrorException()
        {
        }

        public DatabaseErrorException(Exception ex)
        {
            this.Data["Message"] = "Database query error:\n" + ex.Message;
        }

    }
}