# PowerShell script to view detailed contact submissions
$serverName = "(LocalDb)\MSSQLLocalDB"
$databaseName = "CalwayPest"

try {
    Add-Type -AssemblyName "System.Data"
    
    $connectionString = "Server=$serverName;Database=$databaseName;Integrated Security=True;TrustServerCertificate=True"
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "   CONTACT SUBMISSIONS DATABASE CHECK" -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan
    
    $connection.Open()
    
    # Query ContactSubmissions table with full details
    $query = @"
SELECT 
    Id,
    FullName,
    Email,
    Message,
    CreationTime,
    CreatorId
FROM AppContactSubmissions 
ORDER BY CreationTime DESC
"@
    
    $command = New-Object System.Data.SqlClient.SqlCommand($query, $connection)
    $reader = $command.ExecuteReader()
    
    $count = 0
    while ($reader.Read()) {
        $count++
        Write-Host "-------------------------------------------" -ForegroundColor Gray
        Write-Host "Submission #$count" -ForegroundColor Green
        Write-Host "-------------------------------------------" -ForegroundColor Gray
        Write-Host "ID           : " -NoNewline; Write-Host $reader["Id"] -ForegroundColor White
        Write-Host "Full Name    : " -NoNewline; Write-Host $reader["FullName"] -ForegroundColor Yellow
        Write-Host "Email        : " -NoNewline; Write-Host $reader["Email"] -ForegroundColor Cyan
        Write-Host "Message      : " -NoNewline; Write-Host $reader["Message"] -ForegroundColor White
        Write-Host "Submitted On : " -NoNewline; Write-Host $reader["CreationTime"] -ForegroundColor Magenta
        Write-Host ""
    }
    $reader.Close()
    
    if ($count -eq 0) {
        Write-Host "No contact submissions found in the database." -ForegroundColor Yellow
        Write-Host "Submit a test form to create records." -ForegroundColor Yellow
    } else {
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "Total Submissions: $count" -ForegroundColor Green
        Write-Host "========================================`n" -ForegroundColor Cyan
    }
    
    $connection.Close()
    
} catch {
    Write-Host "`nERROR: " -ForegroundColor Red -NoNewline
    Write-Host $_.Exception.Message -ForegroundColor Red
    
    if ($_.Exception.Message -like "*Cannot open database*" -or $_.Exception.Message -like "*does not exist*") {
        Write-Host "`nThe database hasn't been created yet." -ForegroundColor Yellow
        Write-Host "`nTo create the database, run:" -ForegroundColor Cyan
        Write-Host "  cd d:\Development\CalwayPest\src\CalwayPest.DbMigrator" -ForegroundColor White
        Write-Host "  dotnet run" -ForegroundColor White
    }
} finally {
    if ($connection -and $connection.State -eq 'Open') {
        $connection.Close()
    }
}
