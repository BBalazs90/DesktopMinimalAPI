namespace DesktopMinimalAPI.Core.Models.Dtos;
internal sealed class WmRequestDto
{
    public string? RequestId { get; set; }
    public string? Method { get; set; }
    public string? Path { get; set; }
    public string? Body { get; set; }
}
