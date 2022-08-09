using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using static UnityEngine.InputSystem.InputAction;

public class InputManager : MonoBehaviour
{
	[HideInInspector] public UnityEvent OnInputChange;
	[HideInInspector] public UnityEvent OnDeviceChange;
	[SerializeField] private PlayerInput _playerInput;

	public PromptsInput CurrentPrompts { private get; set; }
	public Vector2 NavigationInput { get; private set; }
	public string InputScheme { get { return _playerInput.currentControlScheme; } set { } }


	public void InputChange(PlayerInput playerInput)
	{
		OnInputChange?.Invoke();
	}

	public void DeviceRegained(PlayerInput playerInput)
	{
		OnDeviceChange?.Invoke();
	}
	public void DeviceLost(PlayerInput playerInput)
	{
		OnDeviceChange?.Invoke();
	}

	public void Navigation(CallbackContext callbackContext)
	{
		NavigationInput = callbackContext.ReadValue<Vector2>();
	}

	public void Confirm(CallbackContext callbackContext)
	{
		if (callbackContext.performed)
		{
			CurrentPrompts?.OnConfirm?.Invoke();
		}
	}

	public void Back(CallbackContext callbackContext)
	{
		if (callbackContext.performed)
		{
			CurrentPrompts?.OnBack?.Invoke();
		}
	}

	public void Stage(CallbackContext callbackContext)
	{
		if (callbackContext.performed)
		{
			CurrentPrompts?.OnStage?.Invoke();
		}
	}

	public void Coop(CallbackContext callbackContext)
	{
		if (callbackContext.performed)
		{
			CurrentPrompts?.OnCoop?.Invoke();
		}
	}

	public void Controls(CallbackContext callbackContext)
	{
		if (callbackContext.performed)
		{
			CurrentPrompts?.OnControls?.Invoke();
		}
	}
}
