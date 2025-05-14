# Ivy.EFCore.BigQuery

## Running Tests

The fastest way is to run the powershell script (results saved in TestResults folder):

```powershell .\tests\Ivy.EFCore.BigQuery.Data.Conformance.Tests\tests.ps1```

Run the BQ emulator and start tests from within VS:

```docker compose -f ".\tests\Ivy.EFCore.BigQuery.Conformance.Tests\docker\docker-compose.yml" up -d```

Run with your own BigQuery project by setting `BQ_ADO_CONN_STRING` environment variable to your connection string. Create a `ado_tests` dataset with `select_value` table in your project.

Connection string format:

```AuthMethod=ApplicationDefaultCredentials;ProjectId=your-project-id;DefaultDatasetId=ado_tests```
