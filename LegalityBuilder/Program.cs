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
        public static List<CardSet> AllSets { get; set; }
        public static List<Card> AllCards { get; set; }
        public static HashSet<string> PaperIllegalCards { get; set; } 
        public static HashSet<string> OnlineIllegalCards { get; set; } 

        public static void Main(string[] args)
        {

            PaperIllegalCards = new HashSet<string>();
            OnlineIllegalCards = new HashSet<string>();

            const string filePath = @"AllSetsArray.json";

            if (File.Exists(filePath))
            {
                ReadAllSetsJson(filePath);

                WriteAllSetsJson(@"allSetsCompiled.json");

                PopulateCards(@"OnlineOnly.txt", PaperIllegalCards);
                PopulateCards(@"OnlineIllegal.txt", OnlineIllegalCards);

                AllCards = FlattenSets();

                WriteAllCardsJson(@"allCardsCompiled.json");

                Debug.WriteLine(IsLegalCard("Creature Bond"));
                Debug.WriteLine(IsLegalCard("Creature Bond", onlineOnly: true));
                Debug.WriteLine(IsLegalCard("Creature Bond", paperOnly: true));
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
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (var sr = new StreamReader(filePath))
                {
                    string line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        cardHashSet.Add(line.Trim());
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Debug.WriteLine("The file could not be read:");
                Debug.WriteLine(e.Message);
            }
        }

        private static List<Card> FlattenSets()
        {
            var allCardsSet = new Dictionary<string, Card>();

            foreach (var cardSet in AllSets)
            {
                foreach (var card in cardSet.Cards)
                {
                    if(card.Rarity == Rarity.BasicLand)
                        continue;

                    var cardName = card.Name;
                    if (allCardsSet.ContainsKey(cardName))
                    {
                        var tempCard = allCardsSet[cardName];
                        tempCard.UpdateRarity(card.Rarity, OnlineIllegalCards.Contains(cardName), PaperIllegalCards.Contains(cardName));
                    }
                    else
                    {
                        card.UpdateRarity(card.Rarity, OnlineIllegalCards.Contains(cardName), PaperIllegalCards.Contains(cardName));
                        allCardsSet.Add(card.Name, card);
                    }
                }
            }

            return allCardsSet.Values.Where(card=> card.LegalInPaper || card.LegalOnline).ToList();
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

        private static void WriteAllSetsJson(string compPath)
        {
            if (File.Exists(compPath))
            {
                File.Delete(compPath);
            }

            using (var sw = new StreamWriter(compPath))
            {
                sw.WriteLine(JsonConvert.SerializeObject(AllSets));
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