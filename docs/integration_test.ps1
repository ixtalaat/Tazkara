$ErrorActionPreference = "Stop"

# Use unique emails to prevent conflicts between test runs
$rand = Get-Random -Minimum 1000 -Maximum 9999
$adminEmail = "admin$rand@tazkara.com"
$orgEmail = "org$rand@tazkara.com"
$custEmail = "cust$rand@tazkara.com"

Write-Host "Starting Tazkara API in background..." -ForegroundColor Green
$apiProcess = Start-Process dotnet -ArgumentList "run --project Tazkara.API" -PassThru -NoNewWindow

try {
    Write-Host "Waiting for API to start up (8 seconds)..." -ForegroundColor Yellow
    Start-Sleep -Seconds 8

    $baseUrl = "http://localhost:5037/api"

    # Helper function for JSON requests
    function Invoke-JsonPost($url, $body, $token = $null) {
        $headers = @{ "Content-Type" = "application/json" }
        if ($token) { $headers.Add("Authorization", "Bearer $token") }
        $json = $body | ConvertTo-Json
        return Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $json
    }

    function Invoke-JsonPatch($url, $token) {
        $headers = @{ "Authorization" = "Bearer $token" }
        return Invoke-RestMethod -Uri $url -Method Patch -Headers $headers
    }

    function Invoke-JsonGet($url, $token) {
        $headers = @{ "Authorization" = "Bearer $token" }
        return Invoke-RestMethod -Uri $url -Method Get -Headers $headers
    }

    # 1. Register and Login Admin
    Write-Host "1. Registering and logging in Admin user ($adminEmail)..." -ForegroundColor Cyan
    $regAdminBody = @{
        firstName = "Admin"
        lastName = "User"
        email = $adminEmail
        password = "Password123!"
        role = "Admin"
    }
    $regAdminRes = Invoke-JsonPost "$baseUrl/Auth/register" $regAdminBody
    
    $loginAdminBody = @{ email = $adminEmail; password = "Password123!" }
    $loginAdminRes = Invoke-JsonPost "$baseUrl/Auth/login" $loginAdminBody
    $adminToken = $loginAdminRes.data.token

    # 2. Create Category
    Write-Host "2. Creating event category..." -ForegroundColor Cyan
    $catBody = @{ name = "Concerts-$rand" }
    $catRes = Invoke-JsonPost "$baseUrl/Categories" $catBody $adminToken
    $catId = $catRes.data.id
    Write-Host "Category created: $catId" -ForegroundColor Gray

    # 3. Register and Login Organizer
    Write-Host "3. Registering and logging in Organizer ($orgEmail)..." -ForegroundColor Cyan
    $regOrgBody = @{
        firstName = "Org"
        lastName = "User"
        email = $orgEmail
        password = "Password123!"
        role = "Organizer"
    }
    $regOrgRes = Invoke-JsonPost "$baseUrl/Auth/register" $regOrgBody
    
    $loginOrgBody = @{ email = $orgEmail; password = "Password123!" }
    $loginOrgRes = Invoke-JsonPost "$baseUrl/Auth/login" $loginOrgBody
    $orgToken = $loginOrgRes.data.token

    # 4. Create and Publish Event
    Write-Host "4. Creating and publishing a new event..." -ForegroundColor Cyan
    $eventBody = @{
        title = "Rock Festival $rand"
        description = "An amazing live concert"
        location = "Stadium Arena"
        startDate = (Get-Date).AddDays(5).ToString("o")
        endDate = (Get-Date).AddDays(5).AddHours(3).ToString("o")
        capacity = 10
        price = 150.00
        categoryId = $catId
    }
    $eventRes = Invoke-JsonPost "$baseUrl/Events" $eventBody $orgToken
    $eventId = $eventRes.data.id
    Write-Host "Event created in draft status: $eventId" -ForegroundColor Gray

    $publishRes = Invoke-JsonPatch "$baseUrl/Events/$eventId/publish" $orgToken
    Write-Host "Event published successfully." -ForegroundColor Gray

    # 5. Register and Login Customer
    Write-Host "5. Registering and logging in Customer ($custEmail)..." -ForegroundColor Cyan
    $regCustBody = @{
        firstName = "Cust"
        lastName = "User"
        email = $custEmail
        password = "Password123!"
        role = "Customer"
    }
    $regCustRes = Invoke-JsonPost "$baseUrl/Auth/register" $regCustBody

    $loginCustBody = @{ email = $custEmail; password = "Password123!" }
    $loginCustRes = Invoke-JsonPost "$baseUrl/Auth/login" $loginCustBody
    $custToken = $loginCustRes.data.token

    # 6. Reserve Ticket
    Write-Host "6. Reserving a ticket..." -ForegroundColor Cyan
    $reserveBody = @{ eventId = $eventId }
    $reserveRes = Invoke-JsonPost "$baseUrl/Tickets/reserve" $reserveBody $custToken
    $ticketId = $reserveRes.data.id
    Write-Host "Ticket reserved: $ticketId. Ticket Number: $($reserveRes.data.ticketNumber)" -ForegroundColor Gray

    # 7. Create Payment Session
    Write-Host "7. Creating payment session..." -ForegroundColor Cyan
    $sessionBody = @{ ticketId = $ticketId; provider = 0 } # PayPal
    $sessionRes = Invoke-JsonPost "$baseUrl/Payments/session" $sessionBody $custToken
    $transactionId = $sessionRes.data.transactionId
    Write-Host "Payment Session URL: $($sessionRes.data.paymentUrl)" -ForegroundColor Gray
    Write-Host "Transaction ID: $transactionId" -ForegroundColor Gray

    # 8. Verify Payment
    Write-Host "8. Verifying payment..." -ForegroundColor Cyan
    $verifyBody = @{ transactionId = $transactionId; verificationToken = "dummy_token" }
    $verifyRes = Invoke-JsonPost "$baseUrl/Payments/verify" $verifyBody $custToken
    Write-Host "Payment Status after verification: $($verifyRes.data.status)" -ForegroundColor Gray
    Write-Host "Ticket Status after verification: $($verifyRes.data.ticket.status)" -ForegroundColor Gray

    # 9. Fetch Organizer Dashboard
    Write-Host "9. Fetching Organizer Dashboard..." -ForegroundColor Cyan
    $dashboardRes = Invoke-JsonGet "$baseUrl/Dashboard/organizer" $orgToken
    
    Write-Host "---------------- DASHBOARD METRICS ----------------" -ForegroundColor Green
    Write-Host "Total Events: $($dashboardRes.data.totalEvents)" -ForegroundColor White
    Write-Host "Total Tickets Sold: $($dashboardRes.data.totalTicketsSold)" -ForegroundColor White
    Write-Host "Total Revenue: $($dashboardRes.data.totalRevenue)" -ForegroundColor White
    
    $stats = $dashboardRes.data.eventStats[0]
    Write-Host "Event Breakdown:" -ForegroundColor Green
    Write-Host "  - Title: $($stats.title)" -ForegroundColor White
    Write-Host "  - Price: $($stats.price)" -ForegroundColor White
    Write-Host "  - Tickets Sold: $($stats.ticketsSold)" -ForegroundColor White
    Write-Host "  - Tickets Reserved: $($stats.ticketsReserved)" -ForegroundColor White
    Write-Host "  - Tickets Available: $($stats.ticketsAvailable)" -ForegroundColor White
    Write-Host "  - Revenue: $($stats.revenue)" -ForegroundColor White
    Write-Host "----------------------------------------------------" -ForegroundColor Green

    # 10. Verify Exception Handling middleware
    Write-Host "10. Verifying Global Exception Handling..." -ForegroundColor Cyan
    try {
        # Create invalid event to trigger ValidationFilter / Bad Request Exception
        $invalidEventBody = @{ title = ""; price = -10 }
        $invalidRes = Invoke-JsonPost "$baseUrl/Events" $invalidEventBody $orgToken
    }
    catch {
        $ex = $_.Exception
        Write-Host "Caught expected exception. Status code: $($ex.Response.StatusCode)" -ForegroundColor Gray
        $streamReader = New-Object System.IO.StreamReader($ex.Response.GetResponseStream())
        $errBody = $streamReader.ReadToEnd()
        Write-Host "Formatted error payload returned from API:" -ForegroundColor Yellow
        Write-Host $errBody -ForegroundColor Yellow
    }

    Write-Host "`nAll integration tests passed successfully!" -ForegroundColor Green

} catch {
    Write-Host "An error occurred during verification:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    if ($_.Exception.InnerException) { Write-Host $_.Exception.InnerException.Message -ForegroundColor Red }
} finally {
    Write-Host "Stopping API process..." -ForegroundColor Gray
    Stop-Process -Id $apiProcess.Id -Force
}
