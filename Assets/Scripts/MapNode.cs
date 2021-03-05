using AlderaminUtils;

namespace DefaultNamespace
{
    public class MapNode
    {
        public int px;
        public int py;
        public int cx;
        public int cy;
        private Grid2D<MapNode> _grid2D;

        public bool IsMoved;
        public bool IsFinalMove;
        public bool IsNeedMove;
        public bool IsNeedCombine;

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

        public MapNode(Grid2D<MapNode> grid2D, int x, int y,NodeType type)
        {
            IsMoved = false;
            IsFinalMove = false;
            IsNeedCombine = false;
            IsNeedMove = false;
            cx = px = x;
            cy = py = y;
            _grid2D = grid2D;
            _nodeType = type;
        }
        

        public void SetNodeType(NodeType type)
        {
            _nodeType = type;
            _grid2D.TriggerGridMapValueChangeEvent(cx, cy);
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