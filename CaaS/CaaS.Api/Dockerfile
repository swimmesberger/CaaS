FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CaaS.Api/CaaS.Api.csproj", "CaaS.Api/"]
RUN dotnet restore "CaaS.Api/CaaS.Api.csproj"
COPY . .
WORKDIR "/src/CaaS.Api"
RUN dotnet build "CaaS.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CaaS.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CaaS.Api.dll"]
