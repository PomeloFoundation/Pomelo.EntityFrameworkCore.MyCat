// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    public class MyCatCompositeMemberTranslator : RelationalCompositeMemberTranslator
    {
        public MyCatCompositeMemberTranslator()
        {
            var MyCatTranslators = new List<IMemberTranslator>
            {
                new MyCatStringLengthTranslator(),
                new MyCatDateTimeNowTranslator()
            };

            AddTranslators(MyCatTranslators);
        }
    }
}
