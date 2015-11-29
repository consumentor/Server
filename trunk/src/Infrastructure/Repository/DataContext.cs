using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Globalization;
using System.Reflection;
using Consumentor.ShopGun.Configuration;
using LinqDataContext = System.Data.Linq.DataContext;

namespace Consumentor.ShopGun.Repository
{
    public class DataContext : LinqDataContext
    {
        private readonly Dictionary<Type, ITable> _tables = new Dictionary<Type, ITable>();
        public event EventHandler<EventArgs> DatabaseCreated;

        public DataContext(IConfiguration configuration, IAttributeMappingSource mappingSource)
            : this(configuration.ConnectionString, mappingSource)
        {
        }

        protected DataContext(string connectionString, IAttributeMappingSource mappingSource)
            : base(connectionString)
        {
            Initialize();
            AddTablesForAssemblies(mappingSource.MappingAssemblies);

        }

        private void AddTablesForAssemblies(IEnumerable<Assembly> assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                AddTablesForAssembly(assembly);
            }
        }

        private void AddTablesForAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                Type tableType = GetEntityTableClassForType(type);
                if (tableType != null)
                {
                    ITable table = GetTable(tableType);
                    AddTableToTableCache(type, table);
                }
            }
        }

        private void Initialize()
        {
            //DataLoadOptions options = new DataLoadOptions();
            //options.LoadWith<DefectImage>(c => c.FilePath);
            //LoadOptions = options;
        }

        public Type GetEntityTableClassForType<T>() where T : class
        {
            return GetEntityTableClassForType(typeof(T));
        }

        private Type GetEntityTableClassForType(Type type)
        {
            return GetEntityTableClassForAttributeClass(type);
        }

        private Type GetEntityTableClassForAttributeClass(Type entityType)
        {
            TableAttribute[] attrib = (TableAttribute[])entityType.GetCustomAttributes(typeof(TableAttribute), true);
            while (entityType != typeof(object) && entityType.IsClass && attrib.Length != 1)
            {
                entityType = entityType.BaseType;
                attrib = (TableAttribute[])entityType.GetCustomAttributes(typeof(TableAttribute), true);
            }
            if (attrib.Length == 1)
            {
                return entityType;
            }
            return null;
        }

        private void AddTableToTableCache(Type type, ITable table)
        {
            if (_tables.ContainsKey(type) == false)
            {
                _tables.Add(type, table);
            }
        }

        public ITable GetTableForType(Type type)
        {
            ITable table;
            if (_tables.TryGetValue(type, out table) == false)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Type {0} or its base type doesnt have a table attribute!",
                                                          type.Name));
            }

            return table;
        }

        public new void CreateDatabase()
        {
            base.CreateDatabase();

            if (DatabaseCreated != null)
                DatabaseCreated(this, new EventArgs());
        }
    }
}