using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockParentState : State
{
    public static bool skipKnockback;
    protected AttackSO hurtAttack;
    protected Vector2 start;
    protected Vector2 end;
    protected int knockbackFrame;
    public override void UpdateLogic(PlayerNetwork player)
    {
        if (!player.enter)
        {
            OnEnter(player);
        }

        if (GameSimulation.Hitstop <= 0)
        {
            AfterHitstop(player);
        }
    }

    protected virtual void OnEnter(PlayerNetwork player)
    {
        hurtAttack = PlayerComboSystem.GetComboAttack(player.otherPlayer.playerStats, player.otherPlayer.attackInput, player.otherPlayer.isCrouch, player.otherPlayer.isAir);
        player.player.StartShakeContact();
        player.enter = true;
        GameSimulation.Hitstop = hurtAttack.hitstop;
        player.sound = "Block";
        Vector2 hurtEffectPosition = new Vector2(player.position.x + (5 * player.flip), player.otherPlayer.hitbox.position.y);
        hurtAttack.hurtEffectPosition = hurtEffectPosition;
        if (hurtAttack.isArcana)
        {
            player.SetEffect("Chip", hurtAttack.hurtEffectPosition);
            player.player.SetHealth(player.player.CalculateDamage(hurtAttack));
            player.player.PlayerUI.Damaged();
            player.player.PlayerUI.UpdateHealthDamaged();
        }
        else
        {
            player.SetEffect("Block", hurtAttack.hurtEffectPosition);
        }
        player.animationFrames = 0;
        player.stunFrames = hurtAttack.hitStun;
        knockbackFrame = 0;
        start = player.position;
        end = new Vector2(player.position.x + (hurtAttack.knockbackForce.x * -player.flip), (float)DemonicsPhysics.GROUND_POINT - 0.5f);
        player.velocity = Vector2.zero;
    }

    protected virtual void AfterHitstop(PlayerNetwork player)
    {
        if (!skipKnockback)
        {
            if (hurtAttack.knockbackDuration > 0)
            {
                float ratio = (float)knockbackFrame / (float)hurtAttack.knockbackDuration;
                float distance = end.x - start.x;
                float nextX = Mathf.Lerp(start.x, end.x, ratio);
                float baseY = Mathf.Lerp(start.y, end.y, (nextX - start.x) / distance);
                float arc = hurtAttack.knockbackArc * (nextX - start.x) * (nextX - end.x) / ((-0.25f) * distance * distance);
                Vector2 nextPosition = new Vector2(nextX, baseY + arc);
                nextPosition = new Vector2(nextX, player.position.y);
                player.position = nextPosition;
                knockbackFrame++;
            }
        }
        player.player.StopShakeCoroutine();
        player.stunFrames--;
    }
    public override bool ToHurtState(PlayerNetwork player, AttackSO attack)
    {
        player.enter = false;
        player.state = "Hurt";
        return true;
    }
    public override bool ToBlockState(PlayerNetwork player, AttackSO attack)
    {
        player.enter = false;
        if (player.direction.y < 0)
        {
            player.state = "BlockLow";
        }
        else
        {
            player.state = "Block";
        }
        return true;
    }
}