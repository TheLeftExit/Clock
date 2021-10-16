using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clock {
    public class ClockHand {
        public Color Color { get; }
        public Point[] Shape { get; } // (0.375, 0)

        public ClockHand(Color color, params Point[] shape) {
            Color = color;
            Shape = shape;
        }
        
        public void Draw(Graphics g, int width, float angleDegrees) {
            Matrix origin = g.Transform;
            //g.SetClip(new Rectangle(0, 0, width, width));
            g.ScaleTransform(width / 800f, width / 800f);
            g.TranslateTransform(400, 400);
            g.RotateTransform(angleDegrees);
            g.TranslateTransform(-100, -400);
            using (Brush brush = new SolidBrush(Color))
                g.FillPolygon(brush, Shape);
            g.Transform = origin;
        }
    }
}
