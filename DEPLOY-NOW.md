# 🚀 Deploy V2Ray Now - Simple 3-Step Process

## What You're Deploying
- **Repository**: https://github.com/kazemim99/OutlineVpn
- **Server**: 109.94.164.46 (Ubuntu)
- **Domain**: iransshvpn.com
- **Method**: Git clone on server (cleanest approach)

---

## Before You Start

1. **Commit your changes** (important!):
   ```bash
   cd C:\Users\Mostafa\OutlineVpn
   git add .
   git commit -m "Add deployment configuration"
   git push origin master
   ```

2. **Ensure DNS is configured**:
   - Domain: `iransshvpn.com`
   - A Record → `109.94.164.46`

---

## 3 Simple Steps

### Step 1️⃣: SSH to Server
```bash
ssh root@109.94.164.46
```
Password: `!Q@W3e4r`

### Step 2️⃣: Run Deployment Script
Copy and paste this entire block:
```bash
curl -o server-deploy.sh https://raw.githubusercontent.com/kazemim99/OutlineVpn/master/server-deploy.sh && \
chmod +x server-deploy.sh && \
sudo ./server-deploy.sh
```

This will:
- ✅ Install Docker, Node.js, Git
- ✅ Clone your repository
- ✅ Build Vue.js frontend
- ✅ Set up everything

### Step 3️⃣: Start Application
```bash
cd /var/www/v2ray
docker-compose up -d
```

---

## That's It! 🎉

Your app is now running at:
- **HTTP**: http://iransshvpn.com
- **HTTP (IP)**: http://109.94.164.46

---

## Optional: Add SSL Certificate

After confirming the app works:

```bash
cd /var/www/v2ray

# Get certificate
docker-compose run --rm certbot certonly --webroot \
  --webroot-path /var/www/certbot \
  --email your-email@example.com \
  --agree-tos --no-eff-email \
  -d iransshvpn.com -d www.iransshvpn.com

# Enable HTTPS
rm nginx/conf.d/app-http.conf
mv nginx/conf.d/app-ssl.conf.disabled nginx/conf.d/app.conf
docker-compose restart nginx
```

Now visit: **https://iransshvpn.com** 🔒

---

## Useful Commands

```bash
# View logs
docker-compose logs -f

# Restart services
docker-compose restart

# Stop everything
docker-compose down

# Update app (after pushing changes)
cd /var/www/v2ray
git pull origin master
cd vue/v2ray && npm run build
cd ../..
docker-compose up -d --build
```

---

## Troubleshooting

**Script fails to download?**
```bash
# Alternative: Clone and run manually
git clone https://github.com/kazemim99/OutlineVpn.git /var/www/v2ray
cd /var/www/v2ray
sudo ./server-deploy.sh
```

**Container won't start?**
```bash
docker-compose logs -f        # Check logs
docker-compose ps             # Check status
sudo ufw status               # Check firewall
```

**Can't access website?**
- Check DNS: `nslookup iransshvpn.com`
- Check firewall: Ports 80, 443 must be open
- Wait 5 minutes for DNS propagation

---

## 📚 More Info

- [QUICK-START.md](QUICK-START.md) - Detailed instructions
- [DEPLOYMENT.md](DEPLOYMENT.md) - Full documentation
