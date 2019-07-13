using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnaDB.MySQL
{
    /// <summary>
    /// This exception is raised when a SqlGenerator method is being called with differences
    /// between the keys and the values in the parameters
    /// </summary>
    public class DisparityException : Exception
    {
        /// <summary>
        /// This exception is raised when a SqlGenerator method is being called with differences
        /// between the keys and the values in the parameters 
        /// </summary>
        /// <param name="message">This exception's message</param>
        public DisparityException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Raised when a Table (or other queryable database object) doesn't exist this exception is raised
    /// </summary>
    public class InvalidTableException : Exception
    {
        /// <summary>
        /// Raised when a Table (or other queryable database object) doesn't exist this exception is raised
        /// </summary>
        /// <param name="message">This exception's message</param>
        /// <param name="ex">The InnerException that was raised</param>
        public InvalidTableException(string message, Exception ex = null) : base(message, ex)
        {
        }
    }

    /// <summary>
    /// Raised when a Key Object contains nulls or invalid values
    /// </summary>
    public class InvalidKeyException : Exception
    {
        /// <summary>
        /// Raised when a Key Object contains nulls or invalid values
        /// </summary>
        /// <param name="message">This exception's message</param>
        /// <param name="ex">The InnerException that was raised</param>
        public InvalidKeyException(string message, Exception ex = null) : base(message, ex)
        {
        }
    }

    /// <summary>
    /// Raised when the existing model does not correctly matchup with the database object.
    /// Check for all the existing columns in the table within the database.
    /// </summary>
    public class InvalidModelException : Exception
    {
        /// <summary>
        /// Raised when the existing model does not correctly matchup with the database object.
        /// Check for all the existing columns in the table within the database.
        /// </summary>
        /// <param name="message">This exception's message</param>
        /// <param name="ex">The InnerException that was raised</param>
        public InvalidModelException(string message, Exception ex = null) : base(message, ex)
        {

        }
    }

    /// <summary>
    /// Raised when an error occurs while connecting to the Database or Database Server
    /// </summary>
    public class DbConnectionException : Exception
    {
        /// <summary>
        /// Raised when the existing model does not correctly matchup with the database object.
        /// Check for all the existing columns in the table within the database.
        /// </summary>
        /// <param name="message">This exception's message</param>
        /// <param name="ex">The InnerException that was raised</param>
        public DbConnectionException(string message, Exception ex = null) : base(message, ex)
        {
        }
    }
}
