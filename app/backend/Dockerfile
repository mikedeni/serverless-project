# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["ConstructionSaaS.Api.csproj", "./"]
RUN dotnet restore "ConstructionSaaS.Api.csproj"

# Copy the rest of the code and build
COPY . .
RUN dotnet build "ConstructionSaaS.Api.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "ConstructionSaaS.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create directory for uploads
RUN mkdir -p wwwroot/uploads

EXPOSE 5154
ENV ASPNETCORE_URLS=http://+:5154
ENTRYPOINT ["dotnet", "ConstructionSaaS.Api.dll"]
