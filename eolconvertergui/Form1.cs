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

        public bool addFile(string path)
        {
            if (File.Exists(path))
            {
                string[] extensions = new string[] { ".php", ".css", ".html", ".htm", ".js", ".txt", ".htaccess", ".gitattributes", ".gitignore", ".xml", ".sql" };
                if (Array.IndexOf(extensions, Path.GetExtension(path)) >= 0)
                {
                    int index = this.fileslist.Items.Add(Path.GetFileName(path)).Index;
                    this.fileslist.Items[index].SubItems.Add(Path.GetFullPath(path));
                    this.fileslist.Items[index].SubItems.Add("В очереди");
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
            if (selectFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                addFile(selectFile.FileName);
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
            System.Windows.Forms.MessageBox.Show("EOL Converter преобразует окончания строк в CRLF или LF.\nСайт автора: http://mobyman.org.\nGithub: https://github.com/mobyman.", "О программе");
        }
    }

    public class FileItem
    {
        public int Index { get; set; }
        public string Filename { get; set; }
    }
}
