using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spline;

namespace Tower_defense_game.Other
{
    public static class PathMaker
    {
        static GraphicsDevice Graphics { get; set; }

        static public void SetGraphicsDevice(GraphicsDevice graphics)
        {
            Graphics = graphics;
        }

        /// <summary>
        /// Creates a path with a start and end
        /// </summary>
        /// <param name="start">Startpoint of the</param>
        /// <param name="end"></param>
        /// <returns>SimplePath of the path</returns>
        static public SimplePath CreatePath(Vector2 start, Vector2 end)
        {
            SimplePath newPath = new(Graphics); ;

            newPath.Clean();
            newPath.AddPoint(start);
            newPath.AddPoint(new(start.X + (start.X-end.X)*-1/2, start.Y + (start.Y - end.Y) * -1 / 2));
            newPath.AddPoint(end);

            return newPath;
        }

        /// <summary>
        /// Creates a path using many points
        /// </summary>
        /// <param name="points">All vectors that the path croses</param>
        /// <returns>SimplePath of the path</returns>
        static public SimplePath CreatePath(Vector2[] points)
        {
            SimplePath newPath = new(Graphics);

            newPath.Clean();

            for (int x = 0; x < points.Length; x++)
                newPath.AddPoint(points[x]);

            return newPath;
        }

        /// <summary>
        /// Creates a path using many points and an amount of points between them all
        /// </summary>
        /// <param name="points">All vectors that the path croses</param>
        /// <param name="middlePoints">How many points are to be between the points given</param>
        /// <returns>SimplePath of the path</returns>
        static public SimplePath CreatePath(Vector2[] points, int middlePoints)
        {
            SimplePath newPath = new(Graphics);

            newPath.Clean();

            for (int x = 0; x < points.Length; x++)
            {
                newPath.AddPoint(points[x]);

                if (x + 1 < points.Length)
                    for (int y = 1; y <= middlePoints; y++)
                    {
                        int newPointX = (int)(points[x].X + ((points[x].X - points[x + 1].X) * -1 / (middlePoints + 1)) * y);
                        int newPointY = (int)(points[x].Y + ((points[x].Y - points[x + 1].Y) * -1 / (middlePoints + 1)) * y);
                        Vector2 newPoint = new(newPointX, newPointY);
                        newPath.AddPoint(newPoint);
                    }
            }

            return newPath;
        }

        /// <summary>
        /// Creates a path using many points and an amount of points between certain points
        /// </summary>
        /// <param name="points">All vectors that the path croses</param>
        /// <param name="middlePoints">How many points are to be between the points given</param>
        /// <param name="specifiedPoints">Indexes of the points to add points between</param>
        /// <returns>SimplePath of the path</returns>
        static public SimplePath CreatePath(Vector2[] points, int middlePoints, int[] specifiedPoints)
        {
            int[] distinctPoints = specifiedPoints.Distinct().ToArray();
            SimplePath newPath = new(Graphics);

            newPath.Clean();

            for (int x = 0; x < points.Length; x++)
            {
                newPath.AddPoint(points[x]);

                if (x + 1 < points.Length && distinctPoints.Contains(x))
                    for (int y = 1; y <= middlePoints; y++)
                    {
                        float newPointX = points[x].X + ((points[x].X - points[x + 1].X) * -1 / (middlePoints + 1)) * y;
                        float newPointY = points[x].Y + ((points[x].Y - points[x + 1].Y) * -1 / (middlePoints + 1)) * y;
                        Vector2 newPoint = new(newPointX, newPointY);
                        newPath.AddPoint(newPoint);
                    }
            }

            return newPath;
        }

        /// <summary>
        /// Creates a path using many points and an amount of points between certain points
        /// </summary>
        /// <param name="points">All vectors that the path croses</param>
        /// <param name="specifiedMiddlePoints">How many points to add between two points, based upon it's position in array</param>
        /// <returns>SimplePath of the path</returns>
        static public SimplePath CreatePath(Vector2[] points, int[] specifiedMiddlePoints)
        {
            SimplePath newPath = new(Graphics);

            newPath.Clean();

            for (int x = 0; x < points.Length; x++)
            {
                newPath.AddPoint(points[x]);

                if (x + 1 < points.Length && x < specifiedMiddlePoints.Length)
                    for (int y = 1; y <= specifiedMiddlePoints[x]; y++)
                    {
                        float newPointX = points[x].X + ((points[x].X - points[x + 1].X) * -1 / (specifiedMiddlePoints[x] + 1)) * y;
                        float newPointY = points[x].Y + ((points[x].Y - points[x + 1].Y) * -1 / (specifiedMiddlePoints[x] + 1)) * y;
                        Vector2 newPoint = new(newPointX, newPointY);
                        newPath.AddPoint(newPoint);
                    }
            }

            return newPath;
        }
    }
}
