#!/usr/bin/env bash
echo "cartservice"
cd ../eshop_cartservice && git switch develop
echo "userservice"
cd ../eshop_userservice && git switch develop
echo "productservice"
cd ../eshop_productservice && git switch develop
echo "svelte"
cd ../eshop-svelte && git switch develop
