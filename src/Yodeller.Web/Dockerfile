FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

RUN mkdir /out

# Install necessary dependencies
RUN apt update && apt -y upgrade

RUN apt install -y python3 ffmpeg curl

RUN curl -L https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp -o /usr/local/bin/yt-dlp
RUN chmod a+rx /usr/local/bin/yt-dlp

# Uninstall curl which was only used to install yt-dlp
RUN apt remove -y curl

FROM node:latest AS client
WORKDIR /client
COPY Client/package.json .
RUN npm install
COPY Client .
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src
COPY ["src/Yodeller.Web/Yodeller.Web.csproj", "src/Yodeller.Web/"]
RUN dotnet restore "src/Yodeller.Web/Yodeller.Web.csproj"
COPY . .

WORKDIR "/src/src/Yodeller.Web"
RUN rm wwwroot/*
COPY --from=client "/client/dist" "wwwroot"

RUN dotnet build "Yodeller.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Yodeller.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Yodeller.Web.dll"]
