using FinalExam_AMPOON.Models;

namespace FinalExam_AMPOON.Services
{
    public static class ValidationService
    {
        public static bool Validate(InventoryBatch batch, out string error)
        {
            error = "";

            if (string.IsNullOrWhiteSpace(batch.ProductName))
            {
                error = "Product Name is required.";
                return false;
            }

            if (batch.Quantity <= 0)
            {
                error = "Quantity must be greater than zero.";
                return false;
            }

            if (batch.UnitPrice <= 0)
            {
                error = "Unit Price must be greater than zero.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(batch.Supplier))
            {
                error = "Supplier is required.";
                return false;
            }

            return true;
        }
    }
}