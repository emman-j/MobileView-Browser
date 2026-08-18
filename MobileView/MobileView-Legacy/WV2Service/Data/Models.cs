namespace MobileView.WV2Service.Data;
public class HistoryEntry
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public string Title { get; set; } = "";
    public int VisitCount { get; set; }
    public DateTime? LastVisited { get; set; }
}
public class BookmarkFolder
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int? ParentId { get; set; }
}
public class BookmarkEntry
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public string Title { get; set; } = "";
    public int? FolderId { get; set; }
    public string FolderPath { get; set; } = "";
}
public class AddressSuggestion
{
    public string Url { get; set; } = "";
    public string Title { get; set; } = "";
    public override string ToString() => string.IsNullOrWhiteSpace(Title) ? Url : $"{Title} — {Url}";
}