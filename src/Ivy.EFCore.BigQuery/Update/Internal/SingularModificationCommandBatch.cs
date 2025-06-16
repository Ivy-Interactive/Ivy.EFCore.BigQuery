using Microsoft.EntityFrameworkCore.Update;

namespace Ivy.EFCore.BigQuery.Update.Internal
{
    public class SingularModificationCommandBatch : AffectedCountModificationCommandBatch
    {
        /// <summary>
        ///     Creates a new <see cref="SingularModificationCommandBatch" /> instance.
        /// </summary>
        /// <param name="dependencies">Service dependencies.</param>
        public SingularModificationCommandBatch(ModificationCommandBatchFactoryDependencies dependencies)
            : base(dependencies, maxBatchSize: 1)
        {
        }
    }
}