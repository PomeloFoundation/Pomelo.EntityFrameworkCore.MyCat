// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    public class MyCatDbContextOptionsBuilder : RelationalDbContextOptionsBuilder<MyCatDbContextOptionsBuilder, MyCatOptionsExtension>
    {
        public MyCatDbContextOptionsBuilder([NotNull] DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder)
        {
        }

        protected override MyCatOptionsExtension CloneExtension()
            => new MyCatOptionsExtension(OptionsBuilder.Options.GetExtension<MyCatOptionsExtension>());
    }
}
