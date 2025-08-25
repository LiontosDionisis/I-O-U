using System.ComponentModel.DataAnnotations;

namespace Api.IOU.Models;

public class Friendship
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int FriendId { get; set; }

    [Required]
    public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;

    public User user { get; set; } = null!;
    public User Friend { get; set; } = null!;

}