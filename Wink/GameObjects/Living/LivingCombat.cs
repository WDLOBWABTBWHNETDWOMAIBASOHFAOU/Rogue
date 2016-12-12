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
            double hitNumber = GameEnvironment.Random.NextDouble();
            if (hitNumber < HitChance())
            {
                double attackValue = AttackValue();
                target.TakeDamage(attackValue);
            }
            // Display attack missed (feedback on fail)
        }

        /// <summary>
        /// Checks if the defending side dodges the attack, if dodge is unsuccesfull HP decreases. Also displays proper feedback on screen
        /// </summary>
        /// <param name="">Attackvalue of the attacking side</param>
        public void TakeDamage(double attackValue)
        {
            double dodgeNumber = GameEnvironment.Random.NextDouble();
            if (dodgeNumber > DodgeChance())
            {
                double defenceValue =ArmorValue();
                healthPoints = (int)(attackValue / defenceValue);
                //Display damage taken
            }
            // Display attack dodged (feedback on succes)
        }

        protected void Death()
        {
            //What happens on death. Drop equipment/loot, remove itself from world, etc
            level.Remove(this);
        }

        private void DeathFeedback(string idA, string idS)
        {
            PlayAnimation(idA);
            PlaySound(idS);
        }

    }
}
