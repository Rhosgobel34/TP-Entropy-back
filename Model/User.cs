using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP_Entropy_back.Model
{
    [Table("Users")]
    public class User
    {
        [Column("UserId")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Username")]
        [Index (IsUnique = true)]
        [Required]
        public string Username { get; set; }

        [Column("Password")]
        [Required]
        public string Password { get; set; }

        [Column("Email")]
        [Required]
        public string Email { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("LastUpdated")]
        public DateTime LastUpdated { get; set; }
    }
}
