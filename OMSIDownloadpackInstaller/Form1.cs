using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace OMSIDownloadpackInstaller
{
    public partial class Form1 : Form
    {

        private string configDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private string OMSIDirectory;
        private bool validDirectory = false;
        private string[] invalidMaps = new string[] { "aerials", "Chrono"};

        public Form1()
        {
            InitializeComponent();

            label1.Font = new Font("Arial", 36, FontStyle.Bold);
            label3.Font = new Font("Arial", 22, FontStyle.Bold);
            label4.Font = new Font("Arial", 22, FontStyle.Bold);



            if (!(File.ReadAllText(configDirectory) is null))
            {
                OMSIDirectory = File.ReadAllText(configDirectory);
                directoryTextBox.Text = OMSIDirectory;
                CheckValidDirectory(OMSIDirectory);
            }


        }
        
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void LoadMaps()
        {
            string[] directories = Directory.GetDirectories(OMSIDirectory + @"\maps");
            foreach (string d in directories)
            {
                string folderName = d.Substring(d.IndexOf(@"maps\")+5, (d.Length- (d.IndexOf(@"maps\")+5)));
                if (!invalidMaps.Contains(folderName))
                {
                    listBoxMaps.Items.Add(folderName);
                }            
            }

        }

        private void LoadDownloadpacks()
        {
            string addonDirectory = OMSIDirectory + @"\Addons\";
            string[] directories = Directory.GetDirectories(addonDirectory);
            foreach (string d in directories)
            {
                if (d.Contains("Halycon"))
                {
                    
                    string folderName = d.Substring(d.IndexOf("Halycon"), 17);
                    listBoxDownloadpack.Items.Add(folderName);
                }
            }
        }

        private void CheckValidDirectory(string directory)
        {

            string[] files = Directory.GetFiles(directory);

            if (files.Any(x => x.Contains("Omsi.exe")))
            {
                validDirectory = true;
            }
            else
            {
                directoryTextBox.Text = "Invalid Directory!";
                validDirectory = false;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {

                    CheckValidDirectory(fbd.SelectedPath);
                    directoryTextBox.Text = fbd.SelectedPath;

                }
            }

        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (validDirectory)
            {
                LoadDownloadpacks();
                LoadMaps();
            }
        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            foreach (var addon in listBoxDownloadpack.CheckedItems)
            {

                Debug.WriteLine(addon.ToString());

                foreach (var map in listBoxMaps.CheckedItems)
                {
                    Debug.WriteLine("Installed " + addon.ToString() + " to " + map.ToString());
                }

            }

        }

        private string GetAddonData()
        {
            return null;
        }


    }
}
