namespace DMS.Shared.Entities;

public class Document
{
    public int Id { get; set; }
    
    public int DocumentTypeId { get; set; }
    
    public string DocumentTypeName { get; set; }

    public DocumentType DocumentType { get; set; }

    public string FileName { get; set; }

    public DateTime InsertDateTime { get; set; }
}
