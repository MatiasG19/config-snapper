FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0@sha256:72dd743782f2ae7e5476fd64f6a460045e3998dc862218b80e6944cba79a01b0 AS build
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

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/runtime-deps:11.0@sha256:6d3e208d8f5ba599ad9e80c615b5c1a2e355fd903f585cb07342dde29441675a AS runtime

WORKDIR /app

RUN apt-get update && apt-get install -y git nano && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/out .

ENTRYPOINT ["./ConfigSnapperConsole"]

