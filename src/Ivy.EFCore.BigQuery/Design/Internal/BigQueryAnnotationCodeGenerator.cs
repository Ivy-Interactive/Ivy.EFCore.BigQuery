using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivy.EFCore.BigQuery.Design.Internal
{
    internal class BigQueryAnnotationCodeGenerator : AnnotationCodeGenerator
    {
        public BigQueryAnnotationCodeGenerator(AnnotationCodeGeneratorDependencies dependencies)
            : base(dependencies)
        {
            
        }
    }
}