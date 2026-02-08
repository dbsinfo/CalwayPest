# Quick Email Log Checker
Write-Host "`n🔍 Checking Email Logs..." -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

$logFile = "src\CalwayPest.Web\Logs\logs.txt"
$recentLogs = Get-Content $logFile -Tail 50

# Check for email activity
$emailSuccess = $recentLogs | Select-String "Email sent successfully"
$emailFailed = $recentLogs | Select-String "Email send failed"
$contactSubmission = $recentLogs | Select-String "Contact form submission received"

if ($contactSubmission) {
    Write-Host "✓ Contact form submission detected:" -ForegroundColor Green
    $contactSubmission | Select-Object -Last 1 | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
    Write-Host ""
}

if ($emailSuccess) {
    Write-Host "✅ EMAIL SENT SUCCESSFULLY!" -ForegroundColor Green
    $emailSuccess | Select-Object -Last 1 | ForEach-Object { Write-Host "  $_" -ForegroundColor Green }
} elseif ($emailFailed) {
    Write-Host "❌ EMAIL FAILED TO SEND" -ForegroundColor Red
    $emailFailed | Select-Object -Last 1 | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
    
    # Show more details
    Write-Host "`nError details:" -ForegroundColor Yellow
    $recentLogs | Select-String "SSL|TLS|MailKit" | Select-Object -Last 3 | ForEach-Object {
        Write-Host "  $_" -ForegroundColor Red
    }
} else {
    Write-Host "⏳ No recent email activity found" -ForegroundColor Yellow
    Write-Host "   Submit a contact form and run this script again" -ForegroundColor Gray
}

Write-Host "`n========================================`n" -ForegroundColor Cyan
