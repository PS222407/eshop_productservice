#!/bin/bash
set -e

docker build -t jensr22/eshop_productservice:latest -f ./eshop_productservice/Dockerfile .
docker save jensr22/eshop_productservice:latest -o eshop_productservice.tar
minikube image load eshop_productservice.tar
kubectl apply -f k8s.yml
kubectl rollout restart deployment productservice-deployment