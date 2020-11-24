namespace TheFellowshipOfCode.DotNet.YourAdventure
{
    public class Node
    {
        // calculated values while finding path
        public int gCost;
        public int gridX;
        public int gridY;
        public int hCost;
        public Node parent;

        public float price;

        // is this node walkable?
        public bool walkable;

        public Node(float _price, int _gridX, int _gridY)
        {
            walkable = _price != 0.0f;
            price = _price;
            gridX = _gridX;
            gridY = _gridY;
        }

        public Node(bool _walkable, int _gridX, int _gridY)
        {
            walkable = _walkable;
            price = _walkable ? 1f : 0f;
            gridX = _gridX;
            gridY = _gridY;
        }

        public int fCost => gCost + hCost;

        public void Update(float _price, int _gridX, int _gridY)
        {
            walkable = _price != 0.0f;
            price = _price;
            gridX = _gridX;
            gridY = _gridY;
        }

        public void Update(bool _walkable, int _gridX, int _gridY)
        {
            walkable = _walkable;
            price = _walkable ? 1f : 0f;
            gridX = _gridX;
            gridY = _gridY;
        }
    }
}