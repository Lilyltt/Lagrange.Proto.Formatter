namespace Lagrange.Proto.Formatter.Test;

[ProtoPackable]
public partial class TodoList
{
    [ProtoMember(1)]
    public List<Todo> Todos { get; set; } = new();
}
