using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserDatabasePlugin.Database
{
    public class UserActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string UserName { get; set; }

        [StringLength(32)]
        public string Type { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}