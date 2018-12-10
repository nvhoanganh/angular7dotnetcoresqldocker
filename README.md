# Dockerizing Angular 7, .NET Core Web API with EF core and SQL server using VSCODE on Windows 10 Pro

## Step 1: Install Docker for Windows
1. Install Docker for Windows at [https://docs.docker.com/docker-for-windows/install/](https://docs.docker.com/docker-for-windows/install/)
2. Run Docker in Linux mode

## Step 2: Create Angular 7 docker container image
1. install latest angular-cli using `npm i -g @angular/cli`
2. create new folder called `Docker` somewhere on your computer
3. inside `Docker` folder create new Angular App by using `ng new angular-ui` command
5. make the sure app works by running `ng s --open` inside the `angular-ui` folder
6. inside the `angular-ui` folder, add new file `nginx.conf`
   
```json
worker_processes  1;

events {
    worker_connections  1024;
}

http {
    server {
        listen 80;
        server_name  localhost;

        root   /usr/share/nginx/html;
        index  index.html index.htm;
        include /etc/nginx/mime.types;

        gzip on;
        gzip_min_length 1000;
        gzip_proxied expired no-cache no-store private auth;
        gzip_types text/plain text/css application/json application/javascript application/x-javascript text/xml application/xml application/xml+rss text/javascript;

        location / {
            try_files $uri $uri/ /index.html;
        }
    }
}
```
5. add new `Dockerfile` in `angular-ui` folder

```Dockerfile
FROM nginx:alpine
COPY nginx.conf /etc/nginx/nginx.conf
WORKDIR /usr/share/nginx/html
COPY dist/angular-ui/ .
```

6. Build prod using `ng build --prod` , this will create `dist\angular-ui` folder
7. create new docker image by running `docker image build . -t newangular7app` command inside of the angular-ui folder. **Note**:
   1. `.` represent the Path to the `Dockerfile`.
   2. `newangular7app` is the name and the tag of the docker image (you can change to anything you want)
   3. you can run this command from root (`Docker` folder) by specifying the `-f` parameter: 
    > `docker build -f angular-ui/Dockerfile angular-ui -t newangular7app`
8. check to make sure image is create by running `docker image ls`
9.  run the newly created image by running `docker run -d --rm -p 4200:80 newangular7app` where
   1.  `-d` will make the container run in the background
   2.  `--rm` will remove the container when the it stops
10. open http://localhost:4200 , you should see the Angular app is running
11. view the running docker container by running `docker ps`
12. stop the running container by running `docker container stop 0f` where `0f` is the first 2 characters of the Container ID (http://localhost:4200 should not stop working)

## Step 3: Create Microsoft SQL Server Container Image
1. login to [https://hub.docker.com/](https://hub.docker.com/)
2. search for `microsoft SQL linux`
3. select [https://hub.docker.com/r/microsoft/mssql-server-linux/](https://hub.docker.com/r/microsoft/mssql-server-linux/)
4. check instructions on how to use this image
5. now pull down the latest version of this image by running `docker pull microsoft/mssql-server-linux:latest`
6. start the container using SQLExpress mode by running `docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Password1' -e 'MSSQL_PID=Express' -p 1433:1433 -d microsoft/mssql-server-linux:latest` (change `SA_PASSWORD` to something else)
7. you now can connect to this SQL server using your prefered tool.

## Step 4: Create new .NET Core WebApi

### Create new .NET Core Web Api using dotnet cli
1. from the root folder (`Docker`), create new folder called `dotnetcore-api` using `mkdir dotnetcore-webapi`
2. cd into the `dotnetcore-webapi` and run `dotnet new webapi` 
3. edit the `Start.cs` file and comment out `app.UseHsts();` and `app.UseHttpsRedirection();` line (we don't need to worry about SSL in this tutorial)
4. run the app using `dotnet run`
5. navigate to http://localhost:5000/api/values and you should get 

```json
[
"value1",
"value2"
]
```

### Add Dockerfile
1. edit `dotnetcore-webapi.csproj` and `<DockerDefaultTargetOS>Linux</DockerDefaultTargetOS>` element under `<TargetFramework>netcoreapp2.1</TargetFramework>` element
2. in the dotnetcore-webapi folder create `Dockerfile`
3. copy the following content into this Dockerfile (see official [docker doc](https://docs.docker.com/engine/examples/dotnetcore/#create-a-dockerfile-for-an-aspnet-core-application))

```markdown
FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "dotnetcore-webapi.dll"]
```
4. In the same folder add `.dockerignore` file and copy the following content
```json
bin\
obj\
```
5. Build the docker image by running `docker build . -t dotnetcore-webapi` from the `dotnetcore-webapi` folder
6. now run the webapi by running `docker run -d -p 8080:80 dotnetcore-webapi` 
7. open http://localhost:8080/api/values and you should be able to see the response from the API

## Step 5: Put it all together using Docker-compose file

### Add the Angular App
1. First verify and all 3 docker containers are running by running `docker ps` command. You should see something like this
```cmd
$ docker ps
CONTAINER ID        IMAGE                                 COMMAND                  CREATED             STATUS              PORTS
     NAMES
bce7e98b4f04        dotnetcore-webapi                     "dotnet dotnetcore-w…"   7 seconds ago       Up 6 seconds        0.0.0.0:8080->80/tcp     angry_cocks
29dc8dffcd10        microsoft/mssql-server-linux:latest   "/opt/mssql/bin/sqls…"   34 minutes ago      Up 34 minutes       0.0.0.0:1433->1433/tcp   silly_cohen
64b9cd383d32        newangular7app                        "nginx -g 'daemon of…"   36 minutes ago      Up 36 minutes       0.0.0.0:4200->80/tcp     eloquent_allen
```

2. now stop all running containers by running `docker container stop bc 29 64` where `bc, 29, 64` are the prefix of the Container IDs
3. go to the root level (`Docker` folder) and create new file called `docker-compose.yml`. This file describe how containers are started and stopped 

```YAML
version: '3.4'

services:
  dockerui:
    image: newangular7app
    build: angular-ui/.
    environment:
      NODE_ENV: production
    ports:
      - 80:80
```

4. now run `docker-compose up -d`
5. open http://localhost and make sure you can see the Angular app
6. stop the running container by running `docker-componse down`

### Add Dotnet Core Api to the docker-compose file
1. modify the `docker-compose.yml` file 

```yaml
version: '3.4'

services:
  dockerui:
    image: newangular7app
    build: angular-ui/.
    environment:
      NODE_ENV: production
    ports:
      - 80:80
  dockerwebapi:
    image: dotnetcoreapi
    build: dotnetcore-webapi/.
    environment:
      ASPNETCORE_ENVIRONMENT: Production
    ports:
      - 8080:80
 
```
2. run `docker-compose up -d` and make sure http://localhost and http://localhost:8080 works

### Add SQL Server to the docker-compose file
1. modify the `docker-compose.yml` file

```yaml
version: '3.4'

services:
  dockerui:
    image: newangular7app
    build: angular-ui/.
    environment:
      NODE_ENV: production
    ports:
      - 80:80
  dockerwebapi:
    image: dotnetcoreapi
    build: dotnetcore-webapi/.
    environment:
      ASPNETCORE_ENVIRONMENT: Production
    ports:
      - 8080:80
  db:
    image: 'microsoft/mssql-server-linux'
    environment:
      SA_PASSWORD: 'Password1'
      ACCEPT_EULA: 'Y'
      MSSQL_PID: 'Express'
    ports:
      - 1433:1433
```

2. run `docker-compose up -d` and then run `docker ps` and verify all 3 containers are running

### Add dependency 
1. modify `docker-compose.yml` file

```yaml
version: '3.4'

services:
  dockerui:
    image: newangular7app
    build: angular-ui/.
    environment:
      NODE_ENV: production
    ports:
      - 80:80
    depends_on:
      - dockerwebapi
      - db
  dockerwebapi:
    image: dotnetcoreapi
    build: dotnetcore-webapi/.
    environment:
      ASPNETCORE_ENVIRONMENT: Production
    ports:
      - 8080:80
    depends_on:
      - db
  db:
    image: 'microsoft/mssql-server-linux'
    environment:
      SA_PASSWORD: 'Password1'
      ACCEPT_EULA: 'Y'
      MSSQL_PID: 'Express'
    ports:
      - 1433:1433

```
2. run `docker-compose up -d` again and you should see the start up order is now SQL > WebApi > UI