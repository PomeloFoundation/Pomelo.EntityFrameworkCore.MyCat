// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore
{
    public static class MyCatMetadataExtensions
    {
        public static IRelationalEntityTypeAnnotations MyCat([NotNull] this IEntityType entityType)
               => new RelationalEntityTypeAnnotations(Check.NotNull(entityType, nameof(entityType)), MyCatFullAnnotationNames.Instance);

        public static RelationalEntityTypeAnnotations MyCat([NotNull] this IMutableEntityType entityType)
            => (RelationalEntityTypeAnnotations)MyCat((IEntityType)entityType);

        public static IRelationalForeignKeyAnnotations MyCat([NotNull] this IForeignKey foreignKey)
            => new RelationalForeignKeyAnnotations(Check.NotNull(foreignKey, nameof(foreignKey)), MyCatFullAnnotationNames.Instance);

        public static RelationalForeignKeyAnnotations MyCat([NotNull] this IMutableForeignKey foreignKey)
            => (RelationalForeignKeyAnnotations)MyCat((IForeignKey)foreignKey);

        public static IMyCatIndexAnnotations MyCat([NotNull] this IIndex index)
            => new MyCatIndexAnnotations(Check.NotNull(index, nameof(index)));

        public static RelationalIndexAnnotations MyCat([NotNull] this IMutableIndex index)
            => (MyCatIndexAnnotations)MyCat((IIndex)index);

        public static IRelationalKeyAnnotations MyCat([NotNull] this IKey key)
            => new RelationalKeyAnnotations(Check.NotNull(key, nameof(key)), MyCatFullAnnotationNames.Instance);

        public static RelationalKeyAnnotations MyCat([NotNull] this IMutableKey key)
            => (RelationalKeyAnnotations)MyCat((IKey)key);

        public static IMyCatModelAnnotations MyCat([NotNull] this IModel model)
            => new MyCatModelAnnotations(Check.NotNull(model, nameof(model)));

        public static MyCatModelAnnotations MyCat([NotNull] this IMutableModel model)
            => (MyCatModelAnnotations)MyCat((IModel)model);

        public static IRelationalPropertyAnnotations MyCat([NotNull] this IProperty property)
            => new RelationalPropertyAnnotations(Check.NotNull(property, nameof(property)), MyCatFullAnnotationNames.Instance);

        public static RelationalPropertyAnnotations MyCat([NotNull] this IMutableProperty property)
            => (RelationalPropertyAnnotations)MyCat((IProperty)property);

    }
}
