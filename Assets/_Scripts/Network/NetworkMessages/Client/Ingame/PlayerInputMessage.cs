using System;
using Newtonsoft.Json;

[Flags]
public enum InputFlags
{
    None = 0,
    Dash = 1 << 0,
    Move = 1 << 1,
    Attack = 1 << 2,
    Pickup = 1 << 3,
    Drop = 1 << 4,
    Transfer = 1 << 5,
    Craft = 1 << 6,
    Submit = 1 << 7,
}

[Serializable]
public class PlayerInputMessage : ClientMessage, IInputSnapshot
{
    public int InputSequence;
    public int Flags;
    public float MoveDirX;
    public float MoveDirY;
    public int SelectedSlot;
    public PlayerInputMessage(PlayerInputSnapshot playerInputSnapshot) : base(NetworkMessageTypes.Client.Ingame.Input)
    {
        MoveDirX = playerInputSnapshot.MoveDir.x;
        MoveDirY = playerInputSnapshot.MoveDir.y;

        Flags = playerInputSnapshot.DashPressed ? Flags |= (int)InputFlags.Dash : Flags ;

        Flags = playerInputSnapshot.AttackPressed ? Flags |= (int)InputFlags.Attack : Flags ;

        Flags = playerInputSnapshot.PickupPressed ? Flags |= (int)InputFlags.Pickup : Flags ;
        Flags = playerInputSnapshot.DropPressed ? Flags |= (int)InputFlags.Drop : Flags ;
        Flags = playerInputSnapshot.TransferPressed ? Flags |= (int)InputFlags.Transfer : Flags ;
        Flags = playerInputSnapshot.CombinePressed ? Flags |= (int)InputFlags.Craft : Flags ;
        Flags = playerInputSnapshot.SubmitPressed ? Flags |= (int)InputFlags.Submit : Flags ;

    }

    int IInputSnapshot.InputSequence => InputSequence;
}
