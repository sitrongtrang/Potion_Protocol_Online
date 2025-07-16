using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction
{

    [SerializeField] private PlayerController _player;
    private PlayerInventory _playerInventory;
    private InputManager _inputManager;
    private InputAction[] _inputAction;

    
    private List<GameObject> _itemsInCollision = new List<GameObject>();

    public void Initialize(PlayerController player, InputManager inputManager)
    {
        _player = player;
        _inputManager = inputManager;

        _inputAction = new InputAction[GameConstants.MaxSlot] {
            _inputManager.controls.Player.ChooseSlot1,
            _inputManager.controls.Player.ChooseSlot2,
            _inputManager.controls.Player.ChooseSlot3,
            _inputManager.controls.Player.ChooseSlot4,
            _inputManager.controls.Player.ChooseSlot5
        };

        for (int i = 0; i < GameConstants.MaxSlot; i++)
        {
            int index = i;
            _inputAction[i].performed += ctx => _playerInventory.ChooseSlot(index);
        }

        _inputManager.controls.Player.Nextslot.performed += ctx => _playerInventory.NextSlot();
        _inputManager.controls.Player.Pickup.performed += ctx => OnPickupPerformed();
        _inputManager.controls.Player.Drop.performed += ctx => OnDropPerformed();
        _inputManager.controls.Player.Transfer.performed += ctx => OnTransferPerformed();
        _inputManager.controls.Player.Submit.performed += ctx => OnSubmitPerformed();
    }

    private void OnPickupPerformed()
    {
        if (_itemsInCollision.Count == 0)
        {
            Debug.LogWarning("No items in collision to pick up.");
            return;
        }

        int idx = _playerInventory.GetAddSlot();
        if (idx < 0 || idx >= GameConstants.MaxSlot)
        {
            Debug.LogWarning("No empty slot available to pick up the item.");
            return;
        }

        PlayerPickupInputMessage message = new PlayerPickupInputMessage
        {
            CurrentPosition = _player.transform.position,
            SelectedSlot = idx,
            PickupKeyDown = true
        };
        NetworkManager.Instance.SendMessage(message);
    }

    private void OnDropPerformed()
    {
        int idx = _playerInventory.GetRemoveSlot();
        if (idx < 0 || idx >= GameConstants.MaxSlot)
        {
            Debug.LogWarning("No item in slot to drop.");
            return;
        }

        PlayerDropInputMessage message = new PlayerDropInputMessage
        {
            CurrentPosition = _player.transform.position,
            SelectedSlot = idx,
            DropKeyDown = true
        };
        NetworkManager.Instance.SendMessage(message);
    }

    private void OnTransferPerformed()
    {
        int idx = _playerInventory.GetRemoveSlot();
        if (idx < 0 || idx >= GameConstants.MaxSlot)
        {
            Debug.LogWarning("No item in slot to transfer.");
            return;
        }

        PlayerTransferToStationInputMessage message = new PlayerTransferToStationInputMessage
        {
            CurrentPosition = _player.transform.position,
            SelectedSlot = idx,
            PutToStationKeyDown = true
        };
        NetworkManager.Instance.SendMessage(message);
    }

    private void OnSubmitPerformed()
    {
        int idx = _playerInventory.GetRemoveSlot();
        if (idx < 0 || idx >= GameConstants.MaxSlot)
        {
            Debug.LogWarning("No item in slot to submit.");
            return;
        }

        PlayerSubmitInputMessage message = new PlayerSubmitInputMessage
        {
            CurrentPosition = _player.transform.position,
            SelectedSlot = idx,
            SubmitKeyDown = true
        };
        NetworkManager.Instance.SendMessage(message);
    }

    public void OnCollide(Collider2D collision, bool isEntering)
    {
        string[] collideableTag = { "Item", "SubmissionPoint", "Station", "CraftPoint" };
        for (int i = 0; i < collideableTag.Length; i++)
        {
            if (collision.CompareTag(collideableTag[i]))
            {
                PlayerCollideMessage message = new PlayerCollideMessage
                {
                    PlayerId = _player.PlayerId,
                    Tag = collision.tag,
                    IsEntering = isEntering,
                    CollisionPosition = collision.transform.position
                };
                NetworkManager.Instance.SendMessage(message);
                break;
            }
        }
    }
}