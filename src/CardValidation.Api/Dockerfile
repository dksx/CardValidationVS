ARG DOTNET_VERSION=8.0
ARG BASE_IMAGE=mcr.microsoft.com/dotnet/aspnet

FROM ${BASE_IMAGE}:${DOTNET_VERSION} AS base

WORKDIR /app
RUN chown -R app /app

FROM base AS final

ARG BUILD_CONFIG=Release
ARG APP_VERSION

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app
COPY ./bin/${BUILD_CONFIG}/net8.0/linux-x64/ ./

ENV ASPNETCORE_URLS=http://+:8080
ENV APP_VERSION=${APP_VERSION}
EXPOSE 8080

ENTRYPOINT ["dotnet", "CardValidation.Api.dll"]