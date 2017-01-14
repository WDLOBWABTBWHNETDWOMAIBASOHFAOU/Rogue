using System.Collections.Generic;

namespace Wink
{
    public abstract partial class Living : AnimatedGameObject
    {
        /// <summary>
        /// Handles attacking an opponent. First checks if attacker hits opponent, if succesfull sends attackvalue to defending side, if unsuccesfull displays miss feedback
        /// </summary>
        /// <param name="target"></param>
        public void Attack(Living target)
        {
            int damageDealt = 0;
            DamageType damageType = DamageType.physical;
            if (weapon.SlotItem != null)
            {
                WeaponEquipment weaponItem = weapon.SlotItem as WeaponEquipment;
                damageType = weaponItem.DamageType;
            }

            double hitNumber = GameEnvironment.Random.NextDouble();
            if (hitNumber < HitChance())
            {
                double attackValue = AttackValue();
                damageDealt = target.TakeDamage(attackValue, damageType);
            }

            if (damageDealt > 0) processReflection(damageDealt, target);
            // Display attack missed (feedback on fail)
        }

        public void processReflection(int damage, Living source)
        {
            if (source.EquipmentSlots != null)
                foreach (ItemSlot slot in source.EquipmentSlots.Children)
                {
                    if (slot.SlotItem != null && slot.Id.Contains("ringSlot"))
                    {
                        RingEquipment ring = slot.SlotItem as RingEquipment;
                        foreach (RingEffect effect in ring.RingEffects)
                        {
                            if (effect.EffectType == RingType.reflection)
                            {
                                ReflectionEffect rEffect = effect as ReflectionEffect;
                                double randomNumber = GameEnvironment.Random.NextDouble();
                                double reflectChance = rEffect["chance"] - 1 / source.luck;
                                if (randomNumber <= reflectChance)
                                {
                                    TakeReflectionDamage(source, damage, rEffect["power"]);
                                }
                            }
                        }
                    }
                }
        }

        /// <summary>
        /// Checks if the defending side dodges the attack, if dodge is unsuccesfull HP decreases. Also displays proper feedback on screen
        /// </summary>
        /// <param name="">Attackvalue of the attacking side</param>
        public int TakeDamage(double attackValue, DamageType damageType)
        {
            double dodgeNumber = GameEnvironment.Random.NextDouble();
            if (dodgeNumber > DodgeChance())
            {
                double defenceValue = ArmorValue(damageType);
                int damageTaken = (int)(attackValue / defenceValue);

                healthPoints -= damageTaken;
                //Display damage taken

                // Return damage taken for ring effect <3
                return damageTaken;
            }
            // Display attack dodged (feedback on succes)

            // return no damage was taken
            return 0;
        }

        protected void TakeReflectionDamage(Living source, int baseDamage, double power = 1)
        {
            double reflectAmount = 0.8 - 2 / source.Intelligence;
            if (reflectAmount < 0) reflectAmount = 0;
            int damageTaken = (int)(baseDamage * reflectAmount * power);
            
            healthPoints -= damageTaken;
        }

        protected void Death()
        {
            //What happens on death. Drop equipment/loot, remove itself from world, etc
            //level.Remove(this);
            visible = false;
        }

        private void DeathFeedback(string idA, string idS)
        {
            PlayAnimation(idA);
            //PlaySound(idS);
        }

    }
}
