#!/bin/bash
set -e

echo "Building and deploying services to Minikube..."

echo "Building productservice docker image..."
docker build -t jensr22/eshop_productservice:latest -f ./eshop_productservice/Dockerfile .
echo "Saving image to tar file..."
docker save jensr22/eshop_productservice:latest -o eshop_productservice.tar
echo "Loading image into Minikube..."
minikube image load eshop_productservice.tar
echo "Applying Kubernetes configuration..."
kubectl apply -f k8s.yml
echo "Restarting productservice deployment..."
kubectl rollout restart deployment productservice-deployment

echo "Building userservice docker image..."
docker build -t jensr22/eshop_userservice:latest -f ../eshop_userservice/eshop_userservice/Dockerfile ../eshop_userservice/.
echo "Saving image to tar file..."
docker save jensr22/eshop_userservice:latest -o eshop_userservice.tar
echo "Loading image into Minikube..."
minikube image load eshop_userservice.tar
echo "Applying Kubernetes configuration..."
kubectl apply -f k8s.yml
echo "Restarting userservice deployment..."
kubectl rollout restart deployment userservice-deployment

echo "Building cartservice docker image..."
docker build -t jensr22/eshop_cartservice:latest -f ../eshop_cartservice/eshop_cartservice/Dockerfile ../eshop_cartservice/.
echo "Saving image to tar file..."
docker save jensr22/eshop_cartservice:latest -o eshop_cartservice.tar
echo "Loading image into Minikube..."
minikube image load eshop_cartservice.tar
echo "Applying Kubernetes configuration..."
kubectl apply -f k8s.yml
echo "Restarting cartservice deployment..."
kubectl rollout restart deployment cartservice-deployment

echo "Building orderservice docker image..."
docker build -t jensr22/eshop_orderservice:latest -f ../eshop_orderservice/eshop_orderservice/Dockerfile ../eshop_orderservice/.
echo "Saving image to tar file..."
docker save jensr22/eshop_orderservice:latest -o eshop_orderservice.tar
echo "Loading image into Minikube..."
minikube image load eshop_orderservice.tar
echo "Applying Kubernetes configuration..."
kubectl apply -f k8s.yml
echo "Restarting orderservice deployment..."
kubectl rollout restart deployment orderservice-deployment