using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FinalExam_AMPOON.Models;
using FinalExam_AMPOON.Services;

namespace FinalExam_AMPOON
{
    internal class Program
    {
        static FileRepository repository = new FileRepository();

        static void Main(string[] args)
        {
            InitializeStorage();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== INVENTORY BATCH SYSTEM ===\n");
                Console.WriteLine("1. Add Record");
                Console.WriteLine("2. View Records");
                Console.WriteLine("3. Search Record");
                Console.WriteLine("4. Update Record");
                Console.WriteLine("5. Soft Delete");
                Console.WriteLine("6. Hard Delete");
                Console.WriteLine("7. Generate Report");
                Console.WriteLine("8. Exit");

                Console.Write("\nChoose option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddRecord(); break;
                    case "2": ViewRecords(); break;
                    case "3": SearchRecord(); break;
                    case "4": UpdateRecord(); break;
                    case "5": SoftDelete(); break;
                    case "6": HardDelete(); break;
                    case "7": GenerateReport(); break;
                    case "8": return;
                    default: Console.WriteLine("Invalid option."); Pause(); break;
                }
            }
        }

        static void InitializeStorage()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            if (!File.Exists("Data/records.txt"))
                File.Create("Data/records.txt").Close();

            if (!File.Exists("Data/audit.log"))
                File.Create("Data/audit.log").Close();
        }

        static void AddRecord()
        {
            try
            {
                List<InventoryBatch> records = repository.GetAllRecords();
                InventoryBatch batch = new InventoryBatch();

                batch.RecordId = "REC" + (records.Count + 1).ToString("000");

                Console.Write("Product Name: ");
                batch.ProductName = Console.ReadLine();

                Console.Write("Quantity: ");
                int quantity;
                if (!int.TryParse(Console.ReadLine(), out quantity))
                {
                    Console.WriteLine("Invalid quantity.");
                    Pause();
                    return;
                }
                batch.Quantity = quantity;

                Console.Write("Unit Price: ");
                double price;
                if (!double.TryParse(Console.ReadLine(), out price))
                {
                    Console.WriteLine("Invalid price.");
                    Pause();
                    return;
                }
                batch.UnitPrice = price;

                Console.Write("Supplier: ");
                batch.Supplier = Console.ReadLine();

                batch.CreatedAt = DateTime.Now;
                batch.UpdatedAt = DateTime.Now;
                batch.IsActive = true;
                batch.Checksum = repository.GenerateChecksum(batch);

                string error;
                if (!ValidationService.Validate(batch, out error))
                {
                    Console.WriteLine(error);
                    AuditLogger.Log("ERROR", error);
                    Pause();
                    return;
                }

                records.Add(batch);
                repository.SaveAllRecords(records);
                AuditLogger.Log("ADD", batch.RecordId);

                Console.WriteLine("Record added successfully.");
            }
            catch (Exception ex)
            {
                AuditLogger.Log("ERROR", ex.Message);
                Console.WriteLine("Error adding record.");
            }
            Pause();
        }

        static void ViewRecords()
        {
            try
            {
                List<InventoryBatch> records = repository.GetAllRecords();
                Console.WriteLine("\n=== ACTIVE RECORDS ===");
                foreach (InventoryBatch r in records)
                {
                    if (r.IsActive)
                    {
                        Console.WriteLine(r.RecordId + " | " + r.ProductName + " | Qty: " + r.Quantity + " | Price: " + r.UnitPrice + " | Supplier: " + r.Supplier);
                    }
                }
                AuditLogger.Log("READ", "Viewed records");
            }
            catch (Exception ex)
            {
                AuditLogger.Log("ERROR", ex.Message);
                Console.WriteLine("Error viewing records.");
            }
            Pause();
        }

        static void SearchRecord()
        {
            Console.Write("Enter product name keyword: ");
            string keyword = Console.ReadLine();

            List<InventoryBatch> records = repository.GetAllRecords();
            Console.WriteLine("\n=== SEARCH RESULTS ===");

            foreach (InventoryBatch r in records)
            {
                if (r.IsActive && r.ProductName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Console.WriteLine(r.RecordId + " | " + r.ProductName + " | Qty: " + r.Quantity + " | Price: " + r.UnitPrice + " | Supplier: " + r.Supplier);
                }
            }

            AuditLogger.Log("SEARCH", keyword);
            Pause();
        }

        static void UpdateRecord()
        {
            try
            {
                List<InventoryBatch> records = repository.GetAllRecords();

                Console.Write("Enter Record ID to update: ");
                string id = Console.ReadLine();

                InventoryBatch record = records.FirstOrDefault(r => r.RecordId == id);

                if (record == null)
                {
                    Console.WriteLine("Record not found.");
                    Pause();
                    return;
                }

                Console.Write("New Product Name (leave blank to keep current): ");
                string newName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newName))
                    record.ProductName = newName;

                Console.Write("New Quantity (leave blank to keep current): ");
                string qtyInput = Console.ReadLine();
                int newQty;
                if (!string.IsNullOrWhiteSpace(qtyInput) && int.TryParse(qtyInput, out newQty))
                    record.Quantity = newQty;

                Console.Write("New Unit Price (leave blank to keep current): ");
                string priceInput = Console.ReadLine();
                double newPrice;
                if (!string.IsNullOrWhiteSpace(priceInput) && double.TryParse(priceInput, out newPrice))
                    record.UnitPrice = newPrice;

                Console.Write("New Supplier (leave blank to keep current): ");
                string newSupplier = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newSupplier))
                    record.Supplier = newSupplier;

                record.UpdatedAt = DateTime.Now;
                record.Checksum = repository.GenerateChecksum(record);

                repository.SaveAllRecords(records);
                AuditLogger.Log("UPDATE", id);

                Console.WriteLine("Record updated successfully.");
            }
            catch (Exception ex)
            {
                AuditLogger.Log("ERROR", ex.Message);
                Console.WriteLine("Error updating record.");
            }
            Pause();
        }

        static void SoftDelete()
        {
            try
            {
                List<InventoryBatch> records = repository.GetAllRecords();

                Console.Write("Enter Record ID to soft delete: ");
                string id = Console.ReadLine();

                InventoryBatch record = records.FirstOrDefault(r => r.RecordId == id);

                if (record == null)
                {
                    Console.WriteLine("Record not found.");
                    Pause();
                    return;
                }

                record.IsActive = false;
                repository.SaveAllRecords(records);
                AuditLogger.Log("SOFT DELETE", id);

                Console.WriteLine("Record soft deleted successfully.");
            }
            catch (Exception ex)
            {
                AuditLogger.Log("ERROR", ex.Message);
                Console.WriteLine("Error soft deleting record.");
            }
            Pause();
        }

        static void HardDelete()
        {
            try
            {
                List<InventoryBatch> records = repository.GetAllRecords();

                Console.Write("Enter Record ID to hard delete: ");
                string id = Console.ReadLine();

                int removed = records.RemoveAll(r => r.RecordId == id);
                if (removed > 0)
                {
                    repository.SaveAllRecords(records);
                    AuditLogger.Log("HARD DELETE", id);
                    Console.WriteLine("Record permanently deleted.");
                }
                else
                {
                    Console.WriteLine("Record not found.");
                }
            }
            catch (Exception ex)
            {
                AuditLogger.Log("ERROR", ex.Message);
                Console.WriteLine("Error hard deleting record.");
            }
            Pause();
        }

        static void GenerateReport()
        {
            try
            {
                List<InventoryBatch> records = repository.GetAllRecords();
                ReportGenerator.Generate(records);
                AuditLogger.Log("REPORT", "Generated report");
            }
            catch (Exception ex)
            {
                AuditLogger.Log("ERROR", ex.Message);
                Console.WriteLine("Error generating report.");
            }
            Pause();
        }

        static void Pause()
        {
            Console.WriteLine("\nPress ENTER to continue...");
            Console.ReadLine();
        }
    }
}