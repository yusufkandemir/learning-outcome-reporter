FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS debug

# Install Debugger
RUN apt-get update && \
    apt-get install -y unzip && \
    curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg

WORKDIR /src

COPY server.csproj .
RUN dotnet restore

COPY . .

CMD ["dotnet", "watch", "run", "--no-restore", "--urls=http://+:5000"]

# Build & Publish Stage

FROM debug AS publish

RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish --no-restore

# Final Stage

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS final

EXPOSE 80

WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "server.dll"]
