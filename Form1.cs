using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Net;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace ListHosts
{
    public partial class Form1 : Form
    {
        public static string fileLocation = pathtrim(Application.StartupPath) + "ListHosts.csv";
        public List<string> routingInfo = new List<string>();

        public Form1()
        {
            InitializeComponent();
            if (Program.debug)
            {
                DebugOutput dop = new DebugOutput();
                dop.Show();
            }
            Console.WriteLine("Program Started");
            this.Icon = Properties.Resources.MiniLogoH;
            textBox1.Text = fileLocation;
            Console.WriteLine("File location set to " + fileLocation);
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
        }


        #region Primary Methods...



        /// <summary>
        /// Gets List of IP Addresses, MAC Addresses, and Host Names
        /// </summary>
        /// <returns></returns>
        public static List<string> getRoutingInfo()
        {
            Console.WriteLine("Getting Routing Information");
            List<string> arpInfo = new List<string>();
            List<string> pingInfo = new List<string>();
            arpInfo = getARP();            
            
            foreach (string s in arpInfo)
            {
                string host = "";
                string info = "";
                string[] spl = { " " };
                string[] blip = s.Split(spl, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    host = getHost(blip[0]);
                    info = s.Replace(blip[3],"") + "   " + host;
                    pingInfo.Add(info);
                    Console.WriteLine("Returned " + info);
                }
                catch
                {
                    Console.WriteLine("Skipping " + blip[0]);
                }
            }

            return pingInfo;

        }





        #endregion

        #region Secondary Methods...
        /// <summary>
        /// Gets the hostname of a system based on the IP address
        /// </summary>
        /// <param name="remoteAddress">IP address to be resolved</param>
        /// <returns></returns>
        public static string getHost(string remoteAddress)
        {
            IPHostEntry host = Dns.GetHostEntry(remoteAddress);
            return host.HostName;
        }

        /// <summary>
        /// Gets the full Routing Table (runs Verbose)
        /// </summary>
        /// <returns></returns>
        public static List<string> getARP()
        {
            List<string> arp = new List<string>();
            string blip = openapp("cmd", "/c arp -a -v");
            string[] spl = { "\n" };
            string[] blop = blip.Split(spl, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in blop)
            {
                if (!s.Contains("Interface") && !s.Contains("Internet"))
                {
                    arp.Add(s);
                }
            }
            return arp;
        }

        /// <summary>
        /// Opens an application and returns the ouptut if applicable
        /// </summary>
        /// <param name="app">Path to program</param>
        /// <param name="arguments">Programs arguments</param>
        /// <returns></returns>
        public static string openapp(string app, string arguments)
        {
            string op = "";



            ProcessStartInfo procStartInfo = new ProcessStartInfo(app, arguments);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            var proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            string result = proc.StandardOutput.ReadToEnd();
            //proc.WaitForExit();
            Console.WriteLine(result);
            op = result;



            return result;

        }


        /// <summary>
        /// Edits path to ensure it is in correct format.
        /// </summary>
        /// <param name="path">Directory Path</param>
        /// <returns>String Directory Path with "\" at the end</returns>
        public static string pathtrim(string path)
        {
            char[] rem = { '\\', ' ' };
            path = path.TrimEnd(rem);
            path = path + "\\";
            return path;
        }
        #endregion

        #region Events...

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            pictureBox1.Image = Properties.Resources.loading;
            backgroundWorker1.RunWorkerAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult ret = saveFileDialog1.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                fileLocation = saveFileDialog1.FileName;
                textBox1.Text = saveFileDialog1.FileName;
                Console.WriteLine("File location set to " + fileLocation);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            routingInfo = getRoutingInfo();
        }

        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Information gathered.");
            string outPut = "IP Address,MAC Address,Type,Host Name";
            foreach (string s in routingInfo)
            {
                string[] spl = { " " };
                string[] blip = s.Split(spl, StringSplitOptions.RemoveEmptyEntries);
                outPut = outPut + Environment.NewLine + blip[0] + "," + blip[1] + "," + blip[2] + "," + blip[3];
            }
            Console.WriteLine("Saving File");
            File.WriteAllText(fileLocation, outPut);
            Console.WriteLine("Done");
            panel1.Visible = false;
            MessageBox.Show("File saved to " + fileLocation);
        }

        #endregion

    }
}
