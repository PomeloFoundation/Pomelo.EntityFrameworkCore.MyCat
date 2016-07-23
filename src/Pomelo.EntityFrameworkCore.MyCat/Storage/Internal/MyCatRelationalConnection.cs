// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;
using Pomelo.Data.MySql;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MyCatRelationalConnection : RelationalConnection
    {
        private static System.Collections.Generic.List<MyCatRelationalConnection> test = new System.Collections.Generic.List<MyCatRelationalConnection>();

        public MyCatRelationalConnection(
            [NotNull] IDbContextOptions options,
            // ReSharper disable once SuggestBaseTypeForParameter
            [NotNull] ILogger<MySqlConnection> logger)
            : base(options, logger)
        {
            test.Add(this);
        }

        private MyCatRelationalConnection(
            [NotNull] IDbContextOptions options, [NotNull] ILogger logger)
            : base(options, logger)
        {
            test.Add(this);
        }

        // TODO: Consider using DbProviderFactory to create connection instance
        // Issue #774
        protected override DbConnection CreateDbConnection() => new MySqlConnection(ConnectionString);

        public MyCatRelationalConnection CreateMasterConnection()
        {
            var csb = new MySqlConnectionStringBuilder(ConnectionString) {
                Database = "MyCat",
                Pooling = false
            };
            
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMyCat(csb.GetConnectionString(true));
            return new MyCatRelationalConnection(optionsBuilder.Options, Logger);
        }
    }
}
