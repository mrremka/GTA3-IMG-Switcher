using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;

public class RoundedPanel : Panel
{
    public int Radius = 18;
    public Color BorderColor = Color.FromArgb(55, 60, 72);

    public RoundedPanel()
    {
        DoubleBuffered = true;
        BackColor = Color.FromArgb(31, 34, 41);
    }

    public static GraphicsPath RoundRect(Rectangle r, int radius)
    {
        int d = radius * 2;
        GraphicsPath path = new GraphicsPath();
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);

        using (GraphicsPath p = RoundRect(r, Radius))
        using (Pen pen = new Pen(BorderColor, 1))
        {
            e.Graphics.DrawPath(pen, p);
        }
    }
}

public class AnimatedButton : Control
{
    public Color BaseColor = Color.FromArgb(54, 102, 210);
    public Color HoverColor = Color.FromArgb(72, 126, 244);
    public Color DownColor = Color.FromArgb(43, 84, 176);

    private Color current;
    private Color target;
    private Timer timer;

    public AnimatedButton()
    {
        DoubleBuffered = true;
        Cursor = Cursors.Hand;
        current = BaseColor;
        target = BaseColor;

        timer = new Timer();
        timer.Interval = 16;
        timer.Tick += Animate;
        timer.Start();

        MouseEnter += delegate { target = HoverColor; };
        MouseLeave += delegate { target = BaseColor; };
        MouseDown += delegate { target = DownColor; };
        MouseUp += delegate { target = HoverColor; };
    }

    private int Step(int a, int b)
    {
        if (a == b) return a;
        int diff = b - a;
        int step = Math.Max(1, Math.Abs(diff) / 5);
        return a + (diff > 0 ? step : -step);
    }

    private void Animate(object sender, EventArgs e)
    {
        current = Color.FromArgb(
            Step(current.R, target.R),
            Step(current.G, target.G),
            Step(current.B, target.B)
        );
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);

        using (GraphicsPath p = RoundedPanel.RoundRect(r, 14))
        using (SolidBrush br = new SolidBrush(current))
        {
            e.Graphics.FillPath(br, p);
        }

        TextRenderer.DrawText(
            e.Graphics,
            Text,
            Font,
            ClientRectangle,
            ForeColor,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.SingleLine
        );
    }
}

public class MainForm : Form
{
    private const string VERSION = "1.0.0";
    private const string DEFAULT_SUPPORT_URL = "https://github.com/YOUR_USERNAME/GTA3-IMG-Switcher/issues/new";

    private string appDataDir;
    private string logsDir;
    private string reportsDir;
    private string pathFile;
    private string profileFile;
    private string languageFile;
    private string supportFile;
    private string latestLog;

    private string lang = "en";
    private string profileAName = "MODDED";
    private string profileBName = "VANILLA";

    private TextBox pathBox;
    private Label titleLabel;
    private Label subtitleLabel;
    private Label activeLabel;
    private Label activeValue;
    private Label archiveLabel;
    private Label archiveValue;
    private Label gtaLabel;
    private Label gtaValue;
    private Label statusText;
    private Label footerLabel;

    private Button chooseButton;
    private Button refreshButton;
    private Button namesButton;
    private Button openFoldersButton;
    private Button reportButton;
    private Button languageButton;
    private Button aboutButton;

    private AnimatedButton profileAButton;
    private AnimatedButton profileBButton;

    private Color Bg = Color.FromArgb(20, 22, 27);
    private Color Muted = Color.FromArgb(157, 163, 176);
    private Color Green = Color.FromArgb(67, 196, 123);
    private Color Red = Color.FromArgb(235, 92, 92);
    private Color Yellow = Color.FromArgb(242, 184, 75);
    private Color Blue = Color.FromArgb(105, 157, 255);

    public MainForm()
    {
        appDataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GTA3 IMG Switcher"
        );
        logsDir = Path.Combine(appDataDir, "logs");
        reportsDir = Path.Combine(appDataDir, "reports");
        pathFile = Path.Combine(appDataDir, "models_path.txt");
        profileFile = Path.Combine(appDataDir, "profiles.txt");
        languageFile = Path.Combine(appDataDir, "language.txt");
        supportFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "support_url.txt");
        latestLog = Path.Combine(logsDir, "latest.log");

        Directory.CreateDirectory(appDataDir);
        Directory.CreateDirectory(logsDir);
        Directory.CreateDirectory(reportsDir);

        LoadSettings();
        StartNewLog();

        Text = "GTA3 IMG Switcher";
        ClientSize = new Size(820, 610);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = Bg;
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10);

        try
        {
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
            if (File.Exists(iconPath)) Icon = new Icon(iconPath);
        }
        catch { }

        BuildUi();
        LoadSavedPath();
        ApplyLanguage();
        RefreshStatus();

        Log("Application started. Version " + VERSION);
    }

    private void StartNewLog()
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("GTA3 IMG Switcher log");
            sb.AppendLine("Version: " + VERSION);
            sb.AppendLine("Started: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine("OS: " + Environment.OSVersion.VersionString);
            sb.AppendLine("----------------------------------------");
            File.WriteAllText(latestLog, sb.ToString(), Encoding.UTF8);
        }
        catch { }
    }

    private void Log(string message)
    {
        try
        {
            File.AppendAllText(
                latestLog,
                DateTime.Now.ToString("HH:mm:ss") + " | " + message + Environment.NewLine,
                Encoding.UTF8
            );
        }
        catch { }
    }

    private string T(string en, string ru)
    {
        return lang == "ru" ? ru : en;
    }

    private Label MakeLabel(string text, int x, int y, int size, bool bold, Color color)
    {
        Label l = new Label();
        l.Text = text;
        l.Left = x;
        l.Top = y;
        l.AutoSize = true;
        l.ForeColor = color;
        l.Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular);
        return l;
    }

    private Button MakeButton(string text, int x, int y, int w)
    {
        Button b = new Button();
        b.Text = text;
        b.Left = x;
        b.Top = y;
        b.Width = w;
        b.Height = 34;
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderColor = Color.FromArgb(67, 72, 85);
        b.BackColor = Color.FromArgb(45, 49, 59);
        b.ForeColor = Color.White;
        b.Cursor = Cursors.Hand;
        return b;
    }

    private void BuildUi()
    {
        titleLabel = MakeLabel("GTA3 IMG Switcher", 28, 20, 24, true, Color.White);
        Controls.Add(titleLabel);

        subtitleLabel = MakeLabel("", 31, 64, 10, false, Muted);
        Controls.Add(subtitleLabel);

        languageButton = MakeButton("RU", 585, 24, 64);
        languageButton.Click += Language_Click;
        Controls.Add(languageButton);

        aboutButton = MakeButton("About", 659, 24, 133);
        aboutButton.Click += About_Click;
        Controls.Add(aboutButton);

        RoundedPanel pathCard = new RoundedPanel();
        pathCard.Left = 28;
        pathCard.Top = 102;
        pathCard.Width = 764;
        pathCard.Height = 108;
        Controls.Add(pathCard);

        Label pathTitle = MakeLabel("GAME MODELS FOLDER", 20, 14, 9, true, Muted);
        pathTitle.Name = "pathTitle";
        pathCard.Controls.Add(pathTitle);

        pathBox = new TextBox();
        pathBox.Left = 20;
        pathBox.Top = 47;
        pathBox.Width = 565;
        pathBox.ReadOnly = true;
        pathBox.BackColor = Color.FromArgb(24, 27, 33);
        pathBox.ForeColor = Color.White;
        pathBox.BorderStyle = BorderStyle.FixedSingle;
        pathCard.Controls.Add(pathBox);

        chooseButton = MakeButton("Choose", 598, 43, 142);
        chooseButton.Click += Choose_Click;
        pathCard.Controls.Add(chooseButton);

        RoundedPanel statusCard = new RoundedPanel();
        statusCard.Left = 28;
        statusCard.Top = 226;
        statusCard.Width = 764;
        statusCard.Height = 150;
        Controls.Add(statusCard);

        Label statusTitle = MakeLabel("SYSTEM CHECK", 20, 13, 9, true, Muted);
        statusTitle.Name = "statusTitle";
        statusCard.Controls.Add(statusTitle);

        activeLabel = MakeLabel("", 22, 45, 10, false, Muted);
        activeValue = MakeLabel("", 90, 45, 10, true, Color.White);
        statusCard.Controls.Add(activeLabel);
        statusCard.Controls.Add(activeValue);

        archiveLabel = MakeLabel("", 280, 45, 10, false, Muted);
        archiveValue = MakeLabel("", 350, 45, 10, true, Color.White);
        statusCard.Controls.Add(archiveLabel);
        statusCard.Controls.Add(archiveValue);

        gtaLabel = MakeLabel("", 535, 45, 10, false, Muted);
        gtaValue = MakeLabel("", 575, 45, 10, true, Color.White);
        statusCard.Controls.Add(gtaLabel);
        statusCard.Controls.Add(gtaValue);

        refreshButton = MakeButton("", 20, 82, 225);
        refreshButton.Click += delegate { RefreshStatus(); };
        statusCard.Controls.Add(refreshButton);

        namesButton = MakeButton("", 258, 82, 225);
        namesButton.Click += Names_Click;
        statusCard.Controls.Add(namesButton);

        openFoldersButton = MakeButton("", 496, 82, 244);
        openFoldersButton.Click += OpenFolders_Click;
        statusCard.Controls.Add(openFoldersButton);

        reportButton = MakeButton("", 20, 119, 720);
        reportButton.Click += Report_Click;
        statusCard.Controls.Add(reportButton);

        profileAButton = new AnimatedButton();
        profileAButton.Left = 28;
        profileAButton.Top = 400;
        profileAButton.Width = 370;
        profileAButton.Height = 74;
        profileAButton.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        profileAButton.ForeColor = Color.White;
        profileAButton.BaseColor = Color.FromArgb(46, 139, 87);
        profileAButton.HoverColor = Color.FromArgb(59, 171, 108);
        profileAButton.DownColor = Color.FromArgb(35, 112, 69);
        profileAButton.Click += delegate { SwitchTo("a"); };
        Controls.Add(profileAButton);

        profileBButton = new AnimatedButton();
        profileBButton.Left = 422;
        profileBButton.Top = 400;
        profileBButton.Width = 370;
        profileBButton.Height = 74;
        profileBButton.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        profileBButton.ForeColor = Color.White;
        profileBButton.BaseColor = Color.FromArgb(57, 100, 190);
        profileBButton.HoverColor = Color.FromArgb(75, 124, 224);
        profileBButton.DownColor = Color.FromArgb(44, 80, 157);
        profileBButton.Click += delegate { SwitchTo("b"); };
        Controls.Add(profileBButton);

        statusText = new Label();
        statusText.Left = 30;
        statusText.Top = 495;
        statusText.Width = 760;
        statusText.Height = 38;
        statusText.TextAlign = ContentAlignment.MiddleCenter;
        statusText.ForeColor = Muted;
        Controls.Add(statusText);

        footerLabel = MakeLabel("", 28, 558, 9, false, Color.FromArgb(110, 116, 128));
        Controls.Add(footerLabel);
    }

    private void ApplyLanguage()
    {
        subtitleLabel.Text = T(
            "Fast and safe gta3.img profile switching",
            "Быстрое и безопасное переключение профилей gta3.img"
        );

        languageButton.Text = lang == "ru" ? "EN" : "RU";
        aboutButton.Text = T("About", "О программе");

        Control pathTitle = FindControlByName(this, "pathTitle");
        if (pathTitle != null) pathTitle.Text = T("GAME MODELS FOLDER", "ПАПКА MODELS ИГРЫ");

        Control statusTitle = FindControlByName(this, "statusTitle");
        if (statusTitle != null) statusTitle.Text = T("SYSTEM CHECK", "ПРОВЕРКА СИСТЕМЫ");

        chooseButton.Text = T("Choose", "Выбрать");
        activeLabel.Text = T("Active:", "Активен:");
        archiveLabel.Text = T("Archive:", "Архив:");
        gtaLabel.Text = "GTA:";
        refreshButton.Text = T("Refresh status", "Обновить проверку");
        namesButton.Text = T("Profile names", "Названия профилей");
        openFoldersButton.Text = T("Open profile folders", "Открыть папки профилей");
        reportButton.Text = T("Report a problem / Create diagnostic report", "Сообщить о проблеме / Создать отчёт");
        footerLabel.Text = T(
            "v" + VERSION + " • Config and logs: %AppData%\\GTA3 IMG Switcher",
            "v" + VERSION + " • Настройки и логи: %AppData%\\GTA3 IMG Switcher"
        );

        UpdateProfileButtons();
        RefreshStatus();
    }

    private Control FindControlByName(Control parent, string name)
    {
        foreach (Control c in parent.Controls)
        {
            if (c.Name == name) return c;
            Control nested = FindControlByName(c, name);
            if (nested != null) return nested;
        }
        return null;
    }

    private void Language_Click(object sender, EventArgs e)
    {
        lang = lang == "ru" ? "en" : "ru";
        try { File.WriteAllText(languageFile, lang); } catch { }
        Log("Language changed to " + lang);
        ApplyLanguage();
    }

    private void LoadSettings()
    {
        try
        {
            if (File.Exists(languageFile))
            {
                string l = File.ReadAllText(languageFile).Trim().ToLower();
                if (l == "ru" || l == "en") lang = l;
            }
        }
        catch { }

        try
        {
            if (File.Exists(profileFile))
            {
                string[] lines = File.ReadAllLines(profileFile);
                if (lines.Length > 0 && !String.IsNullOrWhiteSpace(lines[0])) profileAName = lines[0].Trim();
                if (lines.Length > 1 && !String.IsNullOrWhiteSpace(lines[1])) profileBName = lines[1].Trim();
            }
        }
        catch { }
    }

    private void LoadSavedPath()
    {
        try
        {
            if (File.Exists(pathFile))
            {
                string p = File.ReadAllText(pathFile).Trim();
                if (Directory.Exists(p)) pathBox.Text = p;
            }
        }
        catch { }
    }

    private void UpdateProfileButtons()
    {
        profileAButton.Text = profileAName;
        profileBButton.Text = profileBName;
    }

    private void Names_Click(object sender, EventArgs e)
    {
        using (Form f = new Form())
        {
            f.Text = T("Profile names", "Названия профилей");
            f.ClientSize = new Size(420, 220);
            f.StartPosition = FormStartPosition.CenterParent;
            f.FormBorderStyle = FormBorderStyle.FixedDialog;
            f.MaximizeBox = false;
            f.MinimizeBox = false;
            f.BackColor = Bg;
            f.ForeColor = Color.White;
            f.Font = new Font("Segoe UI", 10);

            Label la = MakeLabel(T("Profile A", "Профиль A"), 20, 18, 10, true, Muted);
            TextBox a = new TextBox();
            a.Left = 20; a.Top = 46; a.Width = 375; a.Text = profileAName;

            Label lb = MakeLabel(T("Profile B", "Профиль B"), 20, 88, 10, true, Muted);
            TextBox b = new TextBox();
            b.Left = 20; b.Top = 116; b.Width = 375; b.Text = profileBName;

            Button save = MakeButton(T("Save", "Сохранить"), 245, 165, 150);
            save.Click += delegate
            {
                string na = a.Text.Trim();
                string nb = b.Text.Trim();

                string nameError;
                if (!ValidateProfileName(na, out nameError))
                {
                    MessageBox.Show(nameError, T("Invalid name", "Неверное название"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateProfileName(nb, out nameError))
                {
                    MessageBox.Show(nameError, T("Invalid name", "Неверное название"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (String.Equals(na, nb, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        T("Profile names must be different.", "Названия профилей должны отличаться."),
                        T("Invalid names", "Неверные названия"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                string oldAName = profileAName;
                string oldBName = profileBName;

                try
                {
                    RenameProfileFolders(oldAName, oldBName, na, nb);

                    profileAName = na;
                    profileBName = nb;

                    File.WriteAllLines(profileFile, new string[] { profileAName, profileBName });

                    Log("Profile names and folders changed: " +
                        SanitizeFolderPart(oldAName) + " -> " + SanitizeFolderPart(profileAName) + ", " +
                        SanitizeFolderPart(oldBName) + " -> " + SanitizeFolderPart(profileBName));

                    UpdateProfileButtons();
                    RefreshStatus();
                    f.Close();
                }
                catch (Exception ex)
                {
                    Log("Profile folder rename error: " + ex.Message);
                    MessageBox.Show(
                        T(
                            "Could not rename the profile folders.\n\n",
                            "Не удалось переименовать папки профилей.\n\n"
                        ) + ex.Message,
                        T("Rename failed", "Ошибка переименования"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            };

            f.Controls.Add(la);
            f.Controls.Add(a);
            f.Controls.Add(lb);
            f.Controls.Add(b);
            f.Controls.Add(save);
            f.ShowDialog(this);
        }
    }

    private void Choose_Click(object sender, EventArgs e)
    {
        using (FolderBrowserDialog dlg = new FolderBrowserDialog())
        {
            dlg.Description = T(
                "Choose GTA San Andreas models folder",
                "Выберите папку models GTA San Andreas"
            );

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pathBox.Text = dlg.SelectedPath;

                try { File.WriteAllText(pathFile, dlg.SelectedPath); } catch { }

                EnsureStorageFolders();
                Log("Models folder selected: " + SafePathForLog(dlg.SelectedPath));
                RefreshStatus();
            }
        }
    }

    private string SafePathForLog(string path)
    {
        try
        {
            string user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!String.IsNullOrEmpty(user) && path.StartsWith(user, StringComparison.OrdinalIgnoreCase))
                return "%USERPROFILE%" + path.Substring(user.Length);
        }
        catch { }

        return path;
    }

    private DirectoryInfo GameRoot()
    {
        if (String.IsNullOrEmpty(pathBox.Text)) return null;
        DirectoryInfo d = new DirectoryInfo(pathBox.Text);
        return d.Parent;
    }

    private string SanitizeFolderPart(string name)
    {
        if (String.IsNullOrWhiteSpace(name)) return "PROFILE";

        char[] invalid = Path.GetInvalidFileNameChars();
        StringBuilder sb = new StringBuilder();

        foreach (char c in name.Trim())
        {
            bool bad = false;
            foreach (char x in invalid)
            {
                if (c == x)
                {
                    bad = true;
                    break;
                }
            }

            if (!bad)
            {
                if (Char.IsWhiteSpace(c))
                    sb.Append('_');
                else
                    sb.Append(c);
            }
        }

        string result = sb.ToString().Trim('.', ' ');
        while (result.Contains("__"))
            result = result.Replace("__", "_");

        if (String.IsNullOrWhiteSpace(result))
            result = "PROFILE";

        return result;
    }

    private string StorageForName(string name)
    {
        DirectoryInfo root = GameRoot();
        if (root == null) return "";

        return Path.Combine(root.FullName, "gta3_" + SanitizeFolderPart(name));
    }

    private string Storage(string profile)
    {
        return profile == "a"
            ? StorageForName(profileAName)
            : StorageForName(profileBName);
    }

    private void EnsureStorageFolders()
    {
        DirectoryInfo root = GameRoot();
        if (root == null) return;

        string newA = Storage("a");
        string newB = Storage("b");

        string legacyA = Path.Combine(root.FullName, "gta3_profile_a");
        string legacyB = Path.Combine(root.FullName, "gta3_profile_b");

        try
        {
            if (!Directory.Exists(newA) && Directory.Exists(legacyA))
                Directory.Move(legacyA, newA);

            if (!Directory.Exists(newB) && Directory.Exists(legacyB))
                Directory.Move(legacyB, newB);
        }
        catch (Exception ex)
        {
            Log("Legacy profile folder migration warning: " + ex.Message);
        }

        Directory.CreateDirectory(newA);
        Directory.CreateDirectory(newB);
    }

    private bool ValidateProfileName(string name, out string error)
    {
        error = "";

        if (String.IsNullOrWhiteSpace(name))
        {
            error = T("Profile names cannot be empty.", "Названия профилей не могут быть пустыми.");
            return false;
        }

        char[] invalid = Path.GetInvalidFileNameChars();
        foreach (char c in name)
        {
            foreach (char x in invalid)
            {
                if (c == x)
                {
                    error = T(
                        "Profile name contains a character that cannot be used in a Windows folder name.",
                        "Название профиля содержит символ, который нельзя использовать в имени папки Windows."
                    );
                    return false;
                }
            }
        }

        string trimmed = name.Trim().TrimEnd('.', ' ');
        if (String.IsNullOrWhiteSpace(trimmed))
        {
            error = T("Invalid profile name.", "Недопустимое название профиля.");
            return false;
        }

        return true;
    }

    private void RenameProfileFolders(string oldAName, string oldBName, string newAName, string newBName)
    {
        DirectoryInfo root = GameRoot();
        if (root == null) return;

        EnsureStorageFolders();

        string oldA = StorageForName(oldAName);
        string oldB = StorageForName(oldBName);
        string newA = StorageForName(newAName);
        string newB = StorageForName(newBName);

        if (String.Equals(newA, newB, StringComparison.OrdinalIgnoreCase))
            throw new Exception(T(
                "Both profile names produce the same folder name.",
                "Оба названия профилей создают одинаковое имя папки."
            ));

        string tempA = Path.Combine(root.FullName, "gta3.__rename_a_" + Guid.NewGuid().ToString("N"));
        string tempB = Path.Combine(root.FullName, "gta3.__rename_b_" + Guid.NewGuid().ToString("N"));

        bool moveA = Directory.Exists(oldA) && !String.Equals(oldA, newA, StringComparison.OrdinalIgnoreCase);
        bool moveB = Directory.Exists(oldB) && !String.Equals(oldB, newB, StringComparison.OrdinalIgnoreCase);

        // Reject unrelated destination folders. Allow destinations that are the other current profile folder.
        if (moveA && Directory.Exists(newA) &&
            !String.Equals(newA, oldB, StringComparison.OrdinalIgnoreCase))
            throw new Exception(T(
                "A folder with the new Profile A name already exists.",
                "Папка с новым названием Профиля A уже существует."
            ));

        if (moveB && Directory.Exists(newB) &&
            !String.Equals(newB, oldA, StringComparison.OrdinalIgnoreCase))
            throw new Exception(T(
                "A folder with the new Profile B name already exists.",
                "Папка с новым названием Профиля B уже существует."
            ));

        try
        {
            if (moveA) Directory.Move(oldA, tempA);
            if (moveB) Directory.Move(oldB, tempB);

            if (moveA) Directory.Move(tempA, newA);
            if (moveB) Directory.Move(tempB, newB);

            Directory.CreateDirectory(newA);
            Directory.CreateDirectory(newB);
        }
        catch
        {
            // Best-effort rollback.
            try
            {
                if (Directory.Exists(tempA) && !Directory.Exists(oldA))
                    Directory.Move(tempA, oldA);
            }
            catch { }

            try
            {
                if (Directory.Exists(tempB) && !Directory.Exists(oldB))
                    Directory.Move(tempB, oldB);
            }
            catch { }

            throw;
        }
    }

    private bool IsGtaRunning()
    {
        try
        {
            if (Process.GetProcessesByName("gta_sa").Length > 0) return true;
            if (Process.GetProcessesByName("gta-sa").Length > 0) return true;
        }
        catch { }

        return false;
    }

    private string DetectState()
    {
        if (!Directory.Exists(pathBox.Text)) return "no_path";

        string gameFile = Path.Combine(pathBox.Text, "gta3.img");
        if (!File.Exists(gameFile)) return "no_game";

        EnsureStorageFolders();

        bool a = File.Exists(Path.Combine(Storage("a"), "gta3.img"));
        bool b = File.Exists(Path.Combine(Storage("b"), "gta3.img"));

        if (!a && b) return "a_active";
        if (a && !b) return "b_active";
        if (a && b) return "both";
        return "none";
    }

    private void RefreshStatus()
    {
        if (activeValue == null) return;

        string state = DetectState();
        bool running = IsGtaRunning();

        gtaValue.Text = running ? T("Running", "Запущена") : T("Closed", "Закрыта");
        gtaValue.ForeColor = running ? Red : Green;

        if (state == "a_active")
        {
            activeValue.Text = profileAName;
            activeValue.ForeColor = Green;
            archiveValue.Text = "OK";
            archiveValue.ForeColor = Green;
            statusText.Text = T(
                profileAName + " is active. Ready to switch.",
                "Активен " + profileAName + ". Можно переключать."
            );
        }
        else if (state == "b_active")
        {
            activeValue.Text = profileBName;
            activeValue.ForeColor = Blue;
            archiveValue.Text = "OK";
            archiveValue.ForeColor = Green;
            statusText.Text = T(
                profileBName + " is active. Ready to switch.",
                "Активен " + profileBName + ". Можно переключать."
            );
        }
        else if (state == "no_path")
        {
            activeValue.Text = T("Unknown", "Неизвестно");
            activeValue.ForeColor = Muted;
            archiveValue.Text = T("Choose folder", "Выберите папку");
            archiveValue.ForeColor = Yellow;
            statusText.Text = T(
                "Choose your GTA San Andreas models folder.",
                "Выберите папку models вашей GTA San Andreas."
            );
        }
        else if (state == "no_game")
        {
            activeValue.Text = T("Unknown", "Неизвестно");
            activeValue.ForeColor = Muted;
            archiveValue.Text = T("Missing gta3.img", "Нет gta3.img");
            archiveValue.ForeColor = Red;
            statusText.Text = T(
                "gta3.img was not found in models.",
                "В папке models не найден gta3.img."
            );
        }
        else if (state == "both")
        {
            activeValue.Text = T("Unknown", "Неизвестно");
            activeValue.ForeColor = Yellow;
            archiveValue.Text = T("Two parked files", "Два файла");
            archiveValue.ForeColor = Yellow;
            statusText.Text = T(
                "Both profile folders contain gta3.img. Manual check required.",
                "В обеих папках профилей лежит gta3.img. Нужна ручная проверка."
            );
        }
        else
        {
            activeValue.Text = T("Unknown", "Неизвестно");
            activeValue.ForeColor = Yellow;
            archiveValue.Text = T("No parked file", "Нет запасного");
            archiveValue.ForeColor = Yellow;
            statusText.Text = T(
                "Put the inactive gta3.img into one profile folder.",
                "Положите неактивный gta3.img в одну из папок профилей."
            );
        }
    }

    private void OpenFolders_Click(object sender, EventArgs e)
    {
        if (GameRoot() == null)
        {
            MessageBox.Show(T(
                "Choose the models folder first.",
                "Сначала выберите папку models."
            ));
            return;
        }

        EnsureStorageFolders();

        try
        {
            Process.Start("explorer.exe", GameRoot().FullName);
            Log("Game root opened in Explorer.");
        }
        catch (Exception ex)
        {
            Log("Explorer open failed: " + ex.Message);
            MessageBox.Show(ex.Message);
        }
    }

    private void SwitchTo(string target)
    {
        RefreshStatus();

        if (IsGtaRunning())
        {
            Log("Switch blocked: GTA is running.");
            MessageBox.Show(
                T(
                    "GTA San Andreas is running.\n\nClose the game before switching.",
                    "GTA San Andreas запущена.\n\nЗакройте игру перед переключением."
                ),
                T("GTA is running", "GTA запущена"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        string state = DetectState();

        if (target == "a" && state == "a_active")
        {
            statusText.Text = T(profileAName + " is already active.", profileAName + " уже активен.");
            return;
        }

        if (target == "b" && state == "b_active")
        {
            statusText.Text = T(profileBName + " is already active.", profileBName + " уже активен.");
            return;
        }

        if (state != "a_active" && state != "b_active")
        {
            Log("Switch blocked: unsafe state " + state);
            MessageBox.Show(
                T(
                    "The file state is not safe for automatic switching.\n\nCheck the profile folders or open Troubleshooting.",
                    "Состояние файлов небезопасно для автоматического переключения.\n\nПроверьте папки профилей и раздел с решением проблем."
                ),
                T("Safety check failed", "Проверка безопасности не пройдена"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        string gameFile = Path.Combine(pathBox.Text, "gta3.img");
        string targetFile = Path.Combine(Storage(target), "gta3.img");
        string oldProfile = target == "a" ? "b" : "a";
        string parkFile = Path.Combine(Storage(oldProfile), "gta3.img");

        if (!File.Exists(targetFile))
        {
            Log("Switch failed before move: target file missing.");
            MessageBox.Show(
                T("Required gta3.img was not found.", "Нужный gta3.img не найден.") + "\n\n" + targetFile,
                T("Missing file", "Файл не найден"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
            return;
        }

        if (File.Exists(parkFile))
        {
            Log("Switch blocked: park destination already contains gta3.img.");
            MessageBox.Show(
                T(
                    "Safety stop: the destination profile already contains gta3.img.",
                    "Остановка безопасности: в папке назначения уже есть gta3.img."
                ) + "\n\n" + parkFile,
                T("Safety stop", "Остановка безопасности"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        try
        {
            profileAButton.Enabled = false;
            profileBButton.Enabled = false;
            statusText.Text = T("Switching...", "Переключение...");
            Application.DoEvents();

            Log("Switch started. Target profile: " + target);

            File.Move(gameFile, parkFile);

            try
            {
                File.Move(targetFile, gameFile);
            }
            catch
            {
                if (!File.Exists(gameFile) && File.Exists(parkFile))
                {
                    File.Move(parkFile, gameFile);
                    Log("Rollback completed.");
                }
                throw;
            }

            Log("Switch completed successfully.");
            RefreshStatus();
        }
        catch (Exception ex)
        {
            Log("Switch error: " + ex.GetType().Name + " | " + ex.Message);

            MessageBox.Show(
                T(
                    "Switch failed. A log was saved.\n\nUse 'Report a problem' if you need help.",
                    "Переключение не удалось. Лог сохранён.\n\nИспользуйте «Сообщить о проблеме», если нужна помощь."
                ) + "\n\n" + ex.Message,
                T("Switch failed", "Ошибка переключения"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
        finally
        {
            profileAButton.Enabled = true;
            profileBButton.Enabled = true;
        }
    }

    private string BuildDiagnostics()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("GTA3 IMG Switcher diagnostics");
        sb.AppendLine("Version: " + VERSION);
        sb.AppendLine("Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        sb.AppendLine("OS: " + Environment.OSVersion.VersionString);
        sb.AppendLine("64-bit OS: " + Environment.Is64BitOperatingSystem);
        sb.AppendLine("Language: " + lang);
        sb.AppendLine("GTA running: " + IsGtaRunning());
        sb.AppendLine("Detected state: " + DetectState());
        sb.AppendLine("Profile A name: " + profileAName);
        sb.AppendLine("Profile B name: " + profileBName);

        if (Directory.Exists(pathBox.Text))
        {
            sb.AppendLine("Models folder: " + SafePathForLog(pathBox.Text));
            sb.AppendLine("models\\gta3.img exists: " + File.Exists(Path.Combine(pathBox.Text, "gta3.img")));
            sb.AppendLine("profile A gta3.img exists: " + File.Exists(Path.Combine(Storage("a"), "gta3.img")));
            sb.AppendLine("profile B gta3.img exists: " + File.Exists(Path.Combine(Storage("b"), "gta3.img")));

            try
            {
                string gameFile = Path.Combine(pathBox.Text, "gta3.img");
                if (File.Exists(gameFile))
                    sb.AppendLine("Active gta3.img size: " + new FileInfo(gameFile).Length + " bytes");
            }
            catch { }
        }
        else
        {
            sb.AppendLine("Models folder: not selected");
        }

        return sb.ToString();
    }

    private void Report_Click(object sender, EventArgs e)
    {
        try
        {
            string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string folder = Path.Combine(reportsDir, "report_" + stamp);
            Directory.CreateDirectory(folder);

            string diagnostics = BuildDiagnostics();
            File.WriteAllText(Path.Combine(folder, "diagnostics.txt"), diagnostics, Encoding.UTF8);

            if (File.Exists(latestLog))
                File.Copy(latestLog, Path.Combine(folder, "latest.log"), true);

            try
            {
                Clipboard.SetText(diagnostics);
            }
            catch { }

            Log("Diagnostic report created: report_" + stamp);

            Process.Start("explorer.exe", folder);

            string supportUrl = DEFAULT_SUPPORT_URL;
            try
            {
                if (File.Exists(supportFile))
                {
                    string custom = File.ReadAllText(supportFile).Trim();
                    if (!String.IsNullOrEmpty(custom)) supportUrl = custom;
                }
            }
            catch { }

            if (supportUrl.IndexOf("YOUR_USERNAME", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                MessageBox.Show(
                    T(
                        "The diagnostic report folder was created and diagnostic text was copied to the clipboard.\n\nGitHub support URL is not configured yet. Attach diagnostics.txt and latest.log when reporting the problem.",
                        "Папка с диагностикой создана, а текст диагностики скопирован в буфер обмена.\n\nСсылка на GitHub Issues пока не настроена. При сообщении о проблеме приложите diagnostics.txt и latest.log."
                    ),
                    T("Report created", "Отчёт создан"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            DialogResult answer = MessageBox.Show(
                T(
                    "Report created. Open the GitHub issue page now?\n\nAttach diagnostics.txt and latest.log from the opened folder.",
                    "Отчёт создан. Открыть страницу GitHub Issues?\n\nПриложите diagnostics.txt и latest.log из открывшейся папки."
                ),
                T("Report a problem", "Сообщить о проблеме"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (answer == DialogResult.Yes)
                Process.Start(supportUrl);
        }
        catch (Exception ex)
        {
            Log("Report creation error: " + ex.Message);
            MessageBox.Show(ex.Message);
        }
    }

    private void About_Click(object sender, EventArgs e)
    {
        MessageBox.Show(
            "GTA3 IMG Switcher\n" +
            "Version " + VERSION + "\n\n" +
            T(
                "Fast and safe gta3.img profile switching for GTA San Andreas.\n\nLogs contain technical diagnostics only. Nothing is sent automatically.",
                "Быстрое и безопасное переключение профилей gta3.img для GTA San Andreas.\n\nЛоги содержат только техническую диагностику. Ничего не отправляется автоматически."
            ),
            T("About", "О программе"),
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }
}

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
