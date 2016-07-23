// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class MyCatExtension : IMyCatExtension
    {
        readonly IModel _model;
        readonly string _annotationName;

        MyCatExtension(
            [NotNull] IMutableModel model,
            [NotNull] string annotationPrefix,
            [NotNull] string name,
            [CanBeNull] string schema = null)
            : this(model, BuildAnnotationName(annotationPrefix, name, schema))
        {
            Check.NotNull(model, nameof(model));
            Check.NotEmpty(annotationPrefix, nameof(annotationPrefix));
            Check.NotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            SetData(new MyCatExtensionData
            {
                Name = name,
                Schema = schema
            });
        }

        MyCatExtension(IModel model, string annotationName)
        {
            _model = model;
            _annotationName = annotationName;
        }

        public static MyCatExtension GetOrAddMyCatExtension(
            [NotNull] IMutableModel model,
            [NotNull] string annotationPrefix,
            [NotNull] string name,
            [CanBeNull] string schema = null)
            => FindMyCatExtension(model, annotationPrefix, name, schema) ?? new MyCatExtension(model, annotationPrefix, name, schema);

        public static MyCatExtension FindMyCatExtension(
            [NotNull] IMutableModel model,
            [NotNull] string annotationPrefix,
            [NotNull] string name,
            [CanBeNull] string schema = null)
            => (MyCatExtension)FindMyCatExtension((IModel)model, annotationPrefix, name, schema);

        public static IMyCatExtension FindMyCatExtension(
            [NotNull] IModel model,
            [NotNull] string annotationPrefix,
            [NotNull] string name,
            [CanBeNull] string schema = null)
        {
            Check.NotNull(model, nameof(model));
            Check.NotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            var annotationName = BuildAnnotationName(annotationPrefix, name, schema);

            return model[annotationName] == null ? null : new MyCatExtension(model, annotationName);
        }

        static string BuildAnnotationName(string annotationPrefix, string name, string schema)
            => annotationPrefix + schema + "." + name;

        public static IEnumerable<IMyCatExtension> GetMyCatExtensions([NotNull] IModel model, [NotNull] string annotationPrefix)
        {
            Check.NotNull(model, nameof(model));
            Check.NotEmpty(annotationPrefix, nameof(annotationPrefix));

            return model.GetAnnotations()
                .Where(a => a.Name.StartsWith(annotationPrefix, StringComparison.Ordinal))
                .Select(a => new MyCatExtension(model, a.Name));
        }

        public virtual Model Model => (Model)_model;

        public virtual string Name => GetData().Name;

        public virtual string Schema => GetData().Schema ?? Model.Relational().DefaultSchema;

        public virtual string Version
        {
            get { return GetData().Version; }
            set
            {
                var data = GetData();
                data.Version = value;
                SetData(data);
            }
        }

        MyCatExtensionData GetData() => MyCatExtensionData.Deserialize((string)Model[_annotationName]);

        void SetData(MyCatExtensionData data)
        {
            Model[_annotationName] = data.Serialize();
        }

        IModel IMyCatExtension.Model => _model;

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Schema != null)
            {
                sb.Append(Schema);
                sb.Append('.');
            }
            sb.Append(Name);
            if (Version != null)
            {
                sb.Append("' (");
                sb.Append(Version);
                sb.Append(')');
            }
            return sb.ToString();
        }

        class MyCatExtensionData
        {
            public string Name { get; set; }
            public string Schema { get; set; }
            public string Version { get; set; }

            public string Serialize()
            {
                var builder = new StringBuilder();

                EscapeAndQuote(builder, Name);
                builder.Append(", ");
                EscapeAndQuote(builder, Schema);
                builder.Append(", ");
                EscapeAndQuote(builder, Version);

                return builder.ToString();
            }

            public static MyCatExtensionData Deserialize([NotNull] string value)
            {
                Check.NotEmpty(value, nameof(value));

                try
                {
                    var data = new MyCatExtensionData();

                    // ReSharper disable PossibleInvalidOperationException
                    var position = 0;
                    data.Name = ExtractValue(value, ref position);
                    data.Schema = ExtractValue(value, ref position);
                    data.Version = ExtractValue(value, ref position);
                    // ReSharper restore PossibleInvalidOperationException

                    return data;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(RelationalStrings.BadSequenceString, ex);
                }
            }

            private static string ExtractValue(string value, ref int position)
            {
                position = value.IndexOf('\'', position) + 1;

                var end = value.IndexOf('\'', position);

                while ((end + 1 < value.Length)
                       && (value[end + 1] == '\''))
                {
                    end = value.IndexOf('\'', end + 2);
                }

                var extracted = value.Substring(position, end - position).Replace("''", "'");
                position = end + 1;

                return extracted.Length == 0 ? null : extracted;
            }

            private static void EscapeAndQuote(StringBuilder builder, object value)
            {
                builder.Append("'");

                if (value != null)
                {
                    builder.Append(value.ToString().Replace("'", "''"));
                }

                builder.Append("'");
            }
        }
    }
}