// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Net.Http;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.Internal;
using Pomelo.Data.MyCat;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MyCatDatabaseCreator : RelationalDatabaseCreator
    {
        private readonly MyCatRelationalConnection _connection;
        private readonly IMigrationsSqlGenerator _migrationsSqlGenerator;
        private readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;
        private readonly IDbContextOptions _options;
        private readonly MyCatSchemaGenerator _schemaGenerator;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MyCatDatabaseCreator(
            [NotNull] MyCatRelationalConnection connection,
            [NotNull] IMigrationsModelDiffer modelDiffer,
            [NotNull] IMigrationsSqlGenerator migrationsSqlGenerator,
            [NotNull] IMigrationCommandExecutor migrationCommandExecutor,
            [NotNull] IModel model,
            [NotNull] IRawSqlCommandBuilder rawSqlCommandBuilder,
            [NotNull] IDbContextOptions options,
            [NotNull] MyCatSchemaGenerator schemaGenerator)
            : base(model, connection, modelDiffer, migrationsSqlGenerator, migrationCommandExecutor)
        {
            Check.NotNull(rawSqlCommandBuilder, nameof(rawSqlCommandBuilder));

            _connection = connection;
            _migrationsSqlGenerator = migrationsSqlGenerator;
            _rawSqlCommandBuilder = rawSqlCommandBuilder;
            _options = options;
            _schemaGenerator = schemaGenerator;
            
            var schema = _schemaGenerator.Schema;
            var csb = new MyCatConnectionStringBuilder(_connection.ConnectionString);
            // TODO: Generate schema.xml and send to mycat ef core proxy
            using (var client = new HttpClient() { BaseAddress = new Uri("http://" + csb.Server + ":7066") })
            {
                var task = client.PostAsync("/", new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Username", csb.UserID),
                    new KeyValuePair<string, string>("Password", csb.Password),
                    new KeyValuePair<string, string>("Database", csb.Database),
                    new KeyValuePair<string, string>("DataNodes", Newtonsoft.Json.JsonConvert.SerializeObject(_options.FindExtension<MyCatOptionsExtension>().DataNodes)),
                    new KeyValuePair<string, string>("Schema", Newtonsoft.Json.JsonConvert.SerializeObject(schema)),
                }));
                task.Wait();
            }
        }

        public static FieldInfo DbOptions = typeof(DbContext).GetTypeInfo().DeclaredFields.Single(x => x.Name == "_options");

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override void Create()
        {
            var ext = _options.FindExtension<MyCatOptionsExtension>();
            foreach (var x in ext.DataNodes)
            {
                Create(x.Master);
                if (x.Slave != null)
                    Create(x.Slave);
            }
        }

        public void Create(MyCatDatabaseHost node)
        {
            using (var masterConnection = _connection.CreateMasterConnection(node))
            {
                var cmd = CreateCreateOperations(node);
                MigrationCommandExecutor
                    .ExecuteNonQuery(cmd, masterConnection);

                ClearPool();
            }

            Exists(_connection.CreateNodeConnection(node), retryOnNotExists: false);
        }

        public void CreateTables(MyCatDatabaseHost node)
        {
            MigrationCommandExecutor.ExecuteNonQuery(GetCreateTablesCommands(), _connection.CreateNodeConnection(node));
        }

        public async Task CreateTablesAsync(MyCatDatabaseHost node, CancellationToken cancellationToken = default(CancellationToken))
        {
            await MigrationCommandExecutor.ExecuteNonQueryAsync(GetCreateTablesCommands(), _connection.CreateNodeConnection(node), cancellationToken);
        }

        public override async Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var ext = _options.FindExtension<MyCatOptionsExtension>();
            foreach (var x in ext.DataNodes)
            {
                if (!await ExistsAsync(_connection.CreateNodeConnection(x.Master), cancellationToken))
                {
                    await CreateAsync (x.Master, cancellationToken);
                    await CreateTablesAsync(x.Master, cancellationToken);
                    continue;
                }

                if (!HasTables())
                {
                    await CreateTablesAsync(x.Master);
                    continue;
                }

                if (x.Slave != null)
                {
                    if (!Exists(_connection.CreateNodeConnection(x.Slave)))
                    {
                        await CreateAsync(x.Slave, cancellationToken);
                        await CreateTablesAsync(x.Slave, cancellationToken);
                        continue;
                    }

                    if (!HasTables())
                    {
                        await CreateTablesAsync(x.Slave, cancellationToken);
                        continue;
                    }
                }

                return false;
            }
            return true;
        }

        public override bool EnsureCreated()
        {
            var ext = _options.FindExtension<MyCatOptionsExtension>();
            foreach (var x in ext.DataNodes)
            {
                if (!Exists(_connection.CreateNodeConnection(x.Master)))
                {
                    Create(x.Master);
                    CreateTables(x.Master);
                    continue;
                }

                if (!HasTables())
                {
                    CreateTables(x.Master);
                    continue;
                }

                if (x.Slave != null)
                {
                    if (!Exists(_connection.CreateNodeConnection(x.Slave)))
                    {
                        Create(x.Slave);
                        CreateTables(x.Slave);
                        continue;
                    }

                    if (!HasTables())
                    {
                        CreateTables(x.Slave);
                        continue;
                    }
                }

                return false;
            }
            return true;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override async Task CreateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var ext = _options.FindExtension<MyCatOptionsExtension>();
            foreach (var x in ext.DataNodes)
            {
                await CreateAsync(x.Master, cancellationToken);
                if (x.Slave != null)
                    await CreateAsync(x.Slave, cancellationToken);
            }
        }

        public async Task CreateAsync(MyCatDatabaseHost node, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var masterConnection = _connection.CreateMasterConnection(node))
            {
                await MigrationCommandExecutor
                    .ExecuteNonQueryAsync(CreateCreateOperations(node), masterConnection, cancellationToken);

                ClearPool();
            }

            await ExistsAsync(_connection.CreateNodeConnection(node), retryOnNotExists: true, cancellationToken: cancellationToken);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override bool HasTables()
            => (long)CreateHasTablesCommand().ExecuteScalar(_connection) != 0;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override async Task<bool> HasTablesAsync(CancellationToken cancellationToken = default(CancellationToken))
            => (long)await CreateHasTablesCommand().ExecuteScalarAsync(_connection, cancellationToken: cancellationToken) != 0;

        public IRelationalCommand CreateHasTablesCommand()
            => _rawSqlCommandBuilder
                .Build(@"
                    SELECT CASE WHEN COUNT(*) = 0 THEN FALSE ELSE TRUE END
                    FROM information_schema.tables
                    WHERE table_type = 'BASE TABLE' AND table_schema = '" + _connection.DbConnection.Database + "'");

        public IReadOnlyList<MigrationCommand> CreateCreateOperations(MyCatDatabaseHost node)
            => _migrationsSqlGenerator.Generate(new[] { new MyCatCreateDatabaseOperation { Name = node.Database } });

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override bool Exists()
            => true;

        public bool Exists(MyCatDatabaseHost node)
        {
            return Exists(_connection.CreateNodeConnection(node), false);
        }

        public bool Exists(MyCatRelationalConnection conn, bool retryOnNotExists = false)
        {
            var retryCount = 0;
            var giveUp = DateTime.UtcNow + TimeSpan.FromMinutes(1);
            while (true)
            {
                try
                {
                    conn.Open();
                    conn.Close();
                    return true;
                }
                catch (MyCatException e)
                {
                    if (!retryOnNotExists
                        && IsDoesNotExist(e))
                    {
                        return false;
                    }

                    if (DateTime.UtcNow > giveUp
                        || !RetryOnExistsFailure(e, ref retryCount))
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override Task<bool> ExistsAsync(CancellationToken cancellationToken = default(CancellationToken))
            => Task.FromResult(true);

        public Task<bool> ExistsAsync(MyCatDatabaseHost node ,CancellationToken cancellationToken = default(CancellationToken))
        {
            return ExistsAsync(_connection.CreateNodeConnection(node), cancellationToken);
        }

        public Task<bool> ExistsAsync(MyCatRelationalConnection conn, CancellationToken cancellationToken = default(CancellationToken))
            => ExistsAsync(conn, false, cancellationToken);

        public async Task<bool> ExistsAsync(MyCatRelationalConnection conn, bool retryOnNotExists, CancellationToken cancellationToken)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    await conn.OpenAsync(cancellationToken);
                    conn.Close();
                    return true;
                }
                catch (MyCatException e)
                {
                    if (!retryOnNotExists
                        && IsDoesNotExist(e))
                    {
                        return false;
                    }

                    if (!RetryOnExistsFailure(e, ref retryCount))
                    {
                        throw;
                    }
                }
            }
        }

        // Login failed is thrown when database does not exist (See Issue #776)
        public static bool IsDoesNotExist(MyCatException exception) => exception.Number == 1049;

        // See Issue #985
        public bool RetryOnExistsFailure(MyCatException exception, ref int retryCount)
        {
            if (exception.Number == 1049 && ++retryCount < 30)
            {
                ClearPool();
                Thread.Sleep(100);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override void Delete()
        {
            var ext = _options.FindExtension<MyCatOptionsExtension>();
            foreach (var x in ext.DataNodes)
            {
                Delete(x.Master);
                if (x.Slave != null)
                    Delete(x.Slave);
            }
        }

        public void Delete(MyCatDatabaseHost node)
        {
            ClearAllPools();

            using (var masterConnection = _connection.CreateMasterConnection(node))
            {
                MigrationCommandExecutor
                    .ExecuteNonQuery(CreateDropCommands(), masterConnection);
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var ext = _options.FindExtension<MyCatOptionsExtension>();
            foreach (var x in ext.DataNodes)
            {
                await DeleteAsync(x.Master);
                if (x.Slave != null)
                    await DeleteAsync(x.Slave);
            }
        }

        public async Task DeleteAsync(MyCatDatabaseHost node, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClearAllPools();

            using (var masterConnection = _connection.CreateMasterConnection(node))
            {
                await MigrationCommandExecutor
                    .ExecuteNonQueryAsync(CreateDropCommands(), masterConnection, cancellationToken);
            }
        }

        public IReadOnlyList<MigrationCommand> CreateDropCommands()
        {
            var operations = new MigrationOperation[]
            {
                // TODO Check DbConnection.Database always gives us what we want
                // Issue #775
                new MyCatDropDatabaseOperation { Name = _connection.DbConnection.Database }
            };

            var masterCommands = _migrationsSqlGenerator.Generate(operations);
            return masterCommands;
        }

        // Clear connection pools in case there are active connections that are pooled
        public static void ClearAllPools() => MyCatConnection.ClearAllPools();

        // Clear connection pool for the database connection since after the 'create database' call, a previously
        // invalid connection may now be valid.
        public void ClearPool() => MyCatConnection.ClearPool((MyCatConnection)_connection.DbConnection);

        public override bool EnsureDeleted()
        {
            var ext = _options.FindExtension<MyCatOptionsExtension>();
            foreach (var x in ext.DataNodes)
                if (Exists(x.Master))
                    Delete(x.Master);
            return true;
        }

        public override async Task<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var ext = _options.FindExtension<MyCatOptionsExtension>();
            foreach (var x in ext.DataNodes)
                if (await ExistsAsync(x.Master))
                    await DeleteAsync(x.Master);
            return true;
        }
    }
}