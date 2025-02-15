namespace DsBot.Test2.diceGame
{
    public class DiceGame
    {
        public int RollSum { get; set; }

        public DiceGame()
        {
            var random = new System.Random();
            var firstRoll = random.Next(1, 7);
            var secondRoll = random.Next(1, 7);
            RollSum = firstRoll + secondRoll;
        }
    }
}
