using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace abfi_weighing_scale_api.Models.Entities
{
    public class WeighingDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string SerialData { get; set; }

        [Column(TypeName = "numeric(18,3)")]
        public decimal? Qty { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string UoM { get; set; }

        public int? Heads { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string ProdCode { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string PortNumber { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Class { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Remarks { get; set; }
    }
}
