using Microsoft.EntityFrameworkCore.Storage;

namespace Ivy.EFCore.BigQuery.Storage.Internal;

public interface IBigQueryRelationalConnection : IRelationalConnection
{
    IBigQueryRelationalConnection CreateMasterConnection();
}