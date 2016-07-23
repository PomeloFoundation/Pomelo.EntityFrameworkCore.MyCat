// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Query.Sql.Internal;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MyCatDatabaseProviderServices : RelationalDatabaseProviderServices
    {
        public MyCatDatabaseProviderServices([NotNull] IServiceProvider services)
            : base(services)
        {
        }
        public override string InvariantName => GetType().GetTypeInfo().Assembly.GetName().Name;
        public override IBatchExecutor BatchExecutor => GetService<MyCatBatchExecutor>();
        public override IDatabaseCreator Creator => GetService<MyCatDatabaseCreator>();
        public override IRelationalConnection RelationalConnection => GetService<MyCatRelationalConnection>();
        public override ISqlGenerationHelper SqlGenerationHelper => GetService<MyCatSqlGenerationHelper>();
        public override IRelationalDatabaseCreator RelationalDatabaseCreator => GetService<MyCatDatabaseCreator>();
        public override IMigrationsAnnotationProvider MigrationsAnnotationProvider => GetService<MyCatMigrationsAnnotationProvider>();
        public override IHistoryRepository HistoryRepository => GetService<MyCatHistoryRepository>();
        public override IMigrationsSqlGenerator MigrationsSqlGenerator => GetService<MyCatMigrationsSqlGenerationHelper>();
        public override IModelSource ModelSource => GetService<MyCatModelSource>();
        public override IUpdateSqlGenerator UpdateSqlGenerator => GetService<MyCatUpdateSqlGenerator>();
        public override IValueGeneratorCache ValueGeneratorCache => GetService<MyCatValueGeneratorCache>();
        public override IRelationalTypeMapper TypeMapper => GetService<MyCatTypeMapper>();
        public override IConventionSetBuilder ConventionSetBuilder => GetService<MyCatConventionSetBuilder>();
        public override IModificationCommandBatchFactory ModificationCommandBatchFactory => GetService<MyCatModificationCommandBatchFactory>();
        public override IRelationalValueBufferFactoryFactory ValueBufferFactoryFactory => GetService<TypedRelationalValueBufferFactoryFactory>();
        public override IRelationalAnnotationProvider AnnotationProvider => GetService<MyCatAnnotationProvider>();
        public override IMethodCallTranslator CompositeMethodCallTranslator => GetService<MyCatCompositeMethodCallTranslator>();
        public override IMemberTranslator CompositeMemberTranslator => GetService<MyCatCompositeMemberTranslator>();
        public override IQueryCompilationContextFactory QueryCompilationContextFactory => GetService<MyCatQueryCompilationContextFactory>();
        public override IQuerySqlGeneratorFactory QuerySqlGeneratorFactory => GetService<MyCatQuerySqlGenerationHelperFactory>();
    }
}
