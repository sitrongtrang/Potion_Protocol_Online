public class PlayerInputMessage : ClientMessage, IInputSnapshot
{
    public int InputSequence;

    public float MoveDirX;
    public float MoveDirY;
    public bool DashKeyDown;

    public bool AttackKeyDown;

    public bool PickupKeyDown;
    public bool DropKeyDown;
    public bool TransferKeyDown;
    public bool CraftKeyDown;
    public bool SubmitKeyDown;
    public int SelectedSlot;
    public PlayerInputMessage(PlayerInputSnapshot playerInputSnapshot) : base(NetworkMessageTypes.Client.Ingame.Input)
    {
        MoveDirX = playerInputSnapshot.MoveDir.x;
        MoveDirY = playerInputSnapshot.MoveDir.y;
        DashKeyDown = playerInputSnapshot.DashPressed;

        AttackKeyDown = playerInputSnapshot.AttackPressed;

        PickupKeyDown = playerInputSnapshot.PickupPressed;
        DropKeyDown = playerInputSnapshot.DropPressed;
        CraftKeyDown = playerInputSnapshot.CombinePressed;
        TransferKeyDown = playerInputSnapshot.TransferPressed;
        SubmitKeyDown = playerInputSnapshot.SubmitPressed;

    }

    int IInputSnapshot.InputSequence => InputSequence;
}
