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
            string hitSound = "muted_metallic_crash_impact";
            int damageDealt = 0;
            DamageType damageType = DamageType.Physical;
            if (Weapon.SlotItem != null)
            {
                WeaponEquipment weaponItem = Weapon.SlotItem as WeaponEquipment;
                damageType = weaponItem.GetDamageType;
                hitSound = weaponItem.HitSound;
            }

            //double hitNumber = GameEnvironment.Random.NextDouble(); 
            //if (hitNumber < HitChance())
            if (TryHit(target))
            {
                double attackValue = AttackValue();
                damageDealt = target.TakeDamage(attackValue, damageType);
                PlaySound(hitSound);
            }
            else
            {
                // Display attack missed (visual feedback on fail)
                PlaySound("Sounds/Dodge");
            }

            if (damageDealt > 0) ProcessReflection(damageDealt, target);

        }

        public bool TryHit(Living target) //is there still a distinction between dodge and miss?
        {
            double hitChance = HitChance(); // Example: 0.7
            double dodgeChance = target.DodgeChance(); // Example: 0.7
            System.Console.WriteLine((0.5/(System.Math.Sqrt(creatureLevel+target.creatureLevel))) * (hitChance / dodgeChance));
            return (0.5 / (System.Math.Sqrt(creatureLevel + target.creatureLevel))) * (hitChance / dodgeChance) > GameEnvironment.Random.NextDouble();
        }

        public void Special_Attack(Living target, double mod)
        {
            int damageDealt = 0;
            DamageType damageType = DamageType.Magic;

            double modifier = mod;

            double attackValue = Special_AttackValue(modifier);
            damageDealt = target.TakeDamage(attackValue, damageType);

            if (damageDealt > 0) ProcessReflection(damageDealt, target);
            // Display attack missed (feedback on fail)
        }

        /// <summary>
        /// Check if damage should be reflected
        /// </summary>
        /// <param name="damage">Damage taken</param>
        /// <param name="source">The living object that gave the damage</param>
        public void ProcessReflection(int damage, Living source)
        {
            if (source.EquipmentSlots != null)                                      // 
                foreach (ItemSlot slot in source.EquipmentSlots.Children)           // Looking up the ring(s) the living object has equipped
                    if (slot.SlotItem != null && slot.Id.Contains("ringSlot"))      // 
                    {
                        RingEquipment ring = slot.SlotItem as RingEquipment;
                        foreach (RingEffect effect in ring.RingEffects)
                            if (effect.EffectType == EffectType.Reflection)           // Check if the ring is a Ring of Reflection
                            {
                                ReflectionEffect rEffect = effect as ReflectionEffect;
                                double randomNumber = GameEnvironment.Random.NextDouble();
                                double reflectChance = rEffect["chance"] - 1 / source.luck; // Calculate reflection chance
                                if (randomNumber <= reflectChance)
                                {
                                    TakeReflectionDamage(source, damage, rEffect["power"]); // Deal Reflection Damage
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
            //double dodgeNumber = GameEnvironment.Random.NextDouble();
            //if (dodgeNumber > DodgeChance())
            //{
                double defenceValue = ArmorValue(damageType);
                int damageTaken = (int)(attackValue / defenceValue);

                Health -= damageTaken;
                // Display damage taken

                // Return damage taken for ring effect :D:D
                return damageTaken;
            //}
            // Display attack dodged (feedback on succes)

            // return no damage was taken
            //return 0;
        }

        /// <summary>
        /// Take reflection damage based on Ring power and Base damage
        /// </summary>
        /// <param name="source">The living object giving the reflection damage</param>
        /// <param name="baseDamage">The damage the reflection is based on</param>
        /// <param name="power">The power multiplier of the reflection</param>
        protected void TakeReflectionDamage(Living source, int baseDamage, double power = 1)
        {
            double reflectAmount = 0.8 - 2 / source.Intelligence;
            if (reflectAmount < 0) // makes sure you can never heal the opponent by dealing 
                reflectAmount = 0; //     negative reflection damage
            int damageTaken = (int)(baseDamage * reflectAmount * power);
            
            Health -= damageTaken;
            // TODO: Visual feedback of reflection
        }

        /// <summary>
        /// Kills the living object (at least... It should)
        /// </summary>
        public virtual void Death()
        {
            visible = false;
        }

        /// <summary>
        /// Give visual feedback of death
        /// </summary>
        /// <param name="idA">Death animation</param>
        /// <param name="idS">Death sound</param>
        public void DeathFeedback(string idA = "die", string idS = "Sounds/video_game_announcer_grunt")
        {
            PlayAnimation(idA);
            PlaySound(idS);

            if (this is IGUIGameObject)
            {
                (this as IGUIGameObject).CleanupGUI(new Dictionary<string, object>());
            }
        }
    }
}
