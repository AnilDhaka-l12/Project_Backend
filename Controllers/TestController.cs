using Microsoft.AspNetCore.Mvc;

namespace projectBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new 
        { 
            message = "API is working!",
            timestamp = DateTime.Now,
            endpoints = new[] 
            {
                "/api/test",
                "/api/users",
                "/api/products"
            }
        });
    }
    
    [HttpGet("hello")]
    public IActionResult Hello()
    {
        return Ok(new { message = "Hello from .NET API!" });
    }
}