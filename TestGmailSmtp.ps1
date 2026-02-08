# Test Gmail SMTP Connection
# This script tests if we can connect to Gmail's SMTP server and send email

Write-Host "=== Gmail SMTP Connection Test ===" -ForegroundColor Cyan
Write-Host ""

# SMTP Settings from appsettings.json
$smtpServer = "smtp.gmail.com"
$smtpPort = 587
$username = "dbsinfotechinc@gmail.com"
$password = "ybfgmwoskgglrhks"  # App Password
$from = "noreply@calwaypest.ca"
$to = "devendrajoshi45@gmail.com"

# Test 1: TCP Connection to SMTP Server
Write-Host "Test 1: Testing TCP connection to $smtpServer`:$smtpPort..." -ForegroundColor Yellow
try {
    $tcpClient = New-Object System.Net.Sockets.TcpClient
    $tcpClient.Connect($smtpServer, $smtpPort)
    if ($tcpClient.Connected) {
        Write-Host "Success: TCP Connection successful!" -ForegroundColor Green
        $tcpClient.Close()
    }
}
catch {
    Write-Host "Failed: TCP Connection failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "This suggests a network/firewall issue blocking port $smtpPort" -ForegroundColor Red
    exit
}

Write-Host ""

# Test 2: Send Email using .NET SmtpClient
Write-Host "Test 2: Attempting to send test email..." -ForegroundColor Yellow
try {
    $securePassword = ConvertTo-SecureString $password -AsPlainText -Force
    $credential = New-Object System.Management.Automation.PSCredential($username, $securePassword)
    
    $mailParams = @{
        From = $username
        To = $to
        Subject = "Test Email from PowerShell - $(Get-Date)"
        Body = "This is a test email sent via PowerShell to verify SMTP connectivity.<br><br>Sent at: $(Get-Date)"
        BodyAsHtml = $true
        SmtpServer = $smtpServer
        Port = $smtpPort
        UseSsl = $true
        Credential = $credential
    }
    
    Send-MailMessage @mailParams
    Write-Host "Success: Email sent successfully!" -ForegroundColor Green
    Write-Host "Check inbox at: $to" -ForegroundColor Green
}
catch {
    Write-Host "Failed: Email send failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Full Error Details:" -ForegroundColor Yellow
    Write-Host $_.Exception.ToString() -ForegroundColor Gray
    
    if ($_.Exception.Message -like "*authentication*" -or $_.Exception.Message -like "*credentials*") {
        Write-Host ""
        Write-Host "This appears to be an AUTHENTICATION issue." -ForegroundColor Red
        Write-Host "Possible solutions:" -ForegroundColor Yellow
        Write-Host "  1. Verify the App Password is correct and not expired" -ForegroundColor White
        Write-Host "  2. Regenerate a new App Password in Google Account settings" -ForegroundColor White
        Write-Host "  3. Ensure 2-Step Verification is enabled for the Google account" -ForegroundColor White
    }
}

Write-Host ""
Write-Host "=== Test Complete ===" -ForegroundColor Cyan
