FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AuthorizeService/AuthorizeService.csproj", "AuthorizeService/"]
RUN dotnet restore "AuthorizeService/AuthorizeService.csproj"
COPY . .
WORKDIR "/src/AuthorizeService"
RUN dotnet build "AuthorizeService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthorizeService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthorizeService.dll"]
