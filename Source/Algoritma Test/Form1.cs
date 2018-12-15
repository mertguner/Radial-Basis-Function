using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Algoritma_Test.RadialBasisFunctionLibrary;

namespace Algoritma_Test
{
    public partial class Form1 : Form
    {
        Bitmap Destination, BrokenPicture, Final;

        #region Draw bRoken Picture
        bool DrawEnable = false;
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            DrawEnable = true;
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (DrawEnable)
            {
                if (e.X > -1 && e.Y > -1 && e.X < BrokenPicture.Size.Width && e.Y < BrokenPicture.Size.Height)
                {
                    Graphics DrawSurface = Graphics.FromImage(BrokenPicture);
                    // Set background color to white so that pixels can be correctly colorized
                    Color value = Destination.GetPixel(e.X, e.Y);
                    DrawSurface.DrawPolygon(new Pen(value), new Point[] { new Point(e.X, e.Y), new Point(e.X + 1, e.Y), new Point(e.X + 1, e.Y + 1), new Point(e.X, e.Y + 1) });

                    pictureBox2.Image = BrokenPicture;
                }
            }
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            DrawEnable = false;
        }

        private void pictureBox2_MouseHover(object sender, EventArgs e)
        {
            DrawEnable = false;
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            DrawEnable = false;
        }
        #endregion

        #region Delegate Functions
        delegate void GroupBoxText_Change(GroupBox groupBox, string value);
        private void GroupBoxTextChange(GroupBox groupBox, string value)
        {
            try
            {
                if (groupBox.InvokeRequired)
                {
                    GroupBoxText_Change d = new GroupBoxText_Change(GroupBoxTextChange);
                    this.Invoke(d);
                }
                else
                {
                    groupBox.Text = value;
                }
            }
            catch (Exception ex) {  }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            List<HeatPoint> aHeatPoints = new List<HeatPoint>();
            for (int x = 0; x < BrokenPicture.Size.Width; x++)
            {
                for (int y = 0; y < BrokenPicture.Size.Height; y++)
                {
                    Color value = BrokenPicture.GetPixel(x,y);                   
                    if(value.A != 0)
                    {
                        Color _color = Destination.GetPixel(x, y);
                        aHeatPoints.Add(new HeatPoint(x,y, _color));
                    }
                }
            }

            this.BeginInvoke(new GroupBoxText_Change(GroupBoxTextChange), new object[] { groupBox2 , "Broken Picture %" + ((float)aHeatPoints.Count / (float)(BrokenPicture.Size.Width * BrokenPicture.Size.Height) * 100f).ToString("0.0") });

            Final = CreateIntensityMask(Final, aHeatPoints, (float)trackBar1.Value / 100f, (float)trackBar2.Value / 100f);

            pictureBox3.Image = Final;
        }

        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Destination = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = Destination;

            BrokenPicture = new Bitmap(pictureBox2.Size.Width, pictureBox2.Size.Height);
            pictureBox2.Image = BrokenPicture;

            Final = new Bitmap(pictureBox3.Size.Width, pictureBox3.Size.Height);
            pictureBox3.Image = Final;

            Destination = (Bitmap)Bitmap.FromFile(@"Test.bmp");

            pictureBox1.Image = Destination;           
        }
    }
}
