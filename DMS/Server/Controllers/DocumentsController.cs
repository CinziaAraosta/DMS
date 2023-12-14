using DMS.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Server.Controllers;
[ApiController]
[Route("[controller]")]
public class DocumentController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<DocumentController> _logger;

    public DocumentController(ILogger<DocumentController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public Document? Get()
    {
        return new Document
        {
            Id = 1, 
            DocumentType = new DocumentType
            {
                Id = 1, 
                Name = "REPORT"
            }, 
            InsertDateTime = DateTime.Now,
            FileName = "cinzia.png"
        };
    }
}
