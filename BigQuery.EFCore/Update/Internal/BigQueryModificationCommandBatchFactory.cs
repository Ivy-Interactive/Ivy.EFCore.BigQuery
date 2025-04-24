using Microsoft.EntityFrameworkCore.Update;

namespace Ivy.EFCore.BigQuery.Update.Internal
{
    internal class BigQueryModificationCommandBatchFactory : IModificationCommandBatchFactory
    {
        protected virtual ModificationCommandBatchFactoryDependencies Dependencies { get; }

        public BigQueryModificationCommandBatchFactory(
        ModificationCommandBatchFactoryDependencies dependencies) 
            => Dependencies = dependencies;

        public virtual ModificationCommandBatch Create() 
            => new SingularModificationCommandBatch(Dependencies);

    }
}
