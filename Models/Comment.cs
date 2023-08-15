#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _8_The_Wall.Models;
public class Comment
{
    [Key]
    public int CommentId {get;set;}

    [Required]
    [MaxLength(45, ErrorMessage = "Please keep message under 45 characters")]
    public string CommentText {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    public int MessageId {get;set;}
    public Message? Messeage {get;set;}

    public int UserId {get;set;}
    public User? User {get;set;}
}