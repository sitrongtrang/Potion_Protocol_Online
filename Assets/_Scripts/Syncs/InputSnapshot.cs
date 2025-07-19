using UnityEngine;

public class InputSnapshot
{
    public int InputSequence;
}

public class PlayerInputSnapshot : InputSnapshot
{
    public bool PickupPressed;
    public bool DropPressed;
    public bool TransferPressed;
    public bool SubmitPressed;
    public bool CombinePressed;

    public Vector2 MoveDir;

    public PlayerInputSnapshot() { }

    public PlayerInputSnapshot(PlayerInputSnapshot other)
    {
        MoveDir = other.MoveDir;
        PickupPressed = other.PickupPressed;
        DropPressed = other.DropPressed;
        CombinePressed = other.CombinePressed;
        TransferPressed = other.TransferPressed;
        SubmitPressed = other.SubmitPressed;
    }
}