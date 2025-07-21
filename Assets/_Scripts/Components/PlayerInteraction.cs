using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction
{

    [SerializeField] private PlayerController _player;
    private PlayerInventory _playerInventory;
    private InputManager _inputManager;
    private List<Vector2> _itemsInCollision = new();
    private Vector2 _nearbyStation;
    private Vector2 _nearbySubmissionPoint;
    private Vector2 _nearbyCraftPoint;

    public void Initialize(PlayerController player, InputManager inputManager)
    {
        _player = player;
        _inputManager = inputManager;
        _playerInventory = player.Inventory;

        _inputManager.controls.Player.Pickup.performed += ctx => OnPickupPerformed();
        _inputManager.controls.Player.Drop.performed += ctx => OnDropPerformed();
        _inputManager.controls.Player.Transfer.performed += ctx => OnTransferPerformed();
        _inputManager.controls.Player.Submit.performed += ctx => OnSubmitPerformed();
        _inputManager.controls.Player.Combine.performed += ctx => OnCraftPerformed();

        _itemsInCollision.Clear();
        _nearbyStation = Vector2.positiveInfinity;
        _nearbySubmissionPoint = Vector2.positiveInfinity;
        _nearbyCraftPoint = Vector2.positiveInfinity;

        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }

    private void HandleNetworkMessage(ServerMessage message)
    {
        
    }

    private void OnPickupPerformed()
    {
        if (_itemsInCollision.Count == 0)
        {
            Debug.LogWarning("No items nearby to pick up.");
            return;
        }

        int idx = _playerInventory.GetAddSlot();
        if (idx < 0 || idx >= GameConstants.MaxSlot)
        {
            Debug.LogWarning("No empty slot available to pick up the item.");
            return;
        }

        // PlayerPickupInputMessage message = new PlayerPickupInputMessage
        // {
        //     CurrentPosition = _player.transform.position,
        //     SelectedSlot = idx,
        //     PickupKeyDown = true
        // };
        // NetworkManager.Instance.SendMessage(message);
    }

    private void OnDropPerformed()
    {
        int idx = _playerInventory.GetRemoveSlot();
        if (idx < 0 || idx >= GameConstants.MaxSlot)
        {
            Debug.LogWarning("No item in slot to drop.");
            return;
        }

        // PlayerDropInputMessage message = new PlayerDropInputMessage
        // {
        //     CurrentPositionX = _player.transform.position.x,
        //     CurrentPositionY = _player.transform.position.y,
        //     SelectedSlot = idx,
        //     DropKeyDown = true
        // };
        // NetworkManager.Instance.SendMessage(message);
    }

    private void OnTransferPerformed()
    {
        if (_nearbyStation == Vector2.positiveInfinity)
        {
            Debug.LogWarning("No stations nearby to transfer item.");
            return;
        }

        int idx = _playerInventory.GetRemoveSlot();
        if (idx < 0 || idx >= GameConstants.MaxSlot)
        {
            Debug.LogWarning("No item in slot to transfer.");
            return;
        }

        // PlayerTransferToStationInputMessage message = new PlayerTransferToStationInputMessage
        // {
        //     CurrentPositionX = _player.transform.position.x,
        //     CurrentPositionY = _player.transform.position.y,
        //     SelectedSlot = idx,
        //     PutToStationKeyDown = true
        // };
        // NetworkManager.Instance.SendMessage(message);
    }

    private void OnSubmitPerformed()
    {
        if (_nearbySubmissionPoint == Vector2.positiveInfinity)
        {
            Debug.LogWarning("No submission point nearby to submit item.");
            return;
        }

        int idx = _playerInventory.GetRemoveSlot();
        if (idx < 0 || idx >= GameConstants.MaxSlot)
        {
            Debug.LogWarning("No item in slot to submit.");
            return;
        }

        // PlayerSubmitInputMessage message = new PlayerSubmitInputMessage
        // {
        //     CurrentPositionX = _player.transform.position.x,
        //     CurrentPositionY = _player.transform.position.y,
        //     SelectedSlot = idx,
        //     SubmitKeyDown = true
        // };
        // NetworkManager.Instance.SendMessage(message);
    }

    private void OnCraftPerformed()
    {
        if (_nearbyCraftPoint == Vector2.positiveInfinity)
        {
            Debug.LogWarning("No craft point nearby to craft item.");
            return;
        }

        // PlayerCraftInputMessage message = new PlayerCraftInputMessage
        // {
        //     CurrentPositionX = _player.transform.position.x,
        //     CurrentPositionY = _player.transform.position.y,
        //     CraftKeyDown = true
        // };
        // NetworkManager.Instance.SendMessage(message);
    }

    public void OnCollide(Collider2D collision, bool isEntering)
    {
        string[] collideableTag = { "Item", "SubmissionPoint", "Station", "CraftPoint" };
        for (int i = 0; i < collideableTag.Length; i++)
        {
            if (collision.CompareTag(collideableTag[i]))
            {
                // PlayerCollideInputMessage message = new PlayerCollideInputMessage
                // {
                //     Tag = collision.tag,
                //     IsEntering = isEntering,
                //     CurrentPositionX = _player.transform.position.x,
                //     CurrentPositionY = _player.transform.position.y,
                //     CollidePositionX = collision.transform.position.x,
                //     CollidePositionY = collision.transform.position.y
                // };
                // NetworkManager.Instance.SendMessage(message);
                break;
            }
        }
    }

    private void OnCollideItem(Vector2 itemPosition, bool isEntering)
    {
        if (isEntering)
        {
            if (_itemsInCollision.Contains(itemPosition))
            {
                Debug.LogWarning("Item already in collision list.");
                return;
            }
            _itemsInCollision.Add(itemPosition);
        }
        else
        {
            _itemsInCollision.Remove(itemPosition);
        }
    }

    private void OnCollideStation(Vector2 stationPosition, bool isEntering)
    {
        if (isEntering)
        {
            _nearbyStation = stationPosition;
        }
        else
        {
            _nearbyStation = Vector2.positiveInfinity;
        }
    }

    private void OnCollideSubmissionPoint(Vector2 submissionPointPosition, bool isEntering)
    {
        if (isEntering)
        {
            _nearbySubmissionPoint = submissionPointPosition;
        }
        else
        {
            _nearbySubmissionPoint = Vector2.positiveInfinity;
        }
    }

    private void OnCollideCraftPoint(Vector2 craftPointPosition, bool isEntering)
    {
        if (isEntering)
        {
            _nearbyCraftPoint = craftPointPosition;
        }
        else
        {
            _nearbyCraftPoint = Vector2.positiveInfinity;
        }
    }

    private void HandlePlayerCollide(ServerMessage message)
    {
        // var collideMessage = (PlayerCollideMessage)message;
        // if (collideMessage == null) return null;

        // Vector2 collidePosition = Vector2.zero;
        // switch (collideMessage.Tag)
        // {
        //     case "Item":
        //         collidePosition = new Vector2(collideMessage.CollidePositionX, collideMessage.CollidePositionY);
        //         OnCollideItem(collidePosition, collideMessage.IsEntering);
        //         break;
        //     case "Station":
        //         collidePosition = new Vector2(collideMessage.CollidePositionX, collideMessage.CollidePositionY);
        //         OnCollideStation(collidePosition, collideMessage.IsEntering);
        //         break;
        //     case "SubmissionPoint":
        //         collidePosition = new Vector2(collideMessage.CollidePositionX, collideMessage.CollidePositionY);
        //         OnCollideSubmissionPoint(collidePosition, collideMessage.IsEntering);
        //         break;
        //     case "CraftPoint":
        //         collidePosition = new Vector2(collideMessage.CollidePositionX, collideMessage.CollidePositionY);
        //         OnCollideCraftPoint(collidePosition, collideMessage.IsEntering);
        //         break;
        // }

        // return null;
    }
}