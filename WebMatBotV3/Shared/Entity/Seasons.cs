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
    [Table("Seasons")]
    public class Seasons
    {
        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SeasonsID { get; set; }

        public string Game { get; set; }

        public DateTime? EndDate { get; set; }

        public string VersionName { get; set; }

        public float JackPot { get; set; }
        public ICollection<Scores> Scores { get; set; }
        public ICollection<Resources> Resources { get; set; }
    }
}
