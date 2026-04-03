using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace рццс
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect,      // x-coordinate of upper-left corner
        int nTopRect,       // y-coordinate of upper-left corner
        int nRightRect,     // x-coordinate of lower-right corner
        int nBottomRect,    // y-coordinate of lower-right corner
        int nWidthEllipse,  // width of ellipse
        int nHeightEllipse  // height of ellipse
        );
        private int _cornerRadius = 20;

        public int CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = value;
                ApplyRoundedCorners(value);
            }
        }
        private void ApplyRoundedCorners(int radius)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.Region = null; // в полноэкранном режиме убираем скругление
                return;
            }

            IntPtr region = CreateRoundRectRgn(0, 0, this.Width, this.Height, radius, radius);
            this.Region = Region.FromHrgn(region);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyRoundedCorners(_cornerRadius);
        }
        public Form1()
        {
            InitializeComponent();
            ApplyRoundedCorners(20);
        }
        private bool isDragging = false;
        private Point lastCursorPosition;
        private Point lastFormLocation;

        private bool isMaximized = false;
        private Rectangle normalBounds;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursorPosition = Cursor.Position;
                lastFormLocation = this.Location;
            }
        }

        private void свернуть_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void маштабировать_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Maximized;
        }

        private void закрыть_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void ToggleMaximize()
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                // Возврат в оконный режим
                this.WindowState = FormWindowState.Normal;
                this.Bounds = normalBounds;
            }
            else
            {
                // Сохраняем текущее положение и размер перед разворачиванием
                normalBounds = this.Bounds;

                // Разворачиваем на весь экран
                this.WindowState = FormWindowState.Maximized;
            }
        }
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                // Если окно было развёрнуто — сначала возвращаем его в оконный режим
                if (this.WindowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;

                    // Ставим окно примерно под курсор мыши
                    this.Location = new Point(
                        Cursor.Position.X - (this.Width / 2),
                        Cursor.Position.Y - 10);
                }

                Point currentCursor = Cursor.Position;
                int deltaX = currentCursor.X - lastCursorPosition.X;
                int deltaY = currentCursor.Y - lastCursorPosition.Y;

                this.Location = new Point(
                    lastFormLocation.X + deltaX,
                    lastFormLocation.Y + deltaY
                );

                lastCursorPosition = currentCursor;
                lastFormLocation = this.Location;
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ToggleMaximize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel3.Visible = false;  
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            
            ApplyRoundedCorners(_cornerRadius);
        }
    }
}
