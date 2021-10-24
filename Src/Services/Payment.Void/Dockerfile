FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Payment.Void/Payment.Void.csproj", "Payment.Void/"]
RUN dotnet restore "Payment.Void/Payment.Void.csproj"
COPY . .
WORKDIR "/src/Payment.Void"
RUN dotnet build "Payment.Void.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Payment.Void.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.Void.dll"]
