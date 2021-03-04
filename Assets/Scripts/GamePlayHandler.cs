using UnityEngine;

public class GamePlayHandler : MonoBehaviour
{
    public Map Map;
    public bool IsRightMove() => Input.GetKeyDown(KeyCode.D);
    public bool IsLeftMove() => Input.GetKeyDown(KeyCode.A);
    public bool IsUpMove() => Input.GetKeyDown(KeyCode.W);
    public bool IsDownMove() => Input.GetKeyDown(KeyCode.S);
    
    private void Awake()
    {
        Map = new Map(4, 4);
        FindObjectOfType<BlockVisualHandle>().Setup(Map.Grid);
    }

    private void Update()
    {
        InputProcees();
    }

    private void InputProcees()
    {
        if(IsRightMove())
        {
            Map.Move(Map.MoveDirection.Right);
        }

        if (IsLeftMove())
        {
            Map.Move(Map.MoveDirection.Left);
        }

        if (IsUpMove())
        {
            Map.Move(Map.MoveDirection.Up);
        }

        if (IsDownMove())
        {
            Map.Move(Map.MoveDirection.Down);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Map.Undo();
        }
    }
}