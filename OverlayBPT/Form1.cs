using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Tesseract;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;

namespace OverlayBPT
{
    public class OverlayForm : Form
    {
        private Rectangle captureArea;
        private Timer captureTimer;
        private Label expLabel;
        private Label statusLabel;
        private bool isSelectionMode = false;
        private bool isDrawing = false;
        private Point selectionStart;
        private Panel selectionPanel;
        private Panel mainPanel;
        private TesseractEngine tessEngine;
        private List<(DateTime time, long exp)> expHistory;
        private DateTime startTime;
        private long initialExp;
        private bool hasInitialExp = false;

        private Pix ConvertBitmapToPix(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                return Pix.LoadFromMemory(ms.ToArray());
            }
        }

        public OverlayForm()
        {
            InitializeCustomComponents();
            SetupOverlay();
            InitializeTesseract();
            expHistory = new List<(DateTime, long)>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Liberar recursos gerenciados
                tessEngine?.Dispose();
                captureTimer?.Dispose();
            }
            // Liberar recursos não gerenciados
            base.Dispose(disposing); // Chamada obrigatória para liberar recursos da classe base
        }

        private void InitializeTesseract()
        {
            tessEngine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
            tessEngine.SetVariable("tessedit_char_whitelist", "0123456789/.,MBT");
        }

        private void InitializeCustomComponents()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = true;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int margin = 20;
            int topOffset = (int)(screenHeight * 0.15);

            this.Size = new Size(250, 100);
            this.Location = new Point(screenWidth - this.Width - margin, topOffset);

            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(40, 0, 0, 0),
                BorderStyle = BorderStyle.None
            };
            this.Controls.Add(mainPanel);

            Panel titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 25,
                BackColor = Color.FromArgb(60, 0, 0, 0)
            };
            mainPanel.Controls.Add(titleBar);

            Button closeButton = new Button
            {
                Text = "X",
                Size = new Size(25, 25),
                Location = new Point(titleBar.Width - 25, 0),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 0, 0, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => Application.Exit();
            titleBar.Controls.Add(closeButton);

            Label titleLabel = new Label
            {
                Text = "BPT EXP Tracker",
                ForeColor = Color.White,
                Location = new Point(5, 5),
                AutoSize = true
            };
            titleBar.Controls.Add(titleLabel);

            expLabel = new Label
            {
                AutoSize = true,
                ForeColor = Color.Lime,
                BackColor = Color.Transparent,
                Location = new Point(5, 30),
                Font = new Font("Consolas", 9, FontStyle.Bold)
            };
            mainPanel.Controls.Add(expLabel);
            expLabel.Text = "Aguardando dados...";

            statusLabel = new Label
            {
                AutoSize = true,
                ForeColor = Color.Yellow,
                BackColor = Color.Transparent,
                Location = new Point(5, 75),
                Font = new Font("Consolas", 8)
            };
            mainPanel.Controls.Add(statusLabel);
            statusLabel.Text = "Pressione F10 para selecionar área";

            titleBar.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Left)
                {
                    NativeMethods.ReleaseCapture();
                    NativeMethods.SendMessage(Handle, NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HT_CAPTION, 0);
                }
            };

            this.KeyPreview = true;
            this.KeyDown += Form_KeyDown;
        }

        private void SetupOverlay()
        {
            selectionPanel = new Panel
            {
                Visible = false,
                BackColor = Color.FromArgb(50, 0, 255, 0),
                BorderStyle = BorderStyle.FixedSingle
            };

            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10)
            {
                isSelectionMode = !isSelectionMode;
                if (isSelectionMode)
                {
                    statusLabel.Text = "Selecione a área com os números de EXP";
                    if (captureTimer != null)
                    {
                        captureTimer.Stop();
                    }
                    this.Opacity = 0.1;
                }
                else
                {
                    statusLabel.Text = "Pressione F10 para selecionar área";
                    this.Opacity = 1;
                }
            }
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (isSelectionMode && e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                selectionStart = PointToScreen(e.Location);
                selectionPanel.Parent = null;
                selectionPanel.Parent = this;
                selectionPanel.Location = e.Location;
                selectionPanel.Size = new Size(0, 0);
                selectionPanel.Visible = true;
                selectionPanel.BringToFront();
            }
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                Point currentPoint = PointToScreen(e.Location);
                int width = Math.Abs(currentPoint.X - selectionStart.X);
                int height = Math.Abs(currentPoint.Y - selectionStart.Y);
                int x = Math.Min(selectionStart.X, currentPoint.X);
                int y = Math.Min(selectionStart.Y, currentPoint.Y);

                selectionPanel.Location = PointToClient(new Point(x, y));
                selectionPanel.Size = new Size(width, height);
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;
                isSelectionMode = false;
                this.Opacity = 1;

                Point startScreen = PointToScreen(selectionPanel.Location);
                captureArea = new Rectangle(
                    startScreen.X,
                    startScreen.Y,
                    selectionPanel.Width,
                    selectionPanel.Height
                );

                SetupCaptureTimer();
                selectionPanel.Visible = false;
                hasInitialExp = false;
                expHistory.Clear();
                statusLabel.Text = "Capturando...";
            }
        }

        private void SetupCaptureTimer()
        {
            if (captureTimer != null)
            {
                captureTimer.Stop();
                captureTimer.Dispose();
            }

            captureTimer = new Timer
            {
                Interval = 10000 // 10 segundos
            };

            captureTimer.Tick += (s, e) => CaptureAndProcessExp();
            captureTimer.Start();
        }

        private void CaptureAndProcessExp()
        {
            try
            {
                using (Bitmap screenshot = new Bitmap(captureArea.Width, captureArea.Height))
                {
                    using (Graphics g = Graphics.FromImage(screenshot))
                    {
                        g.CopyFromScreen(captureArea.Location, Point.Empty, captureArea.Size);
                    }

                    using (var processed = PreProcessImage(screenshot))
                    using (var processedPix = ConvertBitmapToPix(processed))
                    using (var page = tessEngine.Process(processedPix, PageSegMode.Auto))
                    {
                        string text = page.GetText().Trim();
                        ProcessExpText(text);
                    }
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Erro: {ex.Message}";
            }
        }

        private Bitmap PreProcessImage(Bitmap original)
        {
            Bitmap processed = new Bitmap(original.Width, original.Height);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixel = original.GetPixel(x, y);
                    int grayScale = (int)((pixel.R * 0.3) + (pixel.G * 0.59) + (pixel.B * 0.11));
                    grayScale = grayScale > 128 ? 255 : 0;
                    processed.SetPixel(x, y, Color.FromArgb(255, grayScale, grayScale, grayScale));
                }
            }

            return processed;
        }

        private long ParseExpValue(string text)
        {
            try
            {
                text = text.Trim().ToUpper();

                long multiplier = 1;
                if (text.EndsWith("T"))
                {
                    multiplier = 1000000000000; // Trilhão
                    text = text.TrimEnd('T');
                }
                else if (text.EndsWith("B"))
                {
                    multiplier = 1000000000; // Bilhão
                    text = text.TrimEnd('B');
                }
                else if (text.EndsWith("M"))
                {
                    multiplier = 1000000; // Milhão
                    text = text.TrimEnd('M');
                }

                text = text.Replace(".", "");
                if (double.TryParse(text, out double baseValue))
                {
                    return (long)(baseValue * multiplier);
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private void ProcessExpText(string text)
        {
            try
            {
                if (!text.Any(char.IsDigit))
                {
                    statusLabel.Text = "Aguardando janela de status...";
                    return;
                }

                var matches = Regex.Matches(text, @"[\d.,]+[MBT]");
                if (matches.Count >= 1)
                {
                    string expText = matches[0].Value;
                    long currentExp = ParseExpValue(expText);

                    if (currentExp > 0)
                    {
                        var now = DateTime.Now;

                        if (!hasInitialExp)
                        {
                            hasInitialExp = true;
                            initialExp = currentExp;
                            startTime = now;
                            expHistory.Clear();
                        }

                        expHistory.Add((now, currentExp));
                        expHistory.RemoveAll(e => (now - e.time).TotalHours > 1);

                        long totalExpGained = currentExp - initialExp;
                        TimeSpan activeTime = CalculateActiveTime();
                        double activeHours = activeTime.TotalHours;
                        long avgExpPerHour = activeHours > 0 ? (long)(totalExpGained / activeHours) : 0;

                        string currentExpStr = FormatExpValue(currentExp);
                        string gainedExpStr = FormatExpValue(totalExpGained);
                        string rateStr = FormatExpValue(avgExpPerHour);

                        string nextLevelExp = matches.Count >= 2 ? matches[1].Value : "";
                        string remainingExp = "";
                        if (!string.IsNullOrEmpty(nextLevelExp))
                        {
                            long nextLevel = ParseExpValue(nextLevelExp);
                            long remaining = nextLevel - currentExp;
                            remainingExp = $"\nFalta: {FormatExpValue(remaining)}";

                            if (avgExpPerHour > 0)
                            {
                                double hoursRemaining = remaining / (double)avgExpPerHour;
                                TimeSpan timeRemaining = TimeSpan.FromHours(hoursRemaining);
                                remainingExp += $"\nTempo est.: {FormatTimeSpan(timeRemaining)}";
                            }
                        }

                        expLabel.Text = $"EXP Atual: {currentExpStr}\n" +
                                       $"Ganho: {gainedExpStr}\n" +
                                       $"Média: {rateStr}/h" +
                                       remainingExp;

                        statusLabel.Text = $"Tempo Ativo: {FormatTimeSpan(activeTime)}";
                    }
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Erro: {ex.Message}";
            }
        }

        private TimeSpan CalculateActiveTime()
        {
            if (expHistory.Count < 2) return TimeSpan.Zero;

            var timeSpans = new List<TimeSpan>();
            var lastTime = expHistory[0].time;

            for (int i = 1; i < expHistory.Count; i++)
            {
                var currentTime = expHistory[i].time;
                var gap = currentTime - lastTime;

                if (gap.TotalSeconds <= 30)
                {
                    timeSpans.Add(gap);
                }

                lastTime = currentTime;
            }

            return TimeSpan.FromSeconds(timeSpans.Sum(t => t.TotalSeconds));
        }

        private string FormatExpValue(long value)
        {
            if (value >= 1000000000000) // Trilhões
            {
                return $"{value / 1000000000000.0:N3}T";
            }
            if (value >= 1000000000) // Bilhões
            {
                return $"{value / 1000000000.0:N3}B";
            }
            if (value >= 1000000) // Milhões
            {
                return $"{value / 1000000.0:N3}M";
            }
            return value.ToString("N0");
        }
        private string FormatTimeSpan(TimeSpan time)
        {
            if (time.TotalHours >= 1)
            {
                return $"{(int)time.TotalHours}h {time.Minutes}m";
            }
            return $"{time.Minutes}m {time.Seconds}s";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            tessEngine?.Dispose();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // OverlayForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "OverlayForm";
            this.ResumeLayout(false);

        }
    }

    // Classe para mover a janela
    internal static class NativeMethods
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new OverlayForm());
        }
    }
}