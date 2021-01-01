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
    [Table("Points")]
    public class Points
    {
        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PointsID { get; set; }

        public DateTime Date { get; set; }


        [JsonIgnore]
        [Required]
        public int ScoreID { get; set; }
        [JsonIgnore]
        [ForeignKey("ScoreID")]
        public Scores Score { get; set; }
    }
}
