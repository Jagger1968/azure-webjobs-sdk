﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Microsoft.Azure.WebJobs.Host.Executors
{
    internal interface IBindingSource
    {
        Task<IReadOnlyDictionary<string, IValueProvider>> BindAsync(ValueBindingContext context);
    }
}
