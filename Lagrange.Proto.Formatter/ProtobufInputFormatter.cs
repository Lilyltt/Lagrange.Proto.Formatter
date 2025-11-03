using Microsoft.AspNetCore.Mvc.Formatters;

namespace Lagrange.Proto.Formatter
{
    public class ProtobufInputFormatter<T> : InputFormatter where T : IProtoSerializable<T>
    {
        private readonly ProtobufFormatterOptions _options;

        public ProtobufInputFormatter(ProtobufFormatterOptions protobufFormatterOptions)
        {
            _options = protobufFormatterOptions ?? throw new ArgumentNullException(nameof(protobufFormatterOptions));

            foreach (var contentType in _options.SupportedContentTypes)
            {
                SupportedMediaTypes.Add(contentType);
            }
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(T);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            using var ms = new MemoryStream();
            await context.HttpContext.Request.Body.CopyToAsync(ms);
            var bytes = ms.ToArray();

            var result = ProtoHelper.Deserialize<T>(bytes);
            return await InputFormatterResult.SuccessAsync(result);
        }
    }
}