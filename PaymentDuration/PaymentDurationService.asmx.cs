using Newtonsoft.Json;
using PaymentDuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace PaymentDuration
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class PaymentDurationService : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string GetPayableDatas()
        {
            var providerData = new PayableProvider();
            var echo = int.Parse(HttpContext.Current.Request.Params["sEcho"]);
            var displayLength = int.Parse(HttpContext.Current.Request.Params["iDisplayLength"]);
            var displayStart = int.Parse(HttpContext.Current.Request.Params["iDisplayStart"]);
            var sortOrder = HttpContext.Current.Request.Params["sSortDir_0"].ToString(CultureInfo.CurrentCulture);
            var sortBy = int.Parse(HttpContext.Current.Request.Params["iSortCol_0"]);
            //var searchTerm = HttpContext.Current.Request.Params["sSearch"].ToString(CultureInfo.CurrentCulture);

            var recordData = providerData.GetResultPayables();
                //.Where(p=>   Convert.ToString(p.JournalID).Contains(searchTerm) ||
                //    Convert.ToString(p.JournalNo).Contains(searchTerm) ||
                //    Convert.ToString(p.EditJournalID).Contains(searchTerm)
                //    );

            var recordCount = recordData.Count();

            if (recordData == null)
            {
                return string.Empty;
            }

            switch (sortBy)
            {
                case 1:
                    recordData =
                        sortOrder == "asc" ?
                        recordData.OrderBy(p => p.JournalID) :
                        recordData.OrderByDescending(p => p.JournalID);
                    break;
                case 3:
                    recordData =
                         sortOrder == "asc" ?
                         recordData.OrderBy(p => p.JournalNo) :
                         recordData.OrderByDescending(p => p.JournalNo);
                    break;
                default:
                    recordData =
                        sortOrder == "asc" ?
                        recordData.OrderBy(p => p.JournalNo) :
                        recordData.OrderByDescending(p => p.JournalNo);
                    break;
            }

            var pageResult = recordData.Skip(displayStart).Take(displayLength).ToList();

            return JsonConvert.SerializeObject(new
            {
                sEcho = echo,
                recordsTotal = recordCount,
                recordsFiltered = recordCount,
                iTotalRecords = recordCount,
                iTotalDisplayRecords = recordCount,

                aaData = from payableModel in pageResult
                         select new PayableDurationModel
                         {
                             JournalID = payableModel.JournalID,
                             JournalNo = payableModel.JournalNo,
                             ApplicationNo = payableModel.ApplicationNo,
                             ProcessingDays = providerData.CalculateProcessDay((DateTime)payableModel.PaymentDate, payableModel.CreateDate),
                             LatePayment = providerData.IsPaymentlate(payableModel.CreateDate)
                         }
            });
        }
    }
}
