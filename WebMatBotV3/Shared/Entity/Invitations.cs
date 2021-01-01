using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebMatBotV3.Shared.Entity
{
    [Table("Invitations")]
    public class Invitations
    {
        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvitationsID { get; set; }

        [Required]
        public string Host { get; set; }

        [Required]
        public string Invited { get; set; }

        [JsonIgnore]
        public int? ScoreID { get; set; }
        [JsonIgnore]
        [ForeignKey("ScoreID")]
        public Scores Score { get; set; }

    }
}
