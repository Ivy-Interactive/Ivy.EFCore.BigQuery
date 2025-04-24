using Ivy.EFCore.BigQuery.Extensions;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace Ivy.EFCore.BigQuery.Scaffolding.Internal
{
    public class BigQueryConfigurationCodeGenerator : ProviderCodeGenerator
    {
        public BigQueryConfigurationCodeGenerator(ProviderCodeGeneratorDependencies dependencies) : base(dependencies) { }

        public override MethodCallCodeFragment GenerateUseProvider(string connectionString, MethodCallCodeFragment? providerOptions)
             => new(nameof(BigQueryDbContextOptionsExtensions.UseBigQuery),
                providerOptions == null
                    ? new object[] { connectionString }
                    : new object[] { connectionString, new NestedClosureCodeFragment("x", providerOptions) });
    }
}