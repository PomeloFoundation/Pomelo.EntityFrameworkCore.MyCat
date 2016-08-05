// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;
using Pomelo.Data.MyCat;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MyCatRelationalConnection : RelationalConnection
    {
        public MyCatRelationalConnection(
            [NotNull] IDbContextOptions options,
            // ReSharper disable once SuggestBaseTypeForParameter
            [NotNull] ILogger<MyCatConnection> logger)
            : base(options, logger)
        {
        }

        private MyCatRelationalConnection(
            [NotNull] IDbContextOptions options, [NotNull] ILogger logger)
            : base(options, logger)
        {
        }

        // TODO: Consider using DbProviderFactory to create connection instance
        // Issue #774
        protected override DbConnection CreateDbConnection() => new MyCatConnection(ConnectionString);

        public MyCatRelationalConnection CreateMasterConnection()
        {
            var csb = new MyCatConnectionStringBuilder(ConnectionString) {
                Database = "mysql",
                Pooling = false
            };
            
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMyCat(csb.GetConnectionString(true));
            return new MyCatRelationalConnection(optionsBuilder.Options, Logger);
        }
    }
}
