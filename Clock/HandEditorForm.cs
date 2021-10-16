using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clock {
    public partial class HandEditorForm : Form {
        private class BufferedPanel : Panel { public BufferedPanel() { DoubleBuffered = true; } }

        private double Distance(Point p1, Point p2) => Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

        public ClockHand Result { get; set; }

        private Color color;
        private List<Point> points = new List<Point>();
        private Point mousePosition;

        private Brush brush;
        private Pen pen;

        public HandEditorForm() {
            InitializeComponent();
            LoadColor(Color.Orange);
        }

        private void LoadColor(Color newColor) {
            color = newColor;

            brush?.Dispose();
            brush = new SolidBrush(color);
            pen?.Dispose();
            pen = new Pen(Color.FromArgb(color.A, color.R / 2, color.G / 2, color.B / 2)); // Dimmed color for borders

            Invalidate();
        }

        private void OnPanelMouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                if (points.Count > 0)
                    points.RemoveAt(points.Count - 1);
                return;
            }
            if(e.Button == MouseButtons.Left) {
                Point newLocation = e.Location;
                if (Distance(new Point(100, 400), e.Location) <= 10)
                    newLocation = new Point(100, 400);
                if(points.Count > 0) {
                    if (Distance(points[0], newLocation) < 16) {
                        Result = new ClockHand(color, points.ToArray());
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
                points.Add(newLocation);
            }
        }

        private void OnPanelPaint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.DrawEllipse(Pens.Gray, new Rectangle(90, 390, 20, 20));
            g.DrawEllipse(Pens.Gray, new Rectangle(-300, 0, 800, 800));
            g.DrawLine(Pens.LightGray, new Point(100, 400), new Point(100, 0));

            if (points.Count == 0)
                return;

            g.FillPolygon(brush, points.ToArray()); // I expect garbage leaks with this, but I don't want to limit
            for (int i = 0; i < points.Count - 1; i++)
                g.DrawLine(pen, points[i], points[i + 1]);
            g.DrawLine(pen, points[points.Count - 1], mousePosition);
        }

        private void OnPanelMouseMove(object sender, MouseEventArgs e) {
            mousePosition = e.Location;
            panel1.Invalidate();
        }

        private void OnColorButtonClick(object sender, EventArgs e) {
            ColorDialog colorDialog = new ColorDialog();
            DialogResult dialogResult = colorDialog.ShowDialog();
            if(dialogResult == DialogResult.OK) {
                LoadColor(colorDialog.Color);
            }
        }
    }
}
