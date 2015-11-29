using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Consumentor.ShopGun.Configuration;
using Consumentor.ShopGun.Repository;
using NBehave.Spec.NUnit;
using Rhino.Mocks;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;
using Ignore = NUnit.Framework.IgnoreAttribute;
using LinqDataContext = System.Data.Linq.DataContext;

namespace ShopGunSpecBase
{
    public static class ConnectionStringBuilder
    {
        public static string GetConnectionString(this string databaseName)
        {
            const string defaultConnStr = @"Server={0};Database={1};Trusted_Connection=yes";
            string server = ConfigurationManager.AppSettings["SqlServerToUse"] ?? @".\SqlExpress";
            return string.Format(CultureInfo.CurrentCulture, defaultConnStr, server, databaseName);
        }
    }

    public abstract class DatabaseSpecBase : SpecBase
    {
        protected IConfiguration _configuration;
        protected IAttributeMappingSource _attributeMappingSource;
        protected string _databaseName;

        public void SetupDatabase(string databaseName, Assembly domainAssembly)
        {
            _databaseName = databaseName;
            _configuration = CreateStub<IConfiguration>();
            _configuration.Stub(c => c.ConnectionString).Return(databaseName.GetConnectionString()).Repeat.Any();
            _attributeMappingSource = CreateStub<IAttributeMappingSource>();
            _attributeMappingSource.Stub(x => x.MappingAssemblies).Return(new List<Assembly> { domainAssembly }).Repeat.Any();
            ReCreateDatabaseFromAttributeMapping(GetNewDataContext());
        }

        protected virtual DataContext GetNewDataContext()
        {
            return new DataContext(_configuration, _attributeMappingSource);
        }

        protected DataContext GetNewDataContext(string databaseName, Assembly domainAssembly)
        {
            _configuration = CreateStub<IConfiguration>();
            _configuration.Stub(c => c.ConnectionString).Return(databaseName.GetConnectionString()).Repeat.Any();
            _attributeMappingSource = CreateStub<IAttributeMappingSource>();
            _attributeMappingSource.Stub(x => x.MappingAssemblies).Return(new List<Assembly> { domainAssembly }).Repeat.Any();
            return new DataContext(_configuration, _attributeMappingSource);
        }

        protected void ReCreateDatabaseFromAttributeMapping(DataContext ctx)
        {
            bool success = false;
            bool retry = false;
            int retries = 0;
            do
            {
                try
                {
                    using (ctx)
                    {
                        CloseAllOpenConnections(ctx);
                        ctx.Log = new StringWriter();

                        if (ctx.DatabaseExists())
                        {
                            //drop all connections by disabling multi-user (and immediatly abort all current transactions)
                            ctx.ExecuteCommand("ALTER DATABASE " + _databaseName + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
                            ctx.DeleteDatabase();
                        }
                        try
                        {
                            ctx.CreateDatabase();
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e);
                            Debug.WriteLine(ctx.Log);
                            throw;
                        }

                        //re-enable multi-user (again)
                        ctx.ExecuteCommand("ALTER DATABASE " + _databaseName + " SET MULTI_USER");
                        success = true;
                    }
                }
                catch (SqlException e)
                {
                    retry = false;
                    if (e.Message.IndexOf("was deadlocked on lock resources with another process and has been chosen as the deadlock victim") > 0)
                    {
                        retry = true;
                        retries++;
                    }
                }
            } while (success == false && (retry && retries < 3));
        }

        private void CloseAllOpenConnections(LinqDataContext ctx)
        {
            ctx.Log = new StringWriter();

            if (ctx.DatabaseExists())
            {
                //drop all connections by disabling multi-user (and immediatly abort all current transactions)
                ctx.ExecuteCommand("ALTER DATABASE " + _databaseName + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
                //re-enable multi-user
                ctx.ExecuteCommand("ALTER DATABASE " + _databaseName + " SET MULTI_USER");
            }

        }
    }
}
