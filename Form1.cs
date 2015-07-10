using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Media;
using Microsoft.Win32;

namespace osu_export
{
    public partial class Form1 : Form
    {

        private string installDir;
        private string InstallDir
        {
            get
            {
                return installDir;
            }
            set
            {
                if (installDir != value)
                {
                    installDir = value;
                    textBox1.Text = value;
                }
            }
        }

        private string outputDir;
        private string OutputDir
        {
            get
            {
                return outputDir;
            }
            set
            {
                if (outputDir != value)
                {
                    outputDir = value;
                    textBox2.Text = value;
                }
            }
        }

        public Form1()
        {

            InitializeComponent();

            bool foundosuinstallation = false;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (string subkey_name in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(subkey_name);
                if ((string)subkey.GetValue("DisplayName") == "osu!")
                {
                    foundosuinstallation = true;
                    string uninstallstring = subkey.GetValue("UninstallString").ToString();
                    InstallDir = Path.GetDirectoryName(uninstallstring.Remove(uninstallstring.Length - 11));
                }
            }

            if (!foundosuinstallation)
            {
                MessageBox.Show("It seems like osu! isn't installed on this machine. \nInstall osu! first or try setting the installation directory manually!", "osu!export error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }

            OutputDir = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\osu!export";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            disableGUI();
            Thread thread = new Thread(new ThreadStart(WorkThread));
            thread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                InstallDir = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                OutputDir = folderBrowserDialog2.SelectedPath;
            }
        }

        private void disableGUI()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
        }

        private void enableGUI()
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        private void WorkThread()
        {
            if (Directory.Exists(InstallDir + @"\Songs"))
            {
                string[] directories = Directory.GetDirectories(InstallDir + @"\Songs");
                if (directories.Length > 0)
                {
                    if (!Directory.Exists(OutputDir))
                    {
                        Directory.CreateDirectory(OutputDir);
                    }

                    this.Invoke((MethodInvoker)delegate
                    {
                        progressBar1.Value = 0;
                        progressBar1.Maximum = directories.Length;
                    });

                    foreach (string directory in directories)
                    {
                        string beatmapfile = Directory.GetFiles(directory, "*.osu")[0];
                        string[] lines = File.ReadAllLines(beatmapfile);
                        Dictionary<string, string> metadata = new Dictionary<string, string>();
                        metadata.Add("filename", "");
                        metadata.Add("fileextension", "");
                        metadata.Add("title", "");
                        metadata.Add("artist", "");
                        metadata.Add("artwork", "");
                        foreach (string line in lines)
                        {
                            if (line.Contains("AudioFilename: "))
                            {
                                metadata["filename"] = line.Substring(line.LastIndexOf(":") + 2);
                                metadata["fileextension"] = metadata["filename"].Substring(metadata["filename"].LastIndexOf("."));
                            }

                            if (line.Contains("Title:"))
                            {
                                metadata["title"] = line.Substring(line.LastIndexOf(":") + 1).Replace(" (TV Size)", "");
                            }

                            if (line.Contains("Artist:"))
                            {
                                metadata["artist"] = line.Substring(line.LastIndexOf(":") + 1);
                            }

                            if (File.Exists(directory + @"\bg.jpg"))
                            {
                                metadata["artwork"] = directory + @"\bg.jpg";
                            }
                        }

                        try
                        {
                            File.Copy(directory + @"\" + metadata["filename"], OutputDir + @"\" + metadata["title"] + metadata["fileextension"], true);
                            TagLib.File copiedfile = TagLib.File.Create(OutputDir + @"\" + metadata["title"] + metadata["fileextension"]);
                            copiedfile.RemoveTags(TagLib.TagTypes.Id3v2);
                            copiedfile.Tag.Title = metadata["title"];
                            copiedfile.Tag.Performers = new string[] { metadata["artist"] };
                            if (metadata["artwork"] != "")
                                copiedfile.Tag.Pictures = new TagLib.Picture[] { new TagLib.Picture(metadata["artwork"]) };
                            copiedfile.Save();
                        }
                        catch (Exception error)
                        {
                            MessageBox.Show("An error occurred while trying to copy a file! \n\n" + error.Message, "osu!export error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            progressBar1.PerformStep();
                        });
                    }
                    SystemSounds.Asterisk.Play();
                }
                else
                {
                    MessageBox.Show("You don't have any songs in your osu! \"Songs\" folder!", "osu!export error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                MessageBox.Show("The osu! \"Songs\" folder was not found. \nPlease select your osu! installation directory.", "osu!export error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }

            this.Invoke((MethodInvoker)delegate
            {
                enableGUI();
            });

        }

    }
}
