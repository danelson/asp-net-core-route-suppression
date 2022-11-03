# ASP.NET Core Route Suppression

## Motivation

It is common to have an ASP.NET Core application that utilizes auto-instrumentation, specifically enabling `OpenTelemetry.Instrumentation.AspNetCore` and `OpenTelemetry.Instrumentation.Http`. The API can expose status endpoints (e.g. `/status/live`) that are invoked by something like a Kubernetes liveness probe. Often times, the `/status/live` endpoint may make external http calls to validate dependencies. It is sometimes desirable to suppress all spans that are created from calling `/status/live`.

Filtering the root span via `OpenTelemetry.Instrumentation.AspNetCore.AspNetCoreInstrumentationOptions.Filter` is possible but that causes orphaned auto instrumented http spans. `.OpenTelemetry.Instrumentation.HttpHttpClientInstrumentationOptions` also provides a Filter method but there is not an obvious way how to determine if they are children of `/status/live` and not some other desired endpoint.

Custom `Sampler`s execute before the ASP.NET auto instrumentation and the span name is always of the form `Microsoft.AspNetCore.Hosting.HttpRequestIn` with no way to distinguish from any other endpoints.

## Solution

This repository provides an example for suppressing all spans related to a specific endpoint. Filtering happens twice because `OpenTelemetry.Instrumentation.AspNetCore` is still executing after the `SpanSuppressionMiddleware` returns.

1. As part of the `OpenTelemetry.Instrumentation.AspNetCore.AspNetCoreInstrumentationOptions.Filter`. This is needed to suppress the span created by `OpenTelemetry.Instrumentation.AspNetCore`.
    - Unfortunately `OpenTelemetry.Instrumentation.AspNetCore.AspNetCoreInstrumentationOptions.Filter` executes after `SpanSuppressionMiddleware`
2. As part of a custom middleware `SpanSuppressionMiddleware` which uses `HttpContextHelper` to determine which endpoints to filter. The middleware suppresses all descendants of the span created by `OpenTelemetry.Instrumentation.AspNetCore` by using `OpenTelemetry.SuppressInstrumentationScope`. A custom middleware is used to provide the simplest mechanism for managing the `IDisposable` created by `OpenTelemetry.SuppressInstrumentationScope`.

It would be ideal if this type of advanced suppression had first class support via the OpenTelemetry specification.
