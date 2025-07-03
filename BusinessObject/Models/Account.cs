using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("account")]
    public class Account
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }

        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [Column("gender")]
        public bool Gender { get; set; }

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
        public int? DistrictId { get; set; }  

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("device_id")]
        public string? DeviceId { get; set; }  

        [Column("total_point")]
        public int TotalPoint { get; set; } 

        [Column("achievement_id")]
        public int? AchievementId { get; set; } 

        [Column("is_logged_in")]
        public bool IsLoggedIn { get; set; }

        [Column("account_status")]
        public string Status { get; set; } 

        [Column("refresh_token")]
        public string? RefreshToken { get; set; }  

        [Column("refresh_token_expiry")]
        public DateTime? RefreshTokenExpiry { get; set; }

        [Column("activation_code")]
        public string? ActivationCode { get; set; }

        [Column("code_expiry")]
        public DateTime? CodeExpiry { get; set; }

        [Column("is_biometric_enabled")]
        public bool IsBiometricEnabled { get; set; }


        public Role Role { get; set; }
        public District? District { get; set; }
        public Achievement? Achievement { get; set; }
        public CitizenIdentityCard? CitizenIdentityCard { get; set; }
        public ICollection<Subscription>? Subscriptions { get; set; }
        public ICollection<Payment>? Payments { get; set; }
    }
}
