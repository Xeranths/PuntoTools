using Microsoft.Data.Sqlite;
using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntoTool
{
    internal class SQLite
    {
        public static bool Sqlite() 
        {
            string fileName = "PuntoSQLite.db";
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //on crée le fichier si il n'existe pas, on chope le path
            string dbPath = Path.Combine(folderPath, fileName);

            // Vérifier si le fichier existe déjà
            if (!File.Exists(dbPath))
            {
                // Créer le fichier s'il n'existe pas
                using (File.Create(dbPath))
                {
                    Console.WriteLine("Fichier créé avec succès !");
                }
            }
            else
            {
                Console.WriteLine("Le fichier existe déjà.");
            }

            Console.WriteLine($"Chemin du fichier : {dbPath}");
            string connectionString = "Data Source="+dbPath;
            try
            {
                bool ret = MenuSQLite(connectionString);
                return ret;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Erreur, appuyez sur un bouton pour continuer");
                Console.ReadLine();
                return true;
            }
        }
        public static bool MenuSQLite(string connectionString)
        {
            Console.Clear();
            Console.WriteLine(connectionString);
            Console.WriteLine("Gestion SQLite:");
            Console.WriteLine("1. Insérer une partie");
            Console.WriteLine("2. Effacer une partie");
            Console.WriteLine("3. Mettre à jour une partie");
            Console.WriteLine("4. Obtenir/Transférer une ligne sur une autre base");
            Console.WriteLine("5. Insérer une ligne aléatoire");
            Console.WriteLine("6. Quitter");
            Console.Write("\nEntrez votre choix : ");

            createTablIfNotExist(connectionString);
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
                    if (winners != "Player1" & winners != "Player2" & winners != "Player3" & winners != "Player4" & winners != "Draw")
                    {
                        Console.WriteLine("Entrée incorrecte");
                        return true;
                    }

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
                        command.Parameters.AddWithValue("$Valeur2", winners);

                        // Exemple d'insertion de données
                        try {
                            command.ExecuteNonQuery();
                        } catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    return true;
                case "2":
                    Console.WriteLine("Vous avez choisi de supprimer des informations.");
                    Console.WriteLine("Donnez la date de la partie:");
                    Console.WriteLine("Format JJ/MM/AAAA");
                    var dateDel = Console.ReadLine();//string requires non nullable
                    using (var connection = new SqliteConnection(connectionString))
                    {
                        // Ouvrir la connexion
                        connection.Open();
                        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
                        var command = connection.CreateCommand();
                        command.CommandText = "DELETE FROM puntoGame WHERE date = @Valeur1";
                        command.Parameters.AddWithValue("$Valeur1", dateDel); 

                        // Exemple d'insertion de données
                        try {
                            var rows = command.ExecuteNonQuery();
                            Console.WriteLine("Rows affected:" + rows);
                        } catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    return true;
                case "3":
                    Console.WriteLine("Vous avez choisi d'insérer des informations.");
                    Console.WriteLine("Donnez la date de la partie a modifier:");
                    Console.WriteLine("Format JJ/MM/AAAA");
                    var dateUpMod = Console.ReadLine();//string requires non nullable
                    Console.WriteLine("Donnez la nouvelle date de la partie:");
                    Console.WriteLine("Format JJ/MM/AAAA");
                    var dateUp = Console.ReadLine();//string requires non nullable
                    Console.WriteLine("Donnez les nouveaux gagnants de la partie:");
                    Console.WriteLine("Player1, Player3, Player2, Player4 ou Draw");
                    var winnersUp = Console.ReadLine();//string requires non nullable

                    if (winnersUp != "Player1" & winnersUp != "Player2" & winnersUp != "Player3" & winnersUp != "Player4" & winnersUp != "Draw")
                    {
                        Console.WriteLine("Entrée incorrecte");
                        return true;
                    }

                    using (var connection = new SqliteConnection(connectionString))
                    {
                        // Ouvrir la connexion
                        connection.Open();
                        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
                        var command = connection.CreateCommand();
                        command.CommandText = "UPDATE puntoGame SET date = @Valeur1, winners = @Valeur2 WHERE date = @Valeur3";
                        command.Parameters.AddWithValue("$Valeur1", dateUp);
                        command.Parameters.AddWithValue("$Valeur1", winnersUp);
                        command.Parameters.AddWithValue("$Valeur3", dateUpMod);

                        // Exemple d'insertion de données
                        try
                        {
                            var rows = command.ExecuteNonQuery();
                            Console.WriteLine("Rows affected:" + rows);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    return true;
                case "4":
                    //requete get
                    Console.WriteLine("Vous avez choisi d'obtenir des informations.");
                    Console.WriteLine("Donnez la date de la partie:");
                    Console.WriteLine("Format JJ/MM/AAAA");
                    var dateGet = Console.ReadLine();
                    string selectCommand = "SELECT date, winners FROM puntoGame WHERE date = @date";
                    string dateRet = "";
                    string winnerRet = "";
                    using (var connection = new SqliteConnection(connectionString))
                    {
                        connection.Open();

                        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());

                        using (var command = new SqliteCommand(selectCommand, connection))
                        {
                            command.Parameters.AddWithValue("@date", dateGet);

                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    dateRet = reader.GetString(0);
                                    winnerRet = reader.GetString(1);

                                    Console.WriteLine($"Valeur de Colonne1: {dateRet}");
                                    Console.WriteLine($"Valeur de Colonne2: {winnerRet}");
                                }
                                else
                                {
                                    Console.WriteLine($"Aucune ligne avec la date {dateGet} trouvée dans la table.");
                                }
                            }
                        }

                        connection.Close();
                    }

                    // choix transf
                    Console.WriteLine("1: Mettre sur MongoDB");
                    Console.WriteLine("2: Mettre sur SQL");
                    Console.WriteLine("3: Quitter");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            MongoDB.MongoInsert(dateRet, winnerRet);
                            return true;
                        case "2":
                            MongoDB.SQLinsert(dateRet, winnerRet);
                            return true;
                        case "3":
                            return false;
                    }
                    return false;
                case "5":
                    Console.WriteLine("Nombres d'insertions:");
                    int insert = int.Parse(Console.ReadLine());
                    for (int i = 0; i < insert; insert++)
                    {
                        Random random = new Random();
                        int year = random.Next(1900, DateTime.Today.Year + 1);
                        int month = random.Next(1, 13);
                        int daysInMonth = DateTime.DaysInMonth(year, month);
                        int day = random.Next(1, daysInMonth + 1);
                        string formattedDate = $"{day:D2}/{month:D2}/{year}";
                        string[] playerChoose = { "Player1", "Player2", "Player3", "Player4" };
                        string playerNew = playerChoose[random.Next(playerChoose.Length)];
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
                            command.Parameters.AddWithValue("$Valeur1", formattedDate);
                            command.Parameters.AddWithValue("$Valeur2", playerNew);

                            // Exemple d'insertion de données
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
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

        static void createTablIfNotExist(string connectionString)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());

                string checkTableCommand = "SELECT name FROM sqlite_master WHERE type='table' AND name='puntoGame'";
                using (var checkTable = new SqliteCommand(checkTableCommand, connection))
                {
                    var existingTable = checkTable.ExecuteScalar();

                    if (existingTable == null)
                    {
                        string createTableCommand = "CREATE TABLE puntoGame (date TEXT, winners INTEGER)";
                        using (var createTable = new SqliteCommand(createTableCommand, connection))
                        {
                            createTable.ExecuteNonQuery();
                            Console.WriteLine("La table a été créée avec succès.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("La table existe déjà.");
                    }
                }

                connection.Close();
            }
        }
    }
}
