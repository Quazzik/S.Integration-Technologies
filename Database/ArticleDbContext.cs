using Microsoft.EntityFrameworkCore;
using S.Integration_Technologies.Models;

namespace S.Integration_Technologies.Database;

public class ArticleDbContext : DbContext
{
    public virtual DbSet<ArticleDocument> ArticleDocuments { get; set; }

    public ArticleDbContext(DbContextOptions<ArticleDbContext> options) : base(options)
    {
        base.Database.SetCommandTimeout(30);
    }
}