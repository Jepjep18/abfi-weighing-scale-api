using abfi_weighing_scale_api.Controllers.WeighingDetailsController;
using abfi_weighing_scale_api.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace abfi_weighing_scale_api.Services.WeighingDataProcessorService
{
    public interface IWeighingDataProcessor
    {
        Task<ProcessedWeighingDataDto> ProcessSerialDataAsync(string serialData, string portNumber);
    }

    public class WeighingDataProcessor : IWeighingDataProcessor
    {
        private readonly AppDbContext _context;

        public WeighingDataProcessor(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProcessedWeighingDataDto> ProcessSerialDataAsync(string serialData, string portNumber)
        {
            const string uom = "KG";
            var uomPos = serialData.IndexOf(uom);
            decimal qty = 0;
            string remarks = null;

            // Extract quantity (same logic as SQL GetProdClass procedure)
            if (uomPos > 0)
            {
                var qtyString = serialData.Substring(0, uomPos).Trim();
                decimal.TryParse(qtyString, NumberStyles.Any, CultureInfo.InvariantCulture, out qty);
            }

            // Get class from PortClassification
            var portClass = await _context.PortClassifications
                .Where(p => p.PortNumber == portNumber)
                .Select(p => p.Class)
                .FirstOrDefaultAsync();

            // Validation (same as SQL procedure)
            if (string.IsNullOrEmpty(portClass))
            {
                remarks = "Invalid Port";
            }
            else if (qty == 0)
            {
                remarks = "Invalid Qty";
            }

            // Get product classification
            var prodClassification = await _context.ProdClassifications
                .Where(p => qty >= p.TotalIndvWeight_Min &&
                           qty <= p.TotalIndvWeight_Max &&
                           p.Class == portClass)
                .FirstOrDefaultAsync();

            return new ProcessedWeighingDataDto
            {
                Qty = qty,
                UoM = uom,
                NumHeads = prodClassification?.NumHeads,
                ProdCode = prodClassification?.ProdCode,
                Class = portClass,
                Remarks = remarks
            };
        }
    }
}
