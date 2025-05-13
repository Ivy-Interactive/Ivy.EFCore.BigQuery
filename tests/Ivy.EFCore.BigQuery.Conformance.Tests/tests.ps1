[CmdletBinding()]
param(
    [Parameter()]
    [switch]$Dry,
    [Parameter()]
    [string]$Filter
)
#region Config
$BatchSize = 100
$DockerComposeFile = "./docker/docker-compose.yml"
#$TestListOutputFile = "tests.txt"
$originalLocation = Get-Location
$dockerComposeFullPath = Join-Path (Split-Path $MyInvocation.MyCommand.Definition) $DockerComposeFile
$dockerComposeDir = Split-Path $dockerComposeFullPath
#endregion

#region Functions
function LogInfo {
    param([string]$Message)
    Write-Information -MessageData $Message -InformationAction Continue
}

function Get-TestListOutput() {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [string]$FolderPath
    )

    Set-Location -Path $FolderPath

    $commandResult = & dotnet test -t 2>&1
    $testOutput = @()
    
    foreach ($line in $commandResult ) {
        $testOutput += $line        
    }
    
    return $testOutput
}

function Format-TestListOutput() {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        [string[]]$TestOutput,
        [Parameter(Mandatory = $false)]
        [AllowEmptyString()]
        [string]$FilterString
    )
    $tests = @()
    $inTestListSection = $false

    foreach ($line in $TestOutput) {
        $trimmedLine = $line.Trim()

        if ($trimmedLine -eq "The following Tests are available:") {
            LogInfo "Filtering: Found start marker."
            $inTestListSection = $true
            continue 
        }

        if ($inTestListSection) {
            if ($line -match 'Test execution completed' -or
                $line -match 'A total of' -or
                $line -match 'Failed:' -or
                $line -match 'Passed:' -or
                $line -match 'Skipped:') {
                LogInfo "Filtering: Found end marker or summary line: '$trimmedLine'. Exiting test list section."
                $inTestListSection = $false
                continue
            }

            if ([string]::IsNullOrWhiteSpace($line) -or
                $line -match 'Starting test execution' -or
                $line -match 'Warning:' -or
                $line -match 'Error:') {
                continue
            }

            if (-not [string]::IsNullOrEmpty($line) -and $line -cnotlike "*$FilterString*") {
                continue
            }

            if ($line -match '^\s*\S+\.\S+') {

                $tests += $trimmedLine
            }
            else {
                # LogInfo "Filtered: '$trimmedLine'"
            }
        }
    }
    $tests = $tests | Select-Object -Unique | Sort-Object
    LogInfo "Found $($tests.Count) tests"
    return $tests
}

function Start-DockerContainer() {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [string]$ComposeDir,
        [Parameter(Mandatory = $false)]
        [int]$TimeoutSeconds = 30
    )

    LogInfo "Starting Docker container"

    try {
        Set-Location $ComposeDir
        $dockerUpOutput = docker-compose up -d 2>&1
        if ($LASTEXITCODE -ne 0) {
            LogInfo "Error starting Docker container. Exit code: $LASTEXITCODE"
            $dockerUpOutput | Write-Error
            throw "Docker compose up failed"
        }
        LogInfo "Docker container started. Waiting for readiness"

        $serviceName = "bq-emulator"
        $expectedLog = "REST server listening at"
        $elapsed = 0
        $isReady = $false
        $sleepSeconds = 1

        while ($elapsed -lt $TimeoutSeconds -and -not $isReady) {
            $logs = docker-compose logs --no-color $serviceName 2>&1

            if ($logs -match [regex]::Escape($expectedLog)) {
                $isReady = $true
                LogInfo "Container '$serviceName' is ready"
                break
            }

            Start-Sleep -Seconds $sleepSeconds
            $elapsed += $sleepSeconds
            LogInfo "Waiting for container readiness... ($elapsed/$TimeoutSeconds seconds)"
        }

        if (-not $isReady) {
            throw "Timeout reached. Container '$serviceName' did not report readiness within $TimeoutSeconds seconds."
        }
    }
    finally {
        Set-Location $originalLocation
    }
}

function Invoke-Tests() {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [string]$Dir,
        [Parameter(Mandatory = $true)]
        [string[]]$Tests,
        [Parameter(Mandatory = $true)]
        [int]$BatchNumber,        
        [Parameter(Mandatory = $true)]
        [bool]$ExitOnBatchFail
    )

    $currentBatch = $Tests[1..200]
    $filterString = ($currentBatch | ForEach-Object { "FullyQualifiedName=$_" }) -join "|"
    $quotedFilterString = "`"$filterString`""    

    Start-DockerContainer -ComposeDir $dockerComposeDir

    LogInfo "Starting dotnet test"

    $dotnetArgs = @("test", "--filter", $quotedFilterString, "--logger", "html")
    #$testRunOutput = & dotnet $dotnetArgs 2>&1 | Tee-Object -Variable testRunOutput

    & dotnet $dotnetArgs 2>&1 | Tee-Object -Variable testRunOutput

    if ($LASTEXITCODE -ne 0) {
        LogInfo "Warning: 'dotnet test' failed for batch $batchNumber. Exit code: $LASTEXITCODE"
        $testRunOutput
        if ($ExitOnBatchFail) {
            exit
        }
    }
    else {
        LogInfo "Tests for batch $batchNumber completed successfully."
    }
}
#endregion

#region Main execution

$testOutput = Get-TestListOutput $originalLocation
$tests = Format-TestListOutput -TestOutput $testOutput -FilterString $Filter
$totalTests = $tests.Count
$numberOfBatches = [Math]::Ceiling($totalTests / $BatchSize)
LogInfo "Starting batched test execution. Filter: $Filter Batch size: $BatchSize. Total batches: $numberOfBatches"

for ($i = 0; $i -lt $totalTests; $i += $BatchSize) {
    $batchNumber = ($i / $BatchSize) + 1     
    try {
        Invoke-Tests -dir $originalLocation -Tests $tests -BatchNumber $batchNumber -ExitOnBatchFail $true
    }    
    finally {
        try {
            Set-Location -Path $dockerComposeDir
            $dockerDownOutput = docker-compose down 2>&1
            if ($LASTEXITCODE -ne 0) {
                LogInfo "Warning: 'docker-compose down' failed for batch $batchNumber. Exit code: $LASTEXITCODE"
                $dockerDownOutput | Write-Warning
            }
            else {
                LogInfo "Docker containers stopped and removed successfully."
            }
        }
        finally {
            Set-Location -Path $originalLocation
        }
    }
}

#endregion