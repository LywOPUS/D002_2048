using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AlderaminUtils;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

// mapindex
//  [(0,1),(1,1)]   x " -> " ; y " ^ "
//  [(0,0),(1,0)]
public class Map
{
    private Grid2D<MapNode> _grid2D;
    private int _mapHeight;
    private int _mapWidth;
    private MapNode.NodeType EmptyType = MapNode.NodeType.Empty;
    private Queue<MapNode.NodeType[,]> mapHistoryBuffer;
    private readonly int _mapSize;

    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public Map(int mapHeight, int mapWidth)
    {
        _mapHeight = mapHeight;
        _mapWidth = mapWidth;
        _mapSize = mapHeight * mapWidth;
        _grid2D = new Grid2D<MapNode>(mapWidth, mapHeight, 10, new Vector3(-20, -20, 0),
            (grid2D, x, y) => new MapNode(grid2D, x, y), true);

        #region 设置地图元素

        //TODO:正常情况取消注释 
//        SetUpMap();

        #region 测试左右

        //TODO:测试左右移动算法使用
        //[2,2,0,4]
        SetNodeType(0, 0, MapNode.NodeType.Num2);
        SetNodeType(1, 0, MapNode.NodeType.Num2);
        SetNodeType(2, 0, MapNode.NodeType.Empty);
        SetNodeType(3, 0, MapNode.NodeType.Num4);
        //[2,0,0,2]
        SetNodeType(0, 2, MapNode.NodeType.Num2);
        SetNodeType(1, 2, MapNode.NodeType.Empty);
        SetNodeType(2, 2, MapNode.NodeType.Empty);
        SetNodeType(3, 2, MapNode.NodeType.Num2);

        #endregion

        #endregion 设置地图元素
    }

    public void SetUpMap()
    {
        int count = 2;
        while (count >= 1)
        {
            Debug.Log(count);
            var rix = Random.Range(0, _mapHeight);
            var riy = Random.Range(0, _mapWidth);
            if (_grid2D.GetValue(rix, riy).GetNodeType() == MapNode.NodeType.Empty)
            {
                count--;
                _grid2D.GetValue(rix, riy).SetNodeType(MapNode.NodeType.Num2);
                Debug.Log($"准备中的{rix},{riy} = {_grid2D.GetValue(rix, riy).GetNodeType()}");
                Debug.Log("Change");
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
            if (_grid2D.GetValue(rix, riy).GetNodeType() == MapNode.NodeType.Empty)
            {
                _grid2D.GetValue(rix, riy).SetNodeType(MapNode.NodeType.Num2);
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

        for (int x = 0; x < _mapWidth; x++)
        {
            var dx = x + movedir.Item1;
            for (int y = 0; y < _mapHeight; y++)
            {
                var dy = y + movedir.Item2;
                if (dx >= 0 && dy >= 0 && dx < _mapWidth && dy < _mapHeight)
                {
                    var cur = _grid2D.GetValue(x, y);
                    var next = _grid2D.GetValue(dx, dy);
                    if (cur.GetNodeType() != EmptyType)
                    {
                        if (cur.GetNodeType() == next.GetNodeType())
                        {
                            return true;
                        }

                        if (next.GetNodeType() == EmptyType)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool HaveEmpty()
    {
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                if (_grid2D.GetValue(x, y).GetNodeType() == EmptyType)
                {
                    return true;
                }
            }
        }

        return false;
    }

    #region 移动算法

    public void MoveUp()
    {
        UpRemoveBlank();
        for (int x = 0; x < _mapWidth; x++)
        {
            //将一列中的相同元素合并
            //Element:                 index:       point:                  Target:
            //               0         3,           y     |                 4 
            //               2         2,           k     v     y           0 
            //               0         1,                       k           0
            //               2         0                                    0
            for (int y = _mapHeight - 1; y > 0; y--)
            {
                if (GetNodeType(x, y) != EmptyType)
                {
                    if (GetNodeType(x, y) == GetNodeType(x, y - 1))
                    {
                        SetNodeType(x, y, GetNodeType(x, y - 1) + 1);
                        SetNodeType(x, y - 1, EmptyType);
                    }
                }

                UpRemoveBlank();
            } //列End
        }
    }

    private void UpRemoveBlank()
    {
        for (int x = 0; x < _mapWidth; x++)
        {
            //将一行中的空下移
            //Element:                 index:       point:                  Traget:
            //               0         3,           y     |                 2
            //               2         2,           k     v     y           2
            //               0         1,                       k           0
            //               2         0                                    0
            StringBuilder str = new StringBuilder();
            for (int y = _mapHeight - 1; y >= 0; y--)
            {
                for (int k = y; k >= 0; k--)
                {
                    if (GetNodeType(x, y) == EmptyType && GetNodeType(x, k) != EmptyType)
                    {
                        SetNodeType(x, y, GetNodeType(x, k));
                        SetNodeType(x, k, EmptyType);
                    }

                    if (k == 0)
                    {
                        Debug.Log($"最后「{x}」:{GetNodeType(x, k)}");
                    }
                }

                str.Append($"[{GetNodeType(x, y)}]");
            }
        }
    }

    public void MoveDown()
    {
        DownRemoveBlank();
        for (int x = 0; x < _mapHeight; x++)
        {
            //将一列中的空上移
            //Element:                 index:       point:
            //               0         3,            
            //               2         2,           
            //               0         1,           k
            //               2         0            y
            StringBuilder str = new StringBuilder();
            for (int y = 0; y < _mapHeight - 1; y++)
            {
                if (GetNodeType(x, y) != EmptyType)
                {
                    if (GetNodeType(x, y) == GetNodeType(x, y + 1))
                    {
                        SetNodeType(x, y, GetNodeType(x, y) + 1);
                        SetNodeType(x, y + 1, EmptyType);
                    }
                }

                DownRemoveBlank();
            }
        }
    }

    public void DownRemoveBlank()
    {
        for (int x = 0; x < _mapHeight; x++)
        {
            //将一列中的空上移
            //Element:                 index:       point:
            //               0         3,            
            //               2         2,           
            //               0         1,           k
            //               2         0            y
            StringBuilder str = new StringBuilder();
            for (int y = 0; y < _mapHeight - 1; y++)
            {
                for (int k = y; k <= _mapHeight - 1; k++)
                {
                    if (GetNodeType(x, y) == EmptyType && GetNodeType(x, k) != EmptyType)
                    {
                        SetNodeType(x, y, GetNodeType(x, k));
                        SetNodeType(x, k, EmptyType);
                    }

                    if (k == 3)
                    {
                        Debug.Log($"最后「{x}」:{GetNodeType(x, k)}");
                    }
                }

                str.Append($"[{GetNodeType(x, y)}]");
            }
        }
    }

    public void MoveLeft()
    {
        LeftRemoveBlank();
        for (int y = 0; y < _mapHeight; y++)
        {
            //行
            // [2,0,0,2]
            for (int x = 0; x < _mapWidth - 1; x++)
            {
                if (GetNodeType(x, y) != EmptyType)
                {
                    if (GetNodeType(x, y) == GetNodeType(x + 1, y))
                    {
                        SetNodeType(x, y, GetNodeType(x + 1, y) + 1);
                        SetNodeType(x + 1, y, EmptyType);
                    }
                }

                LeftRemoveBlank();
            }
        }
    }

    public void LeftRemoveBlank()
    {
        for (int y = 0; y < _mapHeight; y++)
        {
            //将一行中的空右移
            //point:     x k          point  ->
            //Element:  [2,0,0,2]     move   <-
            //index:    [0,1,2,3]     
            StringBuilder str = new StringBuilder();
            for (int x = 0; x < _mapWidth - 1; x++)
            {
                for (int k = x; k <= _mapWidth - 1; k++)
                {
                    if (GetNodeType(x, y) == EmptyType && GetNodeType(k, y) != EmptyType)
                    {
                        SetNodeType(x, y, GetNodeType(k, y));
                        SetNodeType(k, y, EmptyType);
                    }

                    if (k == 3)
                    {
                        Debug.Log($"最后「{y}」:{GetNodeType(k, y)}");
                    }
                }

                str.Append($"[{GetNodeType(x, y)}]");
            }

            Debug.Log(str);
        }
    }

    public void MoveRight()
    {
        RightRemoveBlank();
        // RightRemoveBlank();
        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = _mapWidth - 1; x > 0; x--)
            {
                if (GetNodeType(x, y) != EmptyType)
                {
                    if (GetNodeType(x, y) == GetNodeType(x - 1, y))
                    {
                        SetNodeType(x, y, GetNodeType(x - 1, y) + 1);
                        SetNodeType(x - 1, y, EmptyType);
                    }
                }

                RightRemoveBlank();
            }
        }
    }

    public void RightRemoveBlank()
    {
        for (int y = 0; y < _mapHeight; y++)
        {
            //遍历一行中的元素，将空的左移
            //<1>
            // point:               k,x
            // element:        [2,2,0,4]
            // index:           0,1,2,3
            //<2>
            // point:     (1)        k,x        k x    (2)  k   x           
            // element:        [2,2,0,4]    [2,0,2,4]      [2,0,0,2]  [0,0,2,2]
            // index:           0,1,2,3
            for (int x = _mapWidth - 1; x >= 0; x--)
            {
                for (int k = x; k >= 0; k--)
                {
                    if (GetNodeType(x, y) == EmptyType && GetNodeType(k, y) != EmptyType)
                    {
                        SetNodeType(x, y, GetNodeType(k, y));
                        SetNodeType(k, y, EmptyType);
                    }
                }
            }
        }
    }

    public bool Move(MoveDirection direction)
    {
        // if (!(CanMove(MoveDirection.Up) && CanMove(MoveDirection.Down) && CanMove(MoveDirection.Right) &&
        //       CanMove(MoveDirection.Left))) return false;
        if (CanMove(direction))
        {
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

            if (HaveEmpty())
            {
                CreateNewNode();
            }
        }

        return true;
    }

    #endregion 移动算法

    public MapNode.NodeType[,] MapState()
    {
        return new MapNode.NodeType[0, 0];
    }

    public MapNode.NodeType GetNodeType(int x, int y)
    {
        return _grid2D.GetValue(x, y).GetNodeType();
    }

    public void SetNodeType(int x, int y, MapNode.NodeType nodeType)
    {
        _grid2D.GetValue(x, y).SetNodeType(nodeType);
    }
}