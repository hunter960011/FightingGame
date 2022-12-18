using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "Player Stats", menuName = "Scriptable Objects/Player Stat", order = 1)]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Main")]
    public int characterIndex;
    public Sprite[] portraits;
    public AnimationSO _animation;
    public DialogueSO _dialogue;
    public EffectsLibrarySO _effectsLibrary;
    public CharacterTypeEnum characterName;
    [Header("Stats")]
    public int defenseLevel;
    public int arcanaLevel;
    public int speedLevel;
    public int jumpLevel;
    public int dashLevel;
    public bool canDoubleJump = true;
    public float arcanaRecharge = 1;
    [Header("Moves")]
    public AttackSO m2L;
    public AttackSO m5L;
    public AttackSO m2M;
    public AttackSO m5M;
    public AttackSO m2H;
    public AttackSO m5H;
    public AttackSO jL;
    public AttackSO jM;
    public AttackSO jH;
    public AttackSO mThrow;
    public AttackSO mParry;
    public AttackSO mRedFrenzy;
    public ArcanaSO m5Arcana;
    public ArcanaSO m2Arcana;
    public ArcanaSO jArcana;
    [HideInInspector] public int maxHealth = 10000;

    public int Arcana { get { return arcanaLevel; } set { } }
    public float Defense { get { return (defenseLevel - 1) * 0.05f + 0.95f; } set { } }
    public DemonicsFloat SpeedWalk
    {
        get
        {
            switch (speedLevel)
            {
                case 1:
                    return (DemonicsFloat)0.03;
                case 2:
                    return (DemonicsFloat)0.05;
                case 3:
                    return (DemonicsFloat)0.07;
                default:
                    return (DemonicsFloat)0;
            }
        }
        set { }
    }
    public DemonicsFloat SpeedRun
    {
        get
        {
            switch (speedLevel)
            {
                case 1:
                    return (DemonicsFloat)0.15;
                case 2:
                    return (DemonicsFloat)0.18;
                case 3:
                    return (DemonicsFloat)0.21;
                default:
                    return (DemonicsFloat)0;
            }
        }
        set { }
    }
    public DemonicsFloat JumpForce
    {
        get
        {
            switch (jumpLevel)
            {
                case 1:
                    return (DemonicsFloat)0.34;
                case 2:
                    return (DemonicsFloat)0.35;
                case 3:
                    return (DemonicsFloat)0.37;
                default:
                    return (DemonicsFloat)0;
            }
        }
        set { }
    }
    public DemonicsFloat DashForce
    {
        get
        {
            switch (dashLevel)
            {
                case 1:
                    return (DemonicsFloat)0.22;
                case 2:
                    return (DemonicsFloat)0.25;
                case 3:
                    return (DemonicsFloat)0.28;
                default:
                    return (DemonicsFloat)0;
            }
        }
        set { }
    }
}
