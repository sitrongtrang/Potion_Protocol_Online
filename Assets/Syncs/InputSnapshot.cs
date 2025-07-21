using UnityEngine;

public class PlayerInputSnapshot
{
    public bool AttackPressed;
    public bool PickupPressed;
    public bool DropPressed;
    public bool TransferPressed;
    public bool SubmitPressed;
    public bool CombinePressed;
    public bool DashPressed;

    public Vector2 MoveDir;

    public PlayerInputSnapshot() { }

    public PlayerInputSnapshot(PlayerInputSnapshot other)
    {
        MoveDir = other.MoveDir;
        AttackPressed = other.AttackPressed;
        PickupPressed = other.PickupPressed;
        DropPressed = other.DropPressed;
        CombinePressed = other.CombinePressed;
        TransferPressed = other.TransferPressed;
        SubmitPressed = other.SubmitPressed;
        DashPressed = other.DashPressed;
    }
}