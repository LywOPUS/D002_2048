using System;
using System.Collections.Generic;
using AlderaminUtils;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

// mapindex
//  [(0,1),(1,1)]   x " -> " ; y " ^ "
//  [(0,0),(1,0)]


//update view 
//map node change -> node
//
public class Map
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    private readonly int _mapHeight;

    private readonly int _mapSize;
    private readonly int _mapWidth;
    private readonly MapNode.NodeType EmptyType = MapNode.NodeType.Empty;
    private readonly Stack<MapNode.NodeType[,]> mapHistoryBuffer;
    public Action<int, int, int, int> OnMapNodeMove;

    public Map(int mapHeight, int mapWidth)
    {
        _mapHeight = mapHeight;
        _mapWidth = mapWidth;
        _mapSize = mapHeight * mapWidth;
        Grid = new Grid2D<MapNode>(mapWidth, mapHeight, 10, new Vector3(-20, -20, 0),
            (grid2D, x, y) => new MapNode(grid2D, x, y, EmptyType), false);
        mapHistoryBuffer = new Stack<MapNode.NodeType[,]>();

        #region 设置地图元素

        SetUpMap();

        #endregion 设置地图元素

        mapHistoryBuffer.Push(MapState());
    }

    public Grid2D<MapNode> Grid { get; }

    public void SetUpMap()
    {
        var count = 2;
        while (count >= 1)
        {
            var rix = Random.Range(0, _mapHeight);
            var riy = Random.Range(0, _mapWidth);
            if (Grid.GetValue(rix, riy).GetNodeType() == MapNode.NodeType.Empty)
            {
                count--;
                Grid.SetValue(rix, riy, new MapNode(Grid, rix, riy, MapNode.NodeType.Num2));
            }
        }
    }

    private void CreateNewNode()
    {
        var count = _mapSize;
        while (count != 0)
        {
            var rix = Random.Range(0, _mapHeight);
            var riy = Random.Range(0, _mapWidth);
            if (Grid.GetValue(rix, riy).GetNodeType() == MapNode.NodeType.Empty)
            {
                Grid.SetValue(rix, riy, new MapNode(Grid, rix, riy, MapNode.NodeType.Num2));
                break;
            }

            count--;
        }
    }


    public bool CanMove(MoveDirection moveDirection)
    {
        var movedir = (0, 0);
        switch (moveDirection)
        {
            case MoveDirection.Up:
                movedir = (0, 1);
                break;
            case MoveDirection.Down:
                movedir = (0, -1);
                break;
            case MoveDirection.Left:
                movedir = (-1, 0);
                break;
            case MoveDirection.Right:
                movedir = (1, 0);
                break;
        }

        for (var x = 0; x < _mapWidth; x++)
        {
            var dx = x + movedir.Item1;
            for (var y = 0; y < _mapHeight; y++)
            {
                var dy = y + movedir.Item2;
                if (dx >= 0 && dy >= 0 && dx < _mapWidth && dy < _mapHeight)
                {
                    var cur = Grid.GetValue(x, y);
                    var next = Grid.GetValue(dx, dy);
                    if (cur.GetNodeType() != EmptyType)
                    {
                        if (cur.GetNodeType() == next.GetNodeType()) return true;

                        if (next.GetNodeType() == EmptyType) return true;
                    }
                }
            }
        }

        return false;
    }

    public bool HaveEmpty()
    {
        for (var x = 0; x < _mapWidth; x++)
        for (var y = 0; y < _mapHeight; y++)
            if (Grid.GetValue(x, y).GetNodeType() == EmptyType)
                return true;

        return false;
    }

    public MapNode.NodeType[,] MapState()
    {
        var mapstate = new MapNode.NodeType[_mapWidth, _mapHeight];
        for (var x = 0; x < _mapWidth; x++)
        for (var y = 0; y < _mapHeight; y++)
            mapstate[x, y] = GetNodeType(x, y);

        return mapstate;
    }

    public MapNode.NodeType GetNodeType(int x, int y)
    {
        return Grid.GetValue(x, y).GetNodeType();
    }

    public void SetNodeType(int x, int y, MapNode.NodeType nodeType)
    {
        Grid.GetValue(x, y).SetNodeType(nodeType);
    }

    public void Swap(int x1, int y1, int x2, int y2, MapNode node1, MapNode node2)
    {
        //TODO:在移动结束后找个地方重制IsMoved状态
        if (node1.IsMoved == false) node1.IsMoved = true;

        node1.Move(x2, y2);
        if (node2.IsMoved == false) node2.IsMoved = true;

        node2.Move(x1, y1);
    }

    public void Combine(int fx, int fy, int dx, int dy, MapNode upNode)
    {
        upNode.Move(dx, dy);
        upNode.UpNode();
        Grid.SetValue(fx, fy, new MapNode(Grid, fx, fy, EmptyType));
    }

    public void Undo()
    {
        if (mapHistoryBuffer.Count == 0) return;

        var mapState = mapHistoryBuffer.Pop();
        for (var x = 0; x < _mapWidth; x++)
        for (var y = 0; y < _mapHeight; y++)
        {
            SetNodeType(x, y, mapState[x, y]);
            Grid.TriggerGridMapValueChangeEvent(x, y);
        }
    }


    #region 移动算法

    public void MoveUp()
    {
        UpRemoveBlank();
        for (var x = 0; x < _mapWidth; x++)
            //将一列中的相同元素合并
            //Element:                 index:       point:                  Target:
            //               0         3,           y     |                 4 
            //               2         2,           k     v     y           0 
            //               0         1,                       k           0
            //               2         0                                    0
        for (var y = _mapHeight - 1; y > 0; y--)
        {
            if (GetNodeType(x, y) != EmptyType)
                if (GetNodeType(x, y) == GetNodeType(x, y - 1))
                {
                    // SetNodeType(x, y, GetNodeType(x, y - 1) + 1);
                    // SetNodeType(x, y - 1, EmptyType);
                    var upNode = Grid.GetValue(x, y - 1);
                    Combine(x, y - 1, x, y, upNode);
                }

            UpRemoveBlank();
        } //列End
    }

    private void UpRemoveBlank()
    {
        for (var x = 0; x < _mapWidth; x++)
            //将一行中的空下移
            //Element:                 index:       point:                  Traget:
            //               0         3,           y     |                 2
            //               2         2,           k     v     y           2
            //               0         1,                       k           0
            //               2         0                                    0
        for (var y = _mapHeight - 1; y >= 0; y--)
        for (var k = y; k >= 0; k--)
            if (GetNodeType(x, y) == EmptyType && GetNodeType(x, k) != EmptyType)
            {
                // SetNodeType(x, y, GetNodeType(x, k));
                // SetNodeType(x, k, EmptyType);
                var node1 = Grid.GetValue(x, y);
                var node2 = Grid.GetValue(x, k);
                Swap(x, y, x, k, node1, node2);
            }
    }

    public void MoveDown()
    {
        DownRemoveBlank();
        for (var x = 0; x < _mapHeight; x++)
            //将一列中的空上移
            //Element:                 index:       point:
            //               0         3,            
            //               2         2,           
            //               0         1,           k
            //               2         0            y
        for (var y = 0; y < _mapHeight - 1; y++)
        {
            if (GetNodeType(x, y) != EmptyType)
                if (GetNodeType(x, y) == GetNodeType(x, y + 1))
                {
                    // SetNodeType(x, y, GetNodeType(x, y) + 1);
                    // SetNodeType(x, y + 1, EmptyType);
                    var upNode = Grid.GetValue(x, y + 1);
                    Combine(x, y + 1, x, y, upNode);
                }

            DownRemoveBlank();
        }
    }

    public void DownRemoveBlank()
    {
        for (var x = 0; x < _mapHeight; x++)
            //将一列中的空上移
            //Element:                 index:       point:
            //               0         3,            
            //               2         2,           
            //               0         1,           k
            //               2         0            y
        for (var y = 0; y < _mapHeight - 1; y++)
        for (var k = y; k <= _mapHeight - 1; k++)
            if (GetNodeType(x, y) == EmptyType && GetNodeType(x, k) != EmptyType)
            {
                // SetNodeType(x, y, GetNodeType(x, k));
                // SetNodeType(x, k, EmptyType);
                var node1 = Grid.GetValue(x, y);
                var node2 = Grid.GetValue(x, k);
                Swap(x, y, x, k, node1, node2);
            }
    }

    public void MoveLeft()
    {
        LeftRemoveBlank();
        for (var y = 0; y < _mapHeight; y++)
            //行
            // [2,0,0,2]
        for (var x = 0; x < _mapWidth - 1; x++)
        {
            if (GetNodeType(x, y) != EmptyType)
                if (GetNodeType(x, y) == GetNodeType(x + 1, y))
                {
                    // SetNodeType(x, y, GetNodeType(x + 1, y) + 1);
                    // SetNodeType(x + 1, y, EmptyType);
                    var upNode = Grid.GetValue(x + 1, y);
                    Combine(x + 1, y, x, y, upNode);
                }

            LeftRemoveBlank();
        }
    }

    public void LeftRemoveBlank()
    {
        for (var y = 0; y < _mapHeight; y++)
            //将一行中的空右移
            //point:     x k          point  ->
            //Element:  [2,0,0,2]     move   <-
            //index:    [0,1,2,3]     
        for (var x = 0; x < _mapWidth - 1; x++)
        for (var k = x; k <= _mapWidth - 1; k++)
            if (GetNodeType(x, y) == EmptyType && GetNodeType(k, y) != EmptyType)
            {
                // SetNodeType(x, y, GetNodeType(k, y));
                // SetNodeType(k, y, EmptyType);
                var node1 = Grid.GetValue(x, y);
                var node2 = Grid.GetValue(k, y);
                Swap(x, y, k, y, node1, node2);
            }
    }

    public void MoveRight()
    {
        RightRemoveBlank();
        for (var y = 0; y < _mapHeight; y++)
        for (var x = _mapWidth - 1; x > 0; x--)
        {
            if (GetNodeType(x, y) != EmptyType)
                if (GetNodeType(x, y) == GetNodeType(x - 1, y))
                {
                    // SetNodeType(x, y, GetNodeType(x - 1, y) + 1);
                    // SetNodeType(x - 1, y, EmptyType);
                    var upNode = Grid.GetValue(x - 1, y);
                    Combine(x - 1, y, x, y, upNode);
                }

            RightRemoveBlank();
        }
    }

    public void RightRemoveBlank()
    {
        for (var y = 0; y < _mapHeight; y++)
            //遍历一行中的元素，将空的左移
            //<1>
            // point:               k,x
            // element:        [2,2,0,4]
            // index:           0,1,2,3
            //<2>
            // point:     (1)        k,x        k x    (2)  k   x           
            // element:        [2,2,0,4]    [2,0,2,4]      [2,0,0,2]  [0,0,2,2]
            // index:           0,1,2,3
        for (var x = _mapWidth - 1; x >= 0; x--)
        for (var k = x; k >= 0; k--)
            if (GetNodeType(x, y) == EmptyType && GetNodeType(k, y) != EmptyType)
            {
                // SetNodeType(x, y, GetNodeType(k, y));
                // SetNodeType(k, y, EmptyType);
                var node1 = Grid.GetValue(x, y);
                var node2 = Grid.GetValue(k, y);
                Swap(x, y, k, y, node1, node2);
            }
    }

    public bool Move(MoveDirection direction)
    {
        // if (!(CanMove(MoveDirection.Up) && CanMove(MoveDirection.Down) && CanMove(MoveDirection.Right) &&
        //       CanMove(MoveDirection.Left))) return false;
        if (CanMove(direction))
        {
            mapHistoryBuffer.Push(MapState());
            switch (direction)
            {
                case MoveDirection.Up:
                    MoveUp();
                    break;
                case MoveDirection.Down:
                    MoveDown();
                    break;
                case MoveDirection.Left:
                    MoveLeft();
                    break;
                case MoveDirection.Right:
                    MoveRight();
                    break;
            }

            if (HaveEmpty()) CreateNewNode();
        }

        return true;
    }

    #endregion 移动算法
}