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
    [Table("Resources")]
    public class Resources
    {
        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResourcesID { get; set; }

        public string Username { get; set; }

        public int Balls { get; set; }

        [JsonIgnore]
        [Required]
        public int SeasonID { get; set; }

        [JsonIgnore]
        [ForeignKey("SeasonID")]
        public Seasons Season { get; set; }


    }
}
