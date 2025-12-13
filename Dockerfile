# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file to the correct location
COPY ["Lost_&_Found_Tracking_System/Lost_&_Found_Tracking_System.sln", "Lost_&_Found_Tracking_System/"]

# Copy project files
COPY ["Lost_&_Found_Tracking_System/Core/Core.csproj", "Lost_&_Found_Tracking_System/Core/"]
COPY ["Lost_&_Found_Tracking_System/Infrastructure/Infrastructure.csproj", "Lost_&_Found_Tracking_System/Infrastructure/"]
COPY ["Lost_&_Found_Tracking_System/WebAPI/WebAPI.csproj", "Lost_&_Found_Tracking_System/WebAPI/"]

# Restore dependencies
WORKDIR "/src/Lost_&_Found_Tracking_System"
RUN dotnet restore "Lost_&_Found_Tracking_System.sln"

# Copy everything else and build
COPY ["Lost_&_Found_Tracking_System/", "Lost_&_Found_Tracking_System/"]
WORKDIR "/src/Lost_&_Found_Tracking_System/WebAPI"
RUN dotnet build "WebAPI.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
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
