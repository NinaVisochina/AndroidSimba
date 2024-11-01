# spd221-asp

Create docker hub repository - publish
```
docker build -t spd221-asp-api . 
docker run -it --rm -p 5391:8080 --name spd221-asp_container spd221-asp-api
docker run -d --restart=always --name spd221-asp_container -p 5391:8080 spd221-asp-api
docker run -d --restart=always -v d:/volumes/spd221-asp/images:/app/images --name spd221-asp_container -p 5391:8080 spd221-asp-api
docker run -d --restart=always -v /volumes/spd221-asp/images:/app/images --name spd221-asp_container -p 5391:8080 spd221-asp-api
docker ps -a
docker stop spd221-asp_container
docker rm spd221-asp_container

docker images --all
docker rmi spd221-asp-api

docker login
docker tag spd221-asp-api:latest novakvova/spd221-asp-api:latest
docker push novakvova/spd221-asp-api:latest

docker pull novakvova/spd221-asp-api:latest
docker ps -a
docker run -d --restart=always --name spd221-asp_container -p 5391:8080 novakvova/spd221-asp-api

docker run -d --restart=always -v /volumes/spd221-asp/images:/app/images --name spd221-asp_container -p 5391:8080 novakvova/spd221-asp-api


docker pull novakvova/spd221-asp-api:latest
docker images --all
docker ps -a
docker stop spd221-asp_container
docker rm spd221-asp_container
docker run -d --restart=always --name spd221-asp_container -p 5391:8080 novakvova/spd221-asp-api
```

```nginx options /etc/nginx/sites-available/default
server {
    server_name   api-spd221-asp.itstep.click *.api-spd221-asp.itstep.click;
    location / {
       proxy_pass         http://localhost:5391;
       proxy_http_version 1.1;
       proxy_set_header   Upgrade $http_upgrade;
       proxy_set_header   Connection keep-alive;
       proxy_set_header   Host $host;
       proxy_cache_bypass $http_upgrade;
       proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
       proxy_set_header   X-Forwarded-Proto $scheme;
    }
}

server {
		server_name   qubix.itstep.click *.qubix.itstep.click;
		root /var/dist;
		index index.html;

		location / {
			try_files $uri /index.html;
			#try_files $uri $uri/ =404;
		}
}

server {
		server_name   admin-qubix.itstep.click *.admin-qubix.itstep.click;
		root /var/admin-qubix.itstep.click;
		index index.html;

		location / {
			try_files $uri /index.html;
			#try_files $uri $uri/ =404;
		}
}

sudo systemctl restart nginx
certbot
```

/var/api-qubix.itstep.click/