using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerAnimator : MonoBehaviour
{
	[SerializeField] private PlayerStats _playerStats = default;
	private Animator _animator;
	private SpriteLibrary _spriteLibrary;
	private SpriteRenderer _spriteRenderer;

	public PlayerStats PlayerStats { get { return _playerStats; } private set { } }

	void Awake()
	{
		_animator = GetComponent<Animator>();
		_spriteLibrary = GetComponent<SpriteLibrary>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Start()
	{
		_animator.runtimeAnimatorController = _playerStats.PlayerStatsSO.runtimeAnimatorController;
	}

	public void SetMove(bool state)
	{
		_animator.SetBool("IsMoving", state);
	}

	public void SetMovementX(float value)
	{
		_animator.SetFloat("MovementInputX", value);
	}

	public void IsCrouching(bool state)
	{
		_animator.SetBool("IsCrouching", state);
	}

	public void IsJumping(bool state)
	{
		_animator.SetBool("IsJumping", state);
	}

	public void Attack(string attackType)
	{
		_animator.SetTrigger(attackType);
	}

	public void Arcana()
	{
		_animator.SetTrigger("Arcana");
	}

	public void IsHurt(bool state)
	{
		_animator.SetBool("IsHurt", state);
	}

	public void IsBlocking(bool state)
	{
		_animator.SetBool("IsBlocking", state);
	}

	public void IsBlockingLow(bool state)
	{
		_animator.SetBool("IsBlockingLow", state);
	}
	public void IsBlockingAir(bool state)
	{
		_animator.SetBool("IsBlockingAir", state);
	}

	public void IsDashing(bool state)
	{
		_animator.SetBool("IsDashing", state);
	}

	public void IsRunning(bool state)
	{
		_animator.SetBool("IsRunning", state);
	}

	public void Taunt()
	{
		_animator.SetTrigger("Taunt");
	}

	public void Death()
	{
		_animator.SetTrigger("Death");
	}

	public void IsKnockedDown(bool state)
	{
		_animator.SetBool("IsKnockedDown", state);
	}

	public void Rebind()
	{
		_animator.Rebind();
	}

	public Sprite GetCurrentSprite()
	{
		return _spriteRenderer.sprite;
	}

	public int SetSpriteLibraryAsset(int skinNumber)
	{
		if (skinNumber > PlayerStats.PlayerStatsSO.spriteLibraryAssets.Length - 1)
		{
			skinNumber = 0;
		}
		else if (skinNumber < 0)
		{
			skinNumber = PlayerStats.PlayerStatsSO.spriteLibraryAssets.Length - 1;
		}
		_spriteLibrary.spriteLibraryAsset = PlayerStats.PlayerStatsSO.spriteLibraryAssets[skinNumber];
		return skinNumber;
	}
}
