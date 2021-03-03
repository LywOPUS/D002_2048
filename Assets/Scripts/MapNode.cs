using AlderaminUtils;

namespace DefaultNamespace
{
    public class MapNode
    {
        private int x;
        private int y;
        private Grid2D<MapNode> _grid2D;

        public enum NodeType
        {
            Empty,
            Num2,
            Num4,
            Num16,
            Num32,
            Num64,
            Num128,
            Num256,
            Num512,
            Num1024,
            Num2048
        }

        private NodeType _nodeType;

        public MapNode(Grid2D<MapNode> grid2D, int x, int y)
        {
            this.x = x;
            this.y = y;
            _grid2D = grid2D;
            _nodeType = NodeType.Empty;
        }

        public void SetNodeType(NodeType type)
        {
            _nodeType = type;
            _grid2D.TriggerGridMapValueChangeEvent(x, y);
        }

        public NodeType GetNodeType()
        {
            return _nodeType;
        }

        public void RestNode()
        {
            _nodeType = NodeType.Empty;
        }
        public override string ToString()
        {
            return _nodeType.ToString();
        }
    }
}