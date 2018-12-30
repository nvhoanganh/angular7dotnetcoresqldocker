# Angular 7, .NET Core Web API with EF core,MSSQL server, VsCode, Azure DevOps CI/CD, Azure Container Service, Google Kubernetes Engine, Terraform

This is a sample project showing the steps taken to develop a simple 3 tier application using Angular 7, .NET Core Web API with EF core and MSSQL server using open Open Source development tools (VsCode) using a Windows 10 Professional laptop. Azure DevOps is used to automate the CI/CD process and deploy to both Azure Kubernetes Service and Goolge Kubernetes Engine using Terraform

## Step 1: Install Docker for Windows
1. Make sure you have Windows 10 Pro or Enterprise (Docker for Windows require Hyper-V)
2. Install Docker for Windows at [https://docs.docker.com/docker-for-windows/install/](https://docs.docker.com/docker-for-windows/install/)
3. Make sure Docker is running in Linux mode

## Step 2: Create Angular 7 image
1. install latest angular-cli using `npm i -g @angular/cli`
2. create new folder called `Docker` somewhere on your computer
3. inside `Docker` folder create new Angular App by using `ng new angular-ui` command
5. inside the `angular-ui` folder, add new file `nginx.conf` and `Dockerfile`
6. create new docker image by running `docker build --rm -f "angular-ui\Dockerfile" -t nvhoanganh1909/docker-demo-ui:latest angular-ui` command from the root folder. **Note**:
   1. `nvhoanganh1909` is your Docker Hub username.
   2. `docker-demo-ui` is the name of the image
   3. `:latest` is the tag or the version number of the image. 
7. check to make sure image is created by running `docker images` command
8.   run the newly created image by running `docker run -d --rm -p 4200:80 nvhoanganh1909/docker-demo-ui:latest` where
9.  open http://localhost:4200 , you should see the Angular app is running

**Note**:
- view the running docker container by running `docker ps`
- stop the running container by running `docker container stop 0f` where `0f` is the first 2 characters of the Container ID 
- run `docker system prune --volumes -f` to remove all unused images and containers

## Step 3: Create Microsoft SQL Server Container Image
1. login to [https://hub.docker.com/](https://hub.docker.com/)
2. search for `microsoft SQL linux`
3. select [https://hub.docker.com/r/microsoft/mssql-server-linux/](https://hub.docker.com/r/microsoft/mssql-server-linux/)
4. check instructions on how to use this image
5. now pull down the latest version of this image by running `docker pull microsoft/mssql-server-linux:latest`
6. start the container using SQLExpress mode by running `docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Password1' -e 'MSSQL_PID=Express' -p 1433:1433 -d microsoft/mssql-server-linux:latest` (change `SA_PASSWORD` to something else)
7. you now can connect to this SQL server using SQL Management Studio

## Step 4: Create new .NET Core WebApi
1. from the root folder (`Docker`), create new folder called `dotnetcore-api` using `mkdir dotnetcore-webapi`
2. cd into the `dotnetcore-webapi` and run `dotnet new webapi` 
3. edit `dotnetcore-webapi.csproj` and add `<DockerDefaultTargetOS>Linux</DockerDefaultTargetOS>` element under `<TargetFramework>netcoreapp2.1</TargetFramework>` element.
6. in the dotnetcore-webapi folder create `Dockerfile` and `.dockerignore` file
8. Build the docker image by running `docker build --rm -f "dotnetcore-webapi\Dockerfile" -t nvhoanganh1909/docker-demo-user-api:latest dotnetcore-webapi` from the root folder
9.  now run the webapi by running `docker run -d -p 8080:80 nvhoanganh1909/docker-demo-user-api` 
10. open http://localhost:8080/api/values and you should be able to see the response from the API

## Step 5: Create new .NET Core Api Gateway using Ocelot
1. from the root folder (`Docker`), create new folder called `api-gateway`
2. cd into the `api-gateway` and run `dotnet new webapi` 
3. add `ocelot` nuget package
4. modify `Startup.cs` and `Program.cs` accordingly

**NOTE**:
- Ocelot uses information stored in `configuration.json` to route requests to the correct API. During development the configuration.json file under `api-gateway\configuration` 
- When we deploy the app, we will use Volume mapping to replace this file with the correct config file

## Step 6: Wiring up connections between containers
In this step, we will add a simple User management functionality to our system using ASP .NET Identity. 
1. Add ASP .NET Identity using SQL [See Instructions](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-2.2&tabs=visual-studio)
2. Add Data Seeder to create 1 user called **admin**
3. Create `UsersController.cs` which expose CRUD REST endpoints at http://localhost/api/user
5. Modify Angular UI which call the api gateway and return the list of users (calling http://localhost/api/u/users)
6. Now if you run all 3 apps at the same time (Web API + Ocelot API Gateway + Angular) you should be the app running

## Step 7: Run the whole app using Docker-compose file
1. go to the root level (`Docker` folder) and create new file called `docker-compose.yml`. This file describe how containers are started and stopped
2. run `docker-compose up` to run the app
3. run `docker-compose down` to shut down the app

## Step 8: Pushing images to Docker Hub
1. Login to docker hub by running `docker login -u nvhoanganh1909`
2. Tag the local image `docker tag f16 nvhoanganh1909/docker-demo-ui:latest` where `f16` is the first 3 characters of your image
3. Push the newly tagged image `docker push nvhoanganh1909/docker-demo-ui:latest`
4. Login to https://hub.docker.com/ and make sure you can see the new docker image
   
## Step 9: Add Kubernetes
1. From Docker for Windows Settings, turn on Kubernetes option
2. Verify Kubenetes is running by running `kubectl cluster-info` command. You should see something like this

```cmd
Kubernetes master is running at https://localhost:6445
Heapster is running at https://localhost:6445/api/v1/namespaces/kube-system/services/heapster/proxy
KubeDNS is running at https://localhost:6445/api/v1/namespaces/kube-system/services/kube-dns:dns/proxy
monitoring-grafana is running at https://localhost:6445/api/v1/namespaces/kube-system/services/monitoring-grafana/proxy
monitoring-influxdb is running at https://localhost:6445/api/v1/namespaces/kube-system/services/monitoring-influxdb/proxy
```
3. Create new K8s namespace ([k8s-namespace](https://kubernetes.io/docs/concepts/overview/working-with-objects/namespaces/)) by running `kubectl create namespace dev`
4. Register user context by running `kubectl config set-context dev --namespace=dev --cluster=docker-for-desktop-cluster --user=docker-for-desktop`
5. Switch to dev context `kubectl config use-context dev`
6. Create resources defined in the `k8s.yml` in `dev` context by running `kubectl apply -f k8s.yml` 
7. Run `kubectl get all` ande make sure all services and pods are running
8. Browse http://localhost:30004/ and make sure you can see the app running
9. At anytime you can run `kubectl delete daemonsets,replicasets,services,deployments,pods,rc --all --namespace=dev` to remove all created resources in the dev namespace
10. Install `nginx ingress` by running `kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/master/deploy/mandatory.yaml` then run `kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/master/deploy/provider/cloud-generic.yaml`
11. 


## Step 8: Automate CI/CD using Azure DevOps

## Step 9: Deploy to Azure Container Service and Google Kubernetes Engine using Terraform

https://www.hanselman.com/blog/HowToSetUpKubernetesOnWindows10WithDockerForWindowsAndRunASPNETCore.aspx