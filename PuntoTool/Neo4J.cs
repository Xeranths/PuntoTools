using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver;
using System.Threading.Tasks;

namespace PuntoTool
{
    internal class Neo4J
    {
        public static bool MenuNeo4J()
        {
            //Console.Clear();
            Console.WriteLine("Gestion Neo4j:");
            Console.WriteLine("1. Insérer une partie");
            Console.WriteLine("2. Effacer une partie");
            Console.WriteLine("3. Obtenir une ligne");
            Console.WriteLine("4. Générer X lignes");
            Console.WriteLine("5. Print la date de X nodes (jusqu'à 5)");
            Console.WriteLine("6. Lier un tableau des scores de partie");
            Console.WriteLine("7. Quitter");
            tryConnect();
            Console.Write("\nEntrez votre choix : ");
            string choice = Console.ReadLine();
            choiceManagement(choice);
            return true;
        }

        public static bool choiceManagement(string choice)
        {
            switch (choice)
            {
                case "1":
                    insertNeo4J();
                    return true;
                case "2":
                    removeNeo4J();
                    return true;
                case "3":
                    getNeo4J();
                    return true;
                case "4":
                    generateNeo4J();
                    return true;
                case "5":
                    listNeo4J();
                    return true;
                case "6":
                    linkScoreNeo4J();
                    return true;
                case "7":
                    return true;
            }
            return false;
        }

        public static async void insertNeo4J()
        {
            using (var driver = GraphDatabase.Driver("bolt://localhost:7687/"))
            {
                await using (var session = driver.AsyncSession())
                {
                    Console.WriteLine("Date:");
                    string inDate = Console.ReadLine();
                    Console.WriteLine("Winner:");
                    string inWinners = Console.ReadLine();
                    // Creating a node
                    try
                    {
                        var createQuery = "CREATE (p:Game {date: $date, winners: $winners})";
                        var parameters = new { date = inDate, winners = inWinners };
                        await session.WriteTransactionAsync(tx => tx.RunAsync(createQuery, parameters));
                        await driver.CloseAsync();
                    }catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Console.ReadLine();
                    }
                }
            }
        }

        public static async void getNeo4J()
        {
            using (var driver = GraphDatabase.Driver("bolt://localhost:7687/", AuthTokens.Basic("neo4j", "neo4j")))
            {
                IResultCursor cursor;
                string getDate;
                string getWin;
                IAsyncSession session = driver.AsyncSession();
                Console.WriteLine("Date:");
                string myDate = Console.ReadLine();
                try
                {
                    cursor = await session.RunAsync(@$"
                            MATCH (g:Game)
                            WHERE g.date = '{myDate}'
                            RETURN g.date as date, g.winners as winners
                           ");
                    getWin = await cursor.SingleAsync<string>(record => record["winners"].As<string>());
                }
                finally
                {
                    await driver.CloseAsync();
                }
                Console.WriteLine($"La date du node choisi est {myDate} et le gagnant est {getWin} ");
            }
        }

        public static async void generateNeo4J()
        {
            Console.WriteLine("Nombre d'insertions:");
            int nb = Int32.Parse(Console.ReadLine());
            for (int i = 0; i < nb; i++) 
            {
                using (var driver = GraphDatabase.Driver("bolt://localhost:7687/", AuthTokens.Basic("neo4j", "neo4j")))
                {
                    await using (var session = driver.AsyncSession())
                    {
                        //create random vals
                        Random random = new Random();
                        int year = random.Next(1900, DateTime.Today.Year + 1);
                        int month = random.Next(1, 13);
                        int daysInMonth = DateTime.DaysInMonth(year, month);
                        int day = random.Next(1, daysInMonth + 1);
                        string formattedDate = $"{day:D2}/{month:D2}/{year}";
                        string[] playerChoose = { "Player1", "Player2", "Player3", "Player4" };
                        string playerNew = playerChoose[random.Next(playerChoose.Length)];
                        // Creating a node
                        var createQuery = "CREATE (p:Game {date: $date, winners: $winners})";
                        var parameters = new { date = formattedDate, winners = playerNew };
                        await session.ExecuteWriteAsync(tx => tx.RunAsync(createQuery, parameters));
                        await driver.CloseAsync();
                    }
                }
            }
        }

        public static async void removeNeo4J()
        {

            using (var driver = GraphDatabase.Driver("bolt://localhost:7687/", AuthTokens.Basic("neo4j", "neo4j")))
            {
                await using (var session = driver.AsyncSession())
                {
                    try
                    {
                        Console.WriteLine("Date:");
                        string delDate = Console.ReadLine();
                        Console.WriteLine("Winner:");
                        string delWinners = Console.ReadLine();
                        // Del a node
                        var createQuery = "MATCH (g:Game {date: $date, winners: $winners}) DELETE g";
                        var parameters = new { date = delDate, winners = delWinners };
                        await session.ExecuteWriteAsync(tx => tx.RunAsync(createQuery, parameters));
                    } catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    finally
                    {
                        await driver.CloseAsync();
                    }
                }
            }
        }

        public static async void listNeo4J()
        {

            using (var driver = GraphDatabase.Driver("bolt://localhost:7687/", AuthTokens.Basic("neo4j", "neo4j")))
            {
                IResultCursor cursor;
                var gamesDate = new List<string>();
                IAsyncSession session = driver.AsyncSession();
                try
                {
                    cursor = await session.RunAsync(@"MATCH (g:Game)
                            RETURN g.date as date
                            limit 5
                           ");
                    gamesDate = await cursor.ToListAsync(record => record["date"].As<string>());
                }
                finally
                {
                    await driver.CloseAsync();
                }
                gamesDate.ForEach(Console.WriteLine);
            }
        }

        public static async void tryConnect()
        {
            using (var driver = GraphDatabase.Driver("bolt://localhost:7687"))
            {
                try
                {
                    driver.VerifyConnectivityAsync().Wait();
                    Console.WriteLine("Connected");
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public static async void linkScoreNeo4J()
        {
            using (var driver = GraphDatabase.Driver("bolt://localhost:7687/"))
            {
                await using (var session = driver.AsyncSession())
                {
                    Console.WriteLine("Date de partie:");
                    string inDate = Console.ReadLine();
                    int[] score = new int[4];
                    for (int i = 0; i < 4; i++)
                    {
                        Console.WriteLine($"Score du joueur Player{i+1}:");
                        score[i] = Int32.Parse(Console.ReadLine());
                    }
                    // Creating a node
                    try
                    {
                        var createQuery = @$"
                                            MATCH (g:Game {{date: ""{inDate}""}})
                                            MERGE (s:Score {{score: ""{score.ToString}""}})
                                            MERGE (g)-[:HAS_CHILD]->(s)";
                        await session.WriteTransactionAsync(tx => tx.RunAsync(createQuery));
                        await driver.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Console.ReadLine();
                    }
                }
            }
        }

    }

    public class PuntoParty
    {
        public PuntoParty(string date, string winners) 
        {
            date = date;
            winners = winners;
        }
        public string date { 
            get { return date; }
            set { date = value; } 
        }

        public string winners { 
            get { return winners; }
            set { winners = value; }
        }
    }
}
