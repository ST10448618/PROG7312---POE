FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish SmartX.Client/SmartX.Client.csproj -c Release -o /app

FROM nginx:alpine
COPY --from=build /app/wwwroot /usr/share/nginx/html
COPY SmartX.Client/nginx.conf /etc/nginx/conf.d/default.conf