using JadefireToolbelt;
using System.Configuration;

class Program
{
    [STAThread]
    static void Main()
    {
        try
        {
            var bellIntervalSetting = ConfigurationManager.AppSettings["MindfulBell_Interval"];
            var bellInterval = double.TryParse(bellIntervalSetting, out var result) ? result : 0;
            var bell = new MindfulBell(bellInterval);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var iconTextSetting = ConfigurationManager.AppSettings["IconText"];
            var iconPathSetting = ConfigurationManager.AppSettings["IconPath"];
            var icon = !string.IsNullOrWhiteSpace(iconPathSetting) ? new Icon(iconPathSetting) : new Icon("Resources/jadefire.ico");
            var trayIcon = new NotifyIcon()
            {
                Icon = icon,
                Visible = true,
                Text = !string.IsNullOrWhiteSpace(iconTextSetting) ? iconTextSetting : "Jadefire"
            };

            var contextMenu = new ContextMenuStrip();
            
            contextMenu.Items.Add("GUID to Clipboard", null, (sender, args) => Clipboard.SetText(Guid.NewGuid().ToString()));
            
            var mindfulBellMenu = new ToolStripMenuItem("Mindful Bell");
            mindfulBellMenu.DropDownItems.Add("Start", null, (sender, args) => bell.Start());
            mindfulBellMenu.DropDownItems.Add("Stop", null, (sender, args) => bell.Stop());
            
            contextMenu.Items.Add(mindfulBellMenu);
            
            contextMenu.Items.Add("Exit", null, (sender, args) => Application.Exit());
            
            trayIcon.ContextMenuStrip = contextMenu;

            Application.Run();
        }

        catch (SoundPlayFailureException spfe)
        {
            MessageBox.Show(spfe.Message);
        }

        catch (Exception e) 
        {
            MessageBox.Show(e.Message);
        }
    }
}