# Develop Stage
FROM node:lts-alpine as develop
RUN npm install -g @quasar/cli

WORKDIR /app

CMD yarn && quasar dev

# Build Stage
FROM develop as build

COPY package.json ./
COPY yarn.lock ./
RUN yarn

COPY . .

RUN quasar build

# Production stage
FROM nginx:stable-alpine as production
COPY --from=build /app/dist/spa /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
