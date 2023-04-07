using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;
using System.IO;
using System.Net;

namespace NotiGlish
{
    public partial class NotiGlishForm : Form
    {
        bool isNotClosed = true;
        int myRand = 0;
        string[] record;
        string READ_FILE_PATH = string.Format("{0}Resources\\read.txt", Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\")));
        string IMAGE_PATH = string.Format("{0}Resources\\NotiGlish.png", Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\")));
        int numOfNoti = 1;

        public NotiGlishForm()
        {
            InitializeComponent();
            //Handle when activated by click on notification
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                //Get the activation args, if you need those.
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

                //if the app instance just started after clicking on a notification 
                if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
                {
                    //MessageBox.Show("App was not running, " + "but started and activated by click on a notification.");
                    isNotClosed = false;
                    ToastNotificationManagerCompat.History.Clear();
                    Application.Exit();
                }
                else
                {
                    // if the user clicked the notification body
                    //MessageBox.Show("App was running, " + "and activated by click on a notification.");

                    // if the user clicked only the "show an example" button 
                    if (!string.IsNullOrWhiteSpace(toastArgs.Argument))
                    {
                        //MessageBox.Show("App was running, " + "and activated by click on a notification." + toastArgs.Argument);

                        new ToastContentBuilder()
                        .AddText("Example :")
                        /*.AddText("")*/
                        .AddText(record[1])
                        .SetToastDuration(ToastDuration.Long)

                        .AddButton(new ToastButton()
                        .SetContent("Thanks")
                        .SetDismissActivation())

                        .AddButton(new ToastButton()
                        .SetContent("Google Translate")
                        .SetProtocolActivation(new Uri("https://translate.google.com/?sl=en&tl=ar&text=" + record[1] + "&op=translate")))

                        .Show(toast =>
                        {
                            toast.Tag = "1337";
                        });
                    }
                }
            };
        }
        private void NotiGlishForm_Load(object sender, EventArgs e)
        {
            atStartupCheckBox.Checked = true;
            GetResources();
            richTextBox.Text = File.ReadAllText(READ_FILE_PATH);

        }

        private void NotiGlishForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized && isNotClosed)
            {
                ShowInTaskbar = false;
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(1000);
            }
        }

        
        
        /// <summary>
        /// returns a random line from a given file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        string GetLine(string fileName)
        {
            // Get a random number depending on the number of lines in the file.
            Random rand = new Random();
            if (File.ReadLines(fileName).Count() == 0)
            {
                MessageBox.Show("file is empty", "wrong");
                return "";
            }
            else
                myRand = rand.Next(1, File.ReadLines(fileName).Count());
            
            
            // Try to get the line form the file.
            try
            {
                using (var sr = new StreamReader(fileName))
                {
                    for (int i = 1; i < myRand; i++)
                        sr.ReadLine();
                    return sr.ReadLine();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("something wrong occurred !");
                return "";
            }
        }

        void ShowANoti()
        {
            // get a line from the file     Lines format : [Word]#[Example]#[Arabic]
            record = (GetLine(READ_FILE_PATH)).Split('#');

            // if file is not empty
            if (myRand > 0)
            {
                new ToastContentBuilder()
                .AddText(record[0])
                .AddText(record[2])
                .SetToastDuration(ToastDuration.Short)
                

                // Profile (app logo override) image
                .AddAppLogoOverride(new Uri(IMAGE_PATH), ToastGenericAppLogoCrop.Default)

                .AddButton(new ToastButton()
                .SetContent("Thanks")
                .SetDismissActivation())

                .AddButton(new ToastButton()
                .SetContent("Google Translate")
                .SetProtocolActivation(new Uri("https://translate.google.com/?sl=en&tl=ar&text=" + record[0] + "&op=translate")))

                .AddButton(new ToastButton()
                .SetContent("Show an example")
                .AddArgument(Convert.ToString(myRand))
                .SetBackgroundActivation())

                .Show(toast =>
                {
                    //toast.Tag = "1337";
                });
            }
        }

        /// <summary>
        /// This function downloads the required resources from the repository to the local machine.
        /// </summary>
        void GetResources()
        {
            string folder = string.Format("{0}Resources", Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\")));
            string file = folder + "\\read.txt";
            string image = folder + "\\NotiGlish.png";

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);

                // download the read.txt file
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://raw.githubusercontent.com/Ofs3t/NotiGlish/master/Resources/read.txt", file);
                    // 
                }

                // download the image 

                using (var client = new WebClient())
                {
                    client.DownloadFile("https://raw.githubusercontent.com/Ofs3t/NotiGlish/master/Resources/NotiGlishIcon.ico", image);
                }


            }
            // if the folder exists then check the files
            else
            {
                // check the read file
                if (!File.Exists(file))
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile("https://raw.githubusercontent.com/Ofs3t/NotiGlish/master/Resources/read.txt", file);
                    }
                }


                // check the image file
                if (!File.Exists(image))
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile("https://raw.githubusercontent.com/Ofs3t/NotiGlish/master/Resources/NotiGlishIcon.ico", image);
                    }
                }
            }
        }


        private void SaveBtn_Click(object sender, EventArgs e)
        {
            int n;
            try
            {
                if (string.IsNullOrEmpty(notiIntervalTextBox.Text))
                    throw new Exception("Please enter a number");

                if (!int.TryParse(notiIntervalTextBox.Text, out n))
                    throw new Exception("Please enter a valid number");
                else
                {
                    if (n != 0)
                    {
                        showNotiTimer.Interval = n * 60000;
                        showNotiTimer.Enabled = true;
                    }
                    else
                        showNotiTimer.Enabled = false;
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Wrong !");
            }
        }

        private void showNotiBtn_Click(object sender, EventArgs e)
        {
            ShowANoti();
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (searchTextBox.Text != "")
            {
                int start = 0;
                int end = richTextBox.Text.LastIndexOf(searchTextBox.Text);

                richTextBox.SelectAll();
                richTextBox.SelectionBackColor = Color.White;
                richTextBox.BackColor = Color.White;

                while (start < end)
                {
                    richTextBox.Find(searchTextBox.Text, start, richTextBox.Text.Length, RichTextBoxFinds.MatchCase);
                    richTextBox.SelectionBackColor = Color.Yellow;
                    start = richTextBox.Text.IndexOf(searchTextBox.Text, start) + 1;
                }

            }

            else
            {
                richTextBox.SelectAll();
                richTextBox.SelectionBackColor = Color.White;
                richTextBox.BackColor = Color.White;
            }
                
        }

        private void reviewModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This update will be available soon", "Message");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This tool can help you to remember English idioms using notifications sent to you periodically.\n\nThe developer of this tool is Anas, You can follow him on Twitter @ofs3t\nThanks for using the tool ;)", "NotiGlish", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToastNotificationManagerCompat.History.Clear();
            Application.Exit();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showNotiTimer.Enabled == true)
            {
                showNotiTimer.Enabled = false;
                pauseToolStripMenuItem.Checked = true;
            }
            else
            {
                showNotiTimer.Enabled = true;
                pauseToolStripMenuItem.Checked = false;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            notifyIcon.Visible = false;
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            aboutToolStripMenuItem_Click(sender, e);
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ToastNotificationManagerCompat.History.Clear();
            Application.Exit();
        }

        private void showNotiTimer_Tick(object sender, EventArgs e)
        {
            ShowANoti();

            // clear the notis from the notification center after 5 notifications
            if(++numOfNoti%5 == 0)
                ToastNotificationManagerCompat.History.Clear();


        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowInTaskbar = true;
            notifyIcon.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        private void miniTimer_Tick(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            miniTimer.Enabled = false;
            notifyIcon.Visible = true;
        }

        private void atStartupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (atStartupCheckBox.Checked == true)
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key.SetValue("NotiGlish", Application.ExecutablePath);
            }
            else
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key.DeleteValue("NotiGlish", false);
            }
        }

        private void NotiGlishForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ToastNotificationManagerCompat.History.Clear();
        }
    }
}
