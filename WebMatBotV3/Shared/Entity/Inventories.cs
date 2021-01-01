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
    [Table("Inventories")]
    public class Inventories
    {
        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InventoriesID { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public bool IsUsing { get; set; }


        [JsonIgnore]
        [Required]
        public int BallID { get; set; }

        [JsonIgnore]
        [ForeignKey("BallID")]
        public Balls Ball { get; set; }
    }
}
