using System.Collections.Generic;
using System.Diagnostics;

namespace Wink
{
    public abstract partial class Living 
    {
        /// <summary>
        /// Handles attacking an opponent. First checks if attacker hits opponent, if succesfull sends attackvalue to defending side, if unsuccesfull displays miss feedback
        /// </summary>
        /// <param name="target"></param>
        public void Attack(Living target)
        {
            string hitSound = "Sounds/muted_metallic_crash_impact";
            DamageType damageType = DamageType.Physical;

            if (TryHit(target))
            {
                WeaponEquipment weapon = equipedItems.Weapon;
                if (weapon != null)
                {
                    weapon.Attack(this,target);
                    hitSound = weapon.HitSound;
                }
                else
                {
                    Debug.Write("no weapon equiped");
                    double attackValue = AttackValue();
                    target.TakeDamage(attackValue, damageType,this);
                }
                NonAnimationSoundEvent hitSoundEvent = new NonAnimationSoundEvent(hitSound);
                //no annimation for attacks (hit or miss) yet. when inplementing that, include sound effect there and remove this.
                LocalServer.SendToClients(hitSoundEvent);
            }
            else
            {
                //TODO: Display attack missed (visual feedback on fail)

                NonAnimationSoundEvent missSoundEvent = new NonAnimationSoundEvent("Sounds/Dodge");
                //no annimation for attacks (hit or miss) yet. when inplementing that, include sound effect there and remove this.
                LocalServer.SendToClients(missSoundEvent);
            }

        }

        public bool TryHit(Living target) //is there still a distinction between dodge and miss?
        {
            double hitChance = HitChance(); // Example: 0.7
            double dodgeChance = target.DodgeChance(); // Example: 0.3
            return (0.5 / (System.Math.Sqrt(creatureLevel + target.creatureLevel))) * (hitChance / dodgeChance) > GameEnvironment.Random.NextDouble();
        }

        /// <summary>
        /// Checks if the defending side dodges the attack, if dodge is unsuccesfull HP decreases. Also displays proper feedback on screen
        /// </summary>
        /// <param name="">Attackvalue of the attacking side</param>
        public void TakeDamage(double attackValue, DamageType damageType, Living Origin)
        {
            double defenceValue = ArmorValue(damageType);
            int damageTaken = (int)(attackValue / defenceValue);

            Health -= damageTaken;
            if (damageTaken > 0) ExecuteTriggeredEffect(TriggerEffects.Reflection,Origin,damageTaken);
        }

        /// <summary>
        /// Kills the living object
        /// </summary>
        public virtual void Death()
        {
            LocalServer.SendToClients(new DeathAnimationEvent(this, "Sounds/video_game_announcer_grunt"));
            visible = false;
        }

        /// <summary>
        /// Give visual feedback of death
        /// </summary>
        /// <param name="idA">Death animation</param>
        /// <param name="idS">Death sound</param>
        public void DeathFeedback(string idA = "die", string idS = "Sounds/video_game_announcer_grunt")
        {
            //PlayAnimation(idA);
            //PlaySound(idS);

            if (this is IGUIGameObject)
            {
                (this as IGUIGameObject).CleanupGUI(new Dictionary<string, object>());
            }
        }
    }
}
