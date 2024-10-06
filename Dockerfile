# Use the official .NET 8 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory inside the container
WORKDIR /app

# Copy the .csproj file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code and build the project
COPY . ./
RUN dotnet publish -c Release -o out

# Build a runtime image with .NET 8 runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory for the runtime image
WORKDIR /app

# Copy the build output from the build stage
COPY --from=build-env /app/out .

# Expose the application's port (modify according to your app)
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "ClimateStory.dll"]
