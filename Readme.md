# Introduction

![Build Status](https://circleci.com/gh/dnutiu/retroactiune.svg?style=svg)

The following project uses ASP .Net Core 3.1

```bash
dotnet --version
3.1.407
```

This is a side project and it's still work in progress, therefore the lack of documentation. The goal is to create a Web Application for managing Feedback.

## Architecture

Example deployment architecture which uses [Prometheus](https://prometheus.io/) & [Grafana](https://grafana.com/) for monitoring, [Auth0](https://auth0.com/) as a authorization server
and [Sentry](https://sentry.io/welcome/) for error reporting.

![Example deployment architecture](./docs/deploy_architecture.png)

The application code is organized using the [Clean Architecture](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#clean-architecture) approach.

![Example deployment architecture](./docs/app_architecture_layers.png)
