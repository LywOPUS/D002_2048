using System;
using UnityEngine;

public class GamePlayHandler : MonoBehaviour
{
    public enum GAMESTATE
    {
        MainMenu,
        Play,
        GameEnd
    }

    [SerializeField] private GAMESTATE gamestate;

    private BlockVisualHandle BlockVisualHandle;
    public Map Map;


    private void Awake()
    {
        gamestate = GAMESTATE.MainMenu;
    }

    private void Start()
    {
        Map = new Map(4, 4);
        BlockVisualHandle = FindObjectOfType<BlockVisualHandle>();
    }

    private void Update()
    {
        switch (gamestate)
        {
            case GAMESTATE.MainMenu:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    BlockVisualHandle.Setup(Map);
                    gamestate = GAMESTATE.Play;
                }

                break;
            case GAMESTATE.Play:
                InputProcees();
                break;
            case GAMESTATE.GameEnd:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool IsRightMove()
    {
        return Input.GetKeyDown(KeyCode.D);
    }

    public bool IsLeftMove()
    {
        return Input.GetKeyDown(KeyCode.A);
    }

    public bool IsUpMove()
    {
        return Input.GetKeyDown(KeyCode.W);
    }

    public bool IsDownMove()
    {
        return Input.GetKeyDown(KeyCode.S);
    }

    private void InputProcees()
    {
        if (IsRightMove()) Map.Move(Map.MoveDirection.Right);

        if (IsLeftMove()) Map.Move(Map.MoveDirection.Left);

        if (IsUpMove()) Map.Move(Map.MoveDirection.Up);

        if (IsDownMove()) Map.Move(Map.MoveDirection.Down);

        if (Input.GetKeyDown(KeyCode.Z)) Map.Undo();
    }
}