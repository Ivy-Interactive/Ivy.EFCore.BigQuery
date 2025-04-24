using Ivy.EFCore.BigQuery.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace Ivy.EFCore.BigQuery.Storage.Internal
{
    internal sealed class BigQueryDatabase : Database
    {
        private readonly string _projectId;
        private readonly string _datasetId;
        private readonly IBigQueryClientWrapper _client;

        public BigQueryDatabase(
            DatabaseDependencies dependencies,
            //string projectId,
            //string datasetId,
            //string? credentialsPath = null) : base(dependencies)
            IBigQueryClientWrapper client) : base(dependencies)
        {
            //_projectId = projectId;
            //_datasetId = datasetId;

            // Initialize BigQuery client
            //if (credentialsPath != null)
            //{
            //    var credential = GoogleCredential.FromFile(credentialsPath);
            //    _client = IBigQueryClientWrapper.Create(_projectId, credential);
            //}
            //else
            //{
            //    _client = IBigQueryClientWrapper.Create(_projectId);
            //}
            _client = client;
        }

        //public override Func<QueryContext, TResult> CompileQuery<TResult>(Expression query, bool async)
        //{
        //    return base.CompileQuery<TResult>(query, async);
        //}

        public override int SaveChanges(IList<IUpdateEntry> entries)
        {
            throw new NotImplementedException();
        }

        public override async Task<int> SaveChangesAsync(IList<IUpdateEntry> entries, CancellationToken cancellationToken = default)
        {
            var rowsAffected = 0;
            var entriesSaved = new HashSet<IUpdateEntry>();
            var rootEntriesToSave = new HashSet<IUpdateEntry>();

            foreach (var entry in entries)
            {              
                var entityType = entry.EntityType;
            }

            return rowsAffected;
        }
    }
}

