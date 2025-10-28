#!/bin/bash
docker compose up -d
cd ../eshop_productservice/eshop_productservice && dotnet ef database update
cd ../../eshop_userservice/eshop_userservice && dotnet ef database update
cd ../../eshop_productservice/eshop_productservice
