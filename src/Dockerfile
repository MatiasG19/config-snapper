FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG TAG=0.0.1 
ARG TARGETARCH

WORKDIR /app
COPY ConfigSnapper/*.csproj ./ConfigSnapper/
COPY ConfigSnapperConsole/*.csproj ./ConfigSnapperConsole/
COPY Directory.Build.props .

RUN dotnet restore -a $TARGETARCH ConfigSnapperConsole/ConfigSnapperConsole.csproj

COPY . .

RUN dotnet publish -a $TARGETARCH \
            -p:Version=$TAG \
            -p:InternalVersion=$TAG \
            -p:PublishSingleFile=true \
            --self-contained true \
            /p:NativeAot=true \
            -o /app/out \
            ConfigSnapperConsole/ConfigSnapperConsole.csproj

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS runtime

WORKDIR /app

RUN apt-get update && apt-get install -y git nano && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/out .

ENTRYPOINT ["./ConfigSnapperConsole"]
