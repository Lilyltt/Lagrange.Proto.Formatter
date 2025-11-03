using Microsoft.AspNetCore.Mvc.Formatters;

namespace Lagrange.Proto.Formatter;

public class ProtobufOutputFormatter<T> : OutputFormatter where T : IProtoSerializable<T>
{
    private readonly ProtobufFormatterOptions _options;

    public ProtobufOutputFormatter(ProtobufFormatterOptions protobufFormatterOptions)
    {
        _options = protobufFormatterOptions ?? throw new ArgumentNullException(nameof(protobufFormatterOptions));
        foreach (var contentType in _options.SupportedContentTypes)
        {
            SupportedMediaTypes.Add(contentType);
        }
    }

    protected override bool CanWriteType(Type type)
    {
        return type == typeof(T);
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
    {
        var response = context.HttpContext.Response;

        if (context.Object == null)
        {
            await response.Body.FlushAsync();
            return;
        }

        var value = (T)context.Object;
        var memory = ProtoHelper.Serialize<T>(value); // ReadOnlyMemory<byte>
        var data = memory.ToArray(); // 转为 byte[] 以兼容所有流 API

        await response.Body.WriteAsync(data, 0, data.Length);
    }
}