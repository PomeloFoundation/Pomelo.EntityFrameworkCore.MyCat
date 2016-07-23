// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    public class MyCatMathAbsTranslator : MultipleOverloadStaticMethodCallTranslator
    {
        public MyCatMathAbsTranslator()
            : base(typeof(Math), "Abs", "ABS")
        {
        }
    }
}
