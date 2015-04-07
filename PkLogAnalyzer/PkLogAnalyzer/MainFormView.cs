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
    public partial class MainFormView : Form
    {
        public MainFormView()
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


            sw.Stop(); Debug.WriteLine("メモリリード-" + sw.Elapsed);
            sw.Reset();

            sw.Start();
            /*using (StringReader rs = new System.IO.StringReader(_buffer))
            {
                //ストリームの末端まで繰り返す
                while (rs.Peek() > -1)
                {
                    c1FlexGrid1.AddItem(rs.ReadLine());
                }
            }*/
            string [] _buffers = _buffer.Split('\n');

            sw.Stop();Debug.WriteLine("行分割-" + sw.Elapsed);

            sw.Reset(); sw.Start();

            c1FlexGrid1.BeginUpdate();

            c1FlexGrid1.Items.AddRange(_buffers);
            /*for (int ii = 0; ii < _buffers.Length; ii++)
            {
                c1FlexGrid1.Items.Add(_buffers[ii]);
                Application.DoEvents();
            }*/

            c1FlexGrid1.EndUpdate();

            //c1FlexGrid1.FormattingEnabled = true;
            //c1FlexGrid1.HorizontalScrollbar = true;
            //c1FlexGrid1.MultiColumn = true;
            //c1FlexGrid1.ScrollAlwaysVisible = true;
            //c1FlexGrid1.Size = new System.Drawing.Size(120, 95);
            //c1FlexGrid1.TabIndex = 0;
            //c1FlexGrid1.ColumnWidth = 85;
            //for (int i = 0; i < 500; i++)
            //{
            //    c1FlexGrid1.Items.Add(i.ToString());
            //}

            sw.Stop();Debug.WriteLine("リストセット（レンジ）-" + sw.Elapsed);
            

            //c1FlexGrid1.Items.Clear();

            /*sw.Reset(); sw.Start();
            for (int ii = 1; ii < c1FlexGrid1.Rows.Count; ii++)
            {
                c1FlexGrid1[ii, 0] = ii;
            }
            sw.Stop(); Debug.WriteLine("行番号セット-" + sw.Elapsed);*/

            Debug.WriteLine("ここまで-" + sw.ElapsedTicks);

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

        private void c1FlexGrid1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label1.Text = "" + c1FlexGrid1.SelectedIndex;
        }
    }
}