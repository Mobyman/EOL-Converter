using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace eolconvertergui
{

    public partial class Form1 : Form
    {
        List<FileItem> allFileList = new List<FileItem>();
        public Form1()
        {
            InitializeComponent();
        }

        public bool addDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                string[] fileList = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

                foreach (string fileItem in fileList)
                {
                    addFile(fileItem);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool addFile(string path, int index = -1, bool ignoreext = false)
        {
            if (File.Exists(path))
            {
                
                string[] extensions = new string[] { ".php", ".css", ".html", ".htm", ".js", ".txt", ".htaccess", ".gitattributes", ".gitignore", ".xml", ".sql", ".xsl" };
                if (Array.IndexOf(extensions, Path.GetExtension(path)) >= 0 || ignoreext)
                {
                    if (index == -1)
                    {
                        index = this.fileslist.Items.Add(Path.GetFileName(path)).Index;
                        this.fileslist.Items[index].SubItems.Add(Path.GetFullPath(path));
                        this.fileslist.Items[index].SubItems.Add("В очереди");
                    }
                    FileItem fi = new FileItem();
                    fi.Index = index;
                    fi.Filename = path;
                    allFileList.Add(fi);
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        private void adddir_Click(object sender, EventArgs e)
        {
            if (selectFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                addDirectory(selectFolder.SelectedPath);
            }
        }

        private void addfile_Click(object sender, EventArgs e)
        {
            selectFile.Filter = "Текстовые файлы|*.php;*.css,*.html;*.htm;*.js;*.txt;*.htaccess;*.gitattributes;*.gitignore;*.xml;*.sql;*.c;*.cpp;*.h;*.cs;*.sln;*.xsl;*.ini|Все файлы|*.*";
            selectFile.Multiselect = true;
            if (selectFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (String file in selectFile.FileNames)
                {
                    addFile(file, -1, true);
                }
            }
        }

        private void start_Click(object sender, EventArgs e)
        {
            string endline;
           
            if (windowsrnToolStripMenuItem.Checked)
                endline = "\r\n";
            else
                endline = "\n";

            foreach(FileItem fileItem in allFileList) {
                try
                {
                    if (this.fileslist.Items[fileItem.Index].SubItems[2].Text != "OK")
                    {
                        StreamReader streamReader = new StreamReader(fileItem.Filename);
                        string fileContent = "";

                        while (!streamReader.EndOfStream)
                        {
                            fileContent += streamReader.ReadLine() + endline;
                        }
                        streamReader.Close();
                        StreamWriter sw = new StreamWriter(fileItem.Filename);
                        sw.Write(fileContent);
                        sw.Close();
                    }
                    this.fileslist.Items[fileItem.Index].SubItems[2].Text = "OK";
                }
                catch
                {
                    this.fileslist.Items[fileItem.Index].SubItems[2].Text = "IOError";
                }
            }
        }

        private void windowsrnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            linuxnToolStripMenuItem.Checked = false;
            windowsrnToolStripMenuItem.Checked = true;
        }

        private void linuxnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            windowsrnToolStripMenuItem.Checked = false;
            linuxnToolStripMenuItem.Checked = true;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            allFileList.Clear();
            this.fileslist.Items.Clear();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("EOL Converter преобразует окончания строк в CRLF или LF.\nСайт автора: http://mobyman.org.\nGithub: https://github.com/mobyman.", "О программе");
        }

        private void fileslist_KeyDown(object sender, KeyEventArgs e)
        {
            Keys code = e.KeyCode;
            if (code == Keys.Delete || code == Keys.Insert)
            {
                foreach (ListViewItem item in fileslist.SelectedItems)
                {
                    if (item.SubItems[2].Text != "OK")
                    {
                        if (code == Keys.Delete && item.SubItems[2].Text != "Исключен")
                        {
                            for (int i = 0; i < allFileList.Count; i++)
                            {
                                if (allFileList[i].Index.ToString() == item.Index.ToString())
                                {
                                    allFileList.RemoveAt(i);
                                    item.ForeColor = Color.Gray;
                                    item.SubItems[2].Text = "Исключен";
                                }
                            }
                        }

                        if (code == Keys.Insert && item.SubItems[2].Text != "В очереди")
                        {
                            addFile(item.SubItems[1].Text, Int32.Parse(item.Index.ToString()));
                            item.ForeColor = Color.Black;
                            item.SubItems[2].Text = "В очереди";
                        }
                    }
                }
            }
            if (e.Modifiers == Keys.Control && code == Keys.A)
            {
                foreach (ListViewItem item in fileslist.Items)
                {
                    item.Selected = true;
                }
            }
            if (code == Keys.Escape)
            {
                foreach (ListViewItem item in fileslist.Items)
                {
                    item.Selected = false;
                }
            }
            if (code == Keys.Space)
            {
                foreach (ListViewItem item in fileslist.Items)
                {
                    item.Selected = !item.Selected;
                    item.Focused = false;
                }

            }
        }
       
    }

    public class FileItem
    {
        public int Index { get; set; }
        public string Filename { get; set; }
    }
}
