using RLoggerLib;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        private readonly IRLogger _logger = RLogger.Instance;
        public Form1()
        {
            InitializeComponent();
            _logger.LogDebug("Form1 is initialized","ExampleSource");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _logger.LogInfo("Button1 is clicked");
        }
    }
}
