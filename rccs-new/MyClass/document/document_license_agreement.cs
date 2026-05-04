using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.ExtendedProperties;
using rccs.MyClass;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace rccs_new.MyClass.document
{
    internal class document_license_agreement
    {
        public static bool CreateLicenseAgreement(string filePath, string numContract,string zakazchik, string contName, string dateDo)
        {
            try
            {
                Xceed.Words.NET.Licenser.LicenseKey = "WDN52-8ZN4A-PWXMJ-SA7A";

                using (DocX doc = DocX.Create(filePath))
                {
                    doc.SetDefaultFont(new Xceed.Document.NET.Font("Times New Roman"));

                    doc.MarginLeft = 53.86f;
                    doc.MarginRight = 17.01f;
                    doc.MarginTop = 17.01f;
                    doc.MarginBottom = 17.01f;


                    var title = doc.InsertParagraph($"Договор № {numContract}- ТС\nна оказание услуг по техническому и информационному сопровождению");
                    title.FontSize(11).Bold().Alignment = Alignment.center;
                    doc.InsertParagraph();

                    var content1 = doc.InsertParagraph(
                    "г. Мурманск                                                                                              " +
                    DateTime.Now.ToString("dd MMMM yyyy") + " г.\n");
                    content1.FontSize(11).Alignment = Alignment.both;
                    var table1 = doc.AddTable(1, 1);
                    table1.Rows[0].Cells[0].Paragraphs[0].Append($"{zakazchik}").Bold().FontSize(11);
                    var content2 = doc.InsertParagraph("именуемое в дальнейшем  «Заказчик», в лице");
                    content2.FontSize(11).Alignment = Alignment.both;
                    var table2 = doc.AddTable(1, 1);
                    table2.Rows[0].Cells[0].Paragraphs[0].Append($"{contName}").Bold().FontSize(11);
                    var intro = doc.InsertParagraph(
        "с одной стороны, и ООО «Региональный Центр Ценообразования в строительстве», " +
        "именуемое в дальнейшем «Исполнитель», в лице Генерального директора Чертковой Галины Николаевны, " +
        "действующего на основании Устава, с другой стороны, заключили настоящий Договор о нижеследующем:");
                    intro.FontSize(11).Alignment = Alignment.both;
                    intro.IndentationFirstLine = 28.35f;
                    var p1 = doc.InsertParagraph("1. ПРЕДМЕТ ДОГОВОРА");
                    p1.FontSize(11).Bold().Alignment = Alignment.left;

                    var p1_1 = doc.InsertParagraph(
                        "1.1. «Заказчик» поручает, а «Исполнитель» принимает на себя обязательства по техническому и " +
                        "информационному сопровождению комплекса программного обеспечения «А0» для Windows (далее - КПО) " +
                        "и электронных баз данных (далее – ЭБД), установленных на компьютерах Заказчика.");
                    p1_1.FontSize(11).Alignment = Alignment.both;
                    p1_1.IndentationFirstLine = 28.35f;

                    var p1_2 = doc.InsertParagraph(
                        "1.2. Перечень и объем технического и информационного сопровождения определен в п.п.2.1. и 2.2. " +
                        "настоящего договора.");
                    p1_2.FontSize(11).Alignment = Alignment.both;
                    p1_2.IndentationFirstLine = 28.35f;

                    var p1_3 = doc.InsertParagraph(
                        "1.3. Перечень рабочих мест с указанием номеров лицензий и фактического места сопровождения " +
                        "определен в спецификации и является неотъемлемой частью настоящего Договора.");
                    p1_3.FontSize(11).Alignment = Alignment.both;
                    p1_3.IndentationFirstLine = 28.35f;

                    doc.InsertParagraph();
                    var p2 = doc.InsertParagraph("2. ОБЯЗАННОСТИ ИСПОЛНИТЕЛЯ");
                    p2.FontSize(11).Bold().Alignment = Alignment.center;

                    var p2_1 = doc.InsertParagraph(
                        "2.1. «Исполнитель» в рамках технического и информационного сопровождения производит ежеквартальную " +
                        "загрузку разработанных, адаптированных и модифицированных ООО «Региональный Центр Ценообразования " +
                        "в строительстве» следующих ЭБД:\n" +
                        "— «Сборник индексов пересчета стоимости строительно-монтажных работ»;\n" +
                        "— «Сборник текущих сметных цен на основные строительные ресурсы»;\n" +
                        "— «Сборник базисных сметных цен 2000 год на основные строительные ресурсы»;\n" +
                        "— актуализация базы данных «Сборник ТЕР-2001 по Мурманской области», при условии официальной " +
                        "покупки основной базы данных «Электронный комплект сборников ТЕР-2001 по Мурманской области»;\n" +
                        "— обновление версий КПО.\n" +
                        "Официальным разработчиком указанных ЭБД является ООО «Региональный Центр Ценообразования в строительстве».\n " +
        "ЭБД не могут быть переданы другим лицам или тиражированы без разрешения ООО «Региональный Центр Ценообразования в строительстве».");
                    p2_1.FontSize(11).Alignment = Alignment.both;
                    p2_1.IndentationFirstLine = 28.35f;
                    var p2_2 = doc.InsertParagraph(
                       "2.2. «Исполнитель» осуществляет консультации сотрудников «Заказчика» по вопросам эксплуатации КПО и ЭБД РЦЦС по\n" +
                       "МО по телефону или в офисе «Исполнителя». Консультации по телефону проводятся в пределах рабочего времени «Исполнителя»\n" +
                       " и не могут превышать 10 минут в день. Консультации в офисе «Исполнителя» проводятся по предварительной договоренности\n" +
                       "представителя «Заказчика» со специалистом «Исполнителя», но не позднее 3-х рабочих дней с момента обращения, и не могут\n" +
                       "превышать одного часа в месяц на каждое рабочее место");
                    p2_2.FontSize(11).Alignment = Alignment.both;
                    p2_2.IndentationFirstLine = 28.35f;
                    var p2_k = doc.InsertParagraph(
                       "Линия Консультаций: (8152) 400-507, 400-500 доб. 227");
                    p2_k.FontSize(11).UnderlineStyle(UnderlineStyle.singleLine).Alignment = Alignment.both;
                    p2_k.IndentationFirstLine = 28.35f;
                    var p2_3 = doc.InsertParagraph(
                       "2.3.«Исполнитель» обязан в течение 20 (двадцати) рабочих дней с начала квартала передать «Заказчику» обновление ЭБД согласно п. 2.1.\n" +
                       " при условии выполнения «Заказчиком» п.п. 3.1. и 4.4. Услуга по ежеквартальному обслуживанию считается оказанной полностью за квартал\n" +
                       " в момент установки обновлений ЭБД согласно п.п. 2.1.В момент оказания услуги «Исполнитель» передает «Заказчику» Универсальный передаточный\n" +
                       " документ(далее – УПД), далее «Заказчик» действует согласно п.п 3.2.");

                    p2_3.FontSize(11).Alignment = Alignment.both;
                    p2_3.IndentationFirstLine = 28.35f;
                    var p2_4 = doc.InsertParagraph(
                       "2.4. В случае возникновения невосстановимых ошибок ЭБД «Исполнитель» гарантирует их восстановление из архивной копии по состоянию на момент ее\n" +
                       " создания при соблюдении «Заказчиком» п. 3.5. настоящего Договора и при отсутствии проблем общесистемного и технического характера.");

                    p2_4.FontSize(11).Alignment = Alignment.both;
                    p2_4.IndentationFirstLine = 28.35f;
                    var p2_5 = doc.InsertParagraph(
                       "2.5. При необходимости «Исполнитель» осуществляет дополнительные услуги с выездом специалистов к «Заказчику» по инициативе «Заказчика».\n" +
                        "К таким дополнительным услугам относятся:\n" +
                        "-тестирование и исправление ЭБД;\n" +
                        "-переустановка КПО;\n" +
                        "-установка дополнительных рабочих мест;\n" +
                        "-услуги по согласованию сторон.");

                    p2_5.FontSize(11).Alignment = Alignment.both;
                    p2_5.IndentationFirstLine = 28.35f;
                    var p2_6 = doc.InsertParagraph(
                       "2.6. Трудоемкость выполнения дополнительных работ определяется в часах. Минимальная продолжительность работ при каждом выезде к «Заказчику» – 1 час,\n" +
                       "каждый последующий час считается целым. Порядок оплаты и условия оказания перечисленных услуг определяются п.п. 4.3. настоящего договора.");

                    p2_6.FontSize(11).Alignment = Alignment.both;
                    p2_6.IndentationFirstLine = 28.35f;

                    var p2_7 = doc.InsertParagraph(
                      "2.7. «Исполнитель» не несет ответственности за работу КПО на технически неисправном компьютере, при наличии\n" +
                      "нелицензионных программных продуктов на компьютере «Заказчика», при наличии ЭБД в КПО, приобретенных не у разработчика\n" +
                      "(ООО «РЦЦС»), а также на компьютере, зараженном вирусом. Отсутствие в ЭБД ряда нормативных сборников не является основанием\n" +
                      " для отказа в приемке и его использовании. ");

                    p2_7.FontSize(11).Alignment = Alignment.both;
                    p2_7.IndentationFirstLine = 28.35f;

                    var p2_8 = doc.InsertParagraph(
                      "2.8. В случае отсутствия у «Заказчика» в ЭБД необходимых «Заказчику» документов, существующих на момент загрузки, «Исполнитель»\n" +
                      " обязан в течение 20 рабочих дней передать «Заказчику» недостающие документы либо сообщить причину, по которой эти документы\n" +
                      " не могут быть предоставлены.  ");

                    p2_8.FontSize(11).Alignment = Alignment.both;
                    p2_8.IndentationFirstLine = 28.35f;

                    var p2_9 = doc.InsertParagraph(
                     "2.9. «Исполнитель» не несет ответственность по срокам исполнения договора при\n" +
                     " несвоевременном уведомлении об изменении фактического адреса Заказчика в соответствии с\n" +
                     " п.п.3.9.  ");

                    p2_9.FontSize(11).Alignment = Alignment.both;
                    p2_9.IndentationFirstLine = 28.35f;

                    var p3 = doc.InsertParagraph("3. ОБЯЗАННОСТИ ЗАКАЗЧИКА");
                    p3.FontSize(11).Bold().Alignment = Alignment.center;
                    var p3_1 = doc.InsertParagraph(
                     "3.1. «Заказчик» своевременно оплачивает работу «Исполнителя» в размере и сроки,\n" +
                     " предусмотренные в п. 4. настоящего Договора. ");
                    p3_1.FontSize(11).Alignment = Alignment.both;
                    p3_1.IndentationFirstLine = 28.35f;

                    var p3_2 = doc.InsertParagraph(
                    "3.2. «Заказчик» в течение 10 (десяти) рабочих дней после получения услуги по\n" +
                    " ежеквартальному техническому и информационному сопровождению и получения Универсального\n" +
                    " передаточного документа (УПД) обязан направить «Исполнителю» подписанный со своей стороны\n" +
                    " УПД или письменно мотивировать отказ от приемки оказанных Исполнителем услуг.");
                    p3_2.FontSize(11).Alignment = Alignment.both;
                    p3_2.IndentationFirstLine = 28.35f;

                    var p3_3 = doc.InsertParagraph(
                    "3.3. Если «Заказчик» в течение срока согласно п. 3.2. настоящего Договора не подписал УПД и\n" +
                    " не предоставил письменного обоснования отказа, данные услуги считаются «Исполнителем»\n" +
                    " оказанными с надлежащим качеством, в полном объеме и в срок. ");
                    p3_3.FontSize(11).Alignment = Alignment.both;
                    p3_3.IndentationFirstLine = 28.35f;

                    var p3_4 = doc.InsertParagraph(
                    "3.4. «Заказчик» обязуется обеспечить необходимые условия для выполнения работ по\n" +
                    " техническому и информационному сопровождению (исправность и наличие ресурсов программно-\n" +
                    "технических средств, соответствие технических средств для работы с КПО и ЭБД, присутствие\n" +
                    " сотрудника, ответственного за эксплуатацию программного обеспечения).\n" +
                    " К техническим средствам на каждом рабочем месте:\t\n" +
                    "Операционная система: Windows 10, Windows 11  х32 и х64 - разрядная;\n" +
                    "Процессор с тактовой частотой не менее 2 ГГц;\n" +
                    "Рекомендуемый общий объем ОЗУ  8 Гб или более;\n" +
                    "Свободное место на жестком диске не менее 20 Гб(зависит от количества данных);\n" +
                    "Разрешение монитора 1024х768;\n" +
                    "Наличие USB-порта разъем USB стандарта 2.0 / 1.1;\n" +
                    "Сетевой протокол TCP / IP.\n" +
                    "Необходимо, чтобы на компьютере был установлен и активирован пакет приложений\n" +
                    "Microsoft Office(Word, Excel), выпуск 2010 и новее.\n" +
                    "К серверу(в случае использования сервера):\t\n" +
                    "В случае установки MS SQL Server 2014 / 2022 Express(входит в комплект поставки) -\n" +
                    " требованиям, предъявляемым Microsoft для работы с MS SQL.\n" +
                    "При выполнении работ ответственным лицом «Исполнителя», «Заказчик» обеспечивает\n" +
                    " свободный доступ к компьютерам и операционным системам, необходимый для успешного\n" +
                    " выполнения условий Договора.");
                    p3_4.FontSize(11).Alignment = Alignment.both;
                    p3_4.IndentationFirstLine = 28.35f;

                    var p3_5 = doc.InsertParagraph(
                    "3.5. В процессе эксплуатации КПО «Заказчик» обязан ежедневно создавать архивную копию\n" +
                    " базы данных программы с целью исключения потери данных по независящим от сторон причинам.\n" +
                    " Архивная копия создается и хранится «Заказчиком» на носителе, отличном от носителя рабочей базы данных. ");
                    p3_5.FontSize(11).Alignment = Alignment.both;
                    p3_5.IndentationFirstLine = 28.35f;

                    var p3_6 = doc.InsertParagraph(
                    "3.6. «Заказчик» не имеет права вносить любые изменения в конфигурацию КПО и ЭБД\n" +
                    " самостоятельно или с помощью третьего лица, без согласования с «Исполнителем». ");
                    p3_6.FontSize(11).Alignment = Alignment.both;
                    p3_6.IndentationFirstLine = 28.35f;

                    var p3_7 = doc.InsertParagraph(
                   "3.7. Заказчик» обязуется применять только легально приобретенные КПО и ЭБД\n" +
                    " «Исполнителя», только для внутреннего использования предприятия и не допускать незаконного\n" +
                    " тиражирования ЭБД. ");
                    p3_7.FontSize(11).Alignment = Alignment.both;
                    p3_7.IndentationFirstLine = 28.35f;

                    var p3_8 = doc.InsertParagraph(
                   "3.8. «Заказчик» обязуется не распространять третьим лицам ЭБД, установленные на\n" +
                    " компьютерах «Заказчика». Если будет выявлен факт распространения ЭБД, «Заказчик» несет\n" +
                    " материальную ответственность перед «Исполнителем» в размере 30-ти (тридцати) стоимостей ЭБД. ");
                    p3_8.FontSize(11).Alignment = Alignment.both;
                    p3_8.IndentationFirstLine = 28.35f;

                    var p3_9 = doc.InsertParagraph(
                   "3.9. В случае изменения у «Заказчика» регистрационных данных организации, юридического\n" +
                   " и/или фактического адреса, контактных телефонов, платежных реквизитов, при смене руководителя\n" +
                   " организации «Заказчик» обязуется в течение 10 дней сообщить измененную информацию «Исполнителю». ");
                    p3_9.FontSize(11).Alignment = Alignment.both;
                    p3_9.IndentationFirstLine = 28.35f;


                    var p4 = doc.InsertParagraph("4.СТОИМОСТЬ УСЛУГ ИСПОЛНИТЕЛЯ И ПОРЯДОК ОПЛАТЫ");
                    p4.FontSize(11).Bold().Alignment = Alignment.center;
                    var p4_1 = doc.InsertParagraph(
                     "4.1. Цена технического и информационного сопровождения на одно рабочее место (для сетевых\n" +
                   " версий - на основное рабочее место и на дополнительное рабочее место) указывается в действующем\n" +
                   " Прейскуранте, являющимся неотъемлемой частью настоящего договора. Стоимость сопровождения\n" +
                   " определяется исходя из количества рабочих мест. ");
                    p4_1.FontSize(11).Alignment = Alignment.both;
                    p4_1.IndentationFirstLine = 28.35f;

                    var p4_2 = doc.InsertParagraph(
                    "4.2. «Исполнитель» оставляет за собой право вносить изменения в действующий Прейскурант с\n" +
                   " предупреждением «Заказчика» не менее чем за 1 месяц. При изменении Прейскуранта Исполнитель\n" +
                   " извещает об этом «Заказчика» посредством публикации соответствующего уведомления на сайте\n" +
                   " Исполнителя WWW.RCCS.RU.");
                    p4_2.FontSize(11).Alignment = Alignment.both;
                    p4_2.IndentationFirstLine = 28.35f;

                    var p4_3 = doc.InsertParagraph(
                    "4.3. При оказании дополнительных услуг, перечисленных в п.п. 2.5. настоящего Договора,\n" +
                   " оплата услуг «Исполнителя» осуществляется по факту оказания этих услуг в соответствии с\n" +
                   " действующим Прейскурантом. При этом сохраняется условие минимального объема работ при выезде\n" +
                   " к «Заказчику» (1 час). ");
                    p4_3.FontSize(11).Alignment = Alignment.both;
                    p4_3.IndentationFirstLine = 28.35f;

                    var p4_4 = doc.InsertParagraph(
                    "4.4. Оплата обслуживания производится «Заказчиком» ежеквартально, до первого числа\n" +
                   " первого месяца обслуживаемого квартала, в размере 100%. В случае задержки платежа свыше\n" +
                   " указанного срока, обслуживание Заказчика производится в течение 20 (двадцати) рабочих дней с\n" +
                   " момента поступления оплаты.");
                    p4_4.FontSize(11).Alignment = Alignment.both;
                    p4_4.IndentationFirstLine = 28.35f;
                    var p4_42 = doc.InsertParagraph(
                   "Оплата обслуживания производится «Заказчиком» непрерывно. При перерыве в\n" +
                   " ежеквартальной оплате (пропуске квартала обслуживания) «Заказчик» для возобновления\n" +
                   " обслуживания перечисляет «Исполнителю» единовременную плату за актуализацию данных,\n" +
                   " указанную в п. 5 Прейскуранта. Стоимость определяется исходя из количества рабочих мест.");
                    p4_42.FontSize(11).Alignment = Alignment.both;
                    p4_42.IndentationFirstLine = 28.35f;

                    var p4_5 = doc.InsertParagraph(
                    "4.5. В случае нахождения офиса «Заказчика» вне территории г. Мурманска стоимость услуг\n" +
                   " увеличивается на стоимость накладных расходов в части затрат на командирование специалистов.");
                    p4_5.FontSize(11).Alignment = Alignment.both;
                    p4_5.IndentationFirstLine = 28.35f;

                    var p4_6 = doc.InsertParagraph(
                    "4.6. В случае нарушения сроков оплаты по оказанным работам (услугам) «Исполнитель»\n" +
                   " вправе потребовать от «Заказчика» уплаты пени в размере 0,3% от суммы задолженности за каждый\n" +
                   " день просрочки начиная с 5 рабочего дня от момента оказания услуг.");
                    p4_6.FontSize(11).Alignment = Alignment.both;
                    p4_6.IndentationFirstLine = 28.35f;
                    doc.InsertParagraph();
                    var p5 = doc.InsertParagraph("5. СРОК ДЕЙСТВИЯ ДОГОВОРА");
                    p5.FontSize(11).Bold().Alignment = Alignment.center;
                    
                    var p5_1 = doc.InsertParagraph(
                     $"5.1. Настоящий Договор вступает в силу с даты его подписания и заключен на срок {dateDo} года." +
                   " Настоящий договор может быть пролонгирован на новый срок путем\n" +
                   " заключения Дополнительного соглашения. ");
                    p5_1.FontSize(11).Alignment = Alignment.both;
                    p5_1.IndentationFirstLine = 28.35f;

                    var p5_2 = doc.InsertParagraph(
                    "5.2. Все изменения и дополнения к настоящему Договору оформляются Дополнительными\n" +
                   " соглашениями, которые подписываются уполномоченными представителями Сторон и являются\n" +
                   " неотъемлемой частью настоящего Договора.");
                    p5_2.FontSize(11).Alignment = Alignment.both;
                    p5_2.IndentationFirstLine = 28.35f;

                    var p5_3 = doc.InsertParagraph(
                    "5.3. «Заказчик» имеет право в одностороннем порядке отказаться от исполнения настоящего\n" +
                   " договора, с уведомлением об этом «Исполнителя» в письменном виде за один месяц до даты его\n" +
                   " расторжения, при условии полной оплаты «Исполнителю» оказанных услуг, а также фактически\n" +
                   " понесенных им расходов на момент расторжения данного Договора.");
                    p5_3.FontSize(11).Alignment = Alignment.both;
                    p5_3.IndentationFirstLine = 28.35f;

                    var p5_4 = doc.InsertParagraph(
                    "5.4. «Исполнитель» имеет право в одностороннем порядке расторгнуть настоящий Договор, с\n" +
                   " уведомлением об этом «Заказчика» в письменном виде за один месяц до даты его расторжения, если\n" +
                   " «Заказчик» нарушает любой из пунктов настоящего Договора, при этом сохраняется обязанность\n" +
                   " «Заказчика» оплатить уже оказанные «Исполнителем» услуги, если они не были оплачены к\n" +
                   "моменту расторжения Договора.");
                    p5_4.FontSize(11).Alignment = Alignment.both;
                    p5_4.IndentationFirstLine = 28.35f;
                    var p5_5 = doc.InsertParagraph(
                   "5.5. Стороны несут ответственность по настоящему Договору в соответствии с действующим\n" +
                   " законодательством РФ.");
                    p5_5.FontSize(11).Alignment = Alignment.both;
                    p5_5.IndentationFirstLine = 28.35f;

                    var p5_6 = doc.InsertParagraph(
                    "5.6. Настоящий Договор составлен в 2-х подлинных экземплярах, имеющих одинаковую\n" +
                   " юридическую силу, - по одному для каждой из Сторон.");
                    p5_6.FontSize(11).Alignment = Alignment.both;
                    p5_6.IndentationFirstLine = 28.35f;

                    var p5_7 = doc.InsertParagraph(
                    "5.7. Реквизиты сторон:");
                    p5_7.FontSize(11).Alignment = Alignment.both;
                    p5_7.IndentationFirstLine = 28.35f;

                    var table3 = doc.AddTable(6, 4);

                    table3.SetBorder(
                       TableBorderType.Top, new Border(BorderStyle.Tcbs_none, BorderSize.one, 0, Xceed.Drawing.Color.White));
                    table3.SetBorder(
                        TableBorderType.Bottom, new Border(BorderStyle.Tcbs_none, BorderSize.one, 0, Xceed.Drawing.Color.White));
                    table3.SetBorder(
                        TableBorderType.Left, new Border(BorderStyle.Tcbs_none, BorderSize.one, 0, Xceed.Drawing.Color.White));
                    table3.SetBorder(
                        TableBorderType.Right, new Border(BorderStyle.Tcbs_none, BorderSize.one, 0, Xceed.Drawing.Color.White));
                    table3.SetBorder(
                        TableBorderType.InsideH, new Border(BorderStyle.Tcbs_none, BorderSize.one, 0, Xceed.Drawing.Color.White));
                    table3.SetBorder(
                        TableBorderType.InsideV, new Border(BorderStyle.Tcbs_none, BorderSize.one, 0, Xceed.Drawing.Color.White));


                    table3.Rows[0].Cells[0].Paragraphs[0]
                        .Append("Исполнитель\n").Bold()
                        .FontSize(11);
                    table3.Rows[0].Cells[1].Paragraphs[0]
                        .Append("ООО «Региональный Центр Ценообразования в строительстве»\n").Bold()
                        .FontSize(11);
                    table3.Rows[0].Cells[2].Paragraphs[0]
                        .Append("Заказчик \n").Bold()
                        .FontSize(11);
                    ///////////////////
                    table3.Rows[1].Cells[0].Paragraphs[0]
                        .Append("Адрес:\n")
                        .FontSize(11);
                    table3.Rows[1].Cells[1].Paragraphs[0]
                        .Append("183038, г. Мурманск,\n")
                        .Append("ул. Капитана Егорова, 14, оф. 504\n")
                        .Append("тел. 400-500, 400-501\n")
                        .Append("www.RCCS.ru | cc@rccs.ru")
                        .FontSize(11);
                    table3.Rows[1].Cells[2].Paragraphs[0]
                        .Append("Адрес:\n")
                        .FontSize(11);
                    ///////////////////
                    table3.Rows[2].Cells[0].Paragraphs[0]
                        .Append("Р/счет\n")
                        .FontSize(11);
                    table3.Rows[2].Cells[1].Paragraphs[0]
                        .Append("40702810841000109021\n").Bold()
                        .Append("в Мурманское отделение №8627 ПАО Сбербанка")
                        .FontSize(11);
                    table3.Rows[2].Cells[2].Paragraphs[0]
                        .Append("Р/счет\n")
                        .FontSize(11);
                     ///////////////////
                    table3.Rows[3].Cells[0].Paragraphs[0]
                        .Append("Кор/счет\n")
                        .FontSize(11);
                    table3.Rows[3].Cells[1].Paragraphs[0]
                       .Append("30101810300000000615\n")   
                       .FontSize(11);
                    table3.Rows[3].Cells[2].Paragraphs[0]
                       .Append("Кор/счет\n")
                       .FontSize(11);
                    ///////////////////
                    table3.Rows[4].Cells[0].Paragraphs[0]
                        .Append("БИК")
                        .FontSize(11);
                    table3.Rows[4].Cells[1].Paragraphs[0]
                       .Append("044705615")
                       .FontSize(11);
                    table3.Rows[4].Cells[2].Paragraphs[0]
                       .Append("БИК")
                       .FontSize(11);
                    ///////////////////
                    table3.Rows[5].Cells[0].Paragraphs[0]
                        .Append("ИНН/КПП")
                        .FontSize(11);

                    table3.Rows[5].Cells[1].Paragraphs[0]
                        .Append("5190160087/519001001")
                        .FontSize(11);

                    table3.Rows[5].Cells[2].Paragraphs[0]
                        .Append("ИНН/КПП")
                        .FontSize(11);

                    var table4 = doc.AddTable(6, 4);

                    table4.SetBorder(
                       TableBorderType.Top, new Border(BorderStyle.Tcbs_none, BorderSize.one, 0, Xceed.Drawing.Color.White));
                    table4.SetBorder(
                        TableBorderType.Bottom, new Border(BorderStyle.Tcbs_none, BorderSize.one, 0, Xceed.Drawing.Color.White));
                    table4.SetBorder(
                        TableBorderType.Left, new Border(BorderStyle.Tcbs_none, BorderSize.one, 0, Xceed.Drawing.Color.White));
                    table4.SetBorder(
                        TableBorderType.Right, new Border(BorderStyle.Tcbs_none, BorderSize.one, 0, Xceed.Drawing.Color.White));
                    table4.SetBorder(
                        TableBorderType.InsideH, new Border(BorderStyle.Tcbs_none, BorderSize.one, 0, Xceed.Drawing.Color.White));
                    table4.SetBorder(
                        TableBorderType.InsideV, new Border(BorderStyle.Tcbs_none, BorderSize.one, 0, Xceed.Drawing.Color.White));

                    table4.Rows[0].Cells[0].Paragraphs[0]
                        .Append("ИСПОЛНИТЕЛЬ:\n").Bold()
                       .FontSize(11);
                    table4.Rows[0].Cells[1].Paragraphs[0]
                        .Append("ЗАКАЗЧИК:").Bold()
                        .FontSize(11);
                    ///////////////////////
                    table4.Rows[1].Cells[0].Paragraphs[0]
                       .Append("Генеральный директор\n")
                       .Append("ООО «Региональный Центр\nЦенообразования в строительстве»")
                      .FontSize(11);
                    table4.Rows[1].Cells[1].Paragraphs[0]
                        .Append("ЗАКАЗЧИК:").Bold()
                        .FontSize(11);
                    //////////////////////
                    table4.Rows[2].Cells[0].Paragraphs[0]
                        .Append("_____________________________________________\n")
                       .Append("\n\n_____________________________________________")
                      .FontSize(11);
                    doc.InsertParagraph().InsertPageBreakAfterSelf();
                    //doc.InsertTable(table3);
                    //doc.InsertTable(table4);

                    
                    var zag1 = doc.InsertParagraph();
                    zag1.Append("Приложение № 1\n" +
                        $"к Договору № {numContract} ТС от {DateTime.Now.ToString("«dd» MMMM yyyy")} г.\n" +
                        "на оказание услуг по техническому и информационному сопровождению.\n\n")
                      .FontSize(10)
                      .Alignment = Alignment.right;


                    var pp1 = doc.InsertParagraph("СПЕЦИФИКАЦИЯ\n"+
                     "рабочих мест, установленных у Заказчика");
                    pp1.FontSize(11).Bold().Alignment = Alignment.center;

                    var pp1_1 = doc.InsertParagraph("Перечень оказываемых услуг ");
                    pp1_1.FontSize(11).Bold().Alignment = Alignment.center;

                    var table5 = doc.AddTable(6, 4);

                    doc.Save();
                }
                return true;    
            }
            catch
            {
                return false;
            }
        }
        //public static int CountServiece()
        //{
        //    ConnectionBD.mycommand.CommandText = $"SELECT count(*) FROM rccs.service_in_agreement where id_license_agreement= {};"
        //}
    }
}       
