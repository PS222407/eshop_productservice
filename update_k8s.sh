#!/bin/bash
set -e

echo "Building and deploying services to Minikube..."

echo "Building productservice docker image..."
docker build -t jensr22/eshop_productservice:latest -f ./eshop_productservice/Dockerfile .
docker save jensr22/eshop_productservice:latest -o eshop_productservice.tar
minikube image load eshop_productservice.tar
kubectl apply -f k8s.yml
kubectl rollout restart deployment productservice-deployment

echo "Building userservice docker image..."
docker build -t jensr22/eshop_userservice:latest -f ../eshop_userservice/eshop_userservice/Dockerfile ../eshop_userservice/.
docker save jensr22/eshop_userservice:latest -o eshop_userservice.tar
minikube image load eshop_userservice.tar
kubectl apply -f k8s.yml
kubectl rollout restart deployment userservice-deployment

echo "Building cartservice docker image..."
docker build -t jensr22/eshop_cartservice:latest -f ../eshop_cartservice/eshop_cartservice/Dockerfile ../eshop_cartservice/.
docker save jensr22/eshop_cartservice:latest -o eshop_cartservice.tar
minikube image load eshop_cartservice.tar
kubectl apply -f k8s.yml
kubectl rollout restart deployment cartservice-deployment