using UnityEngine;

public class WakeUpState : State
{
	private IdleState _idleState;

	void Awake()
	{
		_idleState = GetComponent<IdleState>();
	}

	public override void Enter()
	{
		base.Enter();
		_playerAnimator.WakeUp();
		_playerAnimator.OnCurrentAnimationFinished.AddListener(ToIdleState);
	}

	private void ToIdleState()
	{
		if (_stateMachine.CurrentState == this)
		{
			_stateMachine.ChangeState(_idleState);
		}
	}

	public override void UpdatePhysics()
	{
		base.UpdatePhysics();
		_rigidbody.velocity = Vector2.zero;
	}

	public override void Exit()
	{
		base.Exit();
		_player.SetHurtbox(true);
	}
}
