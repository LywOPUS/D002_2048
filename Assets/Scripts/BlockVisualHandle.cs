using AlderaminUtils;
using DefaultNamespace;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class BlockVisualHandle : MonoBehaviour
{
    [FormerlySerializedAs("BlockVisualNode")]
    public Transform blockVisualNode;

    [FormerlySerializedAs("_grid")] public Grid2D<MapNode> grid;
    private Map map;
    private Transform[,] VisualNodeArray;


    private void OnGridMapValueChangeHandle(object sender, Grid2D<MapNode>.GridChangeEventArgs eventArgs)
    {
        SetupVisualNode(eventArgs.x, eventArgs.y);
    }

    public void Setup(Map map)
    {
        this.map = map;
        grid = map.Grid;
        VisualNodeArray = new Transform[grid.GetWidth(), grid.GetHeight()];
        for (var x = 0; x < grid.GetWidth(); x++)
        for (var y = 0; y < grid.GetHeight(); y++)
        {
            var vBlock = OnCreateNewBlock(x, y);
        }

        grid.OnGridMapValueChangeEvent += OnGridMapValueChangeHandle;
        Debug.Log("setupVisualNode COMPLETE");
    }


    private void SetupVisualNode(int x, int y)
    {
        var vNode = VisualNodeArray[x, y];
        var node = grid.GetValue(x, y);
        var vNodeBackGround = vNode.GetChild(0);
        var vNodeText = vNode.GetChild(1).GetComponent<TextMeshPro>();
        if (node.GetNodeType() != MapNode.NodeType.Empty) vNodeBackGround.gameObject.SetActive(true);

        switch (node.GetNodeType())
        {
            case MapNode.NodeType.Empty:
                vNodeBackGround.gameObject.SetActive(false);
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


    private Transform OnCreateNewBlock(int x, int y)
    {
        //TODO: 需要一个对象池或者工厂？
        var blcok = Instantiate(blockVisualNode, grid.Cell2WorldPos(x, y), quaternion.identity);

        VisualNodeArray[x, y] = blcok;
        blcok.DOScale(new Vector3(5, 5), 1f)
            .From();
        SetupVisualNode(x, y);
        return blcok;
    }
}