using System.ComponentModel.DataAnnotations;

namespace S.Integration_Technologies.Models;

public class ArticleDocument
{
    [Key]
    public int Id { get; set; }

    public string Header { get; set; }  = string.Empty;
    public string Content { get; set; } = string.Empty;
}