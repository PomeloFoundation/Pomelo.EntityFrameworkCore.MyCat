// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class MyCatAnnotationProvider : IRelationalAnnotationProvider
    {
        public virtual IRelationalEntityTypeAnnotations For(IEntityType entityType) => entityType.MyCat();
        public virtual IRelationalForeignKeyAnnotations For(IForeignKey foreignKey) => foreignKey.MyCat();
        public virtual IRelationalIndexAnnotations For(IIndex index) => index.MyCat();
        public virtual IRelationalKeyAnnotations For(IKey key) => key.MyCat();
        public virtual IRelationalModelAnnotations For(IModel model) => model.MyCat();
        public virtual IRelationalPropertyAnnotations For(IProperty property) => property.MyCat();
    }
}
