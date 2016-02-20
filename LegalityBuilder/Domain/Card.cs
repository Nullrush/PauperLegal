using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LegalityBuilder.Domain
{
    public class Card
    {
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Rarity Rarity { get; set; }
        public bool LegalOnline { get; set; }
        public bool LegalInPaper { get; set; }

        [JsonIgnore]
        public List<string> RelatedSets { get; set; }

        public void AddSet(string setName)
        {
            if(RelatedSets == null)
                RelatedSets = new List<string>();

            RelatedSets.Add(setName);
        }

        public void UpdateRarity(Rarity rarity, bool onlineIllegal = false, bool paperIllegal = false)
        {
            if (rarity < Rarity)
                Rarity = rarity;
            if (!onlineIllegal && rarity == Rarity.Common)
                LegalOnline = true;
            if (!paperIllegal && rarity == Rarity.Common)
                LegalInPaper = true;
        }

        public void CleanName()
        {
            Name = Regex.Replace(Name.ToLower(), "[^a-zA-Z]+", "", RegexOptions.Compiled);
        }
    }

    public enum Rarity
    {
        [EnumMember(Value = "Basic Land")]
        BasicLand,
        Common,
        Uncommon,
        Rare,
        [EnumMember(Value = "Mythic Rare")]
        MythicRare,
        Special,
        Illegal
    }
}