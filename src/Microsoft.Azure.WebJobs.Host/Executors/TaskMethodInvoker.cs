﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Host.Executors
{
    internal class TaskMethodInvoker<TReflected> : IMethodInvoker<TReflected>
    {
        private readonly Func<TReflected, object[], Task> _lambda;

        public TaskMethodInvoker(Func<TReflected, object[], Task> lambda)
        {
            _lambda = lambda;
        }

        public Task InvokeAsync(TReflected instance, object[] arguments)
        {
            Task task = _lambda.Invoke(instance, arguments);
            ThrowIfWrappedTaskInstance(task);
            return task;
        }

        private static void ThrowIfWrappedTaskInstance(Task task)
        {
            if (task == null)
            {
                return;
            }

            Type taskType = task.GetType();
            Debug.Assert(taskType != null);

            Type innerTaskType = GetTaskInnerTypeOrNull(taskType);

            if (innerTaskType != null && typeof(Task).IsAssignableFrom(innerTaskType))
            {
                throw new InvalidOperationException("Returning a nested Task is not supported. Did you mean to await " +
                    "or Unwrap the task instead of returning it?");
            }
        }

        private static Type GetTaskInnerTypeOrNull(Type taskType)
        {
            Debug.Assert(taskType != null);

            // Fast path: check if type is exactly Task first.
            if (taskType == typeof(Task))
            {
                return null;
            }

            Debug.Assert(taskType.IsGenericType);
            Debug.Assert(!taskType.IsGenericTypeDefinition);
            Type genericTypeDefinition = taskType.GetGenericTypeDefinition();
            Debug.Assert(genericTypeDefinition == typeof(Task<>));
            return taskType.GetGenericArguments()[0];
        }
    }
}
