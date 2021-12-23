using Demonics.Sounds;
using System;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
	[SerializeField] private Player _player = default;
	[SerializeField] private PlayerMovement _playerMovement = default;
	[SerializeField] private InputBuffer _inputBuffer = default;
	[SerializeField] private Audio _audio = default;
	[SerializeField] private TrainingMenu _trainingMenu = default;
	private Animator _animator;


	private void Awake()
	{
		_animator = GetComponent<Animator>();
	}

	public void UnlockMovement()
	{
		_player.IsAttacking = false;
		_playerMovement.SetLockMovement(false);
		_inputBuffer.CheckForInputBufferItem();
	}

	public void CreateEffectAnimationEvent(int isProjectile)
	{
		_player.CreateEffect(Convert.ToBoolean(isProjectile));
	}

	public void ResetTimeScale()
	{
		Time.timeScale = GameManager.Instance.GameSpeed;
	}

	public void ZeroGravity()
	{
		_playerMovement.ZeroGravity();
	}

	public void ResetGravity()
	{
		_playerMovement.ResetGravity();
	}

	public void PlayerFootstepAnimationEvent()
	{
		_audio.SoundGroup("Footsteps").PlayInRandom();
	}

	public void PlayerSoundAnimationEvent(string soundName)
	{
		_audio.Sound(soundName).Play();
	}

	public void StartFrameCount(AnimationEvent animationEvent)
	{
		int recovery = (int)(_animator.GetCurrentAnimatorStateInfo(0).length * -60);
		if (animationEvent.intParameter > 0)
		{
			recovery = -animationEvent.intParameter;
		}
		_trainingMenu.FramedataValue(_player.IsPlayerOne, (int)animationEvent.floatParameter, recovery);
	}
}
