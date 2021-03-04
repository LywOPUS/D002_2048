using System.Collections.Generic;
using AlderaminUtils;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using DG.Tweening;
public class BlockVisualHandle : MonoBehaviour
{
    public Transform BlockVisualNode;
    public Grid2D<MapNode> _grid;
    public List<Transform> visualNodeList;
    public Transform[,] visualNodeArray;
    public List<TextMeshPro> TextColor;

    void OnGridMapValueChangeHandle(object sender, Grid2D<MapNode>.GridChangeEventArgs eventArgs)
    {
        SetupVisualNode(eventArgs.x, eventArgs.y);
    }

    public void Setup(Grid2D<MapNode> grid)
    {
        _grid = grid;
        visualNodeArray = new Transform[_grid.GetWidth(), _grid.GetHeight()];
        for (int x = 0; x < _grid.GetWidth(); x++)
        {
            for (int y = 0; y < _grid.GetHeight(); y++)
            {
                var v = Instantiate(BlockVisualNode, _grid.Cell2WorldPos(x, y), Quaternion.identity);
                Debug.Log(v.position);
                visualNodeList.Add(v);
                visualNodeArray[x, y] = v;
            }
        }

        UpdateVisual(_grid);
        _grid.OnGridMapValueChangeEvent += OnGridMapValueChangeHandle;
        Debug.Log("setupVisualNode COMPLETE");
    }

    void SetupVisualNode(int x, int y)
    {
        var vNode = visualNodeArray[x, y];
        var node = _grid.GetValue(x, y);
        var vNodeText = vNode.GetChild(1).GetComponent<TextMeshPro>();
        switch (node.GetNodeType())
        {
            case MapNode.NodeType.Empty:
                vNodeText.text = " ";
                break;
            case MapNode.NodeType.Num2:
                vNodeText.text = "2";
                break;
            case MapNode.NodeType.Num4:
                vNodeText.text = "4";
                break;
            case MapNode.NodeType.Num16:
                vNodeText.text = "16";
                break;
            case MapNode.NodeType.Num32:
                vNodeText.text = "32";
                break;
            case MapNode.NodeType.Num64:
                vNodeText.text = "64";
                break;
            case MapNode.NodeType.Num128:
                vNodeText.text = "128";
                break;
            case MapNode.NodeType.Num256:
                vNodeText.text = "256";
                break;
            case MapNode.NodeType.Num512:
                vNodeText.text = "512";
                break;
            case MapNode.NodeType.Num1024:
                vNodeText.text = "1024";
                break;
            case MapNode.NodeType.Num2048:
                vNodeText.text = "2048";
                break;
        }
    }

    void UpdateVisual(Grid2D<MapNode> grid2D)
    {
        for (int x = 0; x < grid2D.GetWidth(); x++)
        {
            for (int y = 0; y < grid2D.GetHeight(); y++)
            {
                SetupVisualNode(x, y);
            }
        }
    }
}