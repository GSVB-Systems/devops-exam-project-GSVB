# 1. Create a stage for building the application.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# 2. Copy everything in current directory (server/) into /source on build container
COPY . /source
# 3. Change container working directory to /source
WORKDIR /source
# 4. Build the application with Release configuration and for linux-x64 since Fly
# runs. (output folder er nu en mappe, ikke root)
RUN dotnet publish ./Api/DevOpsAppApi.csproj --configuration Release --no-self-contained -o /source/publish

# 5. Create a new stage for running the application with minimal runtime
# dependencies.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
# 6. Change container working directory to /app
WORKDIR /app
# 7. Copy everything needed to run the app from the "build" stage.
COPY --from=build /source/publish .
# 8. Switch to a non-privileged user (defined in the base image) that the app will run under.
USER $APP_UID
# 9. Describe what port the server will listen on.
EXPOSE 8080
# 10. Start Api
CMD ["dotnet", "DevOpsAppApi.dll"]