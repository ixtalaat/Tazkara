# Database Seeding Guide

## Overview
The Tazkara API includes a database seeding feature that automatically populates the database with initial data (roles, users, categories, and events) when enabled.

## Configuration

### Development Environment
Seeding is **enabled by default** in `appsettings.Development.json`:
```json
"DatabaseSeed": {
  "Enabled": true,
  "AdminEmail": "admin@tazkara.local",
  "AdminPassword": "Admin@123",
  "OrganizerEmail": "organizer@tazkara.local",
  "OrganizerPassword": "Organizer@123"
}
```

### Production Environment
Seeding is **disabled by default** in `appsettings.Production.json` for security reasons.

## Enabling Seeding in Remote/Production Database

### Option 1: Using Environment Variables (Recommended)
Set environment variables before running the application:

```powershell
# PowerShell
$env:DatabaseSeed__Enabled = "true"
$env:DatabaseSeed__AdminEmail = "your-admin@example.com"
$env:DatabaseSeed__AdminPassword = "YourSecurePassword123!"
$env:DatabaseSeed__OrganizerEmail = "your-organizer@example.com"
$env:DatabaseSeed__OrganizerPassword = "YourSecurePassword123!"
```

```bash
# Bash/Linux
export DatabaseSeed__Enabled=true
export DatabaseSeed__AdminEmail=your-admin@example.com
export DatabaseSeed__AdminPassword=YourSecurePassword123!
export DatabaseSeed__OrganizerEmail=your-organizer@example.com
export DatabaseSeed__OrganizerPassword=YourSecurePassword123!
```

### Option 2: Using appsettings Configuration
Update `appsettings.json` or `appsettings.Production.json`:

```json
"DatabaseSeed": {
  "Enabled": true,
  "AdminEmail": "admin@yourdomain.com",
  "AdminPassword": "SecurePassword123!",
  "OrganizerEmail": "organizer@yourdomain.com",
  "OrganizerPassword": "SecurePassword123!"
}
```

### Option 3: Using Azure App Service Configuration
In Azure Portal, add Application Settings:
- `DatabaseSeed__Enabled` = `true`
- `DatabaseSeed__AdminEmail` = your admin email
- `DatabaseSeed__AdminPassword` = your secure password
- `DatabaseSeed__OrganizerEmail` = your organizer email
- `DatabaseSeed__OrganizerPassword` = your secure password

## What Gets Seeded

When seeding is enabled, the following data is automatically created (only if it doesn't already exist):

### Roles
- Admin
- Organizer
- Customer

### Users
- **Admin User**: Created with the configured AdminEmail and AdminPassword
- **Organizer User**: Created with the configured OrganizerEmail and OrganizerPassword

### Event Categories
- Music & Concerts
- Theatre & Performing Arts
- Sports
- Culture & Heritage
- Festivals & Family

### Sample Events
Six sample events are created with details like:
- Cairo Jazz Nights
- Nile Festival of Lights
- Pharaohs Cup Final
- Aida at the Cairo Opera House
- Khan el-Khalili Heritage Walk
- Alexandria Mediterranean Film Week

## Safety Features

✅ **Idempotent**: Seeding is safe to run repeatedly - it checks for existing records before inserting
✅ **Migration-Aware**: Automatically runs pending migrations before seeding
✅ **Non-Destructive**: Only adds data, doesn't delete or modify existing records

## Troubleshooting

### Seeding Fails with "Unable to resolve service"
**Cause**: DatabaseSeeder.Options missing required credentials
**Solution**: Ensure all four DatabaseSeed settings are configured:
- Enabled
- AdminEmail
- AdminPassword
- OrganizerEmail
- OrganizerPassword

### Seeding Doesn't Run
**Cause**: `DatabaseSeed:Enabled` is `false`
**Solution**: Enable seeding via environment variables or configuration file (see options above)

### Migration Errors During Seeding
**Cause**: Database hasn't been initialized
**Solution**: Seeding automatically runs migrations. Ensure database server is accessible and connectionstring is correct.

## Best Practices

🔒 **Security**:
- Never commit credentials to version control
- Use environment variables or Azure Key Vault in production
- Use strong passwords (minimum 8 characters with mixed case, numbers, special characters)
- Change default passwords immediately after seeding

⚡ **Performance**:
- Seeding only runs once at application startup
- Use only in development or initial production setup
- Disable seeding after initial setup for faster startup times

## Manual Database Reset

To reseed your database manually without restarting the application:

1. **Option A**: Delete existing data and restart the app (with seeding enabled)
2. **Option B**: Run EF Core commands:
```powershell
# Remove and re-apply all migrations
dotnet ef database drop --project Tazkara.Infrastructure --startup-project Tazkara.API
dotnet ef database update --project Tazkara.Infrastructure --startup-project Tazkara.API
```

Then restart the application with seeding enabled.
