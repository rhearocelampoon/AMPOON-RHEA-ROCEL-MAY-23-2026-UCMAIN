using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FinalExam_AMPOON.Models;

namespace FinalExam_AMPOON.Services
{
    public class FileRepository
    {
        private readonly string filePath = "Data/records.txt";

        public List<InventoryBatch> GetAllRecords()
        {
            List<InventoryBatch> records = new List<InventoryBatch>();

            try
            {
                if (!File.Exists(filePath))
                {
                    return records;
                }

                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    try
                    {
                        string[] data = line.Split('|');

                        if (data.Length < 9)
                            continue;

                        InventoryBatch batch = new InventoryBatch
                        {
                            RecordId = data[0],
                            ProductName = data[1],
                            Quantity = int.Parse(data[2]),
                            UnitPrice = double.Parse(data[3]),
                            Supplier = data[4],
                            CreatedAt = DateTime.Parse(data[5]),
                            UpdatedAt = DateTime.Parse(data[6]),
                            IsActive = bool.Parse(data[7]),
                            Checksum = data[8]
                        };

                        records.Add(batch);
                    }
                    catch
                    {
                        AuditLogger.Log("ERROR", "Malformed record skipped.");
                    }
                }
            }
            catch (Exception ex)
            {
                AuditLogger.Log("ERROR", ex.Message);
            }

            return records;
        }

        public void SaveAllRecords(List<InventoryBatch> records)
        {
            List<string> lines = new List<string>();

            foreach (InventoryBatch batch in records)
            {
                string line = batch.RecordId + "|" +
                              batch.ProductName + "|" +
                              batch.Quantity + "|" +
                              batch.UnitPrice + "|" +
                              batch.Supplier + "|" +
                              batch.CreatedAt + "|" +
                              batch.UpdatedAt + "|" +
                              batch.IsActive + "|" +
                              batch.Checksum;

                lines.Add(line);
            }

            File.WriteAllLines(filePath, lines);
        }

        public string GenerateChecksum(InventoryBatch batch)
        {
            string rawData = batch.RecordId + batch.ProductName + batch.Quantity + batch.UnitPrice + batch.Supplier;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }
    }
}