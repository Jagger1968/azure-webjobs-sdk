﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Host.Converters;
using Microsoft.Azure.WebJobs.Host.Storage.Queue;

namespace Microsoft.Azure.WebJobs.Host.Queues.Bindings
{
    internal interface IMessageConverterFactory<TInput>
    {
        IConverter<TInput, IStorageQueueMessage> Create(IStorageQueue queue, Guid functionInstanceId);
    }
}
