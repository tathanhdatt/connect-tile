namespace MatchingPairAlgorithm
{
    public class Point
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public bool Equals(Point other)
        {
            return other.X == this.X && other.Y == this.Y;
        }

        public override string ToString()
        {
            return "(" + X + ',' + Y + ')';
        }
    }
}