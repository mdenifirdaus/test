<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaymentPayables.aspx.cs" Inherits="PaymentDuration.PaymentPayables" MasterPageFile="~/Site.Master" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server" ID="maincontent1" >
      <div class="im-row im-bar-title inline">
        <h3>Payment Durations</h3>
    </div>
    
    <div class="im-row im-section">
        <asp:Button Text="Export Excel" OnClick="btnExportExcel_Click" ID="btnExportExcel" runat="server" />
    </div>

    <div class="im-row im-section">
        <table id="tablePayment" class="table tableMain" style="width:100%; font-size:12px">
            <thead>
                <tr>
                    <th>Journal Id</th>
                    <th>Journal No</th>
                    <th>Application No</th>
                    <th>Processing (Days)</th>
                    <th>Late Payment</th>
                </tr>
            </thead>
        </table>
    </div>

    <script>
        $(document).ready(function () {

            var PaymentTableName = $('#tablePayment');

            TablePaymentData();

            function TablePaymentData() {

                //$.extend($.fn.dataTable.defaults, {
                //    language: {
                //        processing: "Please Wait.."
                //    }
                //});

                PaymentTableName.DataTable({
                    filter: true,
                    pagingType: "simple_numbers",
                    order: [[0, "asc"]],
                    info: true,
                    searching: false,
                    processing: true,
                    pageLength: 10,
                    bLengthChange: false,
                    serverSide: true,
                    ajaxSource: "PaymentDurationService.asmx/GetPayableDatas",
                    serverData: function (sSource, aoData, fnCallback) {
                        $.ajax({
                            dataType: 'json',
                            contentType: "application/json; charset=utf-8",
                            type: 'GET',
                            url: sSource,
                            data: aoData,
                            success: function (msg) {
                                var json = jQuery.parseJSON(msg.d);
                                fnCallback(json);
                            },
                            error: function (xhr, textStatus, error) {
                                if (typeof console == "object") {
                                    console.log(xhr.status + "," + xhr.responseText + "," + textStatus + "," + error);
                                }
                            }
                        });
                    },
                    columnDefs: [
                        {
                            targets: [0],
                            data: 'JournalID',
                            className: 'text-right',
                            width: '150px'
                        },
                        {
                            targets: [1],
                            data: 'JournalNo',
                            width: '150px'
                        },
                        {
                            targets: [2],
                            data: 'ApplicationNo',
                            width: '150px'
                        },
                        {
                            targets: [3],
                            data: 'ProcessingDays',
                            className: 'text-right',
                            width: '150px'
                        },
                        {
                            targets: [4],
                            data: 'LatePayment',
                            width: '200px'
                        }
                    ]
                });
            }
        })
    </script>

</asp:Content>
