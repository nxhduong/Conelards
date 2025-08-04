namespace Conelards.Hubs.GameHub
{
    public record Card
    {
        public CardColor? Color { get; set; }
        public byte? Number { get; private set; }
        public CardPower? Action { get; private set; }

        public Card(CardColor? color = null, byte? number = null, CardPower? action = null)
        {
            if (number is null ^ action is null) // Simple but incomplete check; ((color is null ^ action is null) & (number is null || action is null))
            {
                Color = color;
                Number = number;
                Action = action;
            }
            else
            {
                throw new ArgumentException($"Invalid card parameters: {color} {number} {action}");
            }
        }
    }

    public enum CardColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        BlackGray
    }

    public enum CardPower
    {        
        Ban,
        Reverse,
        Wild,
        Shuffle,
        Draw4
    }
}
