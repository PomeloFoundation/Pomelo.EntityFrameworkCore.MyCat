// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Text;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Migrations.Internal
{
    public class MyCatHistoryRepository : HistoryRepository
    {

        private readonly MyCatRelationalConnection _connection;
        private readonly MyCatDatabaseCreator _databaseCreator;
        private readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;

        public MyCatHistoryRepository(
            [NotNull] IDatabaseCreator databaseCreator,
            [NotNull] IRawSqlCommandBuilder sqlCommandBuilder,
            [NotNull] MyCatRelationalConnection connection,
            [NotNull] IDbContextOptions options,
            [NotNull] IMigrationsModelDiffer modelDiffer,
            [NotNull] MyCatMigrationsSqlGenerationHelper migrationsSqlGenerationHelper,
            [NotNull] MyCatAnnotationProvider annotations,
            [NotNull] ISqlGenerationHelper SqlGenerationHelper)
            : base(
                  databaseCreator,
                  sqlCommandBuilder,
                  connection,
                  options,
                  modelDiffer,
                  migrationsSqlGenerationHelper,
                  annotations,
                  SqlGenerationHelper)
        {
            _connection = connection;
            _databaseCreator = (MyCatDatabaseCreator)databaseCreator;
            _rawSqlCommandBuilder = sqlCommandBuilder;
        }

        public bool Exists(MyCatRelationalConnection conn)
        {
           return _databaseCreator.Exists(conn)
           && InterpretExistsResult(
               _rawSqlCommandBuilder.Build(ExistsSql).ExecuteScalar(conn));
        }

        public async Task<bool> ExistsAsync(MyCatRelationalConnection conn, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _databaseCreator.ExistsAsync(conn, cancellationToken)
          && InterpretExistsResult(
              await _rawSqlCommandBuilder.Build(ExistsSql).ExecuteScalarAsync(conn, cancellationToken: cancellationToken));
        }

        protected override void ConfigureTable([NotNull] EntityTypeBuilder<HistoryRow> history)
        {
            base.ConfigureTable(history);
            history.Property(h => h.MigrationId).HasMaxLength(150);
            history.Property(h => h.ProductVersion).HasMaxLength(32).IsRequired();
        }

        protected override string ExistsSql
        {
            get
            {
                var builder = new StringBuilder();

                builder.Append("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE ");
                
                builder
                    .Append("TABLE_SCHEMA='")
                    .Append(SqlGenerationHelper.EscapeLiteral(_connection.DbConnection.Database))
                    .Append("' AND TABLE_NAME='")
                    .Append(SqlGenerationHelper.EscapeLiteral(TableName))
                    .Append("';");

                return builder.ToString();
            }
        }

        protected override bool InterpretExistsResult(object value) => value != DBNull.Value;

        public override string GetCreateIfNotExistsScript()
        {
            return GetCreateScript();
        }

        public override string GetBeginIfNotExistsScript(string migrationId)
        {
            throw new NotSupportedException("Generating idempotent scripts for migration is not currently supported by MyCat");
        }

        public override string GetBeginIfExistsScript(string migrationId)
        {
            throw new NotSupportedException("Generating idempotent scripts for migration is not currently supported by MyCat");
        }

        public override string GetEndIfScript()
        {
            throw new NotSupportedException("Generating idempotent scripts for migration is not currently supported by MyCat");
        }

        public IReadOnlyList<HistoryRow> GetAppliedMigrations(MyCatRelationalConnection connection)
        {
            var rows = new List<HistoryRow>();

            if (Exists(connection))
            {
                var command = _rawSqlCommandBuilder.Build(GetAppliedMigrationsSql);

                using (var reader = command.ExecuteReader(connection))
                {
                    while (reader.DbDataReader.Read())
                    {
                        rows.Add(new HistoryRow(reader.DbDataReader.GetString(0), reader.DbDataReader.GetString(1)));
                    }
                }
            }

            return rows;
        }

        public async Task<IReadOnlyList<HistoryRow>> GetAppliedMigrationsAsync(
            MyCatRelationalConnection connection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var rows = new List<HistoryRow>();

            if (await ExistsAsync(connection, cancellationToken))
            {
                var command = _rawSqlCommandBuilder.Build(GetAppliedMigrationsSql);

                using (var reader = await command.ExecuteReaderAsync(connection, cancellationToken: cancellationToken))
                {
                    while (await reader.DbDataReader.ReadAsync(cancellationToken))
                    {
                        rows.Add(new HistoryRow(reader.DbDataReader.GetString(0), reader.DbDataReader.GetString(1)));
                    }
                }
            }

            return rows;
        }
    }
}
