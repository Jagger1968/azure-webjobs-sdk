﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Dashboard.ViewModels
{
    public class ParameterModel
    {
        // Static info
        public string Name { get; set; }
        public string Description { get; set; }

        // human-readably string version of runtime information.
        // Links provide optional runtime information for further linking to explore arg.
        public string ArgInvokeString { get; set; }

        public BlobBoundParameterModel ExtendedBlobModel { get; set; }

        // Runtime info. This can be structured to provide rich hyperlinks.
        public string Status { get; set; }
    }
}
