using Microsoft.EntityFrameworkCore.Storage;

namespace Ivy.EFCore.BigQuery.Storage.Internal
{
    //Todo implement this and add to service collection
    public class BigQueryDatabaseCreator : RelationalDatabaseCreator
    {
        public BigQueryDatabaseCreator(RelationalDatabaseCreatorDependencies dependencies) : base(dependencies)
        {
        }

        public override void Create()
        {
            throw new NotImplementedException();
        }

        public override void Delete()
        {
            throw new NotImplementedException();
        }

        public override bool Exists()
        {
            throw new NotImplementedException();
        }

        public override bool HasTables()
        {
            throw new NotImplementedException();
        }
    }
}
