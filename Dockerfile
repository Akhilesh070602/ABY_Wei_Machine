FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj from folder
COPY ["WebApplicationTest/WebApplicationTest.csproj", "WebApplicationTest/"]

# Restore
RUN dotnet restore "WebApplicationTest/WebApplicationTest.csproj"

# Copy everything
COPY . .

# Move into project folder
WORKDIR "/src/WebApplicationTest"

# Build
RUN dotnet build "WebApplicationTest.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "WebApplicationTest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApplicationTest.dll"]