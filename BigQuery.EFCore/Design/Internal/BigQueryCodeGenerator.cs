using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ivy.EFCore.BigQuery.Design.Internal
{
    //public class BigQueryCodeGenerator(ProviderCodeGeneratorDependencies dependencies) : ProviderCodeGenerator(dependencies)
    //{
    //    private static readonly MethodInfo _useBigQueryMethodInfo
    //        = typeof(BigQueryOptionsExtension).GetRuntimeMethod(
    //            nameof(BigQueryOptionsExtension.UseBigQuery),
    //            [typeof(DbContextOptionsBuilder), typeof(string), typeof(Action<BigQueryDbContextOptionsBuilder>)])!;

    //    public override MethodCallCodeFragment GenerateUseProvider(string connectionString, MethodCallCodeFragment? providerOptions)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
