# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5005

# Use the SDK image to build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TodoApp.csproj", "./"]
RUN dotnet restore "./TodoApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TodoApp.csproj" -c Release -o /app/build
RUN dotnet publish "TodoApp.csproj" -c Release -o /app/publish

# Use the base image to run the app
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN apt-get update && apt-get install -y curl && \
    mkdir -p /remote_debugger && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /remote_debugger && \
    chmod -R 755 /remote_debugger

ENTRYPOINT ["dotnet", "TodoApp.dll"]