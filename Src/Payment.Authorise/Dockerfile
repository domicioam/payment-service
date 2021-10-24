FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Payment.Authorise/Payment.Authorise.csproj", "Payment.Authorise/"]
COPY ["Payment.Communication.RabbitMq/Payment.Communication.RabbitMq.csproj", "Payment.Communication.RabbitMq/"]
RUN dotnet restore "Payment.Authorise/Payment.Authorise.csproj"
COPY . .
WORKDIR "/src/Payment.Authorise"
RUN dotnet build "Payment.Authorise.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Payment.Authorise.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.Authorise.dll"]
