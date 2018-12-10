# Running Angular 7, .NET Core Web API with EF core and SQL server in Docker container on Windows 10 Pro

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