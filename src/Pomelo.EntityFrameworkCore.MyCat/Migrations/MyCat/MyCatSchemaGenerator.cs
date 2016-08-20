using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Migrations
{
    public class MyCatSchemaGenerator
    {
        private static FieldInfo EntityTypesField = typeof(Model).GetTypeInfo().DeclaredFields.Single(x => x.Name == "_entityTypes");

        private DbContext DB;
        private IDbSetFinder DbSetFinder;
        private IDatabaseProvider MyCatDnContext;
        private IDbContextOptions DbContextOptions;

        public MyCatSchemaGenerator(ICurrentDbContext ctx, IDbSetFinder dbSetFinder, IDatabaseProvider myCatDnContext, IDbContextOptions options)
        {
            DB = ctx.Context;
            DbSetFinder = dbSetFinder;
            MyCatDnContext = myCatDnContext;
            DbContextOptions = options;
        }

        public IList<MyCatTable> Schema
        {
            get
            {
                var ret = new List<MyCatTable>();
                var remove = new List<string>();
                var entities = (IDictionary<string, EntityType>)EntityTypesField.GetValue(DB.Model);
                foreach (var x in entities)
                {
                    var keys = x.Value.GetKeys().Where(y => y.IsPrimaryKey());

                    // Table Name mapping
                    var tableName = ParseTableName(x.Value);

                    // Data Nodes mapping
                    var dataNodes = ParseDataNodes(x.Value);

                    bool IsAutoIncrement = false;

                    if (x.Value.GetKeys().Count(y => y.IsPrimaryKey()) == 1)
                    {
                        IsAutoIncrement = x.Value.GetKeys().FirstOrDefault(y => y.IsPrimaryKey()).GetAnnotations().Where(y => y.Name == "Relational:ValueGeneratedOnAdd") != null;
                    }

                    // 外键涉及的表均不能分片
                    if (x.Value.GetForeignKeys().Count() > 0)
                    {
                        dataNodes = new string[] { "dn0" };
                        foreach(var y in x.Value.GetForeignKeys())
                        {
                            var principalTableName = ParseTableName(y.PrincipalEntityType);
                            remove.Add(principalTableName);
                        }
                    }

                    ret.Add(new MyCatTable { Keys = keys.Select(y => y.Properties.First().Name).ToArray(), TableName = tableName, DataNodes = dataNodes , IsAutoIncrement = IsAutoIncrement});
                }

                foreach(var x in remove)
                {
                    ret.Single(y => y.TableName == x).DataNodes = new string[] { "dn0" };
                }

                return ret;
            }
        }

        protected virtual string ParseTableName(EntityType type)
        {
            string tableName;
            var anno = type.FindAnnotation("Relational:TableName");
            if (anno != null)
                tableName = anno.Value.ToString();
            else
            {
                var prop = DbSetFinder.FindSets(DB).SingleOrDefault(y => y.ClrType == type.ClrType);
                if (!prop.Equals(default(DbSetProperty)))
                    tableName = prop.Name;
                else
                    tableName = type.ClrType.Name;
            }
            return tableName;
        }
        protected virtual string[] ParseDataNodes(EntityType type)
        {
            string[] dataNodes;
            var attr = type.ClrType.GetTypeInfo().GetCustomAttribute<DataNodeAttribute>();
            if (attr != null)
                dataNodes = attr.DataNodes;
            else
            {
                var ext = DbContextOptions.FindExtension<MyCatOptionsExtension>();
                var tmp = new List<string>();
                for (var i = 0; i < ext.DataNodes.Count; i++)
                    tmp.Add("dn" + i);
                dataNodes = tmp.ToArray();
            }
            return dataNodes;
        }
    }
}
