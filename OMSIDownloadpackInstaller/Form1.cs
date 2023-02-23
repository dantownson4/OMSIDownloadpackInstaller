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

            //DEBUG
            configDirectory = @"D:\Users\danie\Desktop\ProgrammingTemp\";

            if (!(File.ReadAllText(configDirectory + "config.txt") is null))
            {
                OMSIDirectory = File.ReadAllText(configDirectory + "config.txt");
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
                if (d.Contains("Halycon_AI"))
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

        //browse button
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
                switch (addon.ToString())
                {
                    case "Halycon_AI_Pack_3":
                        AddHumansToMaps(configDirectory + @"EN\DLP3.txt", 3);
                        break;
                    case "Halycon_AI_Pack_5":
                        AddHumansToMaps(configDirectory + @"EN\DLP5.txt", 5);
                        break;
                    case "Halycon_AI_Pack_6":
                        AddHumansToMaps(configDirectory + @"EN\DLP6.txt", 6);
                        break;
                    case "Halycon_AI_Pack_8":
                        AddHumansToMaps(configDirectory + @"EN\DLP8.txt", 8);
                        break;
                }
            }

        }

        private void AddHumansToMaps(string humansFile, int downloadpackNum)
        {


            foreach (var map in listBoxMaps.CheckedItems)
            {
                string mapDirectory = OMSIDirectory + @"\maps\" + map;
                string existingFile = File.ReadAllText(mapDirectory + @"\humans.txt");

                if (!existingFile.Contains(File.ReadAllText(humansFile)))
                {
                    using (StreamWriter sw = File.AppendText(mapDirectory + @"\humans.txt"))
                    {
                        using (StreamReader sr = File.OpenText(humansFile))
                        {
                            string line = "";
                            while ((line = sr.ReadLine()) != null)
                            {
                                sw.WriteLine("");
                                sw.Write(line);
                            }
                        }
                    }

                    Console.WriteLine(map.ToString() + " humans.txt updated");

                    string ticketPackLocation = "";
                    using (StreamReader sr = File.OpenText(mapDirectory + @"\global.cfg")){

                        string line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Equals("[ticketpack]"))
                            {
                                ticketPackLocation = sr.ReadLine();
                                break;
                            }
                        }

                    }

                    AddTicketpackToMap(Path.GetDirectoryName(OMSIDirectory + @"\" + ticketPackLocation), downloadpackNum);
                    Console.WriteLine(map.ToString() + " TicketPacks updated");

                }
                

            }

        }

        private void AddTicketpackToMap(string ticketPackLocation, int downloadpackNum)
        {

            string[] directories = Directory.GetDirectories(OMSIDirectory + @"\TicketPacks\Berlin_1");
            foreach (string d in directories)
            {
                //English variations
                if (d.Contains("AddOn_DLP-Vol-" + downloadpackNum + "_Man_") || d.Contains("AddOn_DLP-Vol-" + downloadpackNum + "_Woman_") || d.Contains("AddOn_DLP-Vol-" + downloadpackNum + "_Child_"))
                {
                    CopyDirectory(d, ticketPackLocation);

                }
            }

        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            string folderName = sourceDir.Substring(sourceDir.IndexOf(@"Berlin_1\") + 9, (sourceDir.Length - (sourceDir.IndexOf(@"Berlin_1\") + 9)));
            destinationDir += @"\" + folderName;
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }
        }


        private void buttonInstallDE_Click(object sender, EventArgs e)
        {

        }
    }
}
