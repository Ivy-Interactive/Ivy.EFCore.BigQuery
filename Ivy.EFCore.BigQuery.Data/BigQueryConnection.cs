using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using System.Data.Common;
using System.Data;

namespace Ivy.EFCore.BigQuery.Data
{

    public class BigQueryConnection : DbConnection
    {
        private string _connectionString = string.Empty;
        private ConnectionState _state = ConnectionState.Closed;
        private BigQueryClient _client;
        private readonly Dictionary<string, string> _parsedConnectionString = new(StringComparer.OrdinalIgnoreCase);

        private string _dataSource;
        private string _projectId;
        private string _defaultDatasetId;
        private string _location;
        private string _credentialPath;
        private bool _useAdc;
        private int _connectionTimeoutSeconds = 15;
        private bool _isDisposed = false;

        public override event StateChangeEventHandler StateChange;

        public BigQueryConnection() { }

        public BigQueryConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets or sets the string used to open the connection.
        /// Format: "ProjectId=your-project;DefaultDataset=your_dataset;Location=US;AuthMethod=JsonCredentials;CredentialsFile=/path/to/key.json;Timeout=30"
        /// Or: "ProjectId=your-project;AuthMethod=ApplicationDefaultCredentials;"
        /// Supported Keys:
        /// - ProjectId (Required)
        /// - DefaultDataset (Optional)
        /// - Location (Optional): Hint for job location.
        /// - AuthMethod (Required): 'JsonCredentials' or 'ApplicationDefaultCredentials'.
        /// - CredentialsFile (Required if AuthMethod=JsonCredentials): Path to the JSON service account key file.
        /// - Timeout (Optional): Seconds to wait for connection/authentication (default 15).
        /// </summary>
        public override string ConnectionString
        {
            get => _connectionString;
            set
            {
                if (State != ConnectionState.Closed)
                {
                    throw new InvalidOperationException("Cannot change ConnectionString while the connection is open.");
                }
                _connectionString = value ?? string.Empty;
                ParseConnectionString();
            }
        }

        /// <summary>
        /// Default is 15 seconds.
        /// </summary>
        public override int ConnectionTimeout => _connectionTimeoutSeconds;

        public override string Database => _projectId ?? string.Empty;

        public override ConnectionState State => _state;

        public override string DataSource => _dataSource ?? string.Empty;

        public override string ServerVersion => typeof(BigQueryClient).Assembly.GetName().Version?.ToString() ?? "Google.Cloud.BigQuery.V2";

        internal BigQueryClient Client => _client;

        public string DefaultProjectId => _projectId;

        public string DefaultDatasetId => _defaultDatasetId;

        public string Location => _location;

        protected override DbProviderFactory DbProviderFactory => BigQueryProviderFactory.Instance;

        public override async Task OpenAsync(CancellationToken cancellationToken)
        {
            VerifyNotDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (State == ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection is already open.");
            }
            if (string.IsNullOrWhiteSpace(_projectId))
            {
                throw new InvalidOperationException("ProjectId must be specified in the connection string.");
            }

            var previousState = SetState(ConnectionState.Connecting);

            try
            {
                GoogleCredential credential = null;

                var authMethod = _parsedConnectionString.GetValueOrDefault("AuthMethod");
                if (string.Equals(authMethod, "JsonCredentials", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(_credentialPath))
                    {
                        throw new InvalidOperationException("CredentialsFile must be specified when AuthMethod is JsonCredentials.");
                    }
                    if (!File.Exists(_credentialPath))
                    {
                        throw new FileNotFoundException("Credentials JSON file not found.", _credentialPath);
                    }

                    await using var stream = new FileStream(_credentialPath, FileMode.Open, FileAccess.Read);

                    credential = await GoogleCredential.FromStreamAsync(stream, cancellationToken);
                }
                else if (string.Equals(authMethod, "ApplicationDefaultCredentials", StringComparison.OrdinalIgnoreCase) || _useAdc)
                {
                    credential = await GoogleCredential.GetApplicationDefaultAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    throw new InvalidOperationException("AuthMethod must be specified in the connection string (either 'JsonCredentials' or 'ApplicationDefaultCredentials').");
                }

                var clientBuilder = new BigQueryClientBuilder
                {
                    ProjectId = _projectId,
                    Credential = credential,
                };
                if (!string.IsNullOrWhiteSpace(_location))
                {
                    clientBuilder.DefaultLocation = _location;
                }
                if (!string.IsNullOrWhiteSpace(DataSource))
                {
                    clientBuilder.BaseUri = DataSource;
                }


                _client = await clientBuilder.BuildAsync(cancellationToken).ConfigureAwait(false);

                SetState(ConnectionState.Open);
            }
            catch (Exception ex)
            {
                _client = null;
                SetState(ConnectionState.Closed); 

                throw new BigQueryException($"Failed to open connection: {ex.Message}", ex);
            }
        }

        public override void Open()
        {
            try
            {
                OpenAsync(CancellationToken.None).GetAwaiter().GetResult();
            }
            catch (AggregateException ae) when (ae.InnerExceptions.Count == 1)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ae.InnerException).Throw();
                throw;
            }
        }

        public override void Close()
        {
            if (_state == ConnectionState.Closed)
            {
                return;
            }

            _client?.Dispose();
            _client = null;

            SetState(ConnectionState.Closed);
        }

        public override void ChangeDatabase(string databaseName)
        {
            if (State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Cannot change database on a closed connection.");
            }
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("Database name (Default Dataset) cannot be null or empty.", nameof(databaseName));
            }
            if (databaseName.Contains('.'))
            {
                throw new ArgumentException("Database name should only be the Dataset ID, not project.dataset.", nameof(databaseName));
            }

            _defaultDatasetId = databaseName;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotSupportedException("BigQuery does not support ADO.NET style transactions. Use multi-statement queries/scripts within a single command for atomic operations.");
        }

        protected override DbCommand CreateDbCommand()
        {
            var command = new BigQueryCommand();
            command.Connection = this;
            return command;
        }

        private void ParseConnectionString()
        {
            _parsedConnectionString.Clear();
            _projectId = null;
            _defaultDatasetId = null;
            _location = null;
            _credentialPath = null;
            _useAdc = false;
            _connectionTimeoutSeconds = 15; 

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                return;
            }

            var pairs = _connectionString.Split([';'], StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                var kv = pair.Split(['='], 2);
                if (kv.Length == 2)
                {
                    _parsedConnectionString[kv[0].Trim()] = kv[1].Trim();
                }
            }

            _projectId = _parsedConnectionString.GetValueOrDefault("ProjectId");
            _defaultDatasetId = _parsedConnectionString.GetValueOrDefault("DefaultDataset");
            _location = _parsedConnectionString.GetValueOrDefault("Location");
            _credentialPath = _parsedConnectionString.GetValueOrDefault("CredentialsFile");

            string authMethod = _parsedConnectionString.GetValueOrDefault("AuthMethod");
            _useAdc = string.Equals(authMethod, "ApplicationDefaultCredentials", StringComparison.OrdinalIgnoreCase);

            var dataSource = _parsedConnectionString.GetValueOrDefault("DataSource");
            if (!string.IsNullOrWhiteSpace(dataSource))
            {
                _dataSource = dataSource.Trim();
            }

            if (_parsedConnectionString.TryGetValue("Timeout", out var timeoutStr) && int.TryParse(timeoutStr, out var timeoutVal) && timeoutVal >= 0)
            {
                _connectionTimeoutSeconds = timeoutVal;
            }

            if (string.IsNullOrWhiteSpace(_projectId))
            {
                throw new ArgumentException("ProjectId must be specified in the connection string.");
            }

            if (!_useAdc && !string.Equals(authMethod, "JsonCredentials", StringComparison.OrdinalIgnoreCase))
            {
                //todo
                //GoogleCredential.FromJson()
            }
        }

        private ConnectionState SetState(ConnectionState newState)
        {
            if (_state == newState) return _state;

            var previousState = _state;
            _state = newState;
            OnStateChange(new StateChangeEventArgs(previousState, newState));
            return previousState;
        }

        protected virtual void OnStateChange(StateChangeEventArgs args)
        {
            StateChange?.Invoke(this, args);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (State != ConnectionState.Closed)
                {
                    Close();
                }
            }

            _isDisposed = true;
            base.Dispose(disposing);
        }

        private void VerifyNotDisposed()
        {
#if NET7_0_OR_GREATER
            ObjectDisposedException.ThrowIf(_isDisposed, this);
#else
		if (m_isDisposed)
			throw new ObjectDisposedException(GetType().Name);
#endif
        }
    }

    internal static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var value) ? value : default(TValue);
        }
    }
}

