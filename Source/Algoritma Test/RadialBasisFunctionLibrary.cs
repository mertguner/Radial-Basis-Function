using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Algoritma_Test
{
    public class RadialBasisFunctionLibrary
    {
        public struct HeatPoint
        {
            public int X;
            public int Y;
            public Color Color;
            public HeatPoint(int iX, int iY, Color color)
            {
                X = iX;
                Y = iY;
                Color = color;
            }
        }

        public static Color pointValue(Point texturecoordinate, float power, float smoothing, List<HeatPoint> Points)
        {
            float[] nominator = new float[4];
            float denominator = 0;
            byte[] value = new byte[4];

            for (int i = 0; i < Points.Count; i++)
            {
                float dist = 0;
                if (i < Points.Count) dist = (float)Math.Sqrt(Math.Pow((texturecoordinate.X - Points[i].X), 2) + Math.Pow((texturecoordinate.Y - Points[i].Y), 2) + (smoothing * smoothing));
                //If the point is really close to one of the data points, return the data point value to avoid singularities
                if (dist < 0.00000001f)
                    return Points[i].Color;
                if (dist.CompareTo(float.NaN) == 0 || dist == 0) return Points[i].Color;

                if (dist.CompareTo(float.NaN) != 0 && dist != 0)
                {
                    nominator[0] += (Points[i].Color.R / (float)Math.Pow(dist, power));
                    nominator[1] += (Points[i].Color.G / (float)Math.Pow(dist, power));
                    nominator[2] += (Points[i].Color.B / (float)Math.Pow(dist, power));
                    nominator[3] += (Points[i].Color.A / (float)Math.Pow(dist, power));

                    denominator += (1f / (float)Math.Pow(dist, power));
                }
            }
            //Return NODATA if the denominator is zero
            if (denominator > 0)
            {
                value[0] = Convert.ToByte(Math.Max(0, Math.Min(255, nominator[0] / denominator)));
                value[1] = Convert.ToByte(Math.Max(0, Math.Min(255, nominator[1] / denominator)));
                value[2] = Convert.ToByte(Math.Max(0, Math.Min(255, nominator[2] / denominator)));
                value[3] = Convert.ToByte(Math.Max(0, Math.Min(255, nominator[3] / denominator)));
            }
            else value = new byte[4] { 0,0,0,0};

            return Color.FromArgb(value[3], value[0], value[1], value[2]);
        }

        public static Bitmap CreateIntensityMask(Bitmap bSurface, List<HeatPoint> aHeatPoints, float guc, float yumusat)
        {
            // Create new graphics surface from memory bitmap
            Graphics DrawSurface = Graphics.FromImage(bSurface);
            // Set background color to white so that pixels can be correctly colorized
            DrawSurface.Clear(Color.White);
            // Traverse heat point data and draw masks for each heat point
            //foreach (HeatPoint DataPoint in aHeatPoints)
            //{
            //    // Render current heat point on draw surface
            //    DrawHeatPoint(DrawSurface, DataPoint, 15);
            //}
            Color[,] vals = new Color[bSurface.Size.Width, bSurface.Size.Height];
            System.Threading.Tasks.Parallel.For(0, vals.GetLength(0), x =>
            {
                System.Threading.Tasks.Parallel.For(0, vals.GetLength(1), y =>
                {
                    vals[x, y] = pointValue(new Point(x, y), guc, yumusat, aHeatPoints);
                });
            });

            for (int x = 0; x < bSurface.Size.Width; x++)
                for (int y = 0; y < bSurface.Size.Height; y++)
                    DrawSurface.DrawPolygon(new Pen(vals[x,y]), new Point[] { new Point(x, y), new Point(x + 1, y), new Point(x + 1, y + 1), new Point(x, y + 1) });

            return bSurface;
        }
    }
}
