# Quick Start - Deploy V2Ray to Ubuntu Server

## 🚀 Fastest Way to Deploy (Using Git Clone on Server)

### ⭐ Recommended Method: Direct Server Deployment

This method clones the repository directly on the server and builds everything there.

**Step 1: Commit your changes locally**
```bash
cd C:\Users\Mostafa\OutlineVpn
git add .
git commit -m "Update deployment configuration"
git push origin master
```

**Step 2: SSH to your server**
```bash
ssh root@109.94.164.46
# Password: !Q@W3e4r
```

**Step 3: Download and run the deployment script**
```bash
# Download the script directly
curl -o server-deploy.sh https://raw.githubusercontent.com/kazemim99/OutlineVpn/master/server-deploy.sh
chmod +x server-deploy.sh

# Run it
sudo ./server-deploy.sh
```

The script will automatically:
- Install Docker, Docker Compose, Git, and Node.js
- Clone your repository
- Build the Vue.js frontend
- Set up all necessary directories
- Configure the firewall

**Step 4: Start the application**
```bash
cd /var/www/v2ray
docker-compose up -d

# Check status
docker-compose ps

# View logs
docker-compose logs -f
```

**Step 5: Test HTTP access**
Visit: http://iransshvpn.com (or http://109.94.164.46)

**Step 6: Get SSL Certificate**
```bash
cd /var/www/v2ray
docker-compose run --rm certbot certonly --webroot \
  --webroot-path /var/www/certbot \
  --email your-email@example.com \
  --agree-tos \
  --no-eff-email \
  -d iransshvpn.com \
  -d www.iransshvpn.com
```

**Step 7: Enable HTTPS**
```bash
rm nginx/conf.d/app-http.conf
mv nginx/conf.d/app-ssl.conf.disabled nginx/conf.d/app.conf
docker-compose restart nginx
```

**Step 8: Access your app**
- HTTPS: https://iransshvpn.com ✅

---

### Alternative: Manual File Transfer Method

If you prefer to build locally and transfer files:

1. **Run PowerShell script:**
   ```powershell
   cd C:\Users\Mostafa\OutlineVpn
   .\deploy-to-server.ps1
   ```

2. **Transfer using WinSCP** to `/var/www/v2ray/`

3. **Deploy on server** following the script instructions

---

## 📋 Prerequisites Checklist

Before starting, ensure:

- [ ] Domain `iransshvpn.com` DNS A record points to `109.94.164.46`
- [ ] Server ports 22, 80, 443 are open
- [ ] Node.js and npm are installed locally
- [ ] You have SSH access to the server

---

## 🔧 Quick Commands Reference

### On Local Machine
```powershell
# Build and package
.\deploy-to-server.ps1

# Or manual build
cd vue\v2ray
npm run build
```

### On Server
```bash
# View logs
docker-compose logs -f

# Restart services
docker-compose restart

# Stop everything
docker-compose down

# Start everything
docker-compose up -d

# Check SSL certificate
docker-compose run --rm certbot certificates
```

---

## ⚠️ Important Notes

1. **Database Connection**: Check `V2Ray.Api/appsettings.Production.json` line 61 for database connection string
2. **First Time**: Start with HTTP, then add SSL
3. **DNS**: Ensure domain points to server before getting SSL
4. **Firewall**: Open ports 80, 443 on server

---

## 🆘 Troubleshooting

**Build fails?**
- Run `npm install` in `vue\v2ray` folder
- Check Node.js version (should be v14+)

**Can't connect to server?**
- Verify SSH: `ssh root@109.94.164.46`
- Check firewall settings

**Application not accessible?**
```bash
docker-compose ps              # Check if containers are running
docker-compose logs -f         # View logs
sudo ufw status                # Check firewall
```

**SSL certificate fails?**
- Ensure domain DNS is set correctly
- Wait a few minutes for DNS propagation
- Verify port 80 is accessible

---

## 📚 Full Documentation

For detailed instructions, see [DEPLOYMENT.md](DEPLOYMENT.md)

---

## ✅ Success Checklist

After deployment:

- [ ] Application accessible via HTTP
- [ ] SSL certificate obtained
- [ ] HTTPS working with valid certificate
- [ ] API endpoints responding
- [ ] Database connected
- [ ] Logs showing no errors

Your app should be live at: **https://iransshvpn.com**
