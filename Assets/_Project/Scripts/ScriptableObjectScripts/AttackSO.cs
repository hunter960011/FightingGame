using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Scriptable Objects/Attack", order = 2)]
public class AttackSO : ScriptableObject
{
    [Header("Main")]
    public int damage;
    public AttackTypeEnum attackTypeEnum;
    public int hitStun;
    public float travelDistance;
    public Vector2 travelDirection;
    [Range(0.0f, 16.0f)]
    public float knockback;
    public Vector2 knockbackForce;
    public Vector2 knockbackDirection;
    [Range(0.0f, 1.0f)]
    public float knockbackDuration;

    [Header("Properties")]
    public bool isAirAttack;
    public bool isProjectile;
    public bool isArcana;
    public bool causesKnockdown;
    [Header("Sounds")]
    public string attackSound;
    public string impactSound;
    [Header("Visuals")]
    [Range(0, 150)]
    public int hitstop;
    public GameObject hitEffect;
    public Vector2 hitEffectPosition;
    public float hitEffectRotation;
    public GameObject hurtEffect;
    [HideInInspector] public Vector2 hurtEffectPosition;
    public float hurtEffectRotation;
    public CameraShakerSO cameraShaker;
    [Header("Framedata")]
    public int startUpFrames;
    public int activeFrames;
    public int recoveryFrames;
}