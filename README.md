Lagrange.Proto Adapter For ASP.NET

Example Usage
```csharp
var protobufOptions = new ProtobufFormatterOptions
        {
            SupportedContentTypes = { "application/x-protobuf" }
        };
        builder.Services.AddControllers()
            .AddGeneratedProtobufFormatters(protobufOptions);
```