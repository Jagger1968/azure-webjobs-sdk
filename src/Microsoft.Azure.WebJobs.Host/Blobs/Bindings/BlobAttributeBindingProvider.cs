﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Converters;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Indexers;
using Microsoft.Azure.WebJobs.Host.Storage;
using Microsoft.Azure.WebJobs.Host.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.Azure.WebJobs.Host.Blobs.Bindings
{
    internal class BlobAttributeBindingProvider : IBindingProvider
    {
        private readonly INameResolver _nameResolver;
        private readonly IStorageAccountProvider _accountProvider;
        private readonly IBlobArgumentBindingProvider _provider;

        public BlobAttributeBindingProvider(INameResolver nameResolver, IStorageAccountProvider accountProvider,
            IExtensionTypeLocator extensionTypeLocator, IContextGetter<IBlobWrittenWatcher> blobWrittenWatcherGetter)
        {
            if (accountProvider == null)
            {
                throw new ArgumentNullException("accountProvider");
            }

            if (extensionTypeLocator == null)
            {
                throw new ArgumentNullException("extensionTypeLocator");
            }

            if (blobWrittenWatcherGetter == null)
            {
                throw new ArgumentNullException("blobWrittenWatcherGetter");
            }

            _nameResolver = nameResolver;
            _accountProvider = accountProvider;
            _provider = CreateProvider(extensionTypeLocator.GetCloudBlobStreamBinderTypes(), blobWrittenWatcherGetter);
        }

        private static IBlobArgumentBindingProvider CreateProvider(IEnumerable<Type> cloudBlobStreamBinderTypes,
            IContextGetter<IBlobWrittenWatcher> blobWrittenWatcherGetter)
        {
            List<IBlobArgumentBindingProvider> innerProviders = new List<IBlobArgumentBindingProvider>();

            innerProviders.Add(CreateConverterProvider<ICloudBlob, StorageBlobToCloudBlobConverter>());
            innerProviders.Add(CreateConverterProvider<CloudBlockBlob, StorageBlobToCloudBlockBlobConverter>());
            innerProviders.Add(CreateConverterProvider<CloudPageBlob, StorageBlobToCloudPageBlobConverter>());
            innerProviders.Add(new StreamArgumentBindingProvider(blobWrittenWatcherGetter));
            innerProviders.Add(new CloudBlobStreamArgumentBindingProvider(blobWrittenWatcherGetter));
            innerProviders.Add(new TextReaderArgumentBindingProvider());
            innerProviders.Add(new TextWriterArgumentBindingProvider(blobWrittenWatcherGetter));
            innerProviders.Add(new StringArgumentBindingProvider());
            innerProviders.Add(new OutStringArgumentBindingProvider(blobWrittenWatcherGetter));

            if (cloudBlobStreamBinderTypes != null)
            {
                innerProviders.AddRange(cloudBlobStreamBinderTypes.Select(
                    t => CloudBlobStreamObjectBinder.CreateReadBindingProvider(t)));

                innerProviders.AddRange(cloudBlobStreamBinderTypes.Select(
                    t => CloudBlobStreamObjectBinder.CreateWriteBindingProvider(t, blobWrittenWatcherGetter)));
            }

            return new CompositeArgumentBindingProvider(innerProviders);
        }

        private static IBlobArgumentBindingProvider CreateConverterProvider<TValue, TConverter>()
            where TConverter : IConverter<IStorageBlob, TValue>, new()
        {
            return new ConverterArgumentBindingProvider<TValue>(new TConverter());
        }

        public async Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            ParameterInfo parameter = context.Parameter;
            BlobAttribute blob = parameter.GetCustomAttribute<BlobAttribute>(inherit: false);

            if (blob == null)
            {
                return null;
            }

            string resolvedCombinedPath = Resolve(blob.BlobPath);
            IBindableBlobPath path = BindableBlobPath.Create(resolvedCombinedPath);
            path.ValidateContractCompatibility(context.BindingDataContract);

            IBlobArgumentBinding argumentBinding = _provider.TryCreate(parameter, blob.Access);

            if (argumentBinding == null)
            {
                throw new InvalidOperationException("Can't bind Blob to type '" + parameter.ParameterType + "'.");
            }

            IStorageAccount account = await _accountProvider.GetStorageAccountAsync(context.CancellationToken);
            IStorageBlobClient client = account.CreateBlobClient();
            IBinding binding = new BlobBinding(parameter.Name, argumentBinding, client, path);
            return binding;
        }

        private string Resolve(string queueName)
        {
            if (_nameResolver == null)
            {
                return queueName;
            }

            return _nameResolver.ResolveWholeString(queueName);
        }
    }
}
