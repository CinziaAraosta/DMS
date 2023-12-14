#nullable enable
using System.Data.Common;
using System.Data.SQLite;
using DMS.Shared.Entities;

namespace uSim.WebUI.Server.Entities;

public interface IDocumentDAL
{
    List<DocumentType> GetDocumentTypes();

    List<Document> GetDocuments();
}

public class DocumentDAL : IDocumentDAL
{
    #region default instance

    private static DocumentDAL defaultInstance;

    public static DocumentDAL Default => defaultInstance ??= new DocumentDAL();

    #endregion

    #region properties

    public string DatabasePath => $"c:/DMS_Database.db3";

    public string DatabaseConnection => $"Data Source={DatabasePath}; Version=3; Journal Mode=Memory; Cache=Shared; Cache Size=2000; Synchronous=Off;";

    public List<Document> Documents = new();

    public List<DocumentType> DocumentTypes = new();

    #endregion

    #region ctor

    public DocumentDAL()
     {
        if (!TestConnection()) 
            return;

        DocumentTypes = GetDocumentTypes();
        Documents = GetDocuments();
    }

    #endregion

    #region methods

    public bool TestConnection()
    {
        try
        {
            using (var connection = new SQLiteConnection(DatabaseConnection))
            {
                connection.Open();
            }

            return true;
        }
        catch (Exception ex)
        {
                
        }

        return false;
    }
    
    public List<DocumentType> GetDocumentTypes()
    {
        if (DocumentTypes is { Count: > 0 })
            return DocumentTypes;

        var temp = new List<DocumentType>();
        try
        {
            using (var connection = new SQLiteConnection(DatabaseConnection))
            {
                connection.Open();

                var query = $"SELECT * FROM DocumentTypes";

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = query;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var a = new DocumentType();

                            if (!dr.IsDBNull(dr.GetOrdinal("Id")))
                                a.Id = dr.GetInt32(dr.GetOrdinal("Id"));
                            if (!dr.IsDBNull(dr.GetOrdinal("Name")))
                                a.Name = dr.GetString(dr.GetOrdinal("Name")).Trim();
                            if (!dr.IsDBNull(dr.GetOrdinal("Description")))
                                a.Description = dr.GetString(dr.GetOrdinal("Description"));

                            temp.Add(a);
                        }
                    }
                }
            }

            DocumentTypes = temp;
        }
        catch (Exception ex)
        {

        }

        return DocumentTypes;
    }

    public List<Document> GetDocuments()
    {
        if (Documents is { Count: > 0 })
            return Documents;

        var temp = new List<Document>();
        try
        {
            using (var connection = new SQLiteConnection(DatabaseConnection))
            {
                connection.Open();

                var query = $"SELECT * FROM Documents";

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = query;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var a = new Document();

                            if (!dr.IsDBNull(dr.GetOrdinal("Id")))
                                a.Id = dr.GetInt32(dr.GetOrdinal("Id"));
                            if (!dr.IsDBNull(dr.GetOrdinal("DocumentTypeId")))
                                a.Id = dr.GetInt32(dr.GetOrdinal("DocumentTypeId"));

                            temp.Add(a);
                        }
                    }
                }
            }

            foreach (var document in temp)
                document.DocumentType = DocumentTypes.FirstOrDefault(x => x.Id.Equals(document.DocumentTypeId));
            
            Documents = temp;
        }
        catch (Exception ex)
        {

        }

        return Documents;
    }

    public Document? GetLastReport()
    {
        return Documents
                .OrderByDescending(x => x.Id)
                .FirstOrDefault(x => x.DocumentType is { Name: "REPORT" });
    }

    public bool InsertDocuments(List<Document> documents)
    {
        try
        {
            foreach (var document in documents)
                InsertDocument(document);

            return true;
        }
        catch (Exception ex)
        {
            
        }

        return false;
    }

    public bool InsertDocument(Document document)
    {
        try
        {
            using (var connection = new SQLiteConnection(DatabaseConnection))
            {
                connection.Open();

                var query = $"INSERT OR REPLACE INTO [Documents] (" +
                        "[Id], [DocumentTypeId], [FileName], [InsertDateTime]" +
                        ") VALUES (" +
                        $"{document.Id}, {document.DocumentType.Id}, {document.FileName}, {document.InsertDateTime}" +
                        ")";

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = query;
                    var recordCount = cmd.ExecuteNonQuery();
                }
            }

            return true;
        }
        catch (Exception ex)
        {

        }

        return false;
    }

    public bool InsertDocumentsInOneTransaction(List<Document> documents)
    {
        try
        {
            var queries = new List<string>();

            foreach (var document in documents)
            {
                var q = CreateQueryForInsertDocument(document);
                queries.Add(q);
            }

            var query = string.Empty;
            using (var connection = new SQLiteConnection(DatabaseConnection))
            {
                connection.Open();

                using (DbTransaction tx = connection.BeginTransaction())
                {
                    using (DbCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = tx;

                        foreach (var q in queries)
                        {
                            query = q;
                            cmd.CommandText = query;
                            var recordCount = cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {

        }

        return false;
    }

    public string CreateQueryForInsertDocument(Document document)
    {
        return "INSERT OR REPLACE INTO [Documents] (" +
               "[Id], [DocumentTypeId], [FileName], [InsertDateTime]" +
               ") VALUES (" +
               $"{document.Id}, {document.DocumentType.Id}, {document.FileName}, {FieldToSqlString(document.InsertDateTime.ToString())}" +
               ");\n";
    }

    public static string FieldToSqlString(string? parameter)
    {
        if (string.IsNullOrEmpty(parameter))
            return "\'\'";

        return "\'" + parameter.Replace("\'", "\'\'") + "\'";
    }

    public static string DateTimeToSqlString(DateTime dt)
    {
        var formattedDate = dt.ToString("yyyy-MM-dd HH:mm:ss");
        return $"\'{formattedDate}\'";
    }

    #endregion
}