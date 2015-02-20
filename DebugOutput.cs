using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ListHosts
{
    public partial class DebugOutput : Form
    {
        public DebugOutput()
        {
            InitializeComponent();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void DebugOutput_Load(object sender, EventArgs e)
        {
            Console.SetOut(new TextBoxWriter(textBox1));
            Console.WriteLine("Now redirecting output to the text box");
        }
        


    }
}
