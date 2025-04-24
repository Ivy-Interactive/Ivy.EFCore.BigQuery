using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ivy.EFCore.BigQuery.Query.Internal
{
    internal class BigQueryMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
    {
        private readonly List<IMethodCallTranslator> _plugins = [];
        private readonly List<IMethodCallTranslator> _translators = [];

        public BigQueryMethodCallTranslatorProvider(RelationalMethodCallTranslatorProviderDependencies dependencies)
       : base(dependencies)
        { 
          //  _plugins.AddRange(_plugins.SelectMany(p => p.Translators));


        }

        public SqlExpression? Translate(
            IModel model,
            SqlExpression? instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
            => _plugins.Concat(_translators)
                .Select(t => t.Translate(instance, method, arguments, logger))
                .FirstOrDefault(t => t != null);
    }
}
