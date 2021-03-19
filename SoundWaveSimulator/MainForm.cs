using System;
using System.Drawing;
using System.Windows.Forms;

namespace SoundWaveSimulator
{
    public partial class MainForm : Form
    {
        private const double InitialOmegaT = 0.01 * Math.PI;

        private int _waveOffset = 0;
        private Bitmap _mic, _speaker;
        private int _micPosition = 0;
        private double _omegaT = InitialOmegaT;
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _mic = new Bitmap("se1mic-1.png");
            _speaker = new Bitmap("emmiter1-1.png");
            trackBar.Minimum = _speaker.Width;
            trackBar.Maximum = Picture.Width - _mic.Width;
            trackBar.Value = trackBar.Maximum;
            //Picture.Height = Math.Max(_mic.Height, _speaker.Height);
            Picture.Paint += Picture_Paint;
            WavePicture.Paint += WavePicture_Paint;
            Resize += Form1_Resize;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Picture.Invalidate();
            WavePicture.Invalidate();
            trackBar.Maximum = Picture.Width - _mic.Width;
            trackBar.Value = Picture.Width - _mic.Width + _micPosition;
        }

        private void WavePicture_Paint(object sender, PaintEventArgs e)
        {
            var waveAmplitudeCoefficient = WavePicture.Height * 0.4;
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.FillRectangle(Brushes.Black, 0, 0, Picture.Width, Picture.Height);
            var pointsSpk = new PointF[WavePicture.Width];
            var pointsMic = new PointF[WavePicture.Width];
            for (var x = 0; x < WavePicture.Width; x++)
            {
                pointsSpk[x] = new PointF(x, (float)(WavePicture.Height / 2 + waveAmplitudeCoefficient * Sine(x + GetXSpkMembranaAvg())));
                pointsMic[x] = new PointF(x, (float)(WavePicture.Height / 2 + waveAmplitudeCoefficient * Sine(x + GetXMicMembranaAvg())));
            }
            g.DrawLines(Pens.Yellow, pointsSpk);
            g.DrawLines(Pens.Aqua, pointsMic);
        }

        private void Picture_Paint(object sender, PaintEventArgs e)
        {
            var r = 3;
            var n = 4;
            var g = e.Graphics;
            var particleBrush = Brushes.Silver;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.FillRectangle(Brushes.Black, 0, 0, Picture.Width, Picture.Height);
            var rnd = new Random(_waveOffset);
            for (var x = 0; x < (int)(_speaker.Width * 0.8); x++)
            {
                for (var i = 0; i < n / 2; i++)
                {
                    g.FillRectangle(particleBrush, x, rnd.Next(Picture.Height), r, r);
                }
            }
            for (var x = (int)(_speaker.Width * 0.8); x < Picture.Width; x++)
            {
                var k = (int)(Sine(x) * n) + n;
                for (var i = 0; i < k; i++)
                {
                    g.FillRectangle(particleBrush, x, rnd.Next(Picture.Height), r, r);
                }
            }
            g.DrawImageUnscaled(_speaker, 0, (Picture.Height - _mic.Height) / 2);
            g.DrawImageUnscaled(_mic, Picture.Width - _mic.Width + _micPosition, (Picture.Height - _mic.Height) / 2);


            var membranaAmplitude = 20;
            var membrataTop = Picture.Height * 0.1f;
            var membranaBottom = Picture.Height * 0.8f;
            var membranaWidth = 6;

            var xSpkMembranaAvg = GetXSpkMembranaAvg();
            var xSpkMembrana = (float)(xSpkMembranaAvg + Sine(xSpkMembranaAvg) * membranaAmplitude);
            g.FillRectangle(Brushes.Yellow, xSpkMembrana - membranaWidth / 2, membrataTop, membranaWidth, membranaBottom);

            var xMicMembranaAvg = GetXMicMembranaAvg();
            var xMicMembrana = (float)(xMicMembranaAvg + Sine(xMicMembranaAvg) * membranaAmplitude);
            g.FillRectangle(Brushes.Aqua, xMicMembrana - membranaWidth / 2, membrataTop, membranaWidth, membranaBottom);
        }

        private int GetXMicMembranaAvg()
        {
            return Picture.Width - _mic.Width / 2 + _micPosition;
        }

        private int GetXSpkMembranaAvg()
        {
            return _speaker.Width - 15;
        }

        private double Sine(int x)
        {
            return Math.Sin((x + _waveOffset) * _omegaT);
        }

        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            _micPosition = trackBar.Value - (Picture.Width - _mic.Width);
            if (!timer1.Enabled)
            {
                Picture.Invalidate();
                WavePicture.Invalidate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            _omegaT = trackBar1.Value * InitialOmegaT / 10;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _waveOffset -= 5;
            Picture.Invalidate();
            WavePicture.Invalidate();
        }
    }
}
