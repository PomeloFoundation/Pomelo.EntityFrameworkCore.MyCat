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
            [NotNull] Action<MyCatDataNode> BuildDn)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            var extension = GetOrCreateExtension(optionsBuilder);
            var dn = new MyCatDataNode();
            BuildDn?.Invoke(dn);
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
