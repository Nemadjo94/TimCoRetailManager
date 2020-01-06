using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDataManager.Library.Internal.DataAccess
{
    // Should stay internal because we dont want to allow outside calls
    internal class SqlDataAccess : IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private bool isClosed = false;

        public string GetConnectionString(string name)
        {
            // Gets the connection string from Web.config
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        // Loads data from db
        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using(IDbConnection connection = new SqlConnection(connectionString))
            {
                // Makes a connection, does a query and executes a stored procedure which returns some rows of data
                List<T> rows = connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure).ToList();
                return rows;
            }
        }

        // Saves data to db
        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        // Open connect/start transaction method
        public void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);
            // Open connection
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            // Start transaction
            _transaction = _connection.BeginTransaction();

            isClosed = false;
        }

        // Load using the transaction
        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            var rows = _connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();

            return rows;
        }

        // Save using the transaction
        public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {
            _connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);
        }

        // Close connection / stop transaction method
        public void CommitTransaction()
        {
            _transaction?.Commit();
            _connection?.Close();

            isClosed = true;
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _connection?.Close();

            isClosed = true;
        }

        public void Dispose()
        {
            if (!isClosed)
            {
                try
                {
                    CommitTransaction();

                }
                catch (Exception exc)
                {
                    // TODO - Log this issue
                    throw exc;
                }
            }

            _transaction = null;
            _connection = null;
        }
    }
}
