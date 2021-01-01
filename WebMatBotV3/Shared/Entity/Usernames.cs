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
    [Table("Usernames")]
    public class Usernames
    {
        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsernamesID { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public int User_id {get;set;}
    }
}
