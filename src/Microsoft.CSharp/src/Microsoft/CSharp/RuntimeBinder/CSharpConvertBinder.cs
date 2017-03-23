// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Dynamic;

namespace Microsoft.CSharp.RuntimeBinder
{
    /// <summary>
    /// Represents a dynamic conversion in C#, providing the binding semantics and the details about the operation. 
    /// Instances of this class are generated by the C# compiler.
    /// </summary>
    internal sealed class CSharpConvertBinder : ConvertBinder, ICSharpBinder
    {
        CSharpArgumentInfo ICSharpBinder.GetArgumentInfo(int index) => CSharpArgumentInfo.None;

        internal CSharpConversionKind ConversionKind { get; }

        internal bool IsChecked { get; }

        internal Type CallingContext { get; }

        private readonly RuntimeBinder _binder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpConvertBinder" />.
        /// </summary>
        /// <param name="type">The type to convert to.</param>
        /// <param name="conversionKind">The kind of conversion for this operation.</param>
        /// <param name="isChecked">True if the operation is defined in a checked context; otherwise false.</param>        
        public CSharpConvertBinder(
            Type type,
            CSharpConversionKind conversionKind,
            bool isChecked,
            Type callingContext) :
            base(type, conversionKind == CSharpConversionKind.ExplicitConversion)
        {
            ConversionKind = conversionKind;
            IsChecked = isChecked;
            CallingContext = callingContext;
            _binder = RuntimeBinder.GetInstance();
        }

        /// <summary>
        /// Performs the binding of the dynamic convert operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic convert operation.</param>
        /// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public override DynamicMetaObject FallbackConvert(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
#if ENABLECOMBINDER 
            DynamicMetaObject com;
            if (!BinderHelper.IsWindowsRuntimeObject(target) && ComBinder.TryConvert(this, target, out com))
            {
                return com;
            }
#endif 
            return BinderHelper.Bind(this, _binder, new[] { target }, null, errorSuggestion);
        }
    }
}
