#!/bin/bash
docker compose up -d
cd ..
gnome-terminal --title="ProductService" -- bash -c "cd eshop_productservice/eshop_productservice && dotnet run; exec bash"
gnome-terminal --title="UserService" -- bash -c "cd eshop_userservice/eshop_userservice && dotnet run; exec bash"
gnome-terminal --title="CartService" -- bash -c "cd eshop_cartservice/eshop_cartservice && dotnet run; exec bash"
gnome-terminal --title="Frontend Svelte" -- bash -c "cd eshop-svelte && npm i && npm run dev; exec bash"
