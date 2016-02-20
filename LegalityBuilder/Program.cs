using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LegalityBuilder.Domain;
using Newtonsoft.Json;

namespace LegalityBuilder
{
    internal class Program
    {
        public static IEnumerable<CardSet> AllSets { get; set; }
        public static IEnumerable<Card> AllCards { get; set; }
        public static ISet<string> PaperIllegalCards { get; set; }
        public static ISet<string> OnlineIllegalCards { get; set; }
        public static readonly string OutputPath = "Output/";

        public static void Main(string[] args)
        {
            PaperIllegalCards = new HashSet<string>();
            OnlineIllegalCards = new HashSet<string>();

            const string filePath = @"Resources/AllSetsArray.json";

            if (File.Exists(filePath))
            {
                ReadAllSetsJson(filePath);

                WriteOutputJson(@"allSetsCompiled.json", AllSets);

                PopulateCards(@"Resources/PaperIllegal.txt", PaperIllegalCards);
                PopulateCards(@"Resources/OnlineIllegal.txt", OnlineIllegalCards);

                AllCards = FlattenSets();

                WriteOutputJson(@"allCardsCompiled.json", AllCards);
            }
            else
            {
                Debug.WriteLine("file not found: " + filePath);
            }
        }

        private static bool IsLegalCard(string cardName, bool onlineOnly = false, bool paperOnly = false)
        {
            if (onlineOnly)
                return !OnlineIllegalCards.Contains(cardName);
            if (paperOnly)
                return !PaperIllegalCards.Contains(cardName);

            return !PaperIllegalCards.Contains(cardName) || !OnlineIllegalCards.Contains(cardName);
        }

        private static void PopulateCards(string filePath, ISet<string> cardHashSet)
        {
            try
            {
                using (var sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        cardHashSet.Add(line.Trim());
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("The file could not be read:");
                Debug.WriteLine(e.Message);
            }
        }

        private static IEnumerable<Card> FlattenSets()
        {
            var allCardsSet = new Dictionary<string, Card>();

            var allCards = AllSets
                .SelectMany(cs =>
                    cs.Cards.Select(c =>
                    {
                        c.CleanName();
                        c.AddSet(cs.Name);
                        return c;
                    })
                );

            foreach (var card in allCards)
            {
                var cardName = card.Name;
                if (allCardsSet.ContainsKey(cardName))
                {
                    var tempCard = allCardsSet[cardName];
                    tempCard.UpdateRarity(card.Rarity, OnlineIllegalCards.Contains(cardName),
                    PaperIllegalCards.Contains(cardName));
                }
                else
                {
                    card.UpdateRarity(card.Rarity, OnlineIllegalCards.Contains(cardName),
                    PaperIllegalCards.Contains(cardName));
                    allCardsSet.Add(card.Name, card);
                }
            }

            return allCardsSet.Values.ToList();
        }

        private static void ReadAllSetsJson(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            using (var tr = new StreamReader(fs))
            using (JsonReader reader = new JsonTextReader(tr))
            {
                var serializer = new JsonSerializer();

                AllSets = serializer.Deserialize<List<CardSet>>(reader);
            }
        }

        private static void WriteOutputJson(string compPath, IEnumerable<object> things)
        {
            if (string.IsNullOrEmpty(compPath))
                return;

            Directory.CreateDirectory(OutputPath);

            if (File.Exists(compPath))
            {
                File.Delete(compPath);
            }

            using (var sw = new StreamWriter(OutputPath + compPath))
            {
                sw.WriteLine(JsonConvert.SerializeObject(things));
            }
        }

        private static void WriteAllCardsJson(string allcardscompiledJson)
        {
            if (File.Exists(allcardscompiledJson))
            {
                File.Delete(allcardscompiledJson);
            }

            using (var sw = new StreamWriter(allcardscompiledJson))
            {
                sw.WriteLine(JsonConvert.SerializeObject(AllCards));
            }
        }
    }
}