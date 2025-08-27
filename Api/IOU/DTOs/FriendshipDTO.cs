using Api.IOU.Models;

namespace Api.IOU.DTOs;

public class FriendshipDTO
{
    public int Id { get; set; }
    public int FriendId { get; set; }
    public string FriendUsername { get; set; } = null!;
    public FriendshipStatus Status { get; set; }
}