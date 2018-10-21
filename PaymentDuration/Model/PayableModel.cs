using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentDuration
{
    class PayableModel
    {
        public int JournalID { get; set; }
        public string JournalNo { get; set; }
        public int? EditJournalID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string ApplicationNo { get; set; }
    }
}
