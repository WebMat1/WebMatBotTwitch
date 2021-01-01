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
    [Table("Records")]
    public class Records
    {
        public enum ActionType
        {
            Shot,
            Buy,
            SubRedemption,
            Spray,
        }


        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordsID { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public ActionType Action { get; set; }

        public string Arguments { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [JsonIgnore]
        public int? ResourceID { get; set; }
        [JsonIgnore]
        [ForeignKey("ResourceID")]
        public Resources Resource { get; set; }

    }
}
