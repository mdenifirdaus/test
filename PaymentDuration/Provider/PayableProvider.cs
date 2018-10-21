using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentDuration
{
    class PayableProvider
    {
        static List<PayableModel> payables = GetAllPayables().ToList();

        static IEnumerable<PayableModel> GetAllPayables()
        {
            using (var conn = new OracleConnection("DATA SOURCE=10.30.1.15:1521/tmiorcl;PASSWORD=69nu4Yk2099vl4wv6L41M2uX3i2D0B;USER ID=IMART_SYSTEM"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT JOURNAL_ID, JOURNAL_NO, EDIT_SOURCE_JOURNAL_ID, CREATE_APPLICATION_NO, CREATE_DATE FROM FMS_FC_JRNL WHERE JOURNAL_CLASS = 1311";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new PayableModel
                            {
                                JournalID = Convert.ToInt32(reader["JOURNAL_ID"]),
                                EditJournalID = reader["EDIT_SOURCE_JOURNAL_ID"] == DBNull.Value ?
                                                (int?)null : Convert.ToInt32(reader["EDIT_SOURCE_JOURNAL_ID"]),
                                CreateDate = Convert.ToDateTime(reader["CREATE_DATE"]),
                                ApplicationNo = Convert.ToString(reader["CREATE_APPLICATION_NO"])
                            };
                        }
                    }
                }
            }
        }

        static PayableModel FindOrigin(int editId)
        {
            var childPayable = payables.FirstOrDefault(p => p.JournalID == editId);
            if (childPayable.EditJournalID == null)
            {
                return childPayable;
            }
            return FindOrigin((int)childPayable.EditJournalID);
        }

        public IEnumerable<PayableModel> GetValidPayables()
        {
            using (var conn = new OracleConnection("DATA SOURCE=10.30.1.15:1521/tmiorcl;PASSWORD=69nu4Yk2099vl4wv6L41M2uX3i2D0B;USER ID=IMART_SYSTEM"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT a.JOURNAL_ID, a.EDIT_SOURCE_JOURNAL_ID, a.CREATE_DATE, c.POST_DATE, a.CREATE_APPLICATION_NO
                                        FROM FMS_FC_JRNL a
                                        JOIN TMI_ISD_JRNL_PYMNT_UNEDITED c
                                          ON a.JOURNAL_ID = c.BASE_JOURNAL_ID
                                        WHERE a.JOURNAL_CLASS = 1311";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new PayableModel
                            {
                                JournalID = Convert.ToInt32(reader["JOURNAL_ID"]),
                                EditJournalID = reader["EDIT_SOURCE_JOURNAL_ID"] == DBNull.Value ?
                                                (int?)null : Convert.ToInt32(reader["EDIT_SOURCE_JOURNAL_ID"]),
                                CreateDate = Convert.ToDateTime(reader["CREATE_DATE"]),
                                PaymentDate = reader["POST_DATE"] == DBNull.Value ?
                                              (DateTime?)null : Convert.ToDateTime(reader["POST_DATE"]),
                                ApplicationNo = Convert.ToString(reader["CREATE_APPLICATION_NO"])
                            };
                        }
                    }
                }
            }
        }

        public IEnumerable<PayableModel> GetResultPayables()
        {
            var payables = GetValidPayables();

            var results = new List<PayableModel>();

            foreach (var payable in payables)
            {
                if (payable.EditJournalID == null)
                {
                    results.Add(payable);    
                }
                else
                {
                    var originPayable = FindOrigin((int)payable.EditJournalID);

                    payable.CreateDate = originPayable.CreateDate;
                    results.Add(payable);
                }
            }
            return results;
        }

        public string IsPaymentlate(DateTime time)
        {
            var baseTime = new TimeSpan(16, 0, 0);
            var Result = time.TimeOfDay > baseTime ? "Late" : "Not Late";
            return Result;
        }

        public string CalculateProcessDay(DateTime paymentDate, DateTime payableDate)
        {
            var s = paymentDate.Subtract(payableDate);
            return String.Format("{0} Days", s.Days);
        }
    }
}
