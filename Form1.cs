using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExtractorUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //listView1.ListViewItemSorter = null;
            //while(p.StandardOutput.Peek() != -1)
            //{

            //}
            //listBox1.Items.Add(p.StandardOutput.ReadToEnd());
            //p.WaitForExit();
        }

        private void P_Exited(object sender, EventArgs e)
        {
            
            
            this.Invoke(new LBupdate(() =>
            {
                listView1.EndUpdate();
                label1.Text = "All files completed";

            }));

        }

        private void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //if (e.Data != null)
                //this.Invoke(new LBupdate(() => listBox1.Items.Add(e.Data)));

        }
        static int counter;
        public delegate void LBupdate();
        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                counter++;
                listView1.Invoke(new LBupdate(() =>
                {
                    var data = e.Data.Split('\t');
                    ListViewItem lvi = new ListViewItem(data);
                    listView1.Items.Add(lvi);
                    if (counter % 25 == 0)
                    {
                        //listView1.EndUpdate();
                        //listView1.BeginUpdate();
                    }
                }));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                listView1.Items.Clear();
                listView1.BeginUpdate();
                List(openFileDialog1.FileName);
            }
        }

        private void List(string filename)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("Extractor.exe", $"-i \"{filename}\"");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.EnableRaisingEvents = true;
            p.Exited += P_Exited;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += P_OutputDataReceived;
            p.ErrorDataReceived += P_ErrorDataReceived;
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(openFileDialog1.FileName) && File.Exists(openFileDialog1.FileName))
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    Process p2 = new Process();
                    p2.StartInfo = new ProcessStartInfo("Extractor.exe", $"-i \"{openFileDialog1.FileName}\" -o \"{folderBrowserDialog1.SelectedPath}\"");
                    p2.StartInfo.UseShellExecute = false;
                    p2.StartInfo.CreateNoWindow = true;
                    p2.EnableRaisingEvents = true;
                    p2.Exited += P2_Exited; ;
                    p2.StartInfo.RedirectStandardOutput = true;
                    p2.StartInfo.RedirectStandardError = true;
                    p2.OutputDataReceived += P2_OutputDataReceived; ;
                    p2.ErrorDataReceived += P2_ErrorDataReceived; ;
                    p2.Start();
                    p2.BeginOutputReadLine();
                    p2.BeginErrorReadLine();
                }
            }
        }

        private void P2_Exited(object sender, EventArgs e)
        {
                this.Invoke(new LBupdate(() =>
                {
                    label1.Text = "All files extracted";

                }));
        }

        private void P2_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void P2_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Data != null)
                this.Invoke(new LBupdate(() =>
                {
                    label1.Text = e.Data;
                }));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                listView1.Items.Clear();
                List(openFileDialog2.FileName);
                label1.Text = "Working";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = listView1.Items.Count + " files";
        }
    }
}
