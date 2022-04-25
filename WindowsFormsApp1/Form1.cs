using System;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using NcipBase;
using System.Collections.Generic;
using Microsoft.Office.Interop.Word;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void button1_Click(object sender, EventArgs e)
        {
            List<List<string>> listOfData = NewClass.Method();

            Word.Application word = new Word.Application();
            word.Application.Visible = true;
            word.WindowState = WdWindowState.wdWindowStateNormal;

            Document doc = word.Documents.Add();

            Paragraph paragraph = doc.Paragraphs.Add();
            paragraph.Range.Text = "Yana Latysh";
            paragraph.Range.Font.ColorIndex = WdColorIndex.wdBlack;
            paragraph.Range.InsertParagraphAfter();

            Table table = doc.Tables.Add(paragraph.Range, listOfData.Count, 8);
            table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
            table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

            for (int j = 0; j < listOfData.Count; j++)
            {
                Row row = table.Rows[j + 1];
                for (int i = 0; i < listOfData[j].Count; i++) {
                    row.Cells[i + 1].Range.Text = listOfData[j][i];
                }
            }
            //doc.SaveAs2("C:/Users/shoto/source/repos/NcipBase");
            //doc.Close();
            //word.Quit();
        }
    }
}
