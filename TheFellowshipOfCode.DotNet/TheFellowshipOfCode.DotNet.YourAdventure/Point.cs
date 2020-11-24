namespace TheFellowshipOfCode.DotNet.YourAdventure
{
    public struct Point
    {
        // point X
        public int x;

        // point Y
        public int y;

        public Point(int iX, int iY)
        {
            x = iX;
            y = iY;
        }

        public Point(Point b)
        {
            x = b.x;
            y = b.y;
        }
        
        public override int GetHashCode()
        {
            return x ^ y;
        }
        
        public override bool Equals(object obj)
        {
            // check type
            if (!(obj.GetType() == typeof(Point)))
                return false;

            // check if other is null
            var p = (Point) obj;
            if (ReferenceEquals(null, p)) return false;

            // Return true if the fields match:
            return x == p.x && y == p.y;
        }
        
        public bool Equals(Point p)
        {
            // check if other is null
            if (ReferenceEquals(null, p)) return false;

            // Return true if the fields match:
            return x == p.x && y == p.y;
        }
        
        public static bool operator ==(Point a, Point b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(null, a)) return false;
            if (ReferenceEquals(null, b)) return false;
            // Return true if the fields match:
            return a.x == b.x && a.y == b.y;
        }
        
        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }
        
        public Point Set(int iX, int iY)
        {
            x = iX;
            y = iY;
            return this;
        }
    }
}