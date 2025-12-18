// Update your ProdClassification.cs entity
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace abfi_weighing_scale_api.Models.Entities
{
    public class ProdClassification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string ProdCode { get; set; }

        [Column(TypeName = "numeric(18,3)")]
        public decimal? IndvWeight_Min { get; set; }

        [Column(TypeName = "numeric(18,3)")]
        public decimal? IndvWeight_Max { get; set; }

        [Column(TypeName = "numeric(18,3)")]
        public decimal? TotalIndvWeight_Min { get; set; }

        [Column(TypeName = "numeric(18,3)")]
        public decimal? TotalIndvWeight_Max { get; set; }

        [Column(TypeName = "numeric(18,3)")]
        public decimal? CratesWeight_Min { get; set; }

        [Column(TypeName = "numeric(18,3)")]
        public decimal? CratesWeight_Max { get; set; }

        public int? NumHeads { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Class { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string UoMAll { get; set; }
    }
}