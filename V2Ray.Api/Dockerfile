#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["V2Ray.Api/V2Ray.Api.csproj", "V2Ray.Api/"]
RUN dotnet restore "V2Ray.Api/V2Ray.Api.csproj"
COPY . .
WORKDIR "/src/V2Ray.Api"
RUN dotnet build "V2Ray.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "V2Ray.Api.csproj" -c Release -o /app/publish
run docker build -t v2rayapi .
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "V2Ray.Api.dll"]