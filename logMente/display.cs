using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace logMente
{
    public partial class display : Form
    {
        List<string> dataList = new List<string>();

        List<string[]> attrInfo = new List<string[]>();

        private void Console_WriteLine(string caption, string[] param)
        {
            for (int ii = 0; ii < param.Length; ii++)
            {
                Console.WriteLine(caption + param[ii]);
            }

        }

        private string GetProjectFolder(string initialFolder)
        {
            string projectFolder = Path.GetFullPath(initialFolder);
            string rootDir = Directory.GetDirectoryRoot(projectFolder);

            while (!projectFolder.Equals(rootDir))
            {
                projectFolder = Directory.GetParent(projectFolder).FullName;
                string folderName = Path.GetFileName(projectFolder);

                if (folderName.Equals(Application.ProductName))
                {
                    break;
                }
            }
            if (projectFolder.Equals(rootDir))
            {
                projectFolder = initialFolder;
            }

            return projectFolder;
        }

        public display()
        {
            InitializeComponent();

            this.Hide();
            // ファイルリストxml生成時に抽出する場合
            // 指定なし（項目なし）が無視（スルー）
            attrInfo.Add(new string[] { FileAttributes.ReadOnly.ToString(), "true" });  // checked は 取り込む
            attrInfo.Add(new string[] { FileAttributes.ReadOnly.ToString(), "false" }); // unchecked は 除外

            //
            // でも、xpathのクエリーにする（xmlには全フォルダ・ファイルを階層で格納する。）
            //

            // プロジェクトネームは、AssemblyInfo.cs のものを使っている。
            Console.WriteLine("App ProductName          :" + Application.ProductName);
            Console.WriteLine("App ExecutablePath       :" + Application.ExecutablePath);
            Console.WriteLine("App StartupPath          :" + Application.StartupPath);

            Console.WriteLine("Dir GetCurrentDirectory  :" + Directory.GetCurrentDirectory());
            Console.WriteLine("Dir GetDirectoryRoot     :" + Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()));
            Console_WriteLine("Dir GetLogicalDrives     :" , Directory.GetLogicalDrives());
            Console.WriteLine("Dir GetParent            :" + Directory.GetParent(Directory.GetCurrentDirectory()).FullName);

            Console.WriteLine("App CommonAppDataPath    :" + Application.CommonAppDataPath);
            Console.WriteLine("App LocalUserAppDataPath :" + Application.LocalUserAppDataPath);
            Console.WriteLine("App UserAppDataPath      :" + Application.UserAppDataPath);

            Console.WriteLine();
            Console.WriteLine("Dir GetParent GetParent  :" + 
                Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName);
            Console_WriteLine("Dir GetFileSystemEntries :", Directory.GetFileSystemEntries(
                Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName, "*")); // , "*"

            Console.WriteLine();
            Console.WriteLine(@"C:\Users\avenue\Documents\GitHub\Listbox\logMente\logMente.suo");
            Console.WriteLine("File Path.GetFileName         :" + Path.GetFileName(@"C:\Users\avenue\Documents\GitHub\Listbox\logMente\logMente.suo"));
            Console.WriteLine("File Path.GetExtension        :" + Path.GetExtension(@"C:\Users\avenue\Documents\GitHub\Listbox\logMente\logMente.suo"));
            Console.WriteLine("File Path.GetDirectoryName    :" + Path.GetDirectoryName(@"C:\Users\avenue\Documents\GitHub\Listbox\logMente\logMente.suo"));
            Console.WriteLine("File Path.GetFileNameWithou   :" + Path.GetFileNameWithoutExtension(@"C:\Users\avenue\Documents\GitHub\Listbox\logMente\logMente.suo"));
            Console.WriteLine("File Path.GetFullPath         :" + Path.GetFullPath(@"C:\Users\avenue\Documents\GitHub\Listbox\logMente\logMente.suo"));
            Console.WriteLine("File Path.GetPathRoot         :" + Path.GetPathRoot(@"C:\Users\avenue\Documents\GitHub\Listbox\logMente\logMente.suo"));
            Console.WriteLine("     Path.GetTempPath         :" + Path.GetTempPath());
            Console.WriteLine("     Path.GetRandomFileName   :" + Path.GetRandomFileName());
            Console.WriteLine("     Path.GetTempFileName     :" + Path.GetTempFileName());

            Console.WriteLine();
            Console.WriteLine(Application.StartupPath);
            Console.WriteLine("Dir  Path.GetFileName         :" + Path.GetFileName(Application.StartupPath));
            Console.WriteLine("Dir  Path.GetExtension        :" + Path.GetExtension(Application.StartupPath));
            Console.WriteLine("Dir  Path.GetDirectoryName    :" + Path.GetDirectoryName(Application.StartupPath));
            Console.WriteLine("Dir  Path.GetFileNameWithou   :" + Path.GetFileNameWithoutExtension(Application.StartupPath));
            Console.WriteLine("Dir  Path.GetFullPath         :" + Path.GetFullPath(Application.StartupPath));
            Console.WriteLine("Dir  Path.GetPathRoot         :" + Path.GetPathRoot(Application.StartupPath));


            Console.WriteLine();
            Console.WriteLine("Loc GetProjectFolder      :" + GetProjectFolder(Application.StartupPath));
            Console.WriteLine("Loc GetProjectFolder      :" + GetProjectFolder(@"C:\"));
            
            
            
            
            
            
            
            
            
            
            
            
            
            
#if LISTVIEW_TEST            
            Encoding enc = Encoding.GetEncoding("shift_jis");
            string filePath = @"C:\zabbix\log\zabbix_agentd_sjis_1.log";
            Stopwatch sw = new Stopwatch();

            //string _buffer = null;

            sw.Start();
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (TextReader tr = new StreamReader(fs, enc))
                {
                    //_buffer = tr.ReadToEnd();
                    //ストリームの末端まで繰り返す
                    while (tr.Peek() > -1)
                    {
                        dataList.Add(tr.ReadLine());
                    }
                }
            }

            sw.Stop(); Debug.WriteLine("メモリリード-" + sw.Elapsed);
            sw.Reset(); sw.Start();

            /*using (StringReader rs = new System.IO.StringReader(_buffer))
            {
                //ストリームの末端まで繰り返す
                while (rs.Peek() > -1)
                {
                    c1FlexGrid1.AddItem(rs.ReadLine());
                }
            }*/
            //string[] _buffers = _buffer.Split('\n');

            sw.Stop(); Debug.WriteLine("行分割-" + sw.Elapsed);
            sw.Reset(); sw.Start();

            //dataList.AddRange(_buffers);

            sw.Stop(); Debug.WriteLine("データセット-" + sw.Elapsed);
            sw.Reset(); sw.Start();

            //バーチャルモードを有効に変更
            ListView1.VirtualMode = true;
            //バーチャルリストの行数
            ListView1.VirtualListSize = dataList.Count;

            //ListViewItem items = new ListViewItem(_buffers);
            //c1FlexGrid1.Items.Add(items);

            sw.Stop();Debug.WriteLine("バーチャルセット-" + sw.Elapsed);
            Debug.WriteLine("ここまで-" + sw.ElapsedTicks);

#endif
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