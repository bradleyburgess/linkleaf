# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything in one go
COPY . .

# Restore and publish
RUN dotnet restore src/LinkLeaf.Api/LinkLeaf.Api.csproj
RUN dotnet publish src/LinkLeaf.Api/LinkLeaf.Api.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Bind to PORT in code (not Dockerfile)
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "LinkLeaf.Api.dll"]

