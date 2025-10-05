#!/usr/bin/env bash
echo "cartservice"
cd ../eshop_cartservice && git pull
echo "userservice"
cd ../eshop_userservice && git pull
echo "productservice"
cd ../eshop_productservice && git pull
echo "orderservice"
cd ../eshop_orderservice && git pull
echo "svelte"
cd ../eshop-svelte && git pull
