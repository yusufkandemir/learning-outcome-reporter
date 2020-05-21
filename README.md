![Outcome Reporter logo](client/src/assets/logo_text.png)

# Outcome Reporter

An app for managing courses, storing grades and generating outcome reports

## Project Structure
This project is a monorepo which contains 3 packages, namely _client, server and database_. This project is containerized with Docker, every package contains a `Dockerfile`.

- Client: [Quasar Framework](https://quasar.dev)
- Server: [C# .Net Core](https://docs.microsoft.com/en-us/dotnet/core/)
- Database: SQL Server

## Quick Setup

You will need `docker` and `docker-compose` installed. If they are installed, you can skip to [Setup Steps](#setup-steps).

### Install Docker/CE (Community Edition)

Follow the instructions here:

https://docs.docker.com/get-docker/

### Install Docker Compose

Follow the instructions here:

https://docs.docker.com/compose/install/

### Setup Steps

1. Make sure the docker daemon is running. Also nothing should be running on `8080` and `8081` ports.
2. Run `docker-compose up -d` from the root folder. Docker will also build the images when running for the first time.
3. You can access the `client` from `http://localhost:8080`. You can always take a look at [docker-compose.yml](docker-compose.yml) file for more information.

You can refer to [Wiki](https://github.com/yusufkandemir/learning-outcome-reporter/wiki) for user documentation and more information.
