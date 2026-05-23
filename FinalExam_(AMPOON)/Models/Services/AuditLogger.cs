using System;
using System.IO;

namespace FinalExam_AMPOON.Services
{
    public static class AuditLogger
    {
        private static readonly string logPath = "Data/audit.log";

        public static void Log(string action, string details)
        {
            try
            {
                string log = DateTime.Now + " | " + action + " | " + details;
                File.AppendAllText(logPath, log + Environment.NewLine);
            }
            catch
            {
                Console.WriteLine("Failed to write audit log.");
            }
        }
    }
}