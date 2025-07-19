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
    public PlayerInputMessage() : base(NetworkMessageTypes.Client.Ingame.Input) { }

    int IInputSnapshot.InputSequence => InputSequence;
}
