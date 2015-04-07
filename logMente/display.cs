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
    public partial class display : Form
    {
        List<string> dataList = new List<string>();
        
        public display()
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
                    //_buffer = tr.ReadToEnd();
                    //�X�g���[���̖��[�܂ŌJ��Ԃ�
                    while (tr.Peek() > -1)
                    {
                        dataList.Add(tr.ReadLine());
                    }
                }
            }

            sw.Stop(); Debug.WriteLine("���������[�h-" + sw.Elapsed);
            sw.Reset(); sw.Start();

            /*using (StringReader rs = new System.IO.StringReader(_buffer))
            {
                //�X�g���[���̖��[�܂ŌJ��Ԃ�
                while (rs.Peek() > -1)
                {
                    c1FlexGrid1.AddItem(rs.ReadLine());
                }
            }*/
            //string[] _buffers = _buffer.Split('\n');

            sw.Stop(); Debug.WriteLine("�s����-" + sw.Elapsed);
            sw.Reset(); sw.Start();

            //dataList.AddRange(_buffers);

            sw.Stop(); Debug.WriteLine("�f�[�^�Z�b�g-" + sw.Elapsed);
            sw.Reset(); sw.Start();

            //�o�[�`�������[�h��L���ɕύX
            ListView1.VirtualMode = true;
            //�o�[�`�������X�g�̍s��
            ListView1.VirtualListSize = dataList.Count;

            //ListViewItem items = new ListViewItem(_buffers);
            //c1FlexGrid1.Items.Add(items);

            sw.Stop();Debug.WriteLine("�o�[�`�����Z�b�g-" + sw.Elapsed);
            Debug.WriteLine("�����܂�-" + sw.ElapsedTicks);
        }

        private void c1FlexGrid1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            //e.Item = new ListViewItem(e.ItemIndex + 1);

            if (e.ItemIndex < dataList.Count)
            {
                if (e.Item == null)
                {
                    //e.Item = new ListViewItem();
                    e.Item = new ListViewItem();
                }

                //e.Item.ImageIndex = dataList[e.ItemIndex].ImageIndex;
                //e.Item.Text = string.Format("{0:s}", dataList[e.ItemIndex].Name);

                //e.Item.ImageIndex = e.ItemIndex;
                e.Item.Text = dataList[e.ItemIndex];
            }
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListView1.SelectedIndices.Count > 0)
            {
                label1.Text = "" + ListView1.SelectedIndices[0];
            }
        }
    }
}