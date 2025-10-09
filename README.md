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
## Test JWT token
This token is valid for 60 years:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImRhZTU0NzNkLTI2MTYtNDlkMi1iMTY5LWM0NThkZTdmYjBjNiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImFkbWluQGdtYWlsLmNvbSIsInVzZXIiOiJ7XCJJZFwiOlwiZGFlNTQ3M2QtMjYxNi00OWQyLWIxNjktYzQ1OGRlN2ZiMGM2XCIsXCJFbWFpbFwiOlwiYWRtaW5AZ21haWwuY29tXCIsXCJSb2xlc1wiOltdfSIsImV4cCI6MzkwNzIxMDU5MSwiaXNzIjoieW91ckNvbXBhbnlJc3N1ZXIuY29tIiwiYXVkIjoieW91ckNvbXBhbnlBdWRpZW5jZS5jb20ifQ.XGAJDfiLjLZjRlNCUO7ylLwaykveBkNPZkzrwxOjN_E
```
```
{
  "email": "admin@gmail.com",
  "password": "password"
}
```

## Seed data
Find the Product.sql on server. This file was too big for github :(  
You can import using adminer web ui. For the big Products.sql file you must do it in terminal.  
```bash
docker cp eshop_productservice/CSV/Categories.sql eshop-postgres-productservice:/Categories.sql && \
docker cp eshop_productservice/CSV/Products.sql eshop-postgres-productservice:/Products.sql
```
```bash
docker exec -it eshop-postgres-productservice psql -U postgres -d eshop_productservice -f /Categories.sql && \
docker exec -it eshop-postgres-productservice psql -U postgres -d eshop_productservice -f /Products.sql
```

## Create indexes
```sql
CREATE INDEX CONCURRENTLY idx_products_name_gin ON "Products" USING gin(to_tsvector('english', "Name"))
```

## While developing
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

**Coverage Report**
This command is onetime setup: 
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```
Run coverage report:
```bash
dotnet test --settings coverlet.runsettings --collect:"XPlat Code Coverage" --results-directory:"xplat" && \
mv ./xplat/*/coverage.cobertura.xml ./xplat/ 2>/dev/null || true
```
```bash
reportgenerator -reports:xplat/coverage.cobertura.xml -targetdir:xplat/coverage-report -reporttypes:Html && xdg-open xplat/coverage-report/index.html
```
Or use the `coverage.sh` script

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
