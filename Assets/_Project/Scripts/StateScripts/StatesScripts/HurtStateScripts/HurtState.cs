using Demonics.Manager;
using UnityEngine;

public class HurtState : HurtParentState
{
    private HurtState _hurtState;
    private AirHurtState _airHurtState;
    private AirborneHurtState _airborneHurtState;
    private GrabbedState _grabbedState;

    protected override void Awake()
    {
        base.Awake();
        _hurtState = GetComponent<HurtState>();
        _airHurtState = GetComponent<AirHurtState>();
        _airborneHurtState = GetComponent<AirborneHurtState>();
        _grabbedState = GetComponent<GrabbedState>();
    }

    public override void Enter()
    {
        _player.OtherPlayer.UnfreezeComboTimer();
        _player.CheckFlip();
        _playerAnimator.Hurt();
        _hurtFrame = _hurtAttack.hitStun;
        if (!_skipEnter)
        {
            // GameObject effect = ObjectPoolingManager.Instance.Spawn(_hurtAttack.hurtEffect);
            // effect.transform.localPosition = _hurtAttack.hurtEffectPosition;
            base.Enter();
        }
    }

    public override bool ToIdleState()
    {
        _player.OtherPlayer.StopComboTimer();
        _stateMachine.ChangeState(_idleState);
        return true;
    }

    private void ToAttackState()
    {
        _player.OtherPlayer.StopComboTimer();
        _attackState.Initialize(InputEnum.Light, false, false);
        _stateMachine.ChangeState(_attackState);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (!_player.IsInHitstop())
        {
            if (DemonicsWorld.WaitFrames(ref _hurtFrame))
            {
                if (_brainController.ControllerInputName == ControllerTypeEnum.Cpu.ToString() && TrainingSettings.OnHit)
                {
                    ToAttackState();
                }
                else
                {
                    ToIdleState();
                }
            }
        }
    }

    public override bool ToHurtState(AttackSO attack)
    {
        if (attack.knockbackArc > 0 && !attack.causesSoftKnockdown)
        {
            _airHurtState.Initialize(attack);
            _stateMachine.ChangeState(_airHurtState);
        }
        else
        {
            _hurtState.Initialize(attack);
            _stateMachine.ChangeState(_hurtState);
        }
        return true;
    }

    public override bool ToAirborneHurtState(AttackSO attack)
    {
        _airborneHurtState.Initialize(attack);
        _stateMachine.ChangeState(_airborneHurtState);
        return true;
    }

    public override bool ToGrabbedState()
    {
        _stateMachine.ChangeState(_grabbedState);
        return true;
    }
}