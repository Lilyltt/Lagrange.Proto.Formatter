using System;
using System.Collections.Concurrent;
using System.Reflection;
using Lagrange.Proto.Serialization;

namespace Lagrange.Proto.Formatter;

internal static class ProtoHelper
{
    private static readonly ConcurrentQueue<SegmentBufferWriter> BufferPool = new();

    public static void Serialize<T>(ref BinaryPacket dest, T value) where T : IProtoSerializable<T>
    {
        if (!BufferPool.TryDequeue(out var writer))
        {
            writer = new SegmentBufferWriter();
        }

        ProtoSerializer.SerializeProtoPackable(writer, value);
        writer.WriteTo(ref dest);
        writer.Clear();

        BufferPool.Enqueue(writer);
    }

    public static ReadOnlyMemory<byte> Serialize<T>(T value) where T : IProtoSerializable<T>
    {
        if (!BufferPool.TryDequeue(out var writer))
        {
            writer = new SegmentBufferWriter();
        }

        ProtoSerializer.SerializeProtoPackable(writer, value);
        var result = writer.CreateReadOnlyMemory();
        writer.Clear();
        BufferPool.Enqueue(writer);

        return result;
    }

    public static T Deserialize<T>(ReadOnlySpan<byte> src) where T : IProtoSerializable<T> =>
        ProtoSerializer.DeserializeProtoPackable<T>(src);

    public static object Deserialize(Type type, ReadOnlyMemory<byte> src)
    {
        ArgumentNullException.ThrowIfNull(type);
        var method = typeof(ProtoHelper).GetMethod(nameof(Deserialize), BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(ReadOnlyMemory<byte>) }, null);
        if (method == null) throw new InvalidOperationException("Deserialize(ReadOnlyMemory<byte>) overload not found");
        var generic = method.MakeGenericMethod(type);
        return generic.Invoke(null, [src])!;
    }
}
