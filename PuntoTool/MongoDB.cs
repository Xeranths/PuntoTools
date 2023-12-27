using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Connections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PuntoTool
{
    internal class MongoDB
    {
        public static bool MenuMongodDB()
        {
            //Console.Clear();
            Console.WriteLine("Gestion MongoDB:");
            Console.WriteLine("1. Insérer une partie");
            Console.WriteLine("2. Effacer une partie");
            Console.WriteLine("3. Mettre à jour une partie");
            Console.WriteLine("4. Obtenir/Transférer une ligne sur une autre base");
            Console.WriteLine("5. Générer une ligne");
            Console.WriteLine("5. Quitter");
            Console.Write("\nEntrez votre choix : ");
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "SalinasPunto";
            string collectionName = "PuntoGame";
            MongoClient client = new MongoClient(connectionString);
            createCollIfNotExist(client, databaseName, collectionName);
            IMongoDatabase database = client.GetDatabase(databaseName);
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);

            switch (Console.ReadLine())
            {
                case "1":
                    // Logique pour l'insertion
                    Console.WriteLine("Vous avez choisi d'insérer des informations.");
                    Console.WriteLine("Donnez la date de la partie:");
                    Console.WriteLine("Format JJ/MM/AAAA");
                    var date = Console.ReadLine();//string requires non nullable
                    Console.WriteLine("Donnez les gagnants de la partie:");
                    Console.WriteLine("Player1, Player3, Player2, Player4 ou Draw");
                    var winners = Console.ReadLine();//string requires non nullable

                    if(winners != "Player1" & winners != "Player2" & winners != "Player3" & winners != "Player4" & winners != "Draw")
                    {
                        Console.WriteLine("Entrée incorrecte");
                        return true;
                    }

                    var document = new BsonDocument
                    {
                        { "date", date },
                        { "winners", winners }
                    };

                    try
                    {
                        collection.InsertOne(document);
                        Console.WriteLine("Document inséré.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur: " + ex.Message);
                    }
                    Console.WriteLine("Appuyez sur un bouton.");
                    Console.ReadLine();
                    return true;
                case "2":
                    Console.WriteLine("Vous avez choisi de supprimer des informations.");
                    Console.WriteLine("Donnez la date de la partie:");
                    Console.WriteLine("Format JJ/MM/AAAA");
                    var dateDel = Console.ReadLine();//string requires non nullable

                    var filter = Builders<BsonDocument>.Filter.Eq("date", dateDel);
                    try
                    {
                        // Delete the first document that matches the filter
                        var result = collection.DeleteOne(filter);

                        if (result.DeletedCount > 0)
                        {
                            Console.WriteLine("Document effacé.");
                        }
                        else
                        {
                            Console.WriteLine("Aucun document à effacer.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur: " + ex.Message);
                    }

                    return true;
                case "3":
                    Console.WriteLine("Vous avez choisi de modifier des informations.");
                    Console.WriteLine("Donnez la date de la partie a modifier:");
                    Console.WriteLine("Format JJ/MM/AAAA");
                    var dateUpMod = Console.ReadLine();//string requires non nullable
                    Console.WriteLine("Donnez la nouvelle date de la partie:");
                    Console.WriteLine("Format JJ/MM/AAAA");
                    var dateUp = Console.ReadLine();//string requires non nullable
                    Console.WriteLine("Donnez les nouveaux gagnants de la partie:");
                    Console.WriteLine("Player1, Player3,  Player2, Player4 ou Draw");
                    var winnersUp = Console.ReadLine();//string requires non nullable

                    if (winnersUp != "Player1" & winnersUp != "Player2" & winnersUp != "Player3" & winnersUp != "Player4" & winnersUp != "Draw")
                    {
                        Console.WriteLine("Entrée incorrecte");
                        return true;
                    }

                    var filterUp = Builders<BsonDocument>.Filter.Eq("date", dateUp);
                    var update = Builders<BsonDocument>.Update.Set("date", dateUp).Set("winners", winnersUp);

                    try
                    {
                        // Update the first document that matches the filter
                        var result = collection.UpdateOne(filterUp, update);

                        if (result.ModifiedCount > 0)
                        {
                            Console.WriteLine("Document modifié.");
                        }
                        else
                        {
                            Console.WriteLine("Aucun document modifié.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }

                    return true;
                case "4":
                    //requete get
                    Console.WriteLine("Vous avez choisi d'obtenir des informations.");
                    Console.WriteLine("Donnez la date de la partie:");
                    Console.WriteLine("Format JJ/MM/AAAA");
                    var dateGet = Console.ReadLine();
                    var field1 = "";
                    var field2 = "";
                    var filterGet = Builders<BsonDocument>.Filter.Eq("date", dateGet);

                    var projection = Builders<BsonDocument>.Projection.Include("date").Include("winners").Exclude("_id");

                    try
                    {
                        var documents = collection.Find(filterGet).Project(projection).ToList();
                        foreach (var doc in documents)
                        {
                            field1 = doc["date"].AsString;
                            field2 = doc["winners"].AsString;
                            Console.WriteLine($"Field1: {field1}, Field2: {field2}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }

                    // choix transf
                    Console.WriteLine("1: Mettre sur SQL");
                    Console.WriteLine("2: Mettre sur SQLite");
                    Console.WriteLine("3: Quitter");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            SQLinsert(field1, field2);
                            return true;
                        case "2":
                            SQLiteInsert(field1, field2);
                            return true;
                        case "3":
                            return false;
                    }
                    return false;
                case "5":
                    Console.WriteLine("Nombres d'insertions:");
                    int insert = int.Parse(Console.ReadLine());
                    for (int i = 0; i < insert; i++)
                    {
                        Random random = new Random();
                        int year = random.Next(1900, DateTime.Today.Year + 1);
                        int month = random.Next(1, 13);
                        int daysInMonth = DateTime.DaysInMonth(year, month);
                        int day = random.Next(1, daysInMonth + 1);
                        string formattedDate = $"{day:D2}/{month:D2}/{year}";
                        string[] playerChoose = { "Player1", "Player2", "Player3", "Player4" };
                        string playerNew = playerChoose[random.Next(playerChoose.Length)];
                        MongoInsert(formattedDate, playerNew);
                    }
                    return false;
                case "6":
                    // Quitter l'application
                    return false;
                default:
                    Console.WriteLine("Choix invalide. Veuillez choisir une option valide.");
                    Console.ReadLine(); // Attendre une entrée pour revenir au menu
                    return true;
            }
        }

        static void createCollIfNotExist(MongoClient client, string db, string col)
        {
            var dbList = client.ListDatabaseNames().ToList();
            if (!dbList.Contains(db))
            {
                Console.WriteLine("La base n'existe pas. Ce n'est bientôt plus le cas...");
                client.GetDatabase(db);
            }
            else
            {
                Console.WriteLine("La base existe déjà.");
            }
            IMongoDatabase database = client.GetDatabase(db);

            var collectionList = database.ListCollectionNames().ToList();
            if (!collectionList.Contains(col))
            {
                Console.WriteLine("La collection n'existe pas. En création...");
                database.CreateCollection(col);
            }
            else
            {
                Console.WriteLine("La collectionexiste déjà.");
            }
        }

        public static void SQLiteInsert(string date, string winner)
        {
            string connectionString = "Data Source=C:\\Users\\thmoa\\AppData\\Local\\PuntoSQLite.db";
            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    // Ouvrir la connexion
                    connection.Open();
                    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"INSERT INTO puntoGame (date, winners) 
                            VALUES ($Valeur1, $Valeur2)
                        ";
                    command.Parameters.AddWithValue("$Valeur1", date);
                    command.Parameters.AddWithValue("$Valeur2", winner);

                    // Exemple d'insertion de données
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur:");
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mauvais Path");

            }
        }

        public static void SQLinsert(string date, string winners)
        {
            Console.WriteLine("Adresse serveur ?");
            var addr = Console.ReadLine();
            Console.WriteLine("Nom d'utilisateur de la base ?");
            var user = Console.ReadLine();
            Console.WriteLine("Mot de passe ?");
            var pwd = Console.ReadLine();
            string connectionString = "Server=" + addr + ";Database=puntoGame;User " + user + ";Password=" + pwd + ";";
            try
            {
                SQL.createTablIfNotExist(connectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Ouvrir la connexion
                    connection.Open();

                    // Exemple d'insertion de données
                    string insertQuery = "INSERT INTO puntoGame (id, date, winners) VALUES (" + null + "," + date + ", " + winners + ")";
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        // Exécuter la commande
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} ligne(s) insérée(s) avec succès.");
                    }
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Mauvaises informations");
            }
        }

        //https://tutoriels.edu.lat/pub/mongodb/mongodb-datatype/mongodb-types-de-donnees
        public static void MongoInsert(string date, string winners)
        {
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "SalinasPunto";
            string collectionName = "PuntoGame";
            int[] playerScore = new int[4];
            Random rand = new Random();
            for(int j = 0; j < 4; j++)
            {
                playerScore[j] = rand.Next(2);
            }
            if (winners == "Player1")
            {
                playerScore[0] = 2;
            } 
            else if (winners == "Player2")
            {
                playerScore[1] = 2;
            }
            else if(winners == "Player3")
            {
                playerScore[2] = 2;
            } 
            else if (winners == "Player4")
            {
                playerScore[3] = 2;
            }
            MongoClient client = new MongoClient(connectionString);
            createCollIfNotExist(client, databaseName, collectionName);
            IMongoDatabase database = client.GetDatabase(databaseName);
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);
            string playerScoreInsert = "[" + playerScore[0]+", " + playerScore[1]+", " + playerScore[2] +", "+ playerScore[3] +"]";
            var document = new BsonDocument
                    {
                        { "date", date },
                        { "winners", winners },
                        { "playerScore", playerScoreInsert }
                    };

            try
            {
                collection.InsertOne(document);
                Console.WriteLine("Document inséré.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur: " + ex.Message);
            }
        }
    }
}
