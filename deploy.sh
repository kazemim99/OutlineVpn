#!/bin/bash

# Deployment script for V2Ray Application
# Server: 109.94.164.46
# Domain: iransshvpn.com

echo "=========================================="
echo "V2Ray Application Deployment Script"
echo "=========================================="

# Update system packages
echo "Updating system packages..."
sudo apt update && sudo apt upgrade -y

# Install Docker
echo "Installing Docker..."
if ! command -v docker &> /dev/null; then
    curl -fsSL https://get.docker.com -o get-docker.sh
    sudo sh get-docker.sh
    sudo usermod -aG docker $USER
    rm get-docker.sh
else
    echo "Docker is already installed"
fi

# Install Docker Compose
echo "Installing Docker Compose..."
if ! command -v docker-compose &> /dev/null; then
    sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    sudo chmod +x /usr/local/bin/docker-compose
else
    echo "Docker Compose is already installed"
fi

# Create application directory
echo "Setting up application directory..."
sudo mkdir -p /var/www/v2ray
cd /var/www/v2ray

# Create necessary directories
mkdir -p nginx/conf.d
mkdir -p certbot/conf
mkdir -p certbot/www
mkdir -p V2Ray.Api/storage

echo "=========================================="
echo "Setup complete!"
echo "=========================================="
echo ""
echo "Next steps:"
echo "1. Copy your application files to /var/www/v2ray"
echo "2. Update appsettings.Production.json with correct database connection"
echo "3. Run: docker-compose up -d"
echo "4. Get SSL certificate: sudo docker-compose run --rm certbot certonly --webroot --webroot-path /var/www/certbot -d iransshvpn.com -d www.iransshvpn.com"
echo "5. Restart nginx: docker-compose restart nginx"
