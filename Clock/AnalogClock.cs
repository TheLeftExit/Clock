using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel;

namespace Clock {
    public partial class AnalogClock : UserControl {
        public ClockHand SecondHand { get => secondHand; set { secondHand = value; Invalidate(); } }
        public ClockHand MinuteHand { get => minuteHand; set { minuteHand = value; Invalidate(); } }
        public ClockHand HourHand { get => hourHand; set { hourHand = value; Invalidate(); } }

        private static Point[] debugShape = { new Point(100, 400), new Point(50, 200), new Point(100, 0), new Point(150, 200) };

        private ClockHand secondHand = new ClockHand(Color.Orange, debugShape);
        private ClockHand minuteHand = new ClockHand(Color.DarkOrange, debugShape);
        private ClockHand hourHand = new ClockHand(Color.Brown, debugShape);

        public bool OneMinuteMarkers { get => oneMinuteMarkersEnabled; set { oneMinuteMarkersEnabled = value; Invalidate(); } }
        public bool FiveMinuteMarkers { get => fiveMinuteMarkersEnabled; set { fiveMinuteMarkersEnabled = value; Invalidate(); } }
        public bool NumberMarkers { get => numberMarkersEnabled; set { numberMarkersEnabled = value; Invalidate(); } }

        private bool oneMinuteMarkersEnabled = true;
        private bool fiveMinuteMarkersEnabled = true;
        private bool numberMarkersEnabled = true;

        private Timer timer;

        public AnalogClock() {
            InitializeComponent();
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            GenerateAreas();
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (sender, e) => {
                timer.Interval = 1000 - DateTime.Now.Millisecond;
                timer.Start();
                Invalidate();
            };
            timer.Start();
        }

        private String[] romanNumbers = { "XII", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI" };
        private Font romanFont = new Font(FontFamily.GenericSansSerif, 10);
        private int Scale(int source, double factor) => (int)(source * factor);
        private Rectangle CenteredSquare(Point c, int radius) => new Rectangle(c.X - radius, c.Y - radius, radius * 2, radius * 2);
        private Point OffsetAngled(Point source, double distance, double angleDegrees) {
            double angleRadians = Math.PI * angleDegrees / 180;
            return new Point(source.X + (int)(distance * Math.Sin(angleRadians)), source.Y - (int)(distance * Math.Cos(angleRadians)));
        }

        private List<Rectangle> oneMinuteMarkerLocations = new List<Rectangle>();
        private List<Rectangle> fiveMinuteMarkerLocations = new List<Rectangle>();
        private List<Point> numberMarkerLocations = new List<Point>();

        private void GenerateAreas() {
            oneMinuteMarkerLocations.Clear();
            fiveMinuteMarkerLocations.Clear();
            numberMarkerLocations.Clear();

            int halfSide = Width / 2;
            Point center = new Point(halfSide, halfSide);
            int handLength = Scale(halfSide, 0.8);
            double minuteMarkerDistance = halfSide * 0.95;
            double oneMinuteMarkerRadius = halfSide * 0.023;
            double fiveMinuteMarkerRadius = halfSide * 0.035;
            double numberMarkerDistance = halfSide * 0.75;

            for (int i = 0; i < 60; i++) {
                double radius = i % 5 == 0 ? fiveMinuteMarkerRadius : oneMinuteMarkerRadius;
                double distance = minuteMarkerDistance - radius;
                Point circleCenter = OffsetAngled(center, distance, i * 6);
                Rectangle circlePosition = CenteredSquare(circleCenter, (int)radius);
                if (i % 5 == 0) fiveMinuteMarkerLocations.Add(circlePosition); else oneMinuteMarkerLocations.Add(circlePosition);
            }

            romanFont?.Dispose();
            romanFont = new Font(FontFamily.GenericSansSerif, (float)(halfSide * 0.1));

            for (int i = 0; i < 12; i++) {
                double distance = numberMarkerDistance;
                Point textCenter = OffsetAngled(center, distance, i * 30);
                Size textSize;
                using(Graphics g = CreateGraphics())
                    textSize = g.MeasureString(romanNumbers[i], romanFont).ToSize();
                Point textPosition = new Point(textCenter.X - textSize.Width / 2, textCenter.Y - textSize.Height / 2);
                numberMarkerLocations.Add(textPosition);
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (oneMinuteMarkersEnabled)
                foreach (Rectangle p in oneMinuteMarkerLocations)
                    g.DrawEllipse(Pens.Black, p);

            if (fiveMinuteMarkersEnabled)
                foreach (Rectangle p in fiveMinuteMarkerLocations)
                    g.DrawEllipse(Pens.Black, p);


            if (numberMarkersEnabled)
                for (int i = 0; i < 12; i++)
                    g.DrawString(romanNumbers[i], romanFont, Brushes.Black, numberMarkerLocations[i]);

            g.DrawEllipse(Pens.LightGray, new Rectangle(Point.Empty, Size - new Size(1, 1)));

            DateTime now = DateTime.Now;
            float secondAngle = now.Second * 6;
            float minuteAngle = now.Minute * 6 + secondAngle / 60f;
            float hourAngle = now.Hour * 30 + minuteAngle / 12f;

            secondHand.Draw(g, Width, secondAngle);
            minuteHand.Draw(g, Width, minuteAngle);
            hourHand.Draw(g, Width, hourAngle);

        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            int side = Math.Min(Width, Height);
            Width = side; Height = side;
            GenerateAreas();
            Invalidate();
        }
    }
}
