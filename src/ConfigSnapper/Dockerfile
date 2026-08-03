FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:72dd743782f2ae7e5476fd64f6a460045e3998dc862218b80e6944cba79a01b0 AS build
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -r linux-x64 --self-contained -p:PublishAot=true -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0@sha256:53d7e74e5178092627d7f5d7aa7cb27eee50054885d99ee902014b3c11382efa AS runtime
RUN apt-get update && \
    apt-get install -y git && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./ConfigSnapper"]
