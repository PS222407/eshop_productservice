# Setup For Development
If you use windows, please run inside WSL  
```bash
git clone git@github.com:PS222407/eshop_productservice.git  
```
```bash
cd eshop_productservice/  
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
docker build -t eshop:0.1 -f ./eshop_productservice/Dockerfile .  
```
```bash
docker run --name eshop --network eshop-network -p 8080:8080 -p 8081:8081 -d eshop:0.1  
```
Now you can access http://localhost:8080/swagger/index.html  

# k8s
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
```bash
minikube service eshop-productservice
```
View kubernetes dashboard (if minikube is used):  
```bash
minikube dashboard  
```
