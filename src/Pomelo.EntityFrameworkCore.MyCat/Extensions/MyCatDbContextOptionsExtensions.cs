// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.Data.MyCat;

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore
{
    public static class MyCatDbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder UseDataNode(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] string Server,
            [NotNull] string Database,
            [NotNull] string UserId,
            [NotNull] string Password,
            uint Port = 3306)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            var extension = GetOrCreateExtension(optionsBuilder);
            var dn = new MyCatDataNode
            {
                Master = new MyCatDatabaseHost
                {
                    Host = Server,
                    Database = Database,
                    Password = Password,
                    Username = UserId,
                    Port = Port
                }
            };
            extension.DataNodes.Add(dn);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            
            return optionsBuilder;
        }

        public static DbContextOptionsBuilder UseDataNode(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] string MasterServer,
            [NotNull] string MasterDatabase,
            [NotNull] string MasterUserId,
            [NotNull] string MasterPassword,
            [NotNull] string SlaveServer,
            [NotNull] string SlaveDatabase,
            [NotNull] string SlaveUserId,
            [NotNull] string SlavePassword,
            uint MasterPort = 3306,
            uint SlavePort = 3306)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            var extension = GetOrCreateExtension(optionsBuilder);
            var dn = new MyCatDataNode
            {
                Master = new MyCatDatabaseHost
                {
                    Host = MasterServer,
                    Database = MasterDatabase,
                    Password = MasterPassword,
                    Username = MasterUserId,
                    Port = MasterPort
                },
                Slave = new MyCatDatabaseHost
                {
                    Host = SlaveServer,
                    Database = SlaveDatabase,
                    Password = SlavePassword,
                    Username = SlaveUserId,
                    Port = SlavePort
                }
            };
            extension.DataNodes.Add(dn);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }

        public static DbContextOptionsBuilder UseMyCat(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] string connectionString,
            [CanBeNull] Action<MyCatDbContextOptionsBuilder> MyCatOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotEmpty(connectionString, nameof(connectionString));

            var csb = new MyCatConnectionStringBuilder(connectionString)
            {
                AllowUserVariables = true
            };
            connectionString = csb.ConnectionString;

            var extension = GetOrCreateExtension(optionsBuilder);
            extension.ConnectionString = connectionString;
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            MyCatOptionsAction?.Invoke(new MyCatDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        public static DbContextOptionsBuilder UseMyCat(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] DbConnection connection,
            [CanBeNull] Action<MyCatDbContextOptionsBuilder> MyCatOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotNull(connection, nameof(connection));

            var csb = new MyCatConnectionStringBuilder(connection.ConnectionString)
            {
                AllowUserVariables = true
            };

            connection.ConnectionString = csb.ConnectionString;
            var extension = GetOrCreateExtension(optionsBuilder);
            extension.Connection = connection;
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            MyCatOptionsAction?.Invoke(new MyCatDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        public static DbContextOptionsBuilder<TContext> UseMyCat<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] string connectionString,
            [CanBeNull] Action<MyCatDbContextOptionsBuilder> MyCatOptionsAction = null)
            where TContext : DbContext
        {
            return (DbContextOptionsBuilder<TContext>)UseMyCat(
                (DbContextOptionsBuilder)optionsBuilder, new MyCatConnection(connectionString), MyCatOptionsAction);
        }

        public static DbContextOptionsBuilder<TContext> UseMyCat<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] DbConnection connection,
            [CanBeNull] Action<MyCatDbContextOptionsBuilder> MyCatOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseMyCat(
                (DbContextOptionsBuilder)optionsBuilder, connection, MyCatOptionsAction);

        public static MyCatOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        {
            var existing = optionsBuilder.Options.FindExtension<MyCatOptionsExtension>();
            return existing != null
                ? existing
                : new MyCatOptionsExtension();
        }
    }
}
