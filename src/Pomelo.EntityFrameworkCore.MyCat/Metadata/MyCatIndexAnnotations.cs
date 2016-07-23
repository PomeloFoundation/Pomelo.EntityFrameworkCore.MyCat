// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class MyCatIndexAnnotations : RelationalIndexAnnotations, IMyCatIndexAnnotations
    {
        public MyCatIndexAnnotations([NotNull] IIndex index)
            : base(index, MyCatFullAnnotationNames.Instance)
        {
        }

        protected MyCatIndexAnnotations([NotNull] RelationalAnnotations annotations)
            : base(annotations, MyCatFullAnnotationNames.Instance)
        {
        }

        public string Method
        {
            get { return (string)Annotations.GetAnnotation(MyCatFullAnnotationNames.Instance.IndexMethod, null); }
            set { Annotations.SetAnnotation(MyCatFullAnnotationNames.Instance.IndexMethod, null, value); }
        }
    }
}
