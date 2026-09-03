#!/bin/sh

set -e

echo "Starting Pharmacy Angular frontend..."
echo "API_URL: ${API_URL}"

if [ -z "${API_URL}" ]; then
    echo "ERROR: API_URL environment variable is not set."
    exit 1
fi

echo "Generating NGINX configuration..."

envsubst '${API_URL}' \
    < /etc/nginx/templates/default.conf.template \
    > /etc/nginx/conf.d/default.conf

echo "Generated NGINX configuration:"
cat /etc/nginx/conf.d/default.conf

echo "Testing NGINX configuration..."

nginx -t

echo "Starting NGINX..."

exec nginx -g 'daemon off;'