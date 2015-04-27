using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Data.Odbc;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.Drawing.Drawing2D;

namespace logMente
{
    public partial class display : Form
    {
        //XMLRead()
        //toolStripButtonSearch_Click(null, null);
        //_buffer = tr.ReadToEnd();
        //�X�g���[���̖��[�܂ŌJ��Ԃ�

        enum FlexCol { Title, Check, Jogai, Index, Name, Value, Cols };
        private XmlDocument _xmldoc = new XmlDocument();
        private List<string[]> attrInfo = new List<string[]>();

        enum ListCol { Line, Cols };
        private List<string[]> dataList = new List<string[]>();

        public display()
        {
            InitializeComponent();
            ListView1.Columns[0].Width = 10000;

            Encoding enc = Encoding.GetEncoding("shift_jis");

            string filePath = @"C:\zabbix\log\zabbix_agentd_sjis_1.log";

            ListView1.BeginUpdate();
            ListView1.VirtualListSize = 0;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (TextReader tr = new StreamReader(fs, enc))
                {
                    while (tr.Peek() > -1)
                    {
                        string [] dataLine= new string [(int)ListCol.Cols] {tr.ReadLine()};
                        dataList.Add(dataLine);
                    }
                }
            }

            //�o�[�`�������X�g�̍s��
            ListView1.VirtualListSize = dataList.Count;
            ListView1.EndUpdate();
        }

        private void ListView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(Brushes.Aqua, e.Bounds);
                e.DrawFocusRectangle();
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
                e.DrawFocusRectangle();
            }

            e.DrawText();
        }

        private static bool FindVersion(string [] line)
        {

            if (line[(int)ListCol.Line].Contains("active check"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ListView1.BeginUpdate();
            ListView1.VirtualListSize = 0;

            dataList = dataList.FindAll(FindVersion);

            ListView1.VirtualListSize = dataList.Count;
            ListView1.EndUpdate();
        }

        private void XMLRead()
        {
            c1FlexGrid1.Rows.RemoveRange(1, c1FlexGrid1.Rows.Count - 1);

            // XML�t�@�C����ǂݍ���
            string xml = Properties.Resources.saveCondition;
            _xmldoc.LoadXml(xml); //using (StringReader tr = new StringReader(xml)) _xmldoc.Load(tr); 

            //XmlNodeList xlist = _xmldoc.SelectNodes(@"//node()");
            foreach (XmlElement element in _xmldoc.DocumentElement)
            {
                string[] setvalues = new string[(int)FlexCol.Cols];
                foreach (XmlElement node in element.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "checked":
                            int value = stateDataToValue(node.InnerText);
                            setvalues[(int)FlexCol.Check] = stateValueToCheck(value)[0];
                            setvalues[(int)FlexCol.Jogai] = stateValueToCheck(value)[1];
                            break;
                        case "index":
                            setvalues[(int)FlexCol.Index] = node.InnerText;
                            break;
                        case "name":
                            setvalues[(int)FlexCol.Name] = node.InnerText;
                            break;
                        case "value":
                            setvalues[(int)FlexCol.Value] = node.InnerText;
                            break;
                    }
                }
                c1FlexGrid1.AddItem(setvalues);
            }
            c1FlexGrid1.Sort(C1.Win.C1FlexGrid.SortFlags.Ascending, (int)FlexCol.Index);
        }


        private void c1FlexGrid1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex < dataList.Count)
            {
                if (e.Item == null)
                {
                    e.Item = new ListViewItem(dataList[e.ItemIndex][(int)ListCol.Line]);
                }
            }
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListView1.SelectedIndices.Count > 0)
            {
                label1.Text = "" + ListView1.SelectedIndices[0];
            }
        }











        // �\���̒�`
        enum CsvParameter
        {
            ColumnIndex = 0,
            ColumnName = 1,
            ColumnValue = 2,
        };
        readonly string[] CSV_COLUMN_ATTRBUTE = { "ColumnIndex", "ColumnName", "ColumnValue" };

        enum LinkParameter
        {
            Index = 0,
            MachineName = 1,
            UserName = 2,
            LinkName = 3,
            LinkTarget = 4,
        };
        readonly string[] LINK_COLUMN_ATTRBUTE = { "Index", "MachineName", "UserName", "LinkName", "LinkTarget" };

        const string FILE_NAME_SCHEMA = "schema.ini";



        // ���C������
        private void toolStripButtonSearch_Click(object sender, EventArgs e)
        {
            // ����CSV�̐���
            DataTable dtResult = new DataTable("Table");
            try
            {
                string csvPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string csvFile = "input.csv";

                if (File.Exists(csvPath + @"\" + "schema.ini"))
                {
                    File.Delete(csvPath + @"\" + "schema.ini");
                }

                // XSLT�t�@�C��(���\�[�X��̃e�L�X�g)����CSV�̍\�����擾����
                string xsltText = Properties.Resources.transformXslt;
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(xsltText);
                List<Dictionary<string, string>> colList = xsltAnalyze(xmldoc.ChildNodes[0]);

                // �C���v�b�g�̃X�L�[�}�w��
                //writeSchema(csvPath, csvFile, colList, "True");

                // CSV�t�@�C�����e�[�u���Ƃ��Ď擾
                using (DataTable dtCsv = getDtFromCsv(csvPath, csvFile))
                {
                    // ����CSV�̃J��������
                    string[] headerLine = new string[colList.Count];
                    foreach (Dictionary<string, string> column in colList)
                    {
                        DataColumn dataColumn = new DataColumn(column[CSV_COLUMN_ATTRBUTE[(int)CsvParameter.ColumnName]]);
                        //dataColumn.Caption = column[CSV_COLUMN_ATTRBUTE[(int)CsvParameter.ColumnName]];
                        headerLine.SetValue(column[CSV_COLUMN_ATTRBUTE[(int)CsvParameter.ColumnName]],
                            Int32.Parse(column[CSV_COLUMN_ATTRBUTE[(int)CsvParameter.ColumnIndex]]));
                        dtResult.Columns.Add(dataColumn);
                    }

                    // ����CSV�̃w�b�_�s����
                    dtResult.Rows.Add(headerLine);

                    // �[���s�̃}�[�W
                    // �i�}�[�W�͂��Ȃ��B���L�f�[�^�͖{�Ԃł͕K�v�Ȃ��ׂĂ̍s�̃J�����������Ă���O��j
                    //dtResult.Merge(dtCsv, true);

                    // �V���[�g�J�b�g���W
                    List<Dictionary<string, string>> dmyData = searchShortcuts();

                    // ����CSV�t�@�C���ҏW
                    foreach (Dictionary<string, string> dmy in dmyData)
                    {
                        string[] values = new string[dmy.Values.Count];
                        dmy.Values.CopyTo(values, 0);
                        dtResult.Rows.Add(values);
                    }
                }

                // �y�e�[�u��XML�ɃV���A���C�Y�z

                // �o�̓t�@�C��
                string savePath = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string saveFile = "output.csv";

                //File.Delete(savePath + @"\" + saveFile);

                // �A�E�g�v�b�g�̃X�L�[�}�w��
                //writeSchema(savePath, saveFile, colList, "True");

                // �V���A���C�Y
                StringWriter writeBuffer = new StringWriter(new StringBuilder());
                dtResult.WriteXml(writeBuffer, XmlWriteMode.WriteSchema);

                using (TextWriter resot = new StreamWriter(savePath + @"\" + saveFile + ".txt"))
                    resot.Write(writeBuffer.ToString());

                // �y�g�����X�t�H�[�����s�z

                // XSLT�t�@�C�����g�����X�t�H�[���p�ɓǂݍ���
                XmlReaderSettings xsltReadConfig = new XmlReaderSettings();
                TextReader xsltInp = new StringReader(xsltText);
                XmlReader xsltReader = XmlReader.Create(xsltInp, xsltReadConfig);

                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(xsltReader);


                // �y�g�����X�t�H�[�����s�z
                // ���o�̓t�H�[�}�b�g �̎w��
                XmlReaderSettings readConfig = new XmlReaderSettings();

                TextReader inps = new StringReader(writeBuffer.ToString());
                XmlReader reader = XmlReader.Create(inps, readConfig);

                XmlWriterSettings writeConfig = new XmlWriterSettings();
                writeConfig.Indent = false;
                writeConfig.OmitXmlDeclaration = true;
                writeConfig.ConformanceLevel = ConformanceLevel.Auto;


                StringBuilder outs = new StringBuilder();
                XmlWriter writer = XmlWriter.Create(outs, writeConfig);

                xslt.Transform(reader, writer);

                //using (TextWriter resot = new StreamWriter(savePath + @"\" + FILE_NAME_SCHEMA, false, Encoding.GetEncoding(932)))
                using (TextWriter resot = new StreamWriter(savePath + @"\" + saveFile, false, Encoding.GetEncoding(932)))
                {
                    resot.Write(outs.ToString());
                }

                //System.IO.File.Move(savePath + @"\" + FILE_NAME_SCHEMA, savePath + @"\" + saveFile);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            finally
            {
                dtResult.Dispose();
            }

        }

        // xslt����CSV�̍\�����擾����
        private List<Dictionary<string, string>> xsltAnalyze(XmlNode styleSeet)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            /*XmlNamespaceManager contxt = new XmlNamespaceManager(xmldoc.NameTable);
            contxt.AddNamespace("xsl", "value-of");
            XmlNodeList list = xmldoc.SelectNodes("//xsl:value-of", contxt);*/
            int ii = 0;
            foreach (XmlNode nodeTemp in styleSeet.ChildNodes)
            {
                foreach (XmlNode nodeValue in nodeTemp.ChildNodes)
                {
                    if (nodeValue.Name == "xsl:value-of")
                    {
                        Dictionary<string, string> elem = new Dictionary<string, string>();
                        elem.Add(CSV_COLUMN_ATTRBUTE[(int)CsvParameter.ColumnIndex], "" + ii++);
                        elem.Add(CSV_COLUMN_ATTRBUTE[(int)CsvParameter.ColumnName], nodeValue.Attributes["select"].Value);
                        result.Add(elem);

                    }
                }
            }
            return result;
        }


        // CSV�t�@�C����DataTable�Ɏ擾����
        private DataTable getDtFromCsv(string csvDir, string csvFileName)
        {
            DataTable dt = null;

            //�ڑ�������
            string conString = "Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + csvDir + ";Extensions=asc,csv,tab,txt;";
            OdbcConnection con = new System.Data.Odbc.OdbcConnection(conString);

            string commText = "SELECT * FROM [" + csvFileName + "]";
            using (OdbcDataAdapter da = new System.Data.Odbc.OdbcDataAdapter(commText, con))
            {
                //DataTable�Ɋi�[����
                using (dt = new DataTable())
                {
                    da.Fill(dt);
                }
            }

            if (dt == null)
            {
                throw new Exception("CSV�̃e�[�u���W�J�Ɏ��s���܂����B");
            }
            return dt;
        }


        // �X�L�[�}�ݒ�
        /*private void writeSchema(string csvDir, string csvFileName, List<Dictionary<string, string>> colList, string titleOn)
        {
            try
            {
                using (FileStream fsOutput = new FileStream(csvDir + @"\" + FILE_NAME_SCHEMA,
                                         FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter srOutput = new StreamWriter(fsOutput, Encoding.GetEncoding(932)))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("[" + csvFileName + "]");
                        sb.AppendLine("ColNameHeader=" + titleOn);
                        sb.AppendLine("Format=CSVDelimited");
                        //sb.AppendLine("MaxScanRows=100000");
                        //sb.Append("CharacterSet=OEM");
                        for (int ii = 1; ii < colList.Count + 1; ii++)
                        {
                            sb.AppendLine("Col" + ii + "=" + colList[ii - 1][CSV_COLUMN_ATTRBUTE[(int)CsvParameter.ColumnName]] + " LongChar");
                        }
                        //sb.AppendLine("Col2=user_name Char Width 32");
                        //sb.AppendLine("Col3=notes LongChar");
                        srOutput.WriteLine(sb.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }*/

        // �f�[�^�ݒ�
        private List<Dictionary<string, string>> searchShortcuts()
        {
            List<Dictionary<string, string>> dmyData = new List<Dictionary<string, string>>();
            Dictionary<string, string> dmyRow = new Dictionary<string, string>();
            //dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.Index], "0");
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.MachineName], "SJ200OK");
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.UserName], "Administratir");
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.LinkName], "�P�Q�R�S�T�U�V�W.lnk");
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.LinkTarget], @"C:\Documents\RUN.exe");
            dmyData.Add(dmyRow);

            dmyRow = new Dictionary<string, string>();
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.MachineName], "SJ201TL");
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.UserName], "TargetLess");
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.LinkName], "�V���[�g�J�b�g.lnk");
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.LinkTarget], @"C:\Documents\SCH.exe");
            dmyData.Add(dmyRow);

            dmyRow = new Dictionary<string, string>();
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.MachineName], "SJ202NL");
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.UserName], "LinkNameLess");
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.LinkName], "�V���[�g�J�b�g�Q.lnk");
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.LinkTarget], @"C:\Documents\SCH02.exe");
            dmyData.Add(dmyRow);

            dmyRow = new Dictionary<string, string>();
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.MachineName], "SJ205UL");
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.UserName], "z050257.AD");
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.LinkName], "�P�Q�R�S�T�U�V�W.lnk");
            dmyRow.Add(LINK_COLUMN_ATTRBUTE[(int)LinkParameter.LinkTarget], @"C:\Documents\RUN.exe");
            dmyData.Add(dmyRow);

            return dmyData;
        }

        private int stateDataToValue(string data)
        {
            int value = 0;
            bool flag;
            if (Boolean.TryParse(data, out flag))
            {
                if (flag)
                {
                    value = 1;
                }
            }
            else
            {
                Int32.TryParse(data, out value);
            }

            return value;
        }

        private string[] stateValueToCheck(int value)
        {
            switch (value)
            {
                case 1:
                    return new string[] { Boolean.TrueString, Boolean.FalseString };
                case 2:
                    return new string[] { Boolean.FalseString, Boolean.TrueString };
                default:
                    return new string[] { Boolean.FalseString, Boolean.FalseString };
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

        private void Console_WriteLine(string caption, string[] param)
        {
            for (int ii = 0; ii < param.Length; ii++)
            {
                Console.WriteLine(caption + param[ii]);
            }

        }

#if false   
            //
            // �ۑ��R�[�h
            //

            // �e�[�u���p�̃V���A���C�U�𐶐����ăV���A���C�Y
            /*XmlSerializer serializer = new XmlSerializer(dtCsv.GetType());
            serializer.Serialize(writeBuffer, dtCsv);*/
            //dtCsv.TableName = "Table";

            // �t�@�C�����X�gxml�������ɒ��o����ꍇ
            // �w��Ȃ��i���ڂȂ��j�������i�X���[�j
            attrInfo.Add(new string[] { FileAttributes.ReadOnly.ToString(), "true" });  // checked �� ��荞��
            attrInfo.Add(new string[] { FileAttributes.ReadOnly.ToString(), "false" }); // unchecked �� ���O

            //
            // �ł��Axpath�̃N�G���[�ɂ���ixml�ɂ͑S�t�H���_�E�t�@�C�����K�w�Ŋi�[����B�j
            //

            // �v���W�F�N�g�l�[���́AAssemblyInfo.cs �̂��̂��g���Ă���B
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
#endif
    }
}