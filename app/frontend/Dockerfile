# Build Stage
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm install

# Copy source code and build
COPY . .
RUN npm run build

# Production Stage (Nginx)
FROM nginx:stable-alpine
COPY --from=build /app/dist /usr/share/nginx/html

# Custom Nginx config to handle SPA routing
RUN printf "server {\n  listen 80;\n  location / {\n    root /usr/share/nginx/html;\n    index index.html;\n    try_files \$uri \$uri/ /index.html;\n  }\n  location /api {\n    proxy_pass http://backend:5154;\n  }\n  location /uploads {\n    proxy_pass http://backend:5154/uploads;\n  }\n}\n" > /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
