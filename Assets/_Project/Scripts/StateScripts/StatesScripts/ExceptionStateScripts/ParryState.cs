using UnityEngine;

public class ParryState : State
{
    [SerializeField] private GameObject _parryEffect = default;
    private IdleState _idleState;
    private HurtState _hurtState;
    private AirborneHurtState _airborneHurtState;
    private GrabbedState _grabbedState;
    private BlockState _blockState;
    private readonly int _parryKnockback = 2;
    private readonly int _hitstopOnParry = 12;
    private readonly int _knockbackDuration = 10;
    private bool _parried;

    private void Awake()
    {
        _idleState = GetComponent<IdleState>();
        _hurtState = GetComponent<HurtState>();
        _airborneHurtState = GetComponent<AirborneHurtState>();
        _grabbedState = GetComponent<GrabbedState>();
        _blockState = GetComponent<BlockState>();
    }

    public override void Enter()
    {
        base.Enter();
        _physics.Velocity = DemonicsVector2.Zero;
        _player.SetSpriteOrderPriority();
        _player.SetResultAttack(0, _player.playerStats.mParry);
        _player.parryConnectsEvent?.Invoke();
        _audio.Sound("ParryStart").Play();
        _player.CheckFlip();
        _playerAnimator.BlueFrenzy();
        _playerAnimator.OnCurrentAnimationFinished.AddListener(ToIdleState);
    }


    private new void ToIdleState()
    {
        _stateMachine.ChangeState(_idleState);
    }

    public override bool ToHurtState(AttackSO attack)
    {
        if (_player.Parrying)
        {
            Parry(attack);
            return true;
        }
        _hurtState.Initialize(attack);
        _stateMachine.ChangeState(_hurtState);
        return true;
    }

    public override bool ToAirborneHurtState(AttackSO attack)
    {
        if (_player.Parrying)
        {
            Parry(attack);
            return true;
        }
        _airborneHurtState.Initialize(attack);
        _stateMachine.ChangeState(_airborneHurtState);
        return true;
    }

    private void Parry(AttackSO attack)
    {
        _audio.Sound("Parry").Play();
        _player.HealthGain();
        _parried = true;
        GameManager.Instance.HitStop(_hitstopOnParry);
        GameObject effect = Instantiate(_parryEffect);
        effect.transform.localPosition = new Vector2(_player.transform.position.x + (_player.transform.localScale.x * 1.5f), _player.transform.position.y + 1.5f);
        if (!attack.isProjectile)
        {
            if (_player.OtherPlayerMovement.IsInCorner)
            {
                _playerMovement.Knockback(new Vector2(_parryKnockback, 0), _knockbackDuration, (int)(_player.OtherPlayer.transform.localScale.x));
            }
            else
            {
                _player.OtherPlayerMovement.Knockback(new Vector2(_parryKnockback, 0), _knockbackDuration, (int)(_player.transform.localScale.x));
            }
        }
    }

    public override bool ToBlockState(AttackSO attack)
    {
        if (_parried)
        {
            _blockState.Initialize(attack);
            _stateMachine.ChangeState(_blockState);
            return true;
        }
        return false;
    }

    public override bool ToParryState()
    {
        if (_parried)
        {
            _stateMachine.ChangeState(this);
            return true;
        }
        return false;
    }

    public override bool ToGrabbedState()
    {
        _stateMachine.ChangeState(_grabbedState);
        return true;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }

    public override void Exit()
    {
        base.Exit();
        _parried = false;
        _playerAnimator.OnCurrentAnimationFinished.RemoveAllListeners();
    }
}