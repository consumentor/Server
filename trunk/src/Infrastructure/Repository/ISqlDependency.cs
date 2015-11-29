using System;
using System.Data.SqlClient;

namespace Consumentor.ShopGun.Repository
{
    public interface ISqlDependency
    {
        void Start(SqlCommand command);
        void Terminate(string connectionString);
        void AddCommandDependency(SqlCommand command);
        event EventHandler<SqlNotificationEventArgs> OnChange;
    }
}