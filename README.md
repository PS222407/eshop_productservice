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
mkdir -p docker_data/kafka && sudo chown -R 1001:1001 docker_data/kafka
```
```bash
docker compose up -d  
```
Run migrations:
```bash
dotnet ef database update --project eshop_productservice
```
Start app
```bash
dotnet run --project eshop_productservice/eshop_productservice.csproj
```

## Seed data
1. **seed into database**  
Find the Product.sql on server. This file was too big for github :(  
You can import using adminer web ui. For the big Products.sql file you must do it in terminal.  
```bash
docker cp ~/Categories.sql eshop-postgres-productservice:/Categories.sql && \
docker cp ~/Products.sql eshop-postgres-productservice:/Products.sql
```
```bash
docker exec -it eshop-postgres-productservice psql -U postgres -d eshop_productservice -f /Categories.sql && \
docker exec -it eshop-postgres-productservice psql -U postgres -d eshop_productservice -f /Products.sql
```
Set stock to 99 instead of 0
```sql
UPDATE "Products" SET "Stock" = '99' WHERE "Stock" = '0';
```
Create indexes to make the search query faster
```sql
CREATE INDEX CONCURRENTLY idx_products_name_gin ON "Products" USING gin(to_tsvector('english', "Name"))
```
2. **Import products form database to typesense searchengine**  
Start application, open swagger and click on the ImportProducts endpoint.

## Test JWT token
This token is valid for 60 years:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImRhZTU0NzNkLTI2MTYtNDlkMi1iMTY5LWM0NThkZTdmYjBjNiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImFkbWluQGdtYWlsLmNvbSIsInVzZXIiOiJ7XCJJZFwiOlwiZGFlNTQ3M2QtMjYxNi00OWQyLWIxNjktYzQ1OGRlN2ZiMGM2XCIsXCJFbWFpbFwiOlwiYWRtaW5AZ21haWwuY29tXCIsXCJSb2xlc1wiOltdfSIsImV4cCI6MzkwNzIxMDU5MSwiaXNzIjoieW91ckNvbXBhbnlJc3N1ZXIuY29tIiwiYXVkIjoieW91ckNvbXBhbnlBdWRpZW5jZS5jb20ifQ.XGAJDfiLjLZjRlNCUO7ylLwaykveBkNPZkzrwxOjN_E
```
```
{
  "id": "dae5473d-2616-49d2-b169-c458de7fb0c6",
  "email": "admin@gmail.com",
  "password": "password"
}
```

## While developing
### Run resharper  
Shutdown running application and run  
```bash
./scripts/resharper.sh
```
### Migrations  
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
### Coverage Report  
This command is onetime setup: 
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```
Run coverage report:
```bash
./scripts/coverage.sh
```

### OWASP ZAP scan  
Use `` or  
Start application as Production ready
```bash
./scripts/owasp.sh
```

### Unit tests
For now I only used Rider's IDE UI

### Load tests
```bash
cd k6loadtests && k6 run index.js
```

### Monitoring grafana
Its running via docker, not combined with k8s. Run project locally.  
Dashboard ID: 19194
Dashboard URL: https://grafana.com/grafana/dashboards/17706-asp-net-otel-metrics/

### Logs grafana
Its running via docker, not combined with k8s. Run project locally.
import dashboard load the json from `./grafana-log-dashboard.json`

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
minikube addons enable ingress && \
    minikube addons enable metrics-server
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
