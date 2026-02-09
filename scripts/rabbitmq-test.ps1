$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot
Set-Location $projectRoot

Write-Host "Project root: $projectRoot"

# Optional rebuild for order-api (uncomment if you want to force rebuild every run)
# docker compose up -d --build order-api

# 1) Register (unique email)
$email = "user$([guid]::NewGuid().ToString('N').Substring(0,8))@demo.local"
$reg = Invoke-RestMethod -Method Post `
  -Uri "http://localhost:8080/api/auth/register" `
  -ContentType "application/json" `
  -Body (@{ email=$email; password="Pass123!" } | ConvertTo-Json)

$token = $reg.accessToken
$userId = $reg.user.id

Write-Host "Registered user: $email"

# 2) Create order (publishes OrderCreatedEvent)
$body = @{
  userId = $userId
  items = @(
    @{
      productId = [guid]::NewGuid()
      productName = "Test Item"
      unitPrice = 10.5
      quantity = 2
    }
  )
} | ConvertTo-Json -Depth 5

$order = Invoke-RestMethod -Method Post `
  -Uri "http://localhost:8080/api/orders" `
  -Headers @{ Authorization = "Bearer $token" } `
  -ContentType "application/json" `
  -Body $body

$orderId = $order.id
Write-Host "Created order: $orderId"

# 3) Wait a bit for async processing
Start-Sleep -Seconds 7

# 4) Check order status (should be Paid/Failed, not Pending)
$orderStatus = Invoke-RestMethod -Method Get `
  -Uri "http://localhost:8080/api/orders/$orderId" `
  -Headers @{ Authorization = "Bearer $token" }

Write-Host "Order status: $($orderStatus.status)"
$orderStatus

# 5) List payments
$payments = Invoke-RestMethod -Method Get `
  -Uri "http://localhost:8080/api/payments" `
  -Headers @{ Authorization = "Bearer $token" }

$payments
