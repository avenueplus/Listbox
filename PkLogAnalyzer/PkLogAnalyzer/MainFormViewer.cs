using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace PkLogAnalyzer
{
    public partial class MainFormViewer : Form
    {
        public MainFormViewer()
        {
            InitializeComponent();

            Encoding enc = Encoding.GetEncoding("shift_jis");
            string _buffer = null;
            string filePath = @"C:\zabbix\log\zabbix_agentd_sjis_1.log";
            Stopwatch sw = new Stopwatch();

            sw.Start();
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (TextReader tr = new StreamReader(fs, enc))
                {
                    _buffer = tr.ReadToEnd();
                }
            }

            
            sw.Stop();Debug.WriteLine("���������[�h-" + sw.Elapsed);
            sw.Reset();
/*
            //StreamReader���g���Ǝ��̂悤�ɂȂ�
            //System.IO.MemoryStream ms = new System.IO.MemoryStream
            //    (System.Text.Encoding.UTF8.GetBytes(TextBox1.Text));
            //System.IO.StreamReader rs = new System.IO.StreamReader(ms);
            sw.Start();
            using (StringReader rs = new System.IO.StringReader(_buffer))
            {
                //�X�g���[���̖��[�܂ŌJ��Ԃ�
                while (rs.Peek() > -1)
                {
                    c1FlexGrid1.AddItem(rs.ReadLine());
                }
            }

            sw.Stop();Debug.WriteLine("�O���b�h�Z�b�g�i�s���j-" + sw.Elapsed);
*/
            Debug.WriteLine("�����܂Łi�s���j-" + sw.ElapsedTicks);
            sw = new Stopwatch();

            c1FlexGrid1.Rows.RemoveRange(1, c1FlexGrid1.Rows.Count - 1);

            sw.Start();
            c1FlexGrid1[0, 1] = "[" + c1FlexGrid1.ClipSeparators + "]";
            c1FlexGrid1.LoadGrid(filePath,
                C1.Win.C1FlexGrid.FileFormatEnum.TextCustom,
                C1.Win.C1FlexGrid.FileFlags.None,
                enc);

            sw.Stop(); Debug.WriteLine("�O���b�h�Z�b�g�i�t�@�C���j-" + sw.Elapsed);
            sw.Reset();

            sw.Start();
            for (int ii = 1; ii < c1FlexGrid1.Rows.Count; ii++) 
            {
                c1FlexGrid1[ii, 0] = ii;
            }

            sw.Stop(); Debug.WriteLine("�s�ԍ��Z�b�g-" + sw.Elapsed);
            Debug.WriteLine("�����܂Łi�s���j-" + sw.ElapsedTicks);

            /*c1FlexGrid1.SaveGrid(@"C:\zabbix\log\zabbix_agentd_sjis_1.log", 
                C1.Win.C1FlexGrid.FileFormatEnum.TextCustom, 
                C1.Win.C1FlexGrid.FileFlags.None,
                enc);*/

            /*using (StreamWriter sw = new StreamWriter(@"C:\zabbix\log\zabbix_agentd_sjis_1.log", true,  enc))
            {
                for (int jj = 1; jj < 5000; jj++)
                {
                    for (int ii = 1; ii < c1FlexGrid1.Rows.Count; ii++)
                    {
                        sw.WriteLine(c1FlexGrid1[ii, 1]);
                    }
                }
            }*/


        }
    }
}