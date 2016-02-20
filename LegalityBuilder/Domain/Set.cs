using System.Collections.Generic;

namespace LegalityBuilder.Domain
{
    public class CardSet
    {
        public string Name { get; set; }
        public IEnumerable<Card> Cards { get; set; }
    }
}
