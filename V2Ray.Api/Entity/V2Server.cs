using System.ComponentModel.DataAnnotations;
using V2Ray.Api.Services.SSHKeyServices.Dto;
using static V2Ray.Api.Entity.SSHKey;
using static V2Ray.Api.Services.Server.ServerService;

namespace V2Ray.Api.Entity
{

    public class Ticket : FullAuditEntity<int>
    {
        public int TicketNumber { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public List<Message> Messages { get; set; }
    }

    public enum TicketStatus
    {
        Open,
        InProgress,
        Closed
    }

    public class Message : FullAuditEntity<int>
    {
        public string Text { get; set; }
        public bool IsAdmin { get; set; }
        public User Sender { get; set; }
        public List<Attachment> Attachments { get; set; }
        public Ticket Ticket { get; set; }
        public int TicketId { get; set; }
    }

    public class Attachment
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public byte[] Data { get; set; }
        public Message Message { get; set; }
        public int MessageId { get; set; }
    }


    public class V2Server : FullAuditEntity<int>, ISoftDelete
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string Title { get; set; }
        public string IP { get; set; }
        public string Url { get; set; }
        public int Capacity { get; set; }
        public int Port { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public List<SSHKey> SSHKeys { get; set; }
        public bool Enable { get; set; }
        public string Token { get; set; }
        public int UserId { get; internal set; }
        public User User { get; set; }
        public bool HasLoadBalance { get; set; }
        public int? MultiUser { get; set; }

    }
    public class SSHKey : FullAuditEntity<int>
    {
        public DateTime ChargeDate { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }

        public bool Enable { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        public List<Order> Orders { get; set; }
        public int DurationId { get; set; }
        public int MultiUser { get; set; }
        public string Server { get; set; }
        public string? Code { get; set; }
        public int? V2Port { get; set; }
        public string? V2Guid { get; set; }
        public int V2Id { get; set; }
        public AccountType AccountType { get; set; }
    }
    public class V2Key : FullAuditEntity<int>, ISoftDelete
    {
        public string Remark { get; set; }
        public int Traffic { get; set; }
        public int ClientPort { get; set; }
        public long ExpireDate { get; set; }
        public bool State { get; set; } = true;
        public Protocol Protocol { get; set; }
        public string Key { get; set; }
        public bool IsDeleted { get; set; }
        public int V2ServerId { get; set; }
        public V2Server V2Server { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public int Port { get; internal set; }
        public int KeyId { get; set; }
    }
}