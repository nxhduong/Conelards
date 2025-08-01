using System.Text.Json;
using System.Text.Json.Serialization;

namespace Conelards.Hubs.GameHub
{
    class Table
    {
        public Dictionary<string, PlayerProperties> Players = [];
        public HashSet<string> Spectators = [];
        List<(string poster, string msg, DateTime timePosted)> ChatHistory = [];

        [JsonIgnore]
        public List<Card> Deck = [
            new Card(CardColor.Red, 0, null),
            new Card(CardColor.Red, 1, null),
            new Card(CardColor.Red, 2, null),
            new Card(CardColor.Red, 3, null),
            new Card(CardColor.Red, 4, null),
            new Card(CardColor.Red, 5, null),
            new Card(CardColor.Red, 6, null),
            new Card(CardColor.Red, 7, null),
            new Card(CardColor.Red, 8, null),
            new Card(CardColor.Red, 9, null),
            new Card(CardColor.Red, null, CardPower.Ban),
            new Card(CardColor.Red, null, CardPower.Ban),
            new Card(CardColor.Red, null, CardPower.Reverse),
            new Card(CardColor.Green, 0, null),
            new Card(CardColor.Green, 1, null),
            new Card(CardColor.Green, 2, null),
            new Card(CardColor.Green, 3, null),
            new Card(CardColor.Green, 4, null),
            new Card(CardColor.Green, 5, null),
            new Card(CardColor.Green, 6, null),
            new Card(CardColor.Green, 7, null),
            new Card(CardColor.Green, 8, null),
            new Card(CardColor.Green, 9, null),
            new Card(CardColor.Green, null, CardPower.Ban),
            new Card(CardColor.Green, null, CardPower.Ban),
            new Card(CardColor.Green, null, CardPower.Reverse),
            new Card(CardColor.Blue, 0, null),
            new Card(CardColor.Blue, 1, null),
            new Card(CardColor.Blue, 2, null),
            new Card(CardColor.Blue, 3, null),
            new Card(CardColor.Blue, 4, null),
            new Card(CardColor.Blue, 5, null),
            new Card(CardColor.Blue, 6, null),
            new Card(CardColor.Blue, 7, null),
            new Card(CardColor.Blue, 8, null),
            new Card(CardColor.Blue, 9, null),
            new Card(CardColor.Blue, null, CardPower.Ban),
            new Card(CardColor.Blue, null, CardPower.Ban),
            new Card(CardColor.Blue, null, CardPower.Reverse),
            new Card(CardColor.Yellow, 0, null),
            new Card(CardColor.Yellow, 1, null),
            new Card(CardColor.Yellow, 2, null),
            new Card(CardColor.Yellow, 3, null),
            new Card(CardColor.Yellow, 4, null),
            new Card(CardColor.Yellow, 5, null),
            new Card(CardColor.Yellow, 6, null),
            new Card(CardColor.Yellow, 7, null),
            new Card(CardColor.Yellow, 8, null),
            new Card(CardColor.Yellow, 9, null),
            new Card(CardColor.Yellow, null, CardPower.Ban),
            new Card(CardColor.Yellow, null, CardPower.Ban),
            new Card(CardColor.Yellow, null, CardPower.Reverse),
            new Card(CardColor.Black, 0, null),
            new Card(CardColor.Black, 1, null),
            new Card(CardColor.Black, 2, null),
            new Card(CardColor.Black, 3, null),
            new Card(CardColor.Black, 4, null),
            new Card(CardColor.Black, 5, null),
            new Card(CardColor.Black, 6, null),
            new Card(CardColor.Black, 7, null),
            new Card(CardColor.Black, 8, null),
            new Card(CardColor.Black, 9, null),
            new Card(CardColor.Black, null, CardPower.Ban),
            new Card(CardColor.Black, null, CardPower.Ban),
            new Card(CardColor.Black, null, CardPower.Reverse),
            new Card(null, null, CardPower.Draw4),
            new Card(null, null, CardPower.Draw4),
            new Card(null, null, CardPower.Draw4),
            new Card(null, null, CardPower.Draw4),
            new Card(null, null, CardPower.Draw4),
            new Card(null, null, CardPower.Draw4),
            new Card(null, null, CardPower.Draw4),
            new Card(null, null, CardPower.Draw4),
            new Card(null, null, CardPower.Draw4),
            new Card(null, null, CardPower.Draw4),
            new Card(null, null, CardPower.Draw4),
            new Card(null, null, CardPower.Draw4),
            new Card(null, null, CardPower.Wild),
            new Card(null, null, CardPower.Wild),
            new Card(null, null, CardPower.Shuffle),
            new Card(null, null, CardPower.Shuffle)
        ];
        //public static readonly CardPower[] ColorfulCards = [CardPower.Wild, CardPower.Shuffle, CardPower.Draw4];
        public Card Discard = default!;
        public byte PenaltyStackCount = 0;

        private byte _currentPlayerIndex = 0;
        public byte CurrentPlayerIndex
        {
            get => _currentPlayerIndex;
            set
            {
                if (value >= 0 && value < Deck.Count)
                {
                    _currentPlayerIndex = value;
                }
                else if (value >= Deck.Count)
                {
                    _currentPlayerIndex = 0;
                }
            }
        }

        public Table()
        {
            // Dealing cards
            Deck = [.. Deck.OrderBy(i => GameHub.Randomizer.Next())];

            byte i = 0;
            do
            {
                Discard = Deck[i];
                i += 1;
            }
            while (Discard.Number is null);

            Deck.RemoveAt(i);
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
