# PowerShell script to check the database using SQL connection
$serverName = "(LocalDb)\MSSQLLocalDB"
$databaseName = "CalwayPest"

try {
    # Load SQL Server assemblies
    Add-Type -AssemblyName "System.Data"
    
    # Create connection
    $connectionString = "Server=$serverName;Database=$databaseName;Integrated Security=True;TrustServerCertificate=True"
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    
    Write-Host "Connecting to database: $databaseName on $serverName" -ForegroundColor Cyan
    $connection.Open()
    
    # Query ContactSubmissions table
    $query = "SELECT TOP 20 Id, FullName, Email, LEFT(Message, 50) AS MessagePreview, CreationTime FROM AppContactSubmissions ORDER BY CreationTime DESC"
    $command = New-Object System.Data.SqlClient.SqlCommand($query, $connection)
    
    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
    $dataset = New-Object System.Data.DataSet
    $adapter.Fill($dataset) | Out-Null
    
    if ($dataset.Tables[0].Rows.Count -eq 0) {
        Write-Host "`nNo contact submissions found in the database." -ForegroundColor Yellow
    } else {
        Write-Host "`nFound $($dataset.Tables[0].Rows.Count) contact submissions:" -ForegroundColor Green
        Write-Host "================================================================" -ForegroundColor Green
        
        $dataset.Tables[0] | Format-Table -AutoSize
    }
    
    # Get total count
    $countQuery = "SELECT COUNT(*) FROM AppContactSubmissions"
    $countCommand = New-Object System.Data.SqlClient.SqlCommand($countQuery, $connection)
    $totalCount = $countCommand.ExecuteScalar()
    
    Write-Host "`nTotal contact submissions in database: $totalCount" -ForegroundColor Cyan
    
    $connection.Close()
    Write-Host "`nDatabase check completed successfully!" -ForegroundColor Green
    
} catch {
    Write-Host "`nError accessing database:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    
    if ($_.Exception.Message -like "*Cannot open database*") {
        Write-Host "`nThe database may not exist yet. Please run the DbMigrator first:" -ForegroundColor Yellow
        Write-Host "cd d:\Development\CalwayPest\src\CalwayPest.DbMigrator" -ForegroundColor Yellow
        Write-Host "dotnet run" -ForegroundColor Yellow
    }
} finally {
    if ($connection.State -eq 'Open') {
        $connection.Close()
    }
}
