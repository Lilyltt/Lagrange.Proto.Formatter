using Microsoft.AspNetCore.Mvc;

namespace Lagrange.Proto.Formatter.Test;

[ApiController]
[Route("todos")]
public class TodoController : ControllerBase
{
    private static readonly Todo[] SampleTodos = new[]
    {
        new Todo
        {
            Id = 1,
            Title = "购买日用品",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)).ToString()
        },
        new Todo
        {
            Id = 2,
            Title = "完成项目报告",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(5)).ToString()
        },
        new Todo
        {
            Id = 3,
            Title = "预约医生",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(3)).ToString()
        },
        new Todo
        {
            Id = 4,
            Title = "准备团队会议资料",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)).ToString()
        },
        new Todo
        {
            Id = 5,
            Title = "修复Bug #123",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(7)).ToString()
        },
        new Todo
        {
            Id = 6,
            Title = "阅读技术文档",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(4)).ToString()
        },
        new Todo
        {
            Id = 7,
            Title = "联系客户确认需求",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(6)).ToString()
        }
    };


    [HttpGet]
    [Produces("application/x-protobuf")]
    public TodoList GetAll()
    {
        return new TodoList { Todos = SampleTodos.ToList() };
    }

    [HttpGet("{id}")]
    [Produces("application/x-protobuf")]
    public ActionResult<Todo> GetById(int id)
    {
        var todo = SampleTodos.FirstOrDefault(a => a.Id == id);
        return todo is null ? NotFound() : Ok(todo);
    }
}
