using DMS.Shared.Entities;

namespace DMS.Client.Models;

public class DocumentModel
{
    public Document Document { get; set; } = new();
    
    public string DocumentTypeName { get; set; }
}
