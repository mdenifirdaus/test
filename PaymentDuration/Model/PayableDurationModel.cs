using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentDuration
{
    class PayableDurationModel
    {
        public int JournalID { get; set; }
        public string JournalNo { get; set; }
        public string LatePayment { get; set; }
        public string ProcessingDays { get; set; } //before is int
        public string ApplicationNo { get; set; }
    }
}
