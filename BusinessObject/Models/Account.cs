using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("account")]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public Guid Id { get; set; } 

        [Column("email")]
        public string Email { get; set; } 

        [Column("password_hash")]
        public string PasswordHash { get; set; }  

        [Column("phone")]
        public string Phone { get; set; }  

        [Column("image_url")]
        public string? ImageUrl { get; set; } 

        [Column("role_id")]
        public int RoleId { get; set; }     

        [Column("district_id")]
        public int DistrictId { get; set; }  

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("device_id")]
        public string DeviceId { get; set; }  

        [Column("total_point")]
        public int TotalPoint { get; set; } 

        [Column("achievement_id")]
        public int? AchievementId { get; set; } 

        [Column("is_active")]
        public bool IsActive { get; set; }  

        [Column("status")]
        public string Status { get; set; } 

        [Column("refresh_token")]
        public string? RefreshToken { get; set; }  

        [Column("refresh_token_expiry")]
        public DateTime? RefreshTokenExpiry { get; set; }  



        public Role Role { get; set; }
        public District District { get; set; }
        public Achievement Achievement { get; set; }
        public CitizenIdentityCard CitizenIdentityCard { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
