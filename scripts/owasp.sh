#!/bin/bash
docker compose up -d
cd ..
rm eshop_productservice/zap-report.html
gnome-terminal --title="ProductService" -- bash -c "cd eshop_productservice/eshop_productservice && dotnet run --launch-profile http_production; exec bash"
sleep 5
docker run --rm --network=host -v "$(pwd):/zap/wrk/:rw" \
  ghcr.io/zaproxy/zaproxy:stable \
  zap-api-scan.py \
  -t http://localhost:5077/api/productservice/swagger/v1/swagger.json \
  -f openapi \
  -r eshop_productservice/zap-report.html \
  -d

xdg-open "eshop_productservice/zap-report.html"