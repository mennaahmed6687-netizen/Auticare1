using System;

namespace auticare.core.DTO
{
    public class AiPredictionResultDto
    {
        // هل يوجد توحد
        public bool Has_Asd { get; set; }

        // نسبة الاحتمال (0 → 1 أو 0 → 100 حسب الـ API)
        public double Probability { get; set; }

        // مجموع الأسكور
        public int Aq_Score { get; set; }

        // رسالة النتيجة
        public string Message { get; set; }

        // مستوى الشدة
        public string Severity_Level { get; set; }

        // مهم: لا تعتمد عليه من الـ API (نحذفه من التحليل)
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}