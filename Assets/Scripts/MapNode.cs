using AlderaminUtils;

namespace DefaultNamespace
{
    public class MapNode
    {
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

        private readonly Grid2D<MapNode> _grid2D;

        private NodeType _nodeType;
        public int cx;
        public int cy;
        public bool IsFinalMove;

        public bool IsMoved;
        public bool IsNeedCombine;
        public bool IsNeedMove;
        public int px;
        public int py;

        public MapNode(Grid2D<MapNode> grid2D, int x, int y, NodeType type)
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
            // _grid2D.TriggerGridMapValueChangeEvent(cx, cy);
        }

        public NodeType GetNodeType()
        {
            return _nodeType;
        }


        public void Move(int tx, int ty)
        {
            px = cx;
            py = cy;
            cx = tx;
            cy = ty;
            //移动mapnode在Grid中的位置
            _grid2D.SetValue(tx, ty, this);
        }

        public void RestNode()
        {
            _nodeType = NodeType.Empty;
        }

        public bool UpNode()
        {
            _nodeType += 1;
            _grid2D.SetValue(cx, cy, this);
            return _nodeType == NodeType.Num2048;
        }

        public override string ToString()
        {
            return _nodeType.ToString();
        }
    }
}