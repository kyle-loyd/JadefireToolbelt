class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        NotifyIcon trayIcon = new()
        {
            Icon = new Icon("./jadefire.ico"),
            Visible = true,
            Text = "Jadefire"
        };

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("GUID to Clipboard", null, (sender, args) => Clipboard.SetText(Guid.NewGuid().ToString()));
        contextMenu.Items.Add("Exit", null, (sender, args) => Application.Exit());
        trayIcon.ContextMenuStrip = contextMenu;

        Application.Run();
    }
}