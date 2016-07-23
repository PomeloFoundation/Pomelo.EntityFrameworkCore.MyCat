// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public static class MyCatInternalMetadataBuilderExtensions
    {
        public static RelationalModelBuilderAnnotations MyCat(
            [NotNull] this InternalModelBuilder builder,
            ConfigurationSource configurationSource)
            => new RelationalModelBuilderAnnotations(builder, configurationSource, MyCatFullAnnotationNames.Instance);

        public static RelationalPropertyBuilderAnnotations MyCat(
            [NotNull] this InternalPropertyBuilder builder,
            ConfigurationSource configurationSource)
            => new RelationalPropertyBuilderAnnotations(builder, configurationSource, MyCatFullAnnotationNames.Instance);

        public static RelationalEntityTypeBuilderAnnotations MyCat(
            [NotNull] this InternalEntityTypeBuilder builder,
            ConfigurationSource configurationSource)
            => new RelationalEntityTypeBuilderAnnotations(builder, configurationSource, MyCatFullAnnotationNames.Instance);

        public static RelationalKeyBuilderAnnotations MyCat(
            [NotNull] this InternalKeyBuilder builder,
            ConfigurationSource configurationSource)
            => new RelationalKeyBuilderAnnotations(builder, configurationSource, MyCatFullAnnotationNames.Instance);

        public static RelationalIndexBuilderAnnotations MyCat(
            [NotNull] this InternalIndexBuilder builder,
            ConfigurationSource configurationSource)
            => new RelationalIndexBuilderAnnotations(builder, configurationSource, MyCatFullAnnotationNames.Instance);

        public static RelationalForeignKeyBuilderAnnotations MyCat(
            [NotNull] this InternalRelationshipBuilder builder,
            ConfigurationSource configurationSource)
            => new RelationalForeignKeyBuilderAnnotations(builder, configurationSource, MyCatFullAnnotationNames.Instance);
    }
}