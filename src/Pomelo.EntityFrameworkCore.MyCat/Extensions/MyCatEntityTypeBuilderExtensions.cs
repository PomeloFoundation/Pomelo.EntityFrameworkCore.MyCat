// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore
{
    public static class MyCatEntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder ForMyCatToTable(
            [NotNull] this EntityTypeBuilder entityTypeBuilder,
            [CanBeNull] string name)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            var relationalEntityTypeBuilder = ((IInfrastructure<InternalEntityTypeBuilder>)entityTypeBuilder).GetInfrastructure()
                .MyCat(ConfigurationSource.Explicit);
            relationalEntityTypeBuilder.TableName = name;

            return entityTypeBuilder;
        }

        public static EntityTypeBuilder<TEntity> ForMyCatToTable<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder,
            [CanBeNull] string name)
            where TEntity : class
            => (EntityTypeBuilder<TEntity>)ForMyCatToTable((EntityTypeBuilder)entityTypeBuilder, name);

        public static EntityTypeBuilder ForMyCatToTable(
            [NotNull] this EntityTypeBuilder entityTypeBuilder,
            [CanBeNull] string name,
            [CanBeNull] string schema)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
            Check.NullButNotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            var relationalEntityTypeBuilder = ((IInfrastructure<InternalEntityTypeBuilder>)entityTypeBuilder).GetInfrastructure()
                .MyCat(ConfigurationSource.Explicit);
            relationalEntityTypeBuilder.TableName = name;
            relationalEntityTypeBuilder.Schema = schema;

            return entityTypeBuilder;
        }

        public static EntityTypeBuilder<TEntity> ForMyCatToTable<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder,
            [CanBeNull] string name,
            [CanBeNull] string schema)
            where TEntity : class
            => (EntityTypeBuilder<TEntity>)ForMyCatToTable((EntityTypeBuilder)entityTypeBuilder, name, schema);
    }
}
