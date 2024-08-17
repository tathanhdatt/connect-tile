#nullable enable
using System;
using System.Collections.Generic;

namespace MatchingPairAlgorithm
{
    public class Algorithm
    {
        private const int UnknownCoordinate = -1;
        private Graph graph;

        public Algorithm(Graph graph)
        {
            this.graph = graph;
        }

        public void SetGraph(Graph other)
        {
            this.graph = other;
        }

        public List<Point>? GetPathFromPointToPoint(Point point1, Point point2)
        {
            var pivot = FindPath(point1, point2);
            if (pivot is null)
            {
                return null;
            }


            var isMatchByLine = pivot.Equals(point2);
            if (isMatchByLine)
            {
                if (point1.X == point2.X)
                {
                    return GetPointsByLineHasSameX(point1, point2);
                }
                return GetPointsByLineHasSameY(point1, point2);
            }

            var isMatchByX = pivot.X == UnknownCoordinate;

            Point pointCorner1;
            Point pointCorner2;
            List<Point> groupPoint1;
            List<Point> groupPoint2;
            List<Point> groupPoint3;
            if (isMatchByX)
            {
                pointCorner1 = new Point(point1.X, pivot.Y);
                pointCorner2 = new Point(point2.X, pivot.Y);
                groupPoint1 = GetPointsByLineHasSameX(point1, pointCorner1);
                groupPoint2 = GetPointsByLineHasSameY(pointCorner1, pointCorner2);
                groupPoint3 = GetPointsByLineHasSameX(pointCorner2, point2);
            }
            else
            {
                pointCorner1 = new Point(pivot.X, point1.Y);
                pointCorner2 = new Point(pivot.X, point2.Y);
                groupPoint1 = GetPointsByLineHasSameY(point1, pointCorner1);
                groupPoint2 = GetPointsByLineHasSameX(pointCorner1, pointCorner2);
                groupPoint3 = GetPointsByLineHasSameY(pointCorner2, point2);
            }
            
            var isCorrectDirection = groupPoint1.Last().Equals(groupPoint2.First());
            if (!isCorrectDirection)
            {
                groupPoint2.Reverse();
            }

            isCorrectDirection = groupPoint2.Last().Equals(groupPoint3.First());
            if (!isCorrectDirection)
            {
                groupPoint3.Reverse();
            }
            groupPoint2.RemoveFirst();
            groupPoint2.RemoveLast();
            groupPoint1.Concat(groupPoint2).Concat(groupPoint3);
            return groupPoint1;  
        }

        private List<Point> GetPointsByLineHasSameX(Point point1, Point point2)
        {
            var x = point1.X;
            var minY = Math.Min(point1.Y, point2.Y);
            var maxY = Math.Max(point1.Y, point2.Y);
            var points = new List<Point>();
            for (var y = minY; y <= maxY; y++)
            {
                points.Add(new Point(x, y));
            }

            if (point1.Y > point2.Y)
            {
                points.Reverse();
            }

            return points;
        }

        private List<Point> GetPointsByLineHasSameY(Point point1, Point point2)
        {
            var y = point1.Y;
            var minX = Math.Min(point1.X, point2.X);
            var maxX = Math.Max(point1.X, point2.X);
            var points = new List<Point>();
            for (var x = minX; x <= maxX; x++)
            {
                points.Add(new Point(x, y));
            }

            if (point1.X > point2.X)
            {
                points.Reverse();
            }

            return points;
        }

        public bool HasPath(Point point1, Point point2)
        {
            if (FindPath(point1, point2) == null) return false;
            graph.SetValue(point1.X, point1.Y, 1);
            graph.SetValue(point2.X, point2.Y, 1);
            return true;

        }

        private Point? FindPath(Point point1, Point point2)
        {
            graph.SetValue(point1.X, point1.Y, 0);
            graph.SetValue(point2.X, point2.Y, 0);

            int newCoordinate;
            if (CheckLineX(point1, point2))
            {
                return point2;
            }

            if (CheckLineY(point1, point2))
            {
                return point2;
            }

            if ((newCoordinate = CheckRectX(point1, point2)) != UnknownCoordinate)
            {
                return new Point(UnknownCoordinate, newCoordinate);
            }

            if ((newCoordinate = CheckRectY(point1, point2)) != UnknownCoordinate)
            {
                return new Point(newCoordinate, UnknownCoordinate);
            }

            if ((newCoordinate = CheckExtraX(point1, point2)) != UnknownCoordinate)
            {
                return new Point(UnknownCoordinate, newCoordinate);
            }

            if ((newCoordinate = CheckExtraY(point1, point2)) != UnknownCoordinate)
            {
                return new Point(newCoordinate, UnknownCoordinate);
            }

            graph.SetValue(point1.X, point1.Y, 1);
            graph.SetValue(point2.X, point2.Y, 1);

            return null;
        }


        // Check 2 points having the same coordinate x are able to connect
        private bool CheckLineX(Point point1, Point point2)
        {
            if (point1.X != point2.X)
            {
                return false;
            }

            var minY = Math.Min(point1.Y, point2.Y);
            var maxY = Math.Max(point1.Y, point2.Y);
            var x = point1.X;

            for (var y = minY; y <= maxY; y++)
            {
                if (graph.GetValue(x, y) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        // Check 2 points having the same coordinate y are able to connect
        private bool CheckLineY(Point point1, Point point2)
        {
            if (point1.Y != point2.Y)
            {
                return false;
            }

            var minX = Math.Min(point1.X, point2.X);
            var maxX = Math.Max(point1.X, point2.X);
            var y = point1.Y;

            for (var x = minX; x <= maxX; x++)
            {
                if (graph.GetValue(x, y) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private int CheckRectX(Point point1, Point point2)
        {
            var pMinY = point1;
            var pMaxY = point2;

            if (point1.Y > point2.Y)
            {
                pMinY = point2;
                pMaxY = point1;
            }

            for (var y = pMinY.Y; y <= pMaxY.Y; y++)
            {
                if (!CheckLineX(pMinY, new Point(pMinY.X, y)))
                {
                    continue;
                }

                if (!CheckLineY(new Point(pMinY.X, y), new Point(pMaxY.X, y)))
                {
                    continue;
                }

                if (!CheckLineX(new Point(pMaxY.X, y), pMaxY))
                {
                    continue;
                }

                return y;
            }

            return UnknownCoordinate;
        }

        private int CheckRectY(Point point1, Point point2)
        {
            var pMinX = point1;
            var pMaxX = point2;

            if (point1.X > point2.X)
            {
                pMinX = point2;
                pMaxX = point1;
            }

            for (var x = pMinX.X; x <= pMaxX.X; x++)
            {
                if (!CheckLineY(pMinX, new Point(x, pMinX.Y)))
                {
                    continue;
                }

                if (!CheckLineX(new Point(x, pMinX.Y), new Point(x, pMaxX.Y)))
                {
                    continue;
                }

                if (!CheckLineY(new Point(x, pMaxX.Y), pMaxX))
                {
                    continue;
                }

                return x;
            }

            return UnknownCoordinate;
        }

        private int CheckExtraX(Point point1, Point point2)
        {
            var pMinY = point1;
            var pMaxY = point2;

            if (point1.Y > point2.Y)
            {
                pMinY = point2;
                pMaxY = point1;
            }

            // Check right
            var y = pMaxY.Y;
            var pCorner = new Point(pMinY.X, y);
            if (CheckLineX(pMinY, pCorner))
            {
                while (y < graph.NumberCol)
                {
                    if (CheckLineX(pCorner, new Point(pCorner.X, y)) &&
                        CheckLineY(new Point(pCorner.X, y), new Point(pMaxY.X, y)) &&
                        CheckLineX(new Point(pMaxY.X, y), pMaxY))
                    {
                        return y;
                    }

                    y++;
                }
            }

            // Check left
            y = pMinY.Y;
            pCorner = new Point(pMaxY.X, y);
            if (!CheckLineX(pMaxY, pCorner)) return UnknownCoordinate;
            while (y >= 0)
            {
                if (
                    CheckLineX(pCorner, new Point(pCorner.X, y)) &&
                    CheckLineY(new Point(pCorner.X, y), new Point(pMinY.X, y)) &&
                    CheckLineX(pMinY, new Point(pMinY.X, y)))
                {
                    return y;
                }

                y--;
            }

            return UnknownCoordinate;
        }

        private int CheckExtraY(Point point1, Point point2)
        {
            var pMinX = point1;
            var pMaxX = point2;

            if (point1.X > point2.X)
            {
                pMinX = point2;
                pMaxX = point1;
            }

            // Check down
            var x = pMaxX.X;
            var pCorner = new Point(x, pMinX.Y);
            if (CheckLineY(pMinX, pCorner))
            {
                while (x < graph.NumberRow)
                {
                    if (CheckLineY(pCorner, new Point(x, pCorner.Y)) &&
                        CheckLineX(new Point(x, pCorner.Y), new Point(x, pMaxX.Y)) &&
                        CheckLineY(new Point(x, pMaxX.Y), pMaxX))
                    {
                        return x;
                    }

                    x++;
                }
            }

            // Check up
            x = pMinX.X;
            pCorner = new Point(x, pMaxX.Y);
            if (!CheckLineY(pMaxX, pCorner)) return UnknownCoordinate;
            while (x >= 0)
            {
                if (
                    CheckLineY(pCorner, new Point(x, pCorner.Y)) &&
                    CheckLineX(new Point(x, pCorner.Y), new Point(x, pMinX.Y)) &&
                    CheckLineY(pMinX, new Point(x, pMinX.Y)))
                {
                    return x;
                }

                x--;
            }

            return UnknownCoordinate;
        }
    }
}