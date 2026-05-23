using System;
using System.Collections.Generic;
using System.Linq;
using FinalExam_AMPOON.Models;

namespace FinalExam_AMPOON.Services
{
    public static class ReportGenerator
    {
        public static void Generate(List<InventoryBatch> records)
        {
            var activeRecords = records.Where(r => r.IsActive).ToList();

            Console.WriteLine("\n===== INVENTORY REPORT =====");
            Console.WriteLine("Total Active Records: " + activeRecords.Count);

            if (activeRecords.Count > 0)
            {
                var highestStock = activeRecords.OrderByDescending(r => r.Quantity).First();
                Console.WriteLine("Highest Stock Product: " + highestStock.ProductName);

                double totalValue = activeRecords.Sum(r => r.Quantity * r.UnitPrice);
                Console.WriteLine("Total Inventory Value: " + totalValue);
            }
        }
    }
}