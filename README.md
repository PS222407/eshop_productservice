# Setup For Development
If you use windows, please run inside WSL  
```bash
git clone git@github.com:PS222407/eshop_productservice.git  
```
```bash
cd eshop_productservice/  
```
Create persistant directory for kafka, this directory must be owned by 1001:1001.
```bash
mkdir data_kafka && sudo chown -R 1001:1001 data_kafka
```
```bash
docker compose up -d  
```
```bash
dotnet run --project eshop_productservice/eshop_productservice.csproj
```

**Run resharper:**
```bash
jb cleanupcode ./eshop_cartservice.sln
```
**Migrations**
Install dotnet-ef cli tools:
```bash
dotnet tool install --global dotnet-ef
```
Create migration:
```bash
dotnet ef migrations add Initial
```
Run migrations:
```bash
dotnet ef database update
```

# Setup For Production
```bash
git clone git@github.com:PS222407/eshop_productservice.git
```
```bash
cd eshop_productservice/  
```
```bash
docker network create eshop-network
```
```bash
docker run --name eshop-mongo --network eshop-network -p 27017:27017 -d mongo:8.0
```
```bash
docker build -t jensr22/eshop_productservice:latest -f ./eshop_productservice/Dockerfile .
```
```bash
docker run --name eshop_productservice --network eshop-network -p 8080:8080 -p 8081:8081 -d jensr22/eshop_productservice:latest
```
Now you can access http://localhost:8080/swagger/index.html  

# k8s
Prerequisites
```bash
minikube addons enable ingress
```
Load local images in minikube that are not hosted in a registry  
```bash
docker save jensr22/eshop_productservice:latest -o eshop_productservice.tar
```
```bash
minikube image load eshop_productservice.tar
```
Ssh into minikube to check by running `docker image ls`
```bash
minikube ssh
```
Start kubernetes (if minikube is used):  
```bash
minikube start
```
```bash
kubectl apply -f k8s.yml
```
```bash
kubectl get pods
```
```bash
kubectl get svc eshop-productservice
```
If new release of docker image is available and kubernetes pods were already running you need to redeploy using this command:
```bash
kubectl rollout restart deployment productservice-deployment
```
View app in browser:
```bash
minikube ip
```
View kubernetes dashboard (if minikube is used):  
```bash
minikube dashboard
```
