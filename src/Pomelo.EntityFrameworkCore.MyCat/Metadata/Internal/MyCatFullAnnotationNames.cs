// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class MyCatFullAnnotationNames : RelationalFullAnnotationNames
    {
        protected MyCatFullAnnotationNames(string prefix)
            : base(prefix)
        {
            Serial = prefix + MyCatAnnotationNames.Serial;
            DefaultSequenceName = prefix + MyCatAnnotationNames.DefaultSequenceName;
            DefaultSequenceSchema = prefix + MyCatAnnotationNames.DefaultSequenceSchema;
            SequenceName = prefix + MyCatAnnotationNames.SequenceName;
            SequenceSchema = prefix + MyCatAnnotationNames.SequenceSchema;
            IndexMethod = prefix + MyCatAnnotationNames.IndexMethod;
            MyCatExtensionPrefix = prefix + MyCatAnnotationNames.MyCatExtensionPrefix;
            DatabaseTemplate = prefix + MyCatAnnotationNames.DatabaseTemplate;
            ValueGeneratedOnAdd = prefix + MyCatAnnotationNames.ValueGeneratedOnAdd;
            ValueGeneratedOnAddOrUpdate = prefix + MyCatAnnotationNames.ValueGeneratedOnAddOrUpdate;
        }

        public new static MyCatFullAnnotationNames Instance { get; } = new MyCatFullAnnotationNames(MyCatAnnotationNames.Prefix);

        public readonly string Serial;
        public readonly string DefaultSequenceName;
        public readonly string DefaultSequenceSchema;
        public readonly string SequenceName;
        public readonly string SequenceSchema;
        public readonly string IndexMethod;
        public readonly string MyCatExtensionPrefix;
        public readonly string DatabaseTemplate;
        public readonly string ValueGeneratedOnAdd;
        public readonly string ValueGeneratedOnAddOrUpdate;
    }
}