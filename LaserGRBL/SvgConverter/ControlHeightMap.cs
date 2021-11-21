/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2021 Sven Hasemann contact: svenhb@web.de

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

/*  Thanks to martin2250  https://github.com/martin2250/OpenCNCPilot for his HeightMap class
 * 2019-02-05 switch to global variables grbl.posWork
 * 2019-04-06 limit digits to 3, bugfix x3d export '.'-','
 * 2019-08-15 add logger
 * 2020-03-18 bug fix: abort btnLoad_Click - causes main GUI to load an empty map
 * 2021-04-30 after cancel, fill up missing coordinates line 561
 * 2021-07-14 code clean up / code quality
 * 2021-07-23 add notifier (by pushbullet or email)
*/

using System;
using System.Collections.Generic;

//#pragma warning disable CA1303
//#pragma warning disable CA1305

namespace LaserGRBL.SvgConverter
{
    public class HeightMap
    {
        public double?[,] Points { get; private set; }
        public int SizeX { get; private set; }
        public int SizeY { get; private set; }

        //     internal int TotalPoints { get { return SizeX * SizeY; } }

        internal Queue<Tuple<int, int>> NotProbed { get; private set; } = new Queue<Tuple<int, int>>();

        internal Vector2 Min { get; private set; }
        internal Vector2 Max { get; private set; }

        internal Vector2 Delta { get { return Max - Min; } }

        public double MinHeight { get; set; } = double.MaxValue;
        public double MaxHeight { get; set; } = double.MinValue;

        //        public event Action MapUpdated;

        public double GridX { get { return (Max.X - Min.X) / (SizeX - 1); } }
        public double GridY { get { return (Max.Y - Min.Y) / (SizeY - 1); } }


        internal HeightMap(double gridSize, Vector2 min, Vector2 max)
        {
            MinHeight = double.MaxValue;
            MaxHeight = double.MinValue;

            if (min.X == max.X) { max.X = min.X + 1; }
            if (min.Y == max.Y) { max.Y = min.Y + 1; }
            //                throw new Exception("Height map can't be infinitely narrow");

            int pointsX = (int)Math.Ceiling((max.X - min.X) / gridSize) + 1;
            int pointsY = (int)Math.Ceiling((max.Y - min.Y) / gridSize) + 1;

            if (pointsX == 0) { pointsX = 1; }
            if (pointsY == 0) { pointsY = 1; }
            //        throw new Exception("Height map must have at least 4 points");

            Points = new double?[pointsX, pointsY];

            if (max.X < min.X)
            {
                double a = min.X;
                min.X = max.X;
                max.X = a;
            }

            if (max.Y < min.Y)
            {
                double a = min.Y;
                min.Y = max.Y;
                max.Y = a;
            }

            Min = min;
            Max = max;

            SizeX = pointsX;
            SizeY = pointsY;

            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                    NotProbed.Enqueue(new Tuple<int, int>(x, y));

                if (++x >= SizeX)
                    break;

                for (int y = SizeY - 1; y >= 0; y--)
                    NotProbed.Enqueue(new Tuple<int, int>(x, y));
            }
        }

        public double InterpolateZ(double x, double y)
        {
            if (x > Max.X || x < Min.X || y > Max.Y || y < Min.Y)
                return MaxHeight;

            x -= Min.X;
            y -= Min.Y;

            x /= GridX;
            y /= GridY;

            int iLX = (int)Math.Floor(x);   //lower integer part
            int iLY = (int)Math.Floor(y);

            int iHX = (int)Math.Ceiling(x); //upper integer part
            int iHY = (int)Math.Ceiling(y);

            //     try
            //     {
            double fX = x - iLX;             //fractional part
            double fY = y - iLY;

            double linUpper = Points[iHX, iHY].Value * fX + Points[iLX, iHY].Value * (1 - fX);       //linear immediates
            double linLower = Points[iHX, iLY].Value * fX + Points[iLX, iLY].Value * (1 - fX);

            return linUpper * fY + linLower * (1 - fY);     //bilinear result
                                                            //   } catch { return MaxHeight; }
        }

        internal Vector2 GetCoordinates(int x, int y, bool applyOffset = true)
        {
            if (applyOffset)
                return new Vector2(x * (Delta.X / (SizeX - 1)) + Min.X, y * (Delta.Y / (SizeY - 1)) + Min.Y);
            else
                return new Vector2(x * (Delta.X / (SizeX - 1)), y * (Delta.Y / (SizeY - 1)));
        }

        private HeightMap()
        { }

        public void AddPoint(int x, int y, double height)
        {
            Points[x, y] = height;

            if (height > MaxHeight)
                MaxHeight = height;
            if (height < MinHeight)
                MinHeight = height;
        }
        public double? GetPoint(int x, int y)
        {
            if ((x >= 0) && (x < SizeX) && (y >= 0) && (y < SizeY))
                return Points[x, y];
            return null;
        }
        public void SetZOffset(double offset)
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    Points[ix, iy] = Points[ix, iy] + offset;
                }
            }
            MaxHeight += offset;
            MinHeight += offset;
        }
        public void SetZZoom(double zoom)
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    Points[ix, iy] = Points[ix, iy] * zoom;
                }
            }
            MaxHeight *= zoom;
            MinHeight *= zoom;
        }
        public void SetZInvert()
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    Points[ix, iy] = -Points[ix, iy];
                }
            }
            double tmp = MaxHeight;
            MaxHeight = -MinHeight;
            MinHeight = -tmp;
        }
        public void SetZCutOff(double limit)
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    if (Points[ix, iy] < limit)
                        Points[ix, iy] = limit;
                }
            }
            MinHeight = limit;
        }

    }


    public struct Vector2 : IEquatable<Vector2>
    {

        private double x;

        private double y;

        public Vector2(double x, double y)
        {
            // Pre-initialisation initialisation
            // Implemented because a struct's variables always have to be set in the constructor before moving control
            this.x = 0;
            this.y = 0;

            // Initialisation
            X = x;
            Y = y;
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Length()
        {
            return (float)Math.Sqrt(x*x+y*y);
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return
            (
                new Vector2
                    (
                        v1.X + v2.X,
                        v1.Y + v2.Y
                    )
            );
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return
            (
                new Vector2
                    (
                        v1.X - v2.X,
                        v1.Y - v2.Y
                    )
            );
        }

        public static Vector2 operator *(Vector2 v1, double s2)
        {
            return
            (
                new Vector2
                (
                    v1.X * s2,
                    v1.Y * s2
                )
            );
        }

        public static Vector2 operator *(double s1, Vector2 v2)
        {
            return v2 * s1;
        }

        public static Vector2 operator /(Vector2 v1, double s2)
        {
            return
            (
                new Vector2
                    (
                        v1.X / s2,
                        v1.Y / s2
                    )
            );
        }

        public static Vector2 operator -(Vector2 v1)
        {
            return
            (
                new Vector2
                    (
                        -v1.X,
                        -v1.Y
                    )
            );
        }

        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            return
            (
                Math.Abs(v1.X - v2.X) <= EqualityTolerence &&
                Math.Abs(v1.Y - v2.Y) <= EqualityTolerence
            );
        }

        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            return !(v1 == v2);
        }

        public bool Equals(Vector2 other)
        {
            return other == this;
        }

        public override bool Equals(object other)
        {
            // Convert object to Vector3
            // Check object other is a Vector3 object
            if (other is Vector2 otherVector)
            {
                // Check for equality
                return otherVector == this;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return
            (
                (int)((X + Y) % Int32.MaxValue)
            );
        }

        public const double EqualityTolerence = double.Epsilon;
    }

}
