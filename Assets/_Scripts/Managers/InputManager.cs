using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager
{
    public InputSystem_Actions controls { get; private set; }

    public InputManager()
    {
        controls = new InputSystem_Actions();
        controls.Enable();
    }
    public InputManager(InputActionAsset asset) // dùng asset từ file json
    {
        controls = new InputSystem_Actions();
        Object.Destroy(controls.asset); // xóa asset mặc định (cũ)
        controls = new InputSystem_Actions(); // tạo mới lại wrapper
        controls.asset.LoadBindingOverridesFromJson(asset.SaveBindingOverridesAsJson());
        controls.Enable();
    }

    public void OnEnable()
    {
        controls.Enable();
    }

    public void OnDisable()
    {
        controls.Disable();
    }

    public void OnDestroy()
    {
        controls.Dispose();
    }
}