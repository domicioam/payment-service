FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Payment.Refund/Payment.Refund.csproj", "Payment.Refund/"]
RUN dotnet restore "Payment.Refund/Payment.Refund.csproj"
COPY . .
WORKDIR "/src/Payment.Refund"
RUN dotnet build "Payment.Refund.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Payment.Refund.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.Refund.dll"]
