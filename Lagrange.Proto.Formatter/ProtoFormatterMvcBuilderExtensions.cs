using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

namespace Lagrange.Proto.Formatter;

public static class ProtobufFormatterMvcBuilderExtensions
{
    public static IMvcBuilder AddProtobufFormatterFor<T>(this IMvcBuilder builder, ProtobufFormatterOptions options)
        where T : IProtoSerializable<T>
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        builder.AddMvcOptions(o =>
        {
            o.InputFormatters.Add(new ProtobufInputFormatter<T>(options));
            o.OutputFormatters.Add(new ProtobufOutputFormatter<T>(options));
        });

        return builder;
    }
}