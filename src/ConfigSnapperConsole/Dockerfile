TAG=0.0.1
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /ConfigSnapper

COPY *.csproj ./
RUN dotnet restore

COPY . ./

WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./

RUN dotnet publish -r ${TARGETOS}-${TARGETARCH} \
          -p:Version=${TAG} \
          -p:InternalVersion=${TAG} \
          -p:PublishSingleFile=true \
          --self-contained true \
          /p:NativeAot=true \
          -o /app/out \
          ConfigSnapperConsole.csproj

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0 AS runtime

WORKDIR /app

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "ConfigSnapperConsole.dll"]
