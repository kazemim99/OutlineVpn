# V2Ray Application Deployment Guide

This guide will help you deploy your V2Ray application to an Ubuntu server with Docker, Nginx, and SSL.

## Server Information
- **IP Address**: 109.94.164.46
- **Username**: root
- **Domain**: iransshvpn.com
- **SSL**: Let's Encrypt

## Prerequisites

Before starting, ensure:
1. Your domain `iransshvpn.com` is pointing to IP `109.94.164.46` (A record)
2. Ports 80, 443, and 5000 are open on your server firewall
3. You have built the Vue.js frontend (run `npm run build` in vue/v2ray folder)

## Step-by-Step Deployment

### Step 1: Build the Vue.js Frontend (on your local machine)

```bash
cd c:\Users\Mostafa\OutlineVpn\vue\v2ray
npm install
npm run build
```

This will build the Vue.js app and copy it to `V2Ray.Api/wwwroot/`

### Step 2: Connect to Your Server

```bash
ssh root@109.94.164.46
```

Password: `!Q@W3e4r`

### Step 3: Run the Deployment Script

On the server, create and run the deployment script:

```bash
# Download the deployment script (or copy it manually)
mkdir -p /var/www/v2ray
cd /var/www/v2ray

# Run the script
chmod +x deploy.sh
./deploy.sh
```

### Step 4: Transfer Application Files

From your local machine, transfer the files to the server:

```bash
# Using SCP (run from local machine in PowerShell or Git Bash)
scp -r "c:\Users\Mostafa\OutlineVpn\V2Ray.Api" root@109.94.164.46:/var/www/v2ray/
scp "c:\Users\Mostafa\OutlineVpn\docker-compose.yml" root@109.94.164.46:/var/www/v2ray/
scp -r "c:\Users\Mostafa\OutlineVpn\nginx" root@109.94.164.46:/var/www/v2ray/
```

Or use an FTP client like FileZilla or WinSCP:
- Host: 109.94.164.46
- Username: root
- Password: !Q@W3e4r
- Upload to: /var/www/v2ray/

### Step 5: Initial Nginx Configuration (HTTP only)

Before getting SSL certificate, we need to start with HTTP only. On the server:

```bash
cd /var/www/v2ray

# Create a temporary nginx config without SSL
cat > nginx/conf.d/app-http.conf << 'EOF'
server {
    listen 80;
    listen [::]:80;
    server_name iransshvpn.com www.iransshvpn.com;

    # Let's Encrypt challenge location
    location /.well-known/acme-challenge/ {
        root /var/www/certbot;
    }

    # Proxy to the backend
    location / {
        proxy_pass http://v2ray-api:5000/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    }

    location /api/ {
        proxy_pass http://v2ray-api:5000/api/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    }
}
EOF

# Remove the SSL config temporarily
mv nginx/conf.d/app.conf nginx/conf.d/app-ssl.conf.disabled
```

### Step 6: Start the Application

```bash
cd /var/www/v2ray

# Start the containers
docker-compose up -d

# Check if containers are running
docker-compose ps

# Check logs
docker-compose logs -f
```

### Step 7: Verify HTTP Access

Open your browser and visit: `http://iransshvpn.com`

If you see your application, proceed to SSL setup.

### Step 8: Get SSL Certificate

```bash
cd /var/www/v2ray

# Get SSL certificate from Let's Encrypt
docker-compose run --rm certbot certonly --webroot \
  --webroot-path /var/www/certbot \
  --email your-email@example.com \
  --agree-tos \
  --no-eff-email \
  -d iransshvpn.com \
  -d www.iransshvpn.com
```

Replace `your-email@example.com` with your actual email address.

### Step 9: Enable SSL Configuration

```bash
cd /var/www/v2ray

# Remove HTTP-only config and enable SSL config
rm nginx/conf.d/app-http.conf
mv nginx/conf.d/app-ssl.conf.disabled nginx/conf.d/app.conf

# Restart nginx
docker-compose restart nginx
```

### Step 10: Verify HTTPS Access

Open your browser and visit: `https://iransshvpn.com`

You should see a secure connection with a valid SSL certificate.

## Useful Commands

### View Logs
```bash
# All logs
docker-compose logs -f

# API logs only
docker-compose logs -f v2ray-api

# Nginx logs only
docker-compose logs -f nginx
```

### Restart Services
```bash
# Restart all
docker-compose restart

# Restart API only
docker-compose restart v2ray-api

# Restart Nginx only
docker-compose restart nginx
```

### Update Application
```bash
# Stop containers
docker-compose down

# Transfer new files from local machine
# Then rebuild and start
docker-compose up -d --build
```

### Check SSL Certificate Status
```bash
docker-compose run --rm certbot certificates
```

### Renew SSL Certificate (auto-renewal is configured)
```bash
docker-compose run --rm certbot renew
docker-compose restart nginx
```

## Troubleshooting

### Application not accessible
1. Check if containers are running: `docker-compose ps`
2. Check logs: `docker-compose logs -f`
3. Check firewall: `sudo ufw status`
4. Ensure ports 80 and 443 are open

### SSL Certificate Issues
1. Ensure domain DNS is pointing to the server
2. Check if port 80 is accessible (needed for Let's Encrypt validation)
3. Try getting the certificate again

### Database Connection Issues
1. Check your connection string in `appsettings.Production.json`
2. Ensure your database server is accessible from the application server
3. Check database credentials

### Application Errors
1. Check logs: `docker-compose logs -f v2ray-api`
2. Check if all required files are present
3. Verify environment variables in docker-compose.yml

## Security Recommendations

1. **Change default passwords** in appsettings.Production.json
2. **Set up firewall rules**:
   ```bash
   sudo ufw allow 22/tcp
   sudo ufw allow 80/tcp
   sudo ufw allow 443/tcp
   sudo ufw enable
   ```
3. **Regular updates**:
   ```bash
   sudo apt update && sudo apt upgrade -y
   ```
4. **Monitor logs regularly**
5. **Set up automated backups** for your database and application files

## DNS Configuration

Ensure your domain DNS has these records:

| Type | Name | Value | TTL |
|------|------|-------|-----|
| A | @ | 109.94.164.46 | 3600 |
| A | www | 109.94.164.46 | 3600 |

## Support

For issues or questions, check:
- Application logs: `docker-compose logs -f`
- Nginx error log: `docker exec nginx-reverse-proxy cat /var/log/nginx/iransshvpn_error.log`
- System logs: `journalctl -xe`
