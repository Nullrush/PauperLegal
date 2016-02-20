using System.Collections.Generic;

namespace LegalityBuilder.Domain
{
    public class CardSet
    {
        public string Name { get; set; }
        public List<Card> Cards { get; set; }
    }
}
