using Microsoft.Office.Interop.Word;
using rccs.MyClass;
using System;
using System.Data;
using System.Windows;
using Word = Microsoft.Office.Interop.Word;

namespace rccs_new.MyClass.document
{
    internal class document_license_agreement
    {
        public static bool CreateLicenseAgreement(
            string templatePath,
            string filePath,
            string numContract,
            string Customers,
            string CustomersName,
            string program,
            string dateFrom,
            string dateUntil,
            string dateConclusion,
            string licenseId)
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
                SetBookmark("Date", dateUntil);
                SetBookmark("Customer", Customers);
                SetBookmark("CustomerName", CustomersName);
                SetBookmark("programName", program);
                SetBookmark("DateUntil", dateUntil);
                SetBookmark("ContractNumber1", numContract);
                SetBookmark("Date1", dateUntil);
                SetBookmark("ContractNumber2", numContract);
                SetBookmark("Date2", dateUntil);

                System.Data.DataTable dt =
                    licenseAgreement.GetLicenseAgreementForPrintTable(
                        int.Parse(licenseId));

                if (!doc.Bookmarks.Exists("ServiceTable"))
                    throw new Exception(
                        "Закладка ServiceTable не найдена!");
                Word.Range tableRange =
                    doc.Bookmarks["ServiceTable"].Range;
                Word.Table table = doc.Tables.Add(
                    tableRange,
                    dt.Rows.Count + 2,8
                );
                table.Borders.Enable = 1;
                table.Cell(1, 1).Range.Text = "№ п.п.";
                table.Cell(1, 2).Range.Text = "Наименование услуги";
                table.Cell(1, 3).Range.Text = "Кол-во";
                table.Cell(1, 4).Range.Text = "Цена без НДС";
                table.Cell(1, 5).Range.Text = "НДС 5%";
                table.Cell(1, 6).Range.Text = "Цена с НДС";
                table.Cell(1, 7).Range.Text = "Сумма НДС";
                table.Cell(1, 8).Range.Text = "Сумма с НДС";
                table.Rows[1].Range.Bold = 1;
                table.Range.Font.Size = 12;
                table.Rows[1].Range.ParagraphFormat.Alignment =
                    Word.WdParagraphAlignment.wdAlignParagraphCenter;
                decimal totalNds = 0;
                decimal totalSum = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];

                    int r = i + 2;

                    string name =
                        $"{row["name"]}\n{row["description"]}";

                    table.Cell(r, 1).Range.Text =
                        (i + 1).ToString();

                    table.Cell(r, 2).Range.Text =
                        name;

                    table.Cell(r, 3).Range.Text =
                         row["kolvo"].ToString();

                    table.Cell(r, 4).Range.Text =
                        row["цена"].ToString();

                    table.Cell(r, 5).Range.Text =
                        row["цена ндс"].ToString();

                    table.Cell(r, 6).Range.Text =
                        row["стоимость"].ToString();

                    table.Cell(r, 7).Range.Text =
                        row["стоимость ндс"].ToString();

                    table.Cell(r, 8).Range.Text =
                        row["общая стоимость"].ToString();



                    totalNds += Convert.ToDecimal(
                        row["стоимость ндс"]);

                    totalSum += Convert.ToDecimal(
                        row["общая стоимость"]);
                }
                int totalRow = dt.Rows.Count + 2;

                
                table.Cell(totalRow, 2).Range.Text = "Итого:";
                for (int i = 2; i <= 6; i++)
                {
                    table.Cell(totalRow, i).Range.Text = "";
                }
                for (int i = 1; i <= 5; i++)
                {
                    table.Cell(totalRow, i).Borders[Word.WdBorderType.wdBorderRight].LineStyle =
                        Word.WdLineStyle.wdLineStyleNone;

                    table.Cell(totalRow, i + 1).Borders[Word.WdBorderType.wdBorderLeft].LineStyle =
                        Word.WdLineStyle.wdLineStyleNone;
                }
                

                table.Cell(totalRow, 7).Range.Text =
                    totalNds.ToString("N2");

                table.Cell(totalRow, 8).Range.Text =
                    totalSum.ToString("N2");
                table.Columns[1].Width = 35.15f;
                table.Columns[2].Width = 155.93f;
                table.Columns[3].Width = 49.61f;
                table.Columns[4].Width = 56.69f;
                table.Columns[5].Width = 62.37f;
                table.Columns[6].Width = 49.61f;
                table.Columns[7].Width = 51.03f;
                table.Columns[8].Width = 63.78f;

                doc.SaveAs2(filePath);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка:\n" + ex.Message);

                return false;
            }
            finally
            {
                if (doc != null)
                    doc.Close();

                if (wordApp != null)
                    wordApp.Quit();

                if (doc != null)
                    System.Runtime.InteropServices.Marshal
                        .ReleaseComObject(doc);

                if (wordApp != null)
                    System.Runtime.InteropServices.Marshal
                        .ReleaseComObject(wordApp);
            }
        }
    }
}
