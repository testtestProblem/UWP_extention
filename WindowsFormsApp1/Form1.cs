using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                // List<ParameterItem> parameters = new List<ParameterItem>();
                // for (int i = 1; i < args.Length; i++)
                // {
                // parameters.Add(new ParameterItem("arg" + i.ToString(), args[i]));
                // }
                // this.DataContext = parameters;

                //textBox1.Text = args[0];
                label1.Text = args[0] + "\n" + args[1] + "\n" + args[2];
                //string parameters2 = ApplicationData.Current.LocalSettings.Values["parameters"] as string;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
