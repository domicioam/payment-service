FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Payment.Capture/Payment.Capture.csproj", "Payment.Capture/"]
RUN dotnet restore "Payment.Capture/Payment.Capture.csproj"
COPY . .
WORKDIR "/src/Payment.Capture"
RUN dotnet build "Payment.Capture.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Payment.Capture.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.Capture.dll"]
