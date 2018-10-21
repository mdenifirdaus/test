using ExportToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PaymentDuration
{
    public partial class PaymentPayables : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            var payableProvider = new PayableProvider();
            var DataSource = payableProvider.GetResultPayables().ToList();
            var CollectionsListData = from payableModel in DataSource
                                      select new PayableDurationModel
                                      {
                                          JournalID = payableModel.JournalID,
                                          JournalNo = payableModel.JournalNo,
                                          ApplicationNo=payableModel.ApplicationNo,
                                          ProcessingDays = payableProvider.CalculateProcessDay((DateTime)payableModel.PaymentDate, payableModel.CreateDate),
                                          LatePayment = payableProvider.IsPaymentlate(payableModel.CreateDate)
                                      };

            var FileName = "PaymentDurationData.xlsx";

            var memoryStream = new MemoryStream();
            CreateExcelFile.CreateExcelDocument(CollectionsListData.ToList(), memoryStream, "Payment Durations Report");

            HttpResponse httpresponse = HttpContext.Current.Response;
            httpresponse.ClearContent();
            httpresponse.ClearHeaders();
            httpresponse.Buffer = true;
            httpresponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            httpresponse.AddHeader("content-Disposition", "attachment;filename=\"" + FileName);
            httpresponse.BinaryWrite(memoryStream.ToArray());
            httpresponse.Flush();
            httpresponse.End();
        }
    }
}