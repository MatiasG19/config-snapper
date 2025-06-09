FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -r linux-x64 --self-contained -p:PublishAot=true -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0 AS runtime
RUN apt-get update && \
    apt-get install -y git && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./ConfigSnapper"]
