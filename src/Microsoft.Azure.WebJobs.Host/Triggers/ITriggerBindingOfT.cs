﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Microsoft.Azure.WebJobs.Host.Triggers
{
    /// <summary>
    /// Interface defining a trigger parameter binding.
    /// </summary>
    /// <typeparam name="TTriggerValue">The trigger value type that this binding binds to.</typeparam>
    public interface ITriggerBinding<TTriggerValue> : ITriggerBinding
    {
        /// <summary>
        /// Perform a bind to the specified trigger value using the specified binding context.
        /// </summary>
        /// <param name="value">The trigger value to bind to.</param>
        /// <param name="context">The binding context.</param>
        /// <returns>A task that returns the <see cref="ITriggerData"/> for the binding.</returns>
        Task<ITriggerData> BindAsync(TTriggerValue value, ValueBindingContext context);

        /// <summary>
        /// Create an <see cref="IListenerFactory"/> for the trigger parameter.
        /// </summary>
        /// <param name="descriptor">The <see cref="FunctionDescriptor"/> of the function to create
        /// a listener for.</param>
        /// <param name="executor">The <see cref="ITriggeredFunctionExecutor"/> that should be used
        /// to invoke the target job function when the trigger fires.</param>
        /// <returns>The <see cref="IListenerFactory"/></returns>
        IListenerFactory CreateListenerFactory(FunctionDescriptor descriptor, ITriggeredFunctionExecutor<TTriggerValue> executor);
    }
}
