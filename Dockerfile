FROM mcr.microsoft.com/dotnet/nightly/aspnet:7.0-jammy-chiseled AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
RUN apt-get update && \
    apt-get install -y curl && \
    apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx  && \
    curl -sL https://deb.nodesource.com/setup_lts.x | bash - && \
    apt-get install -y nodejs && \
    npm install --global yarn
COPY ["src/Ari.Client/Ari.Client.csproj", "src/Ari.Client/"]
RUN dotnet restore "src/Ari.Client/Ari.Client.csproj"
COPY . .
WORKDIR "/src/src/Ari.Client"
RUN dotnet build "Ari.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ari.Client.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ari.Client.dll"]
