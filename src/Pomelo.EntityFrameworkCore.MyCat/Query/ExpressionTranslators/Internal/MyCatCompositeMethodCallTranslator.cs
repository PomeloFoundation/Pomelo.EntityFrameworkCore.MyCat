// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    public class MyCatCompositeMethodCallTranslator : RelationalCompositeMethodCallTranslator
    {
        private static readonly IMethodCallTranslator[] _methodCallTranslators =
        {
            new MyCatStringSubstringTranslator(),
            new MyCatMathAbsTranslator(),
            new MyCatMathCeilingTranslator(),
            new MyCatMathFloorTranslator(),
            new MyCatMathPowerTranslator(),
            new MyCatMathRoundTranslator(),
            new MyCatMathTruncateTranslator(),
            new MyCatStringReplaceTranslator(),
            new MyCatStringToLowerTranslator(),
            new MyCatStringToUpperTranslator(),
            new MyCatRegexIsMatchTranslator(),
        };

        public MyCatCompositeMethodCallTranslator([NotNull] ILogger<MyCatCompositeMethodCallTranslator> logger)
            : base(logger)
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            AddTranslators(_methodCallTranslators);
        }
    }
}
