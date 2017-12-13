using MySql.Data.MySqlClient;
using System;

namespace OverSurgerySystem
{
    public partial class Database : IDisposable
    {
        // Singleton instance and class member.
        private static readonly Database instance   = new Database();
        private MySqlConnection connection          = null;
        
        // Constructors... they're empty at the moment.
        static Database()   {}
        private Database()  {}

        // Property to get the singleton instance.
        public static Database Instance
        {
            get
            {
                return Database.instance;
            }
        }

        // Property to get an SQL connection.
        public static MySqlConnection Connection
        {
            get
            {
                if( Instance.connection == null )
                {
                    MySqlConnectionStringBuilder sqlBuilder = new MySqlConnectionStringBuilder();
                    sqlBuilder.Database                     = DATABASE_NAME;
                    sqlBuilder.Server                       = DATABASE_ADDRESS;
                    sqlBuilder.Port                         = DATABASE_PORT;
                    sqlBuilder.UserID                       = DATABASE_USER;
                    sqlBuilder.Password                     = DATABASE_PASSWORD;
            
                    try
                    {
                        instance.connection = new MySqlConnection( sqlBuilder.ConnectionString );
                        instance.connection.Open();
                        return instance.connection;
                    }
                    catch
                    {
                        instance.connection = null;
                        return null;
                    }
                }
                else
                {
                    return Instance.connection;
                }
            }
        }

        public void Dispose()
        {
            if( connection != null )
                connection.Close();

            connection = null;
        }
    }
}
