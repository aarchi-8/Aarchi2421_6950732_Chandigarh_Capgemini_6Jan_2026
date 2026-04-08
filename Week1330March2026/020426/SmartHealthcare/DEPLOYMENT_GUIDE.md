# 🚀 Smart Healthcare - Deployment Guide

## **PRE-DEPLOYMENT CHECKLIST**

Before deploying to production, verify all items below:

```
☐ API builds without errors (dotnet build)
☐ Web builds without errors (dotnet build)
☐ All tests pass (if applicable)
☐ Database migrations applied to production DB
☐ appsettings.Production.json configured
☐ JWT secret key is strong and production-grade
☐ HTTPS certificates are valid (not self-signed)
☐ Database backups are configured
☐ Logging is properly configured
☐ CORS settings are restrictive
☐ API rate limiting is enabled
☐ Admin account is created
☐ All sensitive data removed from code comments
☐ Security headers configured
☐ Load balancer/reverse proxy configured
☐ Monitoring and alerting enabled
☐ Disaster recovery plan is documented
```

---

## **STEP 1: PREPARE PRODUCTION CONFIGURATION**

### **1.1 Create appsettings.Production.json**

**SmartHealthcare.API/appsettings.Production.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "System": "Warning"
    }
  },
  "AllowedHosts": "*.yourdomain.com",
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server.com,1433;Database=SmartHealthcareDB_Prod;User Id=sa;Password=YOUR_SECURE_PASSWORD;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_NEW_STRONG_SECRET_KEY_MIN_32_CHARS_RECOMMENDED_64_CHARS",
    "ExpiryMinutes": 60,
    "Issuer": "SmartHealthcare",
    "Audience": "SmartHealthcareUsers"
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "/var/log/smarthealthcare/api-.txt",
          "rollingInterval": "Day",
          "fileSizeLimitBytes": 104857600,
          "retainedFileCountLimit": 30,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  },
  "Cors": {
    "AllowedOrigins": "https://yourdomain.com,https://www.yourdomain.com"
  }
}
```

**SmartHealthcare.Web/appsettings.Production.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "AllowedHosts": "*.yourdomain.com",
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server.com,1433;Database=SmartHealthcareDB_Prod;User Id=sa;Password=YOUR_SECURE_PASSWORD;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;"
  },
  "ApiBaseUrl": "https://api.yourdomain.com",
  "SessionTimeout": 3600
}
```

---

## **STEP 2: GENERATE SECURE JWT SECRET**

### **Option A: Using PowerShell**
```powershell
# Generate a 64-character random string for JWT secret
$bytes = New-Object byte[] 32
$rng = [System.Security.Cryptography.RNGCryptoServiceProvider]::Create()
$rng.GetBytes($bytes)
$secret = [Convert]::ToBase64String($bytes)
Write-Host "JWT Secret: $secret"
```

### **Option B: Using OpenSSL (Linux/Mac)**
```bash
openssl rand -base64 32
# Output: YN2Kz7d/hJ+pQ...
```

### **Option C: Using .NET**
```bash
dotnet user-secrets generate-secret
```

---

## **STEP 3: DATABASE MIGRATION**

### **3.1 Create Production Database**

**SQL Server:**
```sql
-- Connect to SQL Server Management Studio
-- Create new database
CREATE DATABASE SmartHealthcareDB_Prod
GO

-- Create login if needed
CREATE LOGIN sa_prod WITH PASSWORD = 'YOUR_SECURE_PASSWORD'
GO

-- Create user
USE SmartHealthcareDB_Prod
GO
CREATE USER [sa_prod] FOR LOGIN [sa_prod]
GO
GRANT CONTROL ON DATABASE::SmartHealthcareDB_Prod TO [sa_prod]
GO
```

### **3.2 Apply Migrations**

```bash
# Navigate to API project
cd SmartHealthcare.API

# Apply migrations with production connection string
dotnet ef database update --configuration Release -- "smarthealthcare_prod_connection_string"

# Or use this if environment-based config is set
set ASPNETCORE_ENVIRONMENT=Production
dotnet ef database update
```

---

## **STEP 4: PUBLISH APPLICATIONS**

### **4.1 Publish API**

```bash
cd SmartHealthcare.API

# Publish for Windows
dotnet publish -c Release -o ./publish-api --self-contained false -r win-x64

# Publish for Linux
dotnet publish -c Release -o ./publish-api --self-contained false -r linux-x64

# Verify output
dir publish-api
```

### **4.2 Publish Web Application**

```bash
cd SmartHealthcare.Web

# Publish for Windows
dotnet publish -c Release -o ./publish-web --self-contained false -r win-x64

# Publish for Linux
dotnet publish -c Release -o ./publish-web --self-contained false -r linux-x64

# Verify output
dir publish-web
```

---

## **STEP 5: DEPLOYMENT OPTIONS**

### **OPTION A: IIS DEPLOYMENT (Windows)**

#### **A.1: Install IIS Hosting Bundle**
```powershell
# Download and install
https://dotnet.microsoft.com/permalink/dotnetcore-hosting

# Or download via Chocolatey
choco install dotnet-hosting-windows
```

#### **A.2: Create IIS Application Pool**
```
1. Open IIS Manager
2. Right-click "Application Pools" → "Add Application Pool"
3. Name: SmartHealthcareAPI
4. .NET CLR version: No Managed Code (for .NET Core)
5. Managed pipeline mode: Integrated
6. Click OK
7. Right-click new pool → "Advanced Settings"
8. Set "Start Mode" to "AlwaysRunning"
9. Set "Iddle Time-out (minutes)" to 0
```

#### **A.3: Create IIS Website**
```
1. Right-click "Sites" → "Add Website"
2. Site name: SmartHealthcareAPI
3. Application pool: SmartHealthcareAPI
4. Physical path: C:\iis\smarthealthcare-api
5. Binding:
   - Type: https
   - IP: *
   - Port: 443
   - SSL certificate: (your production certificate)
6. Click OK
```

#### **A.4: Deploy Files**
```powershell
# Copy published files to IIS folder
Copy-Item -Path "C:\path\to\publish-api\*" -Destination "C:\iis\smarthealthcare-api" -Recurse -Force

# Set permissions
$acl = Get-Acl "C:\iis\smarthealthcare-api"
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSERS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
$acl.AddAccessRule($rule)
Set-Acl -Path "C:\iis\smarthealthcare-api" -AclObject $acl
```

---

### **OPTION B: DOCKER DEPLOYMENT**

#### **B.1: Create Dockerfile for API**

**SmartHealthcare.API/Dockerfile:**
```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SmartHealthcare.API/SmartHealthcare.API.csproj", "SmartHealthcare.API/"]
COPY ["SmartHealthcare.Models/SmartHealthcare.Models.csproj", "SmartHealthcare.Models/"]

RUN dotnet restore "SmartHealthcare.API/SmartHealthcare.API.csproj"

COPY . .

RUN dotnet build "SmartHealthcare.API/SmartHealthcare.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SmartHealthcare.API/SmartHealthcare.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 5125
ENV ASPNETCORE_URLS=http://+:5125
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SmartHealthcare.API.dll"]
```

#### **B.2: Create docker-compose.yml**

```yaml
version: '3.8'

services:
  mssql:
    image: mcr.microsoft.com/mssql/server:latest
    container_name: smarthealthcare-db
    environment:
      ACCEPT_EULA: Y
      SA_PASSWORD: "YOUR_SECURE_PASSWORD"
      MSSQL_PID: Express
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
    networks:
      - smarthealthcare-network

  api:
    build:
      context: .
      dockerfile: SmartHealthcare.API/Dockerfile
    container_name: smarthealthcare-api
    environment:
      ConnectionStrings__DefaultConnection: "Server=mssql,1433;Database=SmartHealthcareDB_Prod;User Id=sa;Password=YOUR_SECURE_PASSWORD;Encrypt=false;TrustServerCertificate=true;"
      ASPNETCORE_ENVIRONMENT: Production
    ports:
      - "5125:5125"
    depends_on:
      - mssql
    networks:
      - smarthealthcare-network
    restart: unless-stopped

  web:
    build:
      context: .
      dockerfile: SmartHealthcare.Web/Dockerfile
    container_name: smarthealthcare-web
    environment:
      ApiBaseUrl: "http://api:5125"
      ASPNETCORE_ENVIRONMENT: Production
    ports:
      - "5272:5272"
    depends_on:
      - api
    networks:
      - smarthealthcare-network
    restart: unless-stopped

volumes:
  sqldata:

networks:
  smarthealthcare-network:
    driver: bridge
```

#### **B.3: Deploy with Docker**

```bash
# Build images
docker-compose build

# Start services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

---

### **OPTION C: AZURE APP SERVICE DEPLOYMENT**

#### **C.1: Create Azure Resources**

```bash
# Install Azure CLI
choco install azure-cli

# Login to Azure
az login

# Create resource group
az group create --name SmartHealthcareRG --location eastus

# Create App Service Plan
az appservice plan create --name SmartHealthcarePlan --resource-group SmartHealthcareRG --sku B2 --is-linux

# Create SQL Database Server
az sql server create --resource-group SmartHealthcareRG --name shdb-server --admin-user sqladmin --admin-password YourPassword123!

# Create SQL Database
az sql db create --resource-group SmartHealthcareRG --server shdb-server --name SmartHealthcareDB_Prod --sku Basic

# Create Web Apps (API)
az webapp create --resource-group SmartHealthcareRG --plan SmartHealthcarePlan --name smarthealthcare-api --runtime "DOTNETCORE:7.0"

# Create Web Apps (Web)
az webapp create --resource-group SmartHealthcareRG --plan SmartHealthcarePlan --name smarthealthcare-web --runtime "DOTNETCORE:7.0"
```

#### **C.2: Configure App Settings**

```bash
# Configure API
az webapp config appsettings set --resource-group SmartHealthcareRG --name smarthealthcare-api --settings \
  "ConnectionStrings__DefaultConnection=Server=shdb-server.database.windows.net,1433;Initial Catalog=SmartHealthcareDB_Prod;User ID=sqladmin;Password=YourPassword123!;Encrypt=true;Connection Timeout=30;" \
  "JwtSettings__SecretKey=YOUR_SECURE_JWT_SECRET" \
  "ASPNETCORE_ENVIRONMENT=Production"

# Configure Web
az webapp config appsettings set --resource-group SmartHealthcareRG --name smarthealthcare-web --settings \
  "ApiBaseUrl=https://smarthealthcare-api.azurewebsites.net" \
  "ASPNETCORE_ENVIRONMENT=Production"
```

#### **C.3: Deploy Using Git**

```bash
# Setup local git
cd SmartHealthcare.API
git init
git add .
git commit -m "Initial deploy"

# Create Azure Git remote
az webapp deployment source config-local-git --resource-group SmartHealthcareRG --name smarthealthcare-api

# Add and push to azure
git remote add azure https://smarthealthcare@smarthealthcare-api.scm.azurewebsites.net:443/smarthealthcare-api.git
git push azure master
```

---

## **STEP 6: POST-DEPLOYMENT VERIFICATION**

### **6.1 Health Check**

```bash
# Check API is running
curl https://api.yourdomain.com/api/health

# Check Web is running
curl https://yourdomain.com

# Check Swagger documentation
curl https://api.yourdomain.com/swagger
```

### **6.2 Test Critical Functions**

```bash
# Test Login
curl -X POST https://api.yourdomain.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"YourPassword123!"}'

# Test Doctor List
curl https://api.yourdomain.com/api/doctors \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### **6.3 Check Logs**

```bash
# Windows/IIS
Event Viewer → Application → Look for errors

# Docker
docker logs smarthealthcare-api

# Azure
az webapp log tail --resource-group SmartHealthcareRG --name smarthealthcare-api

# Linux
tail -f /var/log/smarthealthcare/api-*.txt
```

---

## **STEP 7: SSL/TLS CERTIFICATE SETUP**

### **Option A: Let's Encrypt (Free)**

```bash
# Install Certbot
choco install certbot

# Get certificate
certbot certonly --standalone -d yourdomain.com -d api.yourdomain.com

# Certificate locations:
# /etc/letsencrypt/live/yourdomain.com/fullchain.pem
# /etc/letsencrypt/live/yourdomain.com/privkey.pem

# Auto-renewal
certbot renew --dry-run
```

### **Option B: Azure Certificate**

```bash
# Upload to Key Vault
az keyvault certificate import --vault-name MyKeyVault --name MyAppCert --file /path/to/cert.pfx

# Use in App Service
az webapp config ssl bind --resource-group MyResourceGroup --name MyApp --certificate-name MyAppCert --ssl-type SNI
```

---

## **STEP 8: CONFIGURE CORS & SECURITY HEADERS**

### **Update Program.cs (API)**

```csharp
// In Program.cs (before UseRouting)

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", builder =>
    {
        builder.WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// Security Headers Middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
    
    await next();
});

// Use CORS
app.UseCors("ProductionPolicy");
```

---

## **STEP 9: SETUP MONITORING & ALERTS**

### **Option A: Azure Monitor**

```bash
# Enable Application Insights
az monitor app-insights component create \
  --app smarthealthcare-insights \
  --location eastus \
  --resource-group SmartHealthcareRG \
  --application-type web

# Configure alerts
az monitor metrics alert create \
  --name APIHighErrorRate \
  --resource-group SmartHealthcareRG \
  --scopes /subscriptions/YOUR_SUB_ID/resourceGroups/SmartHealthcareRG/providers/Microsoft.Web/sites/smarthealthcare-api \
  --condition "avg Percentage >= 5" \
  --window-size 5m \
  --evaluation-frequency 1m
```

### **Option B: Configure Serilog for Production**

```csharp
// appsettings.Production.json already has file logging configured
// Ensure logs are sent to centralized location

// Example: Send to Seq (centralized logging)
"WriteTo": [
    {
        "Name": "Seq",
        "Args": {
            "serverUrl": "https://logs.yourdomain.com"
        }
    }
]
```

---

## **STEP 10: BACKUP & DISASTER RECOVERY**

### **Database Backups**

```sql
-- SQL Server Backup
USE master
GO
BACKUP DATABASE SmartHealthcareDB_Prod 
TO DISK = 'D:\Backups\SmartHealthcareDB_Prod.bak'
WITH INIT, COMPRESSION
GO

-- Schedule weekly backups using SQL Server Agent
-- Or use Azure SQL Database automated backups (7-35 day retention)
```

### **File Backups**

```bash
# Daily backup of application files
tar -czf /backups/smarthealthcare-api-$(date +%Y%m%d).tar.gz /app/smarthealthcare-api

# Azure Backup
az backup protection enable-for-vm --vault-name MyVault --resource-group MyRG --vm smarthealthcare-api
```

---

## **STEP 11: CREATE PRODUCTION ADMIN ACCOUNT**

```bash
# Login to application and register admin account via UI
# Or create directly in database

# SQL Insert
USE SmartHealthcareDB_Prod
GO
INSERT INTO [dbo].[Users] 
([UserId], [FullName], [Email], [PasswordHash], [Role], [IsActive], [CreatedAt], [RefreshToken], [RefreshTokenExpiry])
VALUES 
(NEWID(), 'Admin User', 'admin@yourdomain.com', 'HASHED_PASSWORD_HERE', 'Admin', 1, GETUTCDATE(), NULL, NULL)
GO
```

---

## **STEP 12: LOAD TESTING**

### **Using Apache JMeter**

```bash
# Install JMeter
choco install jmeter

# Create test plan targeting production API
# Configure 100 concurrent users
# Run test and analyze results

# Check:
# - Response times (should be < 200ms)
# - Error rate (should be 0%)
# - Database connection pool (should not exhaust)
```

---

## **POST-DEPLOYMENT TASKS**

```
☐ Document deployment steps for future reference
☐ Create runbooks for common operations
☐ Train ops team on monitoring and alerting
☐ Setup automated backups
☐ Configure log aggregation
☐ Test failover procedures
☐ Document rollback procedures
☐ Schedule regular security audits
☐ Setup performance monitoring
☐ Create incident response plan
```

---

## **MONITORING CHECKLIST**

Monitor these metrics regularly:

- **API Response Time:** Should average < 100ms
- **Error Rate:** Should be < 1%
- **Database Connections:** Monitor pool exhaustion
- **CPU Usage:** Should stay < 80%
- **Memory Usage:** Should stay < 80%
- **Disk Space:** Alert at 80% full
- **Log File Size:** Rotate daily to prevent disk issues

---

## **ROLLBACK PROCEDURE**

If deployment has critical issues:

```bash
# 1. Stop current version
az webapp stop --resource-group SmartHealthcareRG --name smarthealthcare-api

# 2. Delete deployed slot
az webapp deployment slot delete --resource-group SmartHealthcareRG --name smarthealthcare-api --slot staging

# 3. Restore database from backup
RESTORE DATABASE SmartHealthcareDB_Prod 
FROM DISK = 'D:\Backups\SmartHealthcareDB_Prod.bak'

# 4. Deploy previous version
# ... redeploy from backup

# 5. Verify
curl https://api.yourdomain.com/api/health
```

---

## **HELPFUL RESOURCES**

- **Azure Documentation:** https://docs.microsoft.com/azure/
- **Docker Documentation:** https://docs.docker.com/
- **SQL Server Backups:** https://docs.microsoft.com/sql/relational-databases/backup-restore/
- **SSL Certificates:** https://certbot.eff.org/
- **Performance Tips:** https://docs.microsoft.com/aspnet/core/performance/

---

**Deployment Guide Complete! 🎉**

For questions or issues, refer to logs and contact the development team.

---
