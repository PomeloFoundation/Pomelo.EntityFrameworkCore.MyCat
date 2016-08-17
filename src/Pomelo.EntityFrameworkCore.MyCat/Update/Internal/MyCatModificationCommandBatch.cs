// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.Data.MyCat;

namespace Microsoft.EntityFrameworkCore.Update.Internal
{
    using RelationalStrings = Microsoft.EntityFrameworkCore.Internal.RelationalStrings;
    public class MyCatModificationCommandBatch : AffectedCountModificationCommandBatch
    {

        private const int DefaultNetworkPacketSizeBytes = 4096;
        private const int MaxScriptLength = 65536 * DefaultNetworkPacketSizeBytes / 2;
        private const int MaxParameterCount = 2100;
        private const int MaxRowCount = 1000;
        private int _parameterCount = 1; // Implicit parameter for the command text
        private readonly int _maxBatchSize;
        private readonly List<ModificationCommand> _bulkInsertCommands = new List<ModificationCommand>();
        private int _commandsLeftToLengthCheck = 50;

        public MyCatModificationCommandBatch(
            [NotNull] IRelationalCommandBuilderFactory commandBuilderFactory,
            [NotNull] ISqlGenerationHelper SqlGenerationHelper,
            [NotNull] IMyCatUpdateSqlGenerator updateSqlGenerator,
            [NotNull] IRelationalValueBufferFactoryFactory valueBufferFactoryFactory,
            [CanBeNull] int? maxBatchSize)
            : base(commandBuilderFactory, SqlGenerationHelper, updateSqlGenerator, valueBufferFactoryFactory)
        {
            if (maxBatchSize.HasValue
                && (maxBatchSize.Value <= 0))
            {
                throw new ArgumentOutOfRangeException(nameof(maxBatchSize), RelationalStrings.InvalidMaxBatchSize);
            }

            _maxBatchSize = Math.Min(maxBatchSize ?? int.MaxValue, MaxRowCount);
        }

        protected new virtual IMyCatUpdateSqlGenerator UpdateSqlGenerator => (IMyCatUpdateSqlGenerator)base.UpdateSqlGenerator;


        protected override bool CanAddCommand(ModificationCommand modificationCommand)
        {
            if (_maxBatchSize <= ModificationCommands.Count)
            {
                return false;
            }

            var additionalParameterCount = CountParameters(modificationCommand);

            if (_parameterCount + additionalParameterCount >= MaxParameterCount)
            {
                return false;
            }

            _parameterCount += additionalParameterCount;
            return true;
        }

        private static int CountParameters(ModificationCommand modificationCommand)
        {
            var parameterCount = 0;
            foreach (var columnModification in modificationCommand.ColumnModifications)
            {
                if (columnModification.ParameterName != null)
                {
                    parameterCount++;
                }

                if (columnModification.OriginalParameterName != null)
                {
                    parameterCount++;
                }
            }

            return parameterCount;
        }

        protected override void ResetCommandText()
        {
            base.ResetCommandText();
            _bulkInsertCommands.Clear();
        }

        protected override bool IsCommandTextValid()
        {
            if (--_commandsLeftToLengthCheck < 0)
            {
                var commandTextLength = GetCommandText().Length;
                if (commandTextLength >= MaxScriptLength)
                {
                    return false;
                }

                var avarageCommandLength = commandTextLength / ModificationCommands.Count;
                var expectedAdditionalCommandCapacity = (MaxScriptLength - commandTextLength) / avarageCommandLength;
                _commandsLeftToLengthCheck = Math.Max(1, expectedAdditionalCommandCapacity / 4);
            }

            return true;
        }
        protected override string GetCommandText()
            => base.GetCommandText() + GetBulkInsertCommandText(ModificationCommands.Count);

        private string GetBulkInsertCommandText(int lastIndex)
        {
            if (_bulkInsertCommands.Count == 0)
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();
            var grouping = UpdateSqlGenerator.AppendBulkInsertOperation(stringBuilder, _bulkInsertCommands, lastIndex);
            for (var i = lastIndex - _bulkInsertCommands.Count; i < lastIndex; i++)
            {
                CommandResultSet[i] = grouping;
            }

            if (grouping != ResultSetMapping.NoResultSet)
            {
                CommandResultSet[lastIndex - 1] = ResultSetMapping.LastInResultSet;
            }

            return stringBuilder.ToString();
        }

        protected override int ConsumeResultSetWithoutPropagation(int commandIndex, [NotNull] DbDataReader reader)
        {
            var expectedRowsAffected = 1;
            while ((++commandIndex < CommandResultSet.Count)
                   && CommandResultSet[commandIndex - 1] == ResultSetMapping.NotLastInResultSet)
            {
                Debug.Assert(!ModificationCommands[commandIndex].RequiresResultPropagation);

                if (expectedRowsAffected != reader.RecordsAffected)
                    throw new DbUpdateConcurrencyException(
                        RelationalStrings.UpdateConcurrencyException(1, 0),
                        ModificationCommands[commandIndex].Entries
                    );
            }

            return commandIndex;
        }

        protected override Task<int> ConsumeResultSetWithoutPropagationAsync(
            int commandIndex, [NotNull] DbDataReader reader, CancellationToken cancellationToken)
        {
            var expectedRowsAffected = 1;
            while ((++commandIndex < CommandResultSet.Count)
                   && CommandResultSet[commandIndex - 1] == ResultSetMapping.NotLastInResultSet)
            {
                Debug.Assert(!ModificationCommands[commandIndex].RequiresResultPropagation);

                if (expectedRowsAffected != reader.RecordsAffected)
                    throw new DbUpdateConcurrencyException(
                        RelationalStrings.UpdateConcurrencyException(1, 0),
                        ModificationCommands[commandIndex].Entries
                    );
            }

            return Task.FromResult(commandIndex);
        }

        protected override void Consume(DbDataReader reader)
        {
            Debug.Assert(CommandResultSet.Count == ModificationCommands.Count);
            var commandIndex = 0;

            try
            {
                var actualResultSetCount = 0;
                do
                {
                    while (commandIndex < CommandResultSet.Count
                           && CommandResultSet[commandIndex] == ResultSetMapping.NoResultSet)
                    {
                        commandIndex++;
                    }

                    if (commandIndex < CommandResultSet.Count)
                    {
                        commandIndex = ModificationCommands[commandIndex].RequiresResultPropagation
                            ? ConsumeResultSetWithPropagation(commandIndex, reader)
                            : ConsumeResultSetWithoutPropagation(commandIndex, reader);
                        actualResultSetCount++;
                    }
                }
                while (commandIndex < CommandResultSet.Count
                       && reader.NextResult());
            }
            catch (DbUpdateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException(
                    RelationalStrings.UpdateStoreException,
                    ex,
                    ModificationCommands[commandIndex].Entries);
            }
        }
    }
}
