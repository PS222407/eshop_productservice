# Setup For Development
If you use windows, please run inside WSL  
git clone git@github.com:PS222407/eshop_productservice.git  
cd eshop_productservice/  
docker compose up -d  
dotnet run  

# Setup For Production
git clone git@github.com:PS222407/eshop_productservice.git  
cd eshop_productservice/  

docker network create eshop-network  

docker run --name eshop-mongo --network eshop-network -p 27017:27017 -d mongo:8.0  

docker build -t eshop:0.1 -f ./EShop/Dockerfile .  
docker run --name eshop --network eshop-network -p 8080:8080 -p 8081:8081 -d eshop:0.1  

Now you can access http://localhost:8080/swagger/index.html  

# k8s
Start kubernetes (if minikube is used):  
minikube start  

kubectl apply -f k8s.yml  
kubectl get pods  
kubectl get svc eshop-productservice  

minikube service eshop-productservice

View kubernetes dashboard (if minikube is used):  
minikube dashboard  