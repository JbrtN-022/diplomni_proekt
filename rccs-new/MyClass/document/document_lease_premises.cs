using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Word = Microsoft.Office.Interop.Word;

namespace rccs_new.MyClass.document
{
    internal class document_lease_premises
    {
        public static bool CreateLeasePremises(
        string templatePath,
        string filePath,
        int printMode,
        string numContract,
        string counterparty,
        string counterpartName,
        string dateFrom,
        string dateUntil,
        string dateConclusion,
        string companyName,
        string actualadress,
        string cops,
        string workerName,
        string floor,
        string office,
        decimal square,
        decimal pricePerM2,
        decimal price,
        decimal totalPrice,
        string bic,
        string inn,
        string correspondentAccount,
        string paymentAccount)
        {
            Word.Application wordApp = null;
            Word.Document doc = null;

            try
            {
                wordApp = new Word.Application();
                wordApp.Visible = false;

                doc = wordApp.Documents.Open(templatePath);

                void SetBookmark(string name, string value)
                {
                    if (doc.Bookmarks.Exists(name))
                    {
                        Word.Range range = doc.Bookmarks[name].Range;
                        range.Text = value ?? "";
                        doc.Bookmarks.Add(name, range);
                    }
                }


                SetBookmark("ContractNumber", numContract);
                SetBookmark("Date", $"{dateUntil} ");
                SetBookmark("Landlord", companyName);
                SetBookmark("LandlordFio", workerName);
                SetBookmark("Tenant", counterparty);
                SetBookmark("TenantFio", counterpartName);

                SetBookmark("adress", actualadress);
                SetBookmark("cops", cops);
                SetBookmark("Office", office);
                SetBookmark("Square", square.ToString("0.##"));
                SetBookmark("PricePerM2", pricePerM2.ToString("0.##"));
                SetBookmark("Price", price.ToString("0.##"));
                SetBookmark("TotalPrice", totalPrice.ToString("0.##"));


                SetBookmark("Conclusion", dateConclusion);
                SetBookmark("DateFrom", dateFrom);
                SetBookmark("DateUntil", dateUntil);

                SetBookmark("Worker", workerName);


                SetBookmark("BIC", bic);
                SetBookmark("INN", inn);
                SetBookmark("CorrespondentAccount", correspondentAccount);
                SetBookmark("PaymentAccount", paymentAccount);

                SetBookmark("contrNum", numContract);
                SetBookmark("dateAkt", $"{dateUntil} ");
                SetBookmark("LanlordAkt", companyName);
                SetBookmark("tenantAkt", counterparty);
                SetBookmark("contrNum1", numContract);
                SetBookmark("dateAkt1", dateConclusion); 
                SetBookmark("adressAkt", actualadress);
                SetBookmark("floorAkt", floor);
                SetBookmark("officeAkt", office);

                SetBookmark("workerAkt", workerName);


                SetBookmark("BICAkt", bic);
                SetBookmark("INNAkt", inn);
                SetBookmark("CorrespondentAccountAkt", correspondentAccount);
                SetBookmark("PaymentAccountAkt", paymentAccount);
                switch (printMode)
                {
                    case 1:
                        DeleteFromPage(doc, 4);
                        break;

                    case 2:
                        DeletePages(doc, 1, 3);
                        break;

                    case 3:
                        break;

                    default:
                        throw new ArgumentException("Неверный режим печати. Используйте 1, 2 или 3.");
                }
                doc.SaveAs2(filePath);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:\n" + ex.Message);
                return false;
            }
            finally
            {
                if (doc != null) doc.Close();
                if (wordApp != null) wordApp.Quit();

                if (doc != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                if (wordApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
            }
        }

        private static void DeletePages(Word.Document doc, int startPage, int endPage)
        {
            try
            {

                object what = Word.WdGoToItem.wdGoToPage;
                object which = Word.WdGoToDirection.wdGoToAbsolute;

                object countStart = startPage;
                Word.Range startRange = doc.GoTo(ref what, ref which, ref countStart);

                object countEnd = endPage + 1;
                Word.Range endRange = doc.GoTo(ref what, ref which, ref countEnd);

                Word.Range deleteRange = doc.Range(startRange.Start, endRange.Start);
                deleteRange.Delete();
            }
            catch { }
        }
        private static void DeleteFromPage(Word.Document doc, int startPage)
        {
            try
            {
                Word.Range range = doc.Content;
                object what = Word.WdGoToItem.wdGoToPage;
                object which = Word.WdGoToDirection.wdGoToAbsolute;
                object count = startPage;

                Word.Range startRange = doc.GoTo(ref what, ref which, ref count);
                range.Start = startRange.Start;
                range.End = doc.Content.End;
                range.Delete();
            }
            catch { }
        }

    }
}

