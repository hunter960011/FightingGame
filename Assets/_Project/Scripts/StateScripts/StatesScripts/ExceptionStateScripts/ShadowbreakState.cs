using UnityEngine;

public class ShadowbreakState : State
{
	[SerializeField] private GameObject _shadowbreakPrefab = default;
	private FallState _fallState;
	private HurtState _hurtState;
	private AirborneHurtState _airborneHurtState;

	private void Awake()
	{
		_fallState = GetComponent<FallState>();
		_hurtState = GetComponent<HurtState>();
		_airborneHurtState = GetComponent<AirborneHurtState>();
	}

	public override void Enter()
	{
		base.Enter();
		_player.DecreaseArcana(1);
		_playerAnimator.OnCurrentAnimationFinished.AddListener(ToFallState);
		_playerAnimator.Shadowbreak();
		_audio.Sound("Shadowbreak").Play();
		CameraShake.Instance.Shake(0.5f, 0.1f);
		Transform shadowbreak = Instantiate(_shadowbreakPrefab, _playerAnimator.transform).transform;
		shadowbreak.position = new Vector2(transform.position.x, transform.position.y + 1.5f);
	}

	private void ToFallState()
	{
		_stateMachine.ChangeState(_fallState);
	}

	public override bool ToHurtState(AttackSO attack)
	{
		_hurtState.Initialize(attack);
		_stateMachine.ChangeState(_hurtState);
		return true;
	}

	public override bool ToAirborneHurtState(AttackSO attack)
	{
		_airborneHurtState.Initialize(attack);
		_stateMachine.ChangeState(_airborneHurtState);
		return true;
	}

	public override void UpdateLogic()
	{
		base.UpdateLogic();
		_rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
	}

	public override void Exit()
	{
		base.Exit();
		_player.OtherPlayer.StopComboTimer();
		_playerAnimator.OnCurrentAnimationFinished.RemoveAllListeners();
	}
}