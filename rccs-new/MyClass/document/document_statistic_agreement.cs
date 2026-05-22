using Microsoft.Win32;
using rccs.MyClass;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Word = Microsoft.Office.Interop.Word;

namespace rccs_new.MyClass.document
{
    internal class document_statistic_agreement
    {
        public static void CreateStatisticAgreementDocument()
        {
            Word.Application wordApp = null;
            Word.Document doc = null;

            try
            {
                wordApp = new Word.Application();
                wordApp.Visible = false;

                doc = wordApp.Documents.Add();
                Word.Range range = doc.Range();
                range.Font.Name = "Times New Roman";
                range.Font.Size = 12;
                range.Font.Bold = 0;
                // ==================== ЗАГОЛОВОК ====================
                range.Text = "СТАТИСТИКА АРЕНДЫ ПОМЕЩЕНИЙ\n\n";
                range.Font.Size = 16;
                range.Font.Bold = 1;
                range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                range.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                // ==================== ОБЩАЯ СТАТИСТИКА ====================
                int currentMonth = StatisticAgreement.GetCurrentMonthLeaseCount();
                int previousMonth = StatisticAgreement.GetPreviousMonthLeaseCount();
                decimal percentDiff = StatisticAgreement.GetLeasePercentDifference();

                string period = $"За период: {DateTime.Now:MMMM yyyy} г.";

                range.InsertAfter(period + "\n\n");
                range.Font.Size = 12;
                range.Font.Bold = 0;
                range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                range.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                range.InsertAfter($"Количество арендованных помещений в текущем месяце: {currentMonth} шт.\n");
                range.Font.Size = 12;
                range.Font.Bold = 0;
                range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                range.InsertAfter($"Количество арендованных помещений в прошлом месяце: {previousMonth} шт.\n");
                range.Font.Size = 12;
                range.Font.Bold = 0;
                range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                range.InsertAfter(StatisticAgreement.GetStatisticText() + "\n\n");
                range.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                // ==================== ТАБЛИЦА ====================
                DataTable dt = StatisticAgreement.GetCurrentMonthLeaseTable();

                if (dt.Rows.Count > 0)
                {
                    range.InsertAfter("Детализация по договорам аренды:\n\n");
                    range.Font.Size = 12;
                    range.Font.Bold = 0;
                    // Создаём таблицу: 4 столбца
                    Word.Table table = doc.Tables.Add(range, dt.Rows.Count + 1, 4);
                    table.Borders.Enable = 1;
                    table.Range.Font.Size = 11;
                   
                    table.Range.Font.Bold = 0;
                    // Заголовки таблицы
                    table.Cell(1, 1).Range.Text = "Помещение";
                    table.Cell(1, 2).Range.Text = "№ Договора";
                    table.Cell(1, 3).Range.Text = "Дата начала";
                    table.Cell(1, 4).Range.Text = "Дата окончания";

                    // Заполнение данных
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];

                        table.Cell(i + 2, 1).Range.Text = row["office_info"].ToString();
                        table.Cell(i + 2, 2).Range.Text = row["id_lease_agreement"].ToString();
                        table.Cell(i + 2, 3).Range.Text = Convert.ToDateTime(row["rental_date_from"]).ToString("dd.MM.yyyy");
                        table.Cell(i + 2, 4).Range.Text = Convert.ToDateTime(row["rental_date_until"]).ToString("dd.MM.yyyy");
                    }

                    // Форматирование таблицы
                    table.Columns[1].Width = 220;
                    table.Columns[2].Width = 80;
                    table.Columns[3].Width = 90;
                    table.Columns[4].Width = 90;

                    range = table.Range;
                    range.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                    range.InsertAfter("\n");
                }
                else
                {
                    range.InsertAfter("В текущем месяце договоры аренды не зарегистрированы.\n\n");
                    range.Font.Size = 12;
                    range.Font.Bold = 0;
                }

                // ==================== ИСПОЛНИТЕЛЬ ====================
                range.InsertAfter("\n\n\n\n\nИСПОЛНИТЕЛЬ:\n\n");
                range.Font.Size = 12;
                range.Font.Bold = 1;
                range.InsertAfter("Генеральный директор\n");
                range.Font.Size = 12;
                range.Font.Bold = 0;
                range.InsertAfter("ООО «Региональный Центр\n");
                range.Font.Size = 12;
                range.Font.Bold = 0;
                range.InsertAfter("Ценообразования в строительстве»\n\n");
                range.Font.Size = 12;
                range.Font.Bold = 0;
                range.InsertAfter("__________________________ Г. Н. Черткова\n");
                range.Font.Size = 12;
                range.Font.Bold = 0;
                range.InsertAfter("                    М.П.\n");
                range.Font.Size = 12;
                range.Font.Bold = 0;

                range.Font.Size = 12;
                range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

                // ==================== СОХРАНЕНИЕ ====================
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Word Document (*.docx)|*.docx",
                    FileName = $"Статистика_аренды_{DateTime.Now:yyyy-MM-dd_HH-mm}",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    doc.SaveAs2(saveFileDialog.FileName);
                    MessageBox.Show("Документ успешно сохранён!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                }

                doc.Close(false);
                wordApp.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании документа:\n" + ex.Message, "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (doc != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                if (wordApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
            }
        }
    }
}