// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.Sql.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MyCatEntityFrameworkServicesBuilderExtensions
    {
        public static IServiceCollection AddEntityFrameworkMyCat([NotNull] this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            services.AddRelational();

            services.TryAddEnumerable(ServiceDescriptor
                .Singleton<IDatabaseProvider, DatabaseProvider<MyCatDatabaseProviderServices, MyCatOptionsExtension>>());

            services.TryAdd(new ServiceCollection()
                .AddSingleton<MyCatValueGeneratorCache>()
                .AddSingleton<MyCatTypeMapper>()
                .AddSingleton<MyCatSqlGenerationHelper>()
                .AddSingleton<MyCatModelSource>()
                .AddSingleton<MyCatAnnotationProvider>()
                .AddSingleton<MyCatMigrationsAnnotationProvider>()
                .AddScoped<MyCatBatchExecutor>()
                .AddScoped(p => GetProviderServices(p).BatchExecutor)
                .AddScoped<MyCatConventionSetBuilder>()
                .AddScoped<TableNameFromDbSetConvention>()
                .AddScoped<IMyCatUpdateSqlGenerator, MyCatUpdateSqlGenerator>()
                .AddScoped<MyCatModificationCommandBatchFactory>()
                .AddScoped<MyCatValueGeneratorSelector>()
                .AddScoped<MyCatDatabaseProviderServices>()
                .AddScoped<MyCatRelationalConnection>()
                .AddScoped<MyCatDatabaseCreator>()
                .AddScoped<MyCatHistoryRepository>()
                .AddScoped<MyCatMigrationsSqlGenerationHelper>()
                .AddScoped<MyCatModificationCommandBatchFactory>()
                .AddQuery());

            services
                .AddScoped<IChangeDetector, MyCatChangeDetector>()
                .AddScoped<IPropertyListener, IChangeDetector>(p => p.GetService<IChangeDetector>());

            return services;
        }

        private static IServiceCollection AddQuery(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<MyCatQueryCompilationContextFactory>()
                .AddScoped<MyCatCompositeMemberTranslator>()
                .AddScoped<MyCatCompositeMethodCallTranslator>()
                .AddScoped<MyCatQuerySqlGenerationHelperFactory>();
        }

        private static IRelationalDatabaseProviderServices GetProviderServices(IServiceProvider serviceProvider)
        {
            var providerServices = serviceProvider.GetRequiredService<IDbContextServices>().DatabaseProviderServices
                as IRelationalDatabaseProviderServices;

            if (providerServices == null)
            {
                throw new InvalidOperationException(RelationalStrings.RelationalNotInUse);
            }

            return providerServices;
        }
    }
}
