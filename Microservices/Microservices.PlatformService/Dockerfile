#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Microservices/Microservices.PlatformService/Microservices.PlatformService.csproj", "Microservices/Microservices.PlatformService/"]
RUN dotnet restore "Microservices/Microservices.PlatformService/Microservices.PlatformService.csproj"
COPY . .
WORKDIR "/src/Microservices/Microservices.PlatformService"
RUN dotnet build "Microservices.PlatformService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Microservices.PlatformService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Microservices.PlatformService.dll"]