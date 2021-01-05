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
    [Table("Balls")]
    public class Balls
    {
        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BallsID { get; set; }

        [Required]
        public TypeBalls Type { get; set; }

        [Required]
        public float BitValue { get; set; }

        [Required]
        public float DonationValue { get; set; }

        [Required]
        public decimal MarketProbability { get; set; }

        [Required]
        public int SoldAmount { get; set; }

        [Required]
        [JsonIgnore]
        public bool IsActive { get; set; }

        [Required]
        public TypeLegacy Legacy { get; set; }

        public enum TypeLegacy
        {
            normal,
            rare,
            legendary,
            mythical
        }

        public enum TypeBalls
        {
            none,
            baseball, // 1
            basketball, // 2
            football, // 3
            golf, // 4
            pokeball, // 5
            soccer, // 6
            tennis, // 7
            volleyball, // 8
            captain, // 9
            mercurio, // 10
            thanos, // 11
            woman, // 12
            fireball, // 13
            eight, // 14
            moon, // 15

        }
    }
}
