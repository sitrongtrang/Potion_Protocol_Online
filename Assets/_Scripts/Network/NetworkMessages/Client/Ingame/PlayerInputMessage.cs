using System;
using Newtonsoft.Json;

[Flags]
public enum InputFlags
{
    None = 0,
    Dash = 1 << 0,
    Attack = 1 << 1,
    Pickup = 1 << 2,
    Drop = 1 << 3,
    Transfer = 1 << 4,
    Craft = 1 << 5,
    Submit = 1 << 6,
}

[Serializable]
public class PlayerInputMessage : ClientMessage, IInputSnapshot
{
    [JsonProperty("inputSequence")]
    public int InputSequence;

    [JsonProperty("moveDirX")]
    public float MoveDirX;
    [JsonProperty("moveDirY")]
    public float MoveDirY;
    [JsonProperty("inputFlags")]
    public int Flags;
    public int SelectedSlot;

    public PlayerInputMessage() : base(NetworkMessageTypes.Client.Ingame.Input) { }
    // public PlayerInputMessage(PlayerInputSnapshot playerInputSnapshot) : base(NetworkMessageTypes.Client.Ingame.Input)
    // {
    //     MoveDirX = playerInputSnapshot.MoveDir.x;
    //     MoveDirY = playerInputSnapshot.MoveDir.y;

    //     Flags = playerInputSnapshot.DashPressed ? Flags |= (int)InputFlags.Dash : Flags ;

    //     Flags = playerInputSnapshot.AttackPressed ? Flags |= (int)InputFlags.Attack : Flags ;

    //     Flags = playerInputSnapshot.PickupPressed ? Flags |= (int)InputFlags.Pickup : Flags ;
    //     Flags = playerInputSnapshot.DropPressed ? Flags |= (int)InputFlags.Drop : Flags ;
    //     Flags = playerInputSnapshot.TransferPressed ? Flags |= (int)InputFlags.Transfer : Flags ;
    //     Flags = playerInputSnapshot.CombinePressed ? Flags |= (int)InputFlags.Craft : Flags ;
    //     Flags = playerInputSnapshot.SubmitPressed ? Flags |= (int)InputFlags.Submit : Flags ;

    // }

    int IInputSnapshot.InputSequence => InputSequence;
}
