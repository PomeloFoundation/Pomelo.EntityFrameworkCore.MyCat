// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class MyCatModelAnnotations : RelationalModelAnnotations, IMyCatModelAnnotations
    {
        public MyCatModelAnnotations([NotNull] IModel model)
            : base(model, MyCatFullAnnotationNames.Instance)
        {
        }

        public MyCatModelAnnotations([NotNull] RelationalAnnotations annotations)
            : base(annotations, MyCatFullAnnotationNames.Instance)
        {
        }

        public virtual IMyCatExtension GetOrAddMyCatExtension([CanBeNull] string name, [CanBeNull] string schema = null)
            => MyCatExtension.GetOrAddMyCatExtension((IMutableModel)Model,
                MyCatFullAnnotationNames.Instance.MyCatExtensionPrefix,
                name,
                schema);

        public virtual IReadOnlyList<IMyCatExtension> MyCatExtensions
            => MyCatExtension.GetMyCatExtensions(Model, MyCatFullAnnotationNames.Instance.MyCatExtensionPrefix).ToList();

        public virtual string DatabaseTemplate
        {
            get { return (string)Annotations.GetAnnotation(MyCatFullAnnotationNames.Instance.DatabaseTemplate, null); }
            [param: CanBeNull]
            set { SetDatabaseTemplate(value); }
        }

        protected virtual bool SetDatabaseTemplate([CanBeNull] string value)
            => Annotations.SetAnnotation(
                MyCatFullAnnotationNames.Instance.DatabaseTemplate,
                null,
                Check.NullButNotEmpty(value, nameof(value)));
    }
}
