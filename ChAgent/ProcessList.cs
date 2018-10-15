using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace ChAgent
{
    public partial class ProcessList : Form
    {
        public ProcessList()
        {
            InitializeComponent();
            ShowProcessList();
        }

        private void ShowProcessList()
        {
            listBox_processList.Items.Clear();

            foreach (var process in Process.GetProcesses())
            {
                if (string.IsNullOrEmpty(process.MainWindowTitle) == false)
                {
                    listBox_processList.Items.Add(process.MainWindowTitle);
                }
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            MainForm.SelectedProcessList.Clear();

            foreach (var item in listBox_processList.SelectedItems)
            {
                MainForm.SelectedProcessList.Add(item.ToString());
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            MainForm.SelectedProcessList.Clear();

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
