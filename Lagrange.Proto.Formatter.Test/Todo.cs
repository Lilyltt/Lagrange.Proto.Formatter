namespace Lagrange.Proto.Formatter.Test;

[ProtoPackable]
public partial class Todo
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string Title { get; set; } = string.Empty;

    [ProtoMember(3)]
    public string? Date { get; set; }
}