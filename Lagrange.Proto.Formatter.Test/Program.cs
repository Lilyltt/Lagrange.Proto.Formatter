using Lagrange.Proto.Formatter.Generated;

namespace Lagrange.Proto.Formatter.Test;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var protobufOptions = new ProtobufFormatterOptions
        {
            SupportedContentTypes = { "application/x-protobuf" }
        };
        builder.Services.AddControllers()
            .AddGeneratedProtobufFormatters(protobufOptions);

        var app = builder.Build();

        app.MapControllers();

        app.Run();
    }
}