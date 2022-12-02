using Demonics.Manager;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour, IHurtboxResponder, IHitboxResponder, IHitstop
{
    [SerializeField] private PlayerStateManager _playerStateManager = default;
    [SerializeField] private PlayerAnimator _playerAnimator = default;
    [SerializeField] private Assist _assist = default;
    [SerializeField] private Pushbox _groundPushbox = default;
    [SerializeField] private Transform _hurtbox = default;
    [SerializeField] protected Transform _effectsParent = default;
    [SerializeField] private Transform _cameraPoint = default;
    [SerializeField] private Transform _keepFlip = default;
    [SerializeField] private GameObject[] _playerIcons = default;
    protected PlayerUI _playerUI;
    private PlayerMovement _playerMovement;
    [HideInInspector] public PlayerStatsSO playerStats;
    protected PlayerComboSystem _playerComboSystem;
    private BrainController _controller;
    private bool _comboTimerPaused;
    private int _comboTimerFrames;
    private int _comboTimerWaitFrames;
    private Color _comboTimerColor;
    private Coroutine _shakeContactCoroutine;
    private readonly DemonicsFloat _damageDecay = (DemonicsFloat)0.97f;
    private readonly DemonicsFloat _whiteHealthDivider = (DemonicsFloat)1.4f;
    [HideInInspector] public UnityEvent hitstopEvent;
    [HideInInspector] public UnityEvent hitConnectsEvent;
    [HideInInspector] public UnityEvent parryConnectsEvent;

    public PlayerStateManager PlayerStateManager { get { return _playerStateManager; } private set { } }
    public PlayerStateManager OtherPlayerStateManager { get; private set; }
    public Player OtherPlayer { get; private set; }
    public PlayerMovement OtherPlayerMovement { get; private set; }
    public PlayerUI OtherPlayerUI { get; private set; }
    public PlayerStatsSO PlayerStats { get { return playerStats; } set { } }
    public PlayerUI PlayerUI { get { return _playerUI; } private set { } }
    public AttackSO CurrentAttack { get; set; }
    public ResultAttack ResultAttack { get; set; }
    public Transform CameraPoint { get { return _cameraPoint; } private set { } }
    public bool CanAirArcana { get; set; }
    public int Health { get; set; }
    public int HealthRecoverable { get; set; }
    public int Lives { get; set; } = 2;
    public bool IsPlayerOne { get; set; }
    public DemonicsFloat AssistGauge { get; set; } = (DemonicsFloat)1;
    public DemonicsFloat ArcanaGauge { get; set; }
    public DemonicsVector2 GrabPoint { get; set; }
    public int ArcaneSlowdown { get; set; } = 6;
    public bool CanShadowbreak { get; set; } = true;
    public bool BlockingLow { get; set; }
    public bool BlockingHigh { get; set; }
    public bool BlockingMiddair { get; set; }
    public bool Parrying { get; set; }
    public bool CanSkipAttack { get; set; }
    public bool Invincible { get; set; }
    public bool Invisible { get; set; }
    public bool LockChain { get; set; }
    public bool HasJuggleForce { get; set; }

    void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerComboSystem = GetComponent<PlayerComboSystem>();
        ResultAttack = new ResultAttack();
    }

    public void SetController()
    {
        _controller = GetComponent<BrainController>();
    }

    public void SetAssist(AssistStatsSO assistStats)
    {
        _assist.SetAssist(assistStats);
        _playerUI.SetAssistName(assistStats.name[0].ToString());
    }

    void Start()
    {
        InitializeStats();
    }

    public void SetPlayerUI(PlayerUI playerUI)
    {
        _playerUI = playerUI;
    }

    public void SetOtherPlayer(Player otherPlayer)
    {
        OtherPlayer = otherPlayer;
        OtherPlayerMovement = otherPlayer.GetComponent<PlayerMovement>();
        OtherPlayerUI = otherPlayer.PlayerUI;
        OtherPlayerStateManager = otherPlayer.PlayerStateManager;
    }

    public void ResetPlayer(Vector2 resetPosition)
    {
        RecallAssist();
        _playerMovement.StopKnockback();
        _playerMovement.Physics.ResetSkipWall();
        _playerMovement.Physics.Position = new DemonicsVector2((DemonicsFloat)resetPosition.x, (DemonicsFloat)resetPosition.y);
        _playerMovement.Physics.Velocity = DemonicsVector2.Zero;
        _playerStateManager.ResetToInitialState();
        SetInvinsible(false);
        transform.rotation = Quaternion.identity;
        _effectsParent.gameObject.SetActive(true);
        SetHurtbox(true);
        AssistGauge = (DemonicsFloat)1;
        transform.SetParent(null);
        if (!GameManager.Instance.InfiniteArcana)
        {
            ArcanaGauge = (DemonicsFloat)0;
        }
        _playerMovement.Physics.EnableGravity(true);
        StopAllCoroutines();
        StopComboTimer();
        _playerMovement.StopAllCoroutines();
        _playerAnimator.OnCurrentAnimationFinished.RemoveAllListeners();
        _playerUI.SetArcana((float)ArcanaGauge);
        _playerUI.SetAssist((float)AssistGauge);
        _playerUI.ResetHealthDamaged();
        InitializeStats();
        _playerUI.ShowPlayerIcon();
        hitstopEvent.RemoveAllListeners();
        LockChain = false;
    }

    public void ResetLives()
    {
        Lives = 2;
        _playerUI.ResetLives();
    }

    public AttackSO shadowbreakKnockback()
    {
        GameManager.Instance.AddHitstop(this);
        return new AttackSO() { hitStun = 30, hitstop = 5, knockbackForce = new Vector2(0.1f, 1), knockbackDuration = 5, hurtEffectPosition = new Vector2(0, (float)_playerMovement.Physics.Position.y + 1) };
    }

    public void MaxHealthStats()
    {
        _playerUI.ResetHealthDamaged();
        Health = playerStats.maxHealth;
        HealthRecoverable = playerStats.maxHealth;
        _playerUI.MaxHealth(Health);
        _playerUI.CheckDemonLimit(Health);
    }

    private void InitializeStats()
    {
        _playerUI.InitializeUI(playerStats, _controller, _playerIcons);
        Health = playerStats.maxHealth;
        HealthRecoverable = playerStats.maxHealth;
        _playerUI.SetHealth(Health);
    }

    void FixedUpdate()
    {
        ArcanaCharge();
        AssistCharge();
        ComboTimer();
    }

    private void AssistCharge()
    {
        if (AssistGauge < (DemonicsFloat)1.0f && !_assist.IsOnScreen && CanShadowbreak && GameManager.Instance.HasGameStarted)
        {
            AssistGauge += (DemonicsFloat)(Time.deltaTime / (10.0f - _assist.AssistStats.assistRecharge));
            if (GameManager.Instance.InfiniteAssist)
            {
                AssistGauge = (DemonicsFloat)1.0f;
            }
            _playerUI.SetAssist((float)AssistGauge);
        }
    }

    private void ArcanaCharge()
    {
        if (ArcanaGauge < (DemonicsFloat)playerStats.Arcana && GameManager.Instance.HasGameStarted)
        {
            ArcanaGauge += (DemonicsFloat)(Time.deltaTime / (ArcaneSlowdown - playerStats.arcanaRecharge));
            if (GameManager.Instance.InfiniteArcana)
            {
                ArcanaGauge = (DemonicsFloat)playerStats.Arcana;
            }
            _playerUI.SetArcana((float)ArcanaGauge);
        }
    }

    public void ArcanaGain(DemonicsFloat arcana)
    {
        if (ArcanaGauge < (DemonicsFloat)playerStats.Arcana && GameManager.Instance.HasGameStarted)
        {
            ArcanaGauge += arcana;
            _playerUI.SetArcana((float)ArcanaGauge);
        }
    }

    public void HealthGain()
    {
        Health = HealthRecoverable;
        _playerUI.UpdateHealth();
        _playerUI.CheckDemonLimit(Health);
    }

    public void SetHealth(int value, bool noRecoverable = false)
    {
        Health -= value;
        if (noRecoverable)
        {
            HealthRecoverable -= value;
        }
        else
        {
            HealthRecoverable -= (int)((DemonicsFloat)value / _whiteHealthDivider);
        }
        _playerUI.SetHealth(Health);
        _playerUI.SetRecoverableHealth(HealthRecoverable);
    }

    public void StartShakeContact()
    {
        _shakeContactCoroutine = StartCoroutine(ShakeContactCoroutine());
    }

    private void StopShakeCoroutine()
    {
        if (_shakeContactCoroutine != null)
        {
            _playerAnimator.transform.localPosition = Vector2.zero;
            StopCoroutine(_shakeContactCoroutine);
        }
    }

    IEnumerator ShakeContactCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.075f);
            _playerAnimator.transform.localPosition = new Vector2(_playerAnimator.transform.localPosition.x - 0.03f, _playerAnimator.transform.localPosition.y);
            yield return new WaitForSeconds(0.075f);
            _playerAnimator.transform.localPosition = Vector2.zero;
        }
    }

    public void CheckFlip()
    {
        if (OtherPlayerMovement.Physics.Position.x > _playerMovement.Physics.Position.x)
        {
            Flip(1);
        }
        else if (OtherPlayerMovement.Physics.Position.x < _playerMovement.Physics.Position.x)
        {
            Flip(-1);
        }
    }

    public void Flip(int xDirection)
    {
        transform.localScale = new Vector2(xDirection, transform.localScale.y);
        _keepFlip.localScale = transform.localScale;
    }

    public bool HasRecoverableHealth()
    {
        float remainingRecoverableHealth = HealthRecoverable - Health;
        if (remainingRecoverableHealth > 0)
        {
            HealthRecoverable = Health;
            _playerUI.SetRecoverableHealth(HealthRecoverable);
            return true;
        }
        return false;
    }

    public bool CheckRecoverableHealth()
    {
        float remainingRecoverableHealth = HealthRecoverable - Health;
        if (remainingRecoverableHealth > 0)
        {
            return true;
        }
        return false;
    }

    public bool AssistAction()
    {
        if (AssistGauge >= (DemonicsFloat)0.5f && GameManager.Instance.HasGameStarted && !_assist.IsOnScreen)
        {
            _assist.Attack();
            DecreaseArcana((DemonicsFloat)0.5f);
            return true;
        }
        return false;
    }

    public DemonicsFloat DemonLimitMultiplier()
    {
        if (Health < 3000)
        {
            return (DemonicsFloat)1.2;
        }
        return (DemonicsFloat)1;
    }

    public void DecreaseArcana(DemonicsFloat value)
    {
        AssistGauge -= value;
        _playerUI.SetAssist((float)AssistGauge);
    }

    public void StartComboTimer(ComboTimerStarterEnum comboTimerStarter)
    {
        _playerUI.SetComboTimerActive(true);
        _comboTimerFrames = 0;
        _comboTimerWaitFrames = ComboTimerStarterTypes.GetComboTimerStarterValue(comboTimerStarter);
        _comboTimerColor = ComboTimerStarterTypes.GetComboTimerStarterColor(comboTimerStarter);
        _playerUI.SetComboTimer((DemonicsFloat)1, _comboTimerColor);
    }

    private void ComboTimer()
    {
        if (_comboTimerWaitFrames > 0 && !_comboTimerPaused)
        {
            DemonicsFloat value = DemonicsFloat.Lerp((DemonicsFloat)1, (DemonicsFloat)0, (DemonicsFloat)_comboTimerFrames / (DemonicsFloat)_comboTimerWaitFrames);
            _playerUI.SetComboTimer(value, _comboTimerColor);
            _comboTimerFrames++;
            if (_comboTimerFrames == _comboTimerWaitFrames)
            {
                OtherPlayer._playerStateManager.TryToIdleState();
                _playerUI.SetComboTimerActive(false);
            }
        }
    }

    public void StopComboTimer()
    {
        _comboTimerWaitFrames = 0;
        _playerUI.SetComboTimerActive(false);
        _playerUI.ResetCombo();
        _comboTimerPaused = false;
    }

    public void FreezeComboTimer()
    {
        if (_comboTimerWaitFrames > 0 && !_comboTimerPaused)
        {
            _playerUI.SetComboTimerLock(true);
            _comboTimerPaused = true;
        }
    }

    public void UnfreezeComboTimer()
    {
        if (_comboTimerWaitFrames > 0 && _comboTimerPaused)
        {
            _playerUI.SetComboTimerLock(false);
            _comboTimerPaused = false;
        }
    }

    public void RecallAssist()
    {
        _assist.Recall();
    }

    public int CalculateDamage(AttackSO hurtAttack)
    {
        int comboCount = OtherPlayerUI.CurrentComboCount;
        DemonicsFloat calculatedDamage = ((DemonicsFloat)hurtAttack.damage / (DemonicsFloat)playerStats.Defense) * OtherPlayer.DemonLimitMultiplier();
        if (comboCount > 1)
        {
            DemonicsFloat damageScale = (DemonicsFloat)1;
            for (int i = 0; i < comboCount; i++)
            {
                damageScale *= _damageDecay;
            }
            calculatedDamage *= damageScale;
        }
        int calculatedIntDamage = (int)calculatedDamage;
        OtherPlayer.SetResultAttack(calculatedIntDamage, hurtAttack);
        return calculatedIntDamage;
    }

    public void SetResultAttack(int calculatedDamage, AttackSO attack)
    {
        ResultAttack.startUpFrames = attack.startUpFrames;
        ResultAttack.activeFrames = attack.activeFrames;
        ResultAttack.recoveryFrames = attack.recoveryFrames;
        ResultAttack.attackTypeEnum = attack.attackTypeEnum;
        ResultAttack.damage = calculatedDamage;
        ResultAttack.comboDamage += calculatedDamage;
    }

    public bool HitboxCollided(Vector2 hurtPosition, Hurtbox hurtbox = null)
    {
        if (!CurrentAttack.isProjectile)
        {
            if (!CurrentAttack.isArcana || CurrentAttack.attackTypeEnum != AttackTypeEnum.Throw)
            {
                GameManager.Instance.AddHitstop(this);
            }
        }
        CurrentAttack.hurtEffectPosition = hurtPosition;
        if (!CurrentAttack.isProjectile)
        {
            if (!CurrentAttack.isArcana)
            {
                CanSkipAttack = true;
            }
            if (OtherPlayerMovement.IsInCorner)
            {
                if (!CurrentAttack.isArcana && CurrentAttack.attackTypeEnum != AttackTypeEnum.Throw && !CurrentAttack.causesKnockdown)
                {
                    if (_playerMovement.IsGrounded || OtherPlayerMovement.IsGrounded)
                    {
                        _playerMovement.Knockback(new Vector2(CurrentAttack.knockbackForce.x, 0), CurrentAttack.knockbackDuration, (int)(OtherPlayer.transform.localScale.x));
                    }
                }
            }
        }
        return hurtbox.TakeDamage(CurrentAttack);
    }

    public virtual void CreateEffect(Vector2 projectilePosition, bool isProjectile = false)
    {
        if (CurrentAttack.hitEffect != null)
        {
            GameObject hitEffect;
            hitEffect = ObjectPoolingManager.Instance.Spawn(CurrentAttack.hitEffect, parent: _effectsParent);
            hitEffect.transform.localPosition = CurrentAttack.hitEffectPosition;
            hitEffect.transform.localRotation = Quaternion.Euler(0, 0, CurrentAttack.hitEffectRotation);
            hitEffect.transform.localScale = new Vector2(-1, 1);
            if (isProjectile)
            {
                hitEffect.transform.SetParent(null);
                hitEffect.transform.localScale = new Vector2(transform.localScale.x, 1);
                hitEffect.GetComponent<Projectile>().SetSourceTransform(transform, projectilePosition, false);
                hitEffect.GetComponent<Projectile>().Direction = new Vector2(transform.localScale.x, 0);
                hitEffect.transform.GetChild(0).GetChild(0).GetComponent<Hitbox>().SetHitboxResponder(transform);
            }
        }
    }

    public void SetInvinsible(bool state)
    {
        Invisible = state;
        _playerAnimator.SetInvinsible(state);
        SetHurtbox(!state);
        SetPushboxTrigger(state);
    }

    public bool TakeDamage(AttackSO attack)
    {
        GameManager.Instance.AddHitstop(this);
        if (attack.attackTypeEnum == AttackTypeEnum.Throw)
        {
            return _playerStateManager.TryToGrabbedState();
        }
        if (Invincible)
        {
            return false;
        }
        if (CanBlock(attack))
        {
            bool blockSuccesful = _playerStateManager.TryToBlockState(attack);
            if (!blockSuccesful)
            {
                return _playerStateManager.TryToHurtState(attack);
            }
            return true;
        }
        return _playerStateManager.TryToHurtState(attack);
    }

    public void SetSpriteOrderPriority()
    {
        _playerAnimator.SetSpriteOrder(1);
        OtherPlayer._playerAnimator.SetSpriteOrder(0);
    }

    public void EnterHitstop()
    {
        _playerMovement.EnterHitstop();
        _playerAnimator.Pause();
    }

    public void ExitHitstop()
    {
        StopShakeCoroutine();
        _playerMovement.ExitHitstop();
        _playerAnimator.Resume();
        _playerAnimator.SpriteNormalEffect();
        hitstopEvent?.Invoke();
        hitstopEvent.RemoveAllListeners();
    }

    public bool IsInHitstop()
    {
        return _playerMovement.IsInHitstop;
    }

    public void HurtOnSuperArmor(AttackSO attack)
    {
        SetHealth(CalculateDamage(attack));
        _playerUI.Damaged();
        _playerUI.UpdateHealthDamaged();
        _playerAnimator.SpriteSuperArmorEffect();
        GameManager.Instance.HitStop(attack.hitstop);
    }

    public bool CanTakeSuperArmorHit(AttackSO attack)
    {
        if (CurrentAttack.hasSuperArmor && !_playerAnimator.InRecovery() && !CanSkipAttack)
        {
            return true;
        }
        return false;
    }

    private bool CanBlock(AttackSO attack)
    {
        if (attack.attackTypeEnum == AttackTypeEnum.Break)
        {
            if (BlockingLeftOrRight())
            {
                _playerUI.DisplayNotification(NotificationTypeEnum.GuardBreak);
            }
            return false;
        }
        if (_playerStateManager.CurrentState is BlockParentState)
        {
            return true;
        }
        if (_controller.ControllerInputName == ControllerTypeEnum.Cpu.ToString() && TrainingSettings.BlockAlways && TrainingSettings.CpuOff)
        {
            return true;
        }
        if (BlockingLeftOrRight())
        {
            if (attack.attackTypeEnum == AttackTypeEnum.Overhead && !_controller.ActiveController.Crouch())
            {
                return true;
            }
            if (attack.attackTypeEnum == AttackTypeEnum.Mid)
            {
                return true;
            }
            if (attack.attackTypeEnum == AttackTypeEnum.Low && _controller.ActiveController.Crouch())
            {
                return true;
            }
        }
        return false;
    }

    public bool BlockingLeftOrRight()
    {
        if (transform.localScale.x == 1 && _controller.ActiveController.InputDirection.x < 0
                   || transform.localScale.x == -1 && _controller.ActiveController.InputDirection.x > 0)
        {
            return true;
        }
        return false;
    }

    public void LoseLife()
    {
        Lives--;
        _playerUI.SetLives();
    }

    public void SetPushboxTrigger(bool state)
    {
        _groundPushbox.SetIsTrigger(state);
    }

    public void SetHurtbox(bool state)
    {
        for (int i = 0; i < _hurtbox.childCount; i++)
        {
            _hurtbox.GetChild(i).gameObject.SetActive(state);
        }
    }

    public void Pause(bool isPlayerOne)
    {
        if (GameManager.Instance.IsTrainingMode)
        {
            _playerUI.OpenTrainingPause(isPlayerOne);
        }
        else
        {
            _playerUI.OpenPauseHold(isPlayerOne);
        }
    }

    public void UnPause()
    {
        if (!GameManager.Instance.IsTrainingMode)
        {
            Debug.Log("A");
            _playerUI.ClosePauseHold();
        }
    }
}
