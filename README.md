# SujaySarma.Sdk.RestApi
Provides a way to interact with REST API services. Uses a shared HttpClient and provides methods to GET, PUT, POST, ... and perform all actions normally done using REST API services. There are also classes for different types of common results returned by the service.

This project has no dependencies on anything that is not a part of the .NET Core SDK/runtime.

---

## Usage
Adds ability to instantiate the client in a number of ways. Two key changes are:

1. Set the request timeout value. RestApiClient.`RequestTimeout` property. This is in **seconds**. You may set different values prior to each call since the property value is attached to the underlying HttpClient on every request. 

2. You can now use non-Json request body types (like form-encoded, etc). But, you need to use the `CallApiMethod` method. The `Get()`, `Post()` and other methods are simply wrappers on this method anyway. When using the `CallApiMethod` method, use its new `contentType` parameter to set the desired content type. This defaults to `application/json`.

Get latest from https://www.nuget.org/packages/SujaySarma.Sdk.RestApi/latest.

*NOTE:* As of v6.2.0, all methods are now `async`.

Release 6.2.0 added Fluid-style initialization. Use v6.2.2 or higher to use this type of code:
```
RestApiClient client = RestApiClient.CreateBuilder()
  .WithBearerToken("akjhakjsdhkahsd")
  .WithHeader("Custom-Header", "Value")
  .WithRequestUri("https://my.fancy.api");

HttpResponseMessage reply = await client.Get();
```

---
