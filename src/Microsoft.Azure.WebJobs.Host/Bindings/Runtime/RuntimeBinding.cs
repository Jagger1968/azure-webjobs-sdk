﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Microsoft.Azure.WebJobs.Host.Bindings.Runtime
{
    internal class RuntimeBinding : IBinding
    {
        private readonly string _parameterName;
        private readonly IContextGetter<IBindingProvider> _bindingProviderGetter;

        public RuntimeBinding(string parameterName, IContextGetter<IBindingProvider> bindingProviderGetter)
        {
            if (bindingProviderGetter == null)
            {
                throw new ArgumentNullException("bindingProviderGetter");
            }

            _parameterName = parameterName;
            _bindingProviderGetter = bindingProviderGetter;
        }

        public bool FromAttribute
        {
            get { return false; }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "context")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private Task<IValueProvider> BindAsync(IAttributeBindingSource binding, ValueBindingContext context)
        {
            IValueProvider provider = new RuntimeValueProvider(binding);
            return Task.FromResult(provider);
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            IAttributeBindingSource binding = value as IAttributeBindingSource;

            if (binding == null)
            {
                throw new InvalidOperationException("Unable to convert value to IAttributeBindingSource.");
            }

            return BindAsync(binding, context);
        }

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            AttributeBindingSource bindingSource = new AttributeBindingSource(_bindingProviderGetter.Value, context.AmbientContext);

            return BindAsync(bindingSource, context.ValueContext);
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new BinderParameterDescriptor
            {
                Name = _parameterName
            };
        }
    }
}
