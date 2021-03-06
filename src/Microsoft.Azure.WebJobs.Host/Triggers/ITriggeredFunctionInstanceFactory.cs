﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Host.Executors;

namespace Microsoft.Azure.WebJobs.Host.Triggers
{
    internal interface ITriggeredFunctionInstanceFactory<TTriggerValue> : IFunctionInstanceFactory
    {
        IFunctionInstance Create(TTriggerValue value, Guid? parentId);
    }
}
