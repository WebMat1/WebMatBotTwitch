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
    [Table("Scores")]
    public class Scores
    {
        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScoresID { get; set; }

        public string Username { get; set; }

        public int Points { get; set; }

        public int PointIntraDay { 
            get
            {
                var count = RecordPoints?.Where(q => q.Date.Date == DateTime.Today).Count();
                return (count == null) ? 0 : count.Value;
            } 
        }

        public int Tries { get; set; }

        [JsonIgnore]
        [Required]
        public int SeasonID { get; set; }
        [JsonIgnore]
        [ForeignKey("SeasonID")]
        public Seasons Season { get; set; }

        [JsonIgnore]
        public ICollection<Points> RecordPoints { get; set; }

        [JsonIgnore]
        public float HitRate
        {
            get
            {
                return ((float)Points / (float)Tries) * 100f;
            }
        }

    }
}
