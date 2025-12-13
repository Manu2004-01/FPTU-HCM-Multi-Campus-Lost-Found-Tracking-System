# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ["Lost_&_Found_Tracking_System/Lost_&_Found_Tracking_System.sln", "./"]

# Copy project files for better layer caching
COPY ["Lost_&_Found_Tracking_System/Core/Core.csproj", "Core/"]
COPY ["Lost_&_Found_Tracking_System/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Lost_&_Found_Tracking_System/WebAPI/WebAPI.csproj", "WebAPI/"]

# Restore dependencies
RUN dotnet restore "Lost_&_Found_Tracking_System.sln"

# Copy all source code
COPY ["Lost_&_Found_Tracking_System/Core/", "Core/"]
COPY ["Lost_&_Found_Tracking_System/Infrastructure/", "Infrastructure/"]
COPY ["Lost_&_Found_Tracking_System/WebAPI/", "WebAPI/"]

# Build solution
RUN dotnet build "Lost_&_Found_Tracking_System.sln" -c Release --no-restore

# Stage 2: Publish
FROM build AS publish
WORKDIR "/src/WebAPI"
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published app
COPY --from=publish /app/publish .

# Create wwwroot/images directories if they don't exist
RUN mkdir -p /app/wwwroot/images/lostitems && \
    mkdir -p /app/wwwroot/images/claims

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the app
ENTRYPOINT ["dotnet", "WebAPI.dll"]
