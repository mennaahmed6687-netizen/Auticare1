using System;

namespace auticare.core
{
    public class AIResult
    {
        public int Id { get; set; }

        // =========================
        // Foreign Key
        // =========================
        public int ChildId { get; set; }

        // =========================
        // AI Prediction Result
        // =========================
        public bool Has_Asd { get; set; }

        public double Probability { get; set; }

        public int Aq_Score { get; set; }

        public string Message { get; set; }

        // =========================
        // Severity Level
        // =========================
        public string Severity_Level { get; set; }

        // =========================
        // Created Date
        // =========================
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
