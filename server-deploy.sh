#!/bin/bash

# V2Ray Application - Server Deployment Script
# This script should be run on the Ubuntu server
# It will clone the repo and deploy everything

set -e  # Exit on error

echo "=========================================="
echo "V2Ray Application - Server Deployment"
echo "=========================================="
echo ""

# Configuration
REPO_URL="git@github.com:kazemim99/OutlineVpn.git"
DEPLOY_DIR="/var/www/v2ray"
DOMAIN="iransshvpn.com"

# Check if running as root
if [ "$EUID" -ne 0 ]; then
    echo "Please run as root (use sudo)"
    exit 1
fi

echo "[1/8] Updating system packages..."
apt update && apt upgrade -y

echo "[2/8] Installing Docker..."
if ! command -v docker &> /dev/null; then
    curl -fsSL https://get.docker.com -o get-docker.sh
    sh get-docker.sh
    systemctl enable docker
    systemctl start docker
    rm get-docker.sh
    echo "Docker installed successfully!"
else
    echo "Docker already installed"
fi

echo "[3/8] Installing Docker Compose..."
if ! command -v docker-compose &> /dev/null; then
    curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    chmod +x /usr/local/bin/docker-compose
    echo "Docker Compose installed successfully!"
else
    echo "Docker Compose already installed"
fi

echo "[4/8] Installing Git..."
if ! command -v git &> /dev/null; then
    apt install -y git
    echo "Git installed successfully!"
else
    echo "Git already installed"
fi

echo "[5/8] Installing Node.js and npm..."
if ! command -v node &> /dev/null; then
    curl -fsSL https://deb.nodesource.com/setup_18.x | bash -
    apt install -y nodejs
    echo "Node.js installed successfully!"
    node --version
    npm --version
else
    echo "Node.js already installed ($(node --version))"
fi

echo "[6/8] Setting up deployment directory..."
mkdir -p $DEPLOY_DIR
cd $DEPLOY_DIR

# Check if repo already cloned
if [ -d ".git" ]; then
    echo "Repository already exists. Pulling latest changes..."
    git pull origin master
else
    echo "Cloning repository..."
    # Remove everything in directory first
    rm -rf ./*

    # For HTTPS clone (no SSH key needed)
    echo "Enter GitHub username (or press Enter to use SSH):"
    read GH_USER

    if [ -z "$GH_USER" ]; then
        echo "Using SSH clone..."
        git clone $REPO_URL .
    else
        echo "Using HTTPS clone..."
        git clone https://github.com/kazemim99/OutlineVpn.git .
    fi
fi

echo "[7/8] Building Vue.js frontend..."
cd $DEPLOY_DIR/vue/v2ray
npm install
npm run build

echo "[8/8] Setting up Docker environment..."
cd $DEPLOY_DIR

# Create necessary directories
mkdir -p nginx/conf.d
mkdir -p certbot/conf
mkdir -p certbot/www
mkdir -p V2Ray.Api/storage

# Create initial HTTP-only nginx config
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
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    location /api/ {
        proxy_pass http://v2ray-api:5000/api/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
EOF

# Disable SSL config if exists
if [ -f "nginx/conf.d/app.conf" ]; then
    mv nginx/conf.d/app.conf nginx/conf.d/app-ssl.conf.disabled
fi

# Set up firewall
echo "Setting up firewall..."
ufw allow 22/tcp
ufw allow 80/tcp
ufw allow 443/tcp
ufw --force enable

echo ""
echo "=========================================="
echo "Setup Complete!"
echo "=========================================="
echo ""
echo "Next steps:"
echo ""
echo "1. Start the application:"
echo "   cd $DEPLOY_DIR"
echo "   docker-compose up -d"
echo ""
echo "2. Check if running:"
echo "   docker-compose ps"
echo ""
echo "3. View logs:"
echo "   docker-compose logs -f"
echo ""
echo "4. Test HTTP access:"
echo "   curl http://localhost"
echo "   or visit: http://$DOMAIN"
echo ""
echo "5. Get SSL certificate:"
echo "   docker-compose run --rm certbot certonly --webroot \\"
echo "     --webroot-path /var/www/certbot \\"
echo "     --email your-email@example.com \\"
echo "     --agree-tos --no-eff-email \\"
echo "     -d $DOMAIN -d www.$DOMAIN"
echo ""
echo "6. Enable HTTPS:"
echo "   rm nginx/conf.d/app-http.conf"
echo "   mv nginx/conf.d/app-ssl.conf.disabled nginx/conf.d/app.conf"
echo "   docker-compose restart nginx"
echo ""
echo "=========================================="
