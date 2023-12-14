using DMS.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using uSim.WebUI.Server.Entities;

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

    private readonly IDocumentDAL _service;

    public DocumentController(ILogger<DocumentController> logger, IDocumentDAL service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public Document? Get()
    {
        var lastReport = _service.GetLastReport();
        if (lastReport is null)
        {
            lastReport = new Document
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
        
        // test per valutare l'inserimento a database, da rimuovere
        var list = new List<Document>();
        var id = lastReport.Id +1 ;
        var doc = new Document
        {
            Id = id, DocumentType = lastReport.DocumentType, InsertDateTime = DateTime.Now,
            FileName = $"AutomaticallyInserted_{id}"
        };
        list.Add(doc);
        _service.InsertDocuments(list);
        ////////////////////////////////////////////////////////////

        return lastReport;
    }

    [HttpPost]
    public bool Post(List<Document> documents)
    {
        var success = _service.InsertDocuments(documents);
        return success;
    }
}
