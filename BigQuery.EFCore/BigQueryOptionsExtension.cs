using Ivy.EFCore.BigQuery.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Ivy.EFCore.BigQuery
{
    public class BigQueryOptionsExtension : RelationalOptionsExtension
    {
        public string ProjectId { get; }
        public string DatasetId { get; }
        public string CredentialsPath { get; }
        private DbContextOptionsExtensionInfo _info;

        public BigQueryOptionsExtension()
        {
            
        }

        public BigQueryOptionsExtension(string projectId, string datasetId)
        //, string credentialsPath)
        {
            ProjectId = projectId;
            DatasetId = datasetId;
            //CredentialsPath = credentialsPath;
        }

        protected BigQueryOptionsExtension(BigQueryOptionsExtension copyFrom) : base(copyFrom)
        {
            ProjectId = copyFrom.ProjectId;
            DatasetId = copyFrom.DatasetId;
        }

        // public void ApplyServices(IServiceCollection services) { }

        public void Validate(IDbContextOptions options) { }

        protected override RelationalOptionsExtension Clone()
        => new BigQueryOptionsExtension(this);

        public override void ApplyServices(IServiceCollection services)
        {
            services.AddEntityFrameworkBigQuery();
        }

        //public DbContextOptionsExtensionInfo Info => new BigQueryOptionsExtensionInfo(this);
        public override DbContextOptionsExtensionInfo Info =>  _info ??= new BigQueryOptionsExtensionInfo(this);

        //public override DbContextOptionsExtensionInfo Info => throw new NotImplementedException();

        private class BigQueryOptionsExtensionInfo : DbContextOptionsExtensionInfo
        {
            public BigQueryOptionsExtensionInfo(IDbContextOptionsExtension extension) : base(extension) { }
            public override bool IsDatabaseProvider => true;
            public override string LogFragment => "BigQuery Provider";

            public override int GetServiceProviderHashCode() => 0;

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                debugInfo["BigQuery:ConnectionString"] =
                    ((BigQueryOptionsExtension)Extension).ConnectionString;
            }

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            {
                return other.Extension is BigQueryOptionsExtension ext && Extension.Equals(ext);
            }
        }


    }





}
