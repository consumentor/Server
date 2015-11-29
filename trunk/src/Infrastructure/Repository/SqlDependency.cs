using System;
using System.Data;
using System.Data.SqlClient;
using Consumentor.ShopGun.Configuration;
using AdoNetSqlDependency = System.Data.SqlClient.SqlDependency;

namespace Consumentor.ShopGun.Repository
{
    /// <summary>
    /// You cant run as LocalSystem and use this class.
    /// For more considerations read http://msdn.microsoft.com/en-us/library/aewzkxxh.aspx
    /// alter database P2SimulatorTest set enable_broker
    /// </summary>
    public class SqlDependency : ISqlDependency, IDisposable
    {
        private readonly string _connectionString;
        private AdoNetSqlDependency _dependency;
        private SqlDataReader _depReader;
        private SqlCommand _command;

        public SqlDependency(IConfiguration configuration)
        {
            _connectionString = configuration.ConnectionString;
        }

        void ISqlDependency.Start(SqlCommand command)
        {
            if (_dependency != null)
                throw new ArgumentException("You need to call  End() before you can call Start() again.");
            _command = command;
            Start();
        }

        private void Start()
        {
            _command = new SqlCommand(_command.CommandText, new SqlConnection(_connectionString));
            OpenConnectionIfClosed(_command);
            AdoNetSqlDependency.Start(_connectionString);
            _dependency = new AdoNetSqlDependency(_command);
            _depReader = _command.ExecuteReader();
            _dependency.OnChange += OnDependencyChanged;
        }

        private void OpenConnectionIfClosed(SqlCommand command)
        {
            if (command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();
        }

        private void OnDependencyChanged(object sender, SqlNotificationEventArgs e)
        {
            if (_eventHandler != null)
                _eventHandler.Invoke(this, e);
            _dependency = null;
            Start();
        }


        void ISqlDependency.Terminate(string connectionString)
        {
            try
            {
                _depReader.Close();
            }
            finally
            {
                try
                {
                    AdoNetSqlDependency.Stop(connectionString);
                }
                finally
                {
                    _dependency = null;
                }
            }
        }


        void ISqlDependency.AddCommandDependency(SqlCommand command)
        {
            _dependency.AddCommandDependency(command);
        }

        private EventHandler<SqlNotificationEventArgs> _eventHandler;
        event EventHandler<SqlNotificationEventArgs> ISqlDependency.OnChange
        {
            add { _eventHandler += value; }
            remove { _eventHandler -= value; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {
                    if (_command != null)
                    {
                        _command.Dispose();
                    }
                    if (_depReader != null)
                    {
                        _depReader.Dispose();
                    }
                }
                _disposed = true;
            }
        }
    }
}