// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using JetBrains.Annotations;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;

//namespace Microsoft.EntityFrameworkCore.Metadata
//{
//    public class MyCatPropertyAnnotations : RelationalPropertyAnnotations, IMyCatPropertyAnnotations
//    {
//        public MyCatPropertyAnnotations([NotNull] IProperty property, [CanBeNull] string providerPrefix) : base(property, MyCatFullAnnotationNames.Instance)
//        {
//        }

//        public MyCatPropertyAnnotations([NotNull] RelationalAnnotations annotations) : base(annotations, MyCatFullAnnotationNames.Instance)
//        {
//        }

//        public virtual MyCatValueGenerationStrategy? ValueGenerationStrategy
//        {
//            get
//            {
//                if ((Property.ValueGenerated != ValueGenerated.OnAdd)
//                    || !Property.ClrType.UnwrapNullableType().IsInteger()
//                    || (Property.MyCat().GeneratedValueSql != null))
//                {
//                    return null;
//                }

//                var value = (MyCatValueGenerationStrategy?)Annotations.GetAnnotation(MyCatAnnotationNames.ValueGenerationStrategy);

//                return value ?? Property.DeclaringEntityType.Model.MyCat().ValueGenerationStrategy;
//            }
//            [param: CanBeNull]
//            set { SetValueGenerationStrategy(value); }
//        }

//        protected virtual bool SetValueGenerationStrategy(MyCatValueGenerationStrategy? value)
//        {
//            if (value != null)
//            {
//                var propertyType = Property.ClrType;

//                if ((value == MyCatValueGenerationStrategy.AutoIncrement)
//                    && (!propertyType.IsInteger()
//                        || (propertyType == typeof(byte))
//                        || (propertyType == typeof(byte?))))
//                {
//                    throw new ArgumentException("Bad identity type");
//                    //Property.Name, Property.DeclaringEntityType.Name, propertyType.Name));
//                }
//            }

//            return Annotations.SetAnnotation(MyCatAnnotationNames.ValueGenerationStrategy, value);
//        }
//    }
//}
