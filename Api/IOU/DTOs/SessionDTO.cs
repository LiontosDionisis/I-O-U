using api.IOU.Models;

namespace Api.IOU.DTOs;

public class SessionDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<ParticipantDTO> Participants { get; set; } = new List<ParticipantDTO>();
}