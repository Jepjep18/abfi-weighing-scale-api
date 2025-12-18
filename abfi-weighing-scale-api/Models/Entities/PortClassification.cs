using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace abfi_weighing_scale_api.Models.Entities
{
    public class PortClassification
    {
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string PortNumber { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Class { get; set; }
    }
}
