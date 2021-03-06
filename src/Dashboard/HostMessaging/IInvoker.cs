﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dashboard.Data;
using Microsoft.Azure.WebJobs.Protocols;

namespace Dashboard.HostMessaging
{
    public interface IInvoker
    {
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Function")]
        Guid TriggerAndOverride(string queueName, FunctionSnapshot function, IDictionary<string, string> arguments, Guid? parentId, ExecutionReason reason);
    }
}
