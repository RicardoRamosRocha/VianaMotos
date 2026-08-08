namespace VianaMotos.Web.ViewModels;

public sealed class PageHeaderViewModel
{
    public required string Eyebrow { get; init; }
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public string Icon { get; init; } = "bi-grid";
    public IReadOnlyList<PageHeaderActionViewModel> Actions { get; init; } = [];
}

public sealed class PageHeaderActionViewModel
{
    public required string Text { get; init; }
    public required string Url { get; init; }
    public string Icon { get; init; } = "bi-arrow-right";
    public bool IsPrimary { get; init; }
    public string? Target { get; init; }
}
