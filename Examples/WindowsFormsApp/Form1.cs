using RLoggerLib;
using RLoggerLib.LoggingTargets;
using System;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        private readonly IRLogger _logger = RLogger.Instance;
        public Form1()
        {
            InitializeComponent();
            _logger.LogDebug("Form1 is initialized", "ExampleSource");
            _logger.AddTextFileLogging(new TextFileLoggingTargetOptions()
            {
                FileNamingConvention = LogFileNamingConvention.CustomDate,
                CustomName = "WindowsFormsApp",
                DateFormat = "yyyy-MM-dd",
            });
            _logger.LogTrace("Text file logging is added", "ExampleSource", "B2");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _logger.LogInfo("Button1 is clicked");
        }
    }
}
