# Base stage for running from Visual Studio in fast mode (Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8082
EXPOSE 8081

# Ensure the apk directory exists and set appropriate permissions for the app user
RUN mkdir -p /app/wwwroot/apk && chown -R app:app /app/wwwroot/apk

# Switch to the app user after setting up permissions
USER app

# Build stage for building the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MealMentor.API/MealMentor.API.csproj", "MealMentor.API/"]
COPY ["MealMentor.Core/MealMentor.Core.csproj", "MealMentor.Core/"]
RUN dotnet restore "./MealMentor.API/MealMentor.API.csproj"
COPY . .
WORKDIR "/src/MealMentor.API"
RUN dotnet build "./MealMentor.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage for copying the published project to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MealMentor.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage for running in production or when running from Visual Studio
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Entrypoint for the application
ENTRYPOINT ["dotnet", "MealMentor.API.dll"]
