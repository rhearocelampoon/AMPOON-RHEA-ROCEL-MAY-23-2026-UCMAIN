using System;

namespace FinalExam_AMPOON.Models
{
    public class InventoryBatch
    {
        public string RecordId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string Supplier { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string Checksum { get; set; }
    }
}