using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntoTool
{
    internal class SQL
    {
        public static bool MySQL()
        {   
            Console.Clear();
            Console.WriteLine("Gestion SQL:");
            Console.WriteLine("Adresse serveur ?");
            var addr = Console.ReadLine();
            Console.WriteLine("Nom d'utilisateur de la base ?");
            var user = Console.ReadLine();
            Console.WriteLine("Mot de passe ?");
            var pwd = Console.ReadLine();
            string connectionString = "Server=" + addr + ";Database=puntoGame;User " + user + ";Password=" + pwd + ";";
            try
            {
                createTablIfNotExist(connectionString);
                return MenuSQL(connectionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mauvaises informations");
                return true;
            }

        }
        public static bool MenuSQL(string connectionString)
        {
            Console.WriteLine("1. Insérer une partie");
            Console.WriteLine("2. Effacer une partie");
            Console.WriteLine("3. Mettre à jour une partie");
            Console.WriteLine("4. Obtenir/Transférer une ligne sur une autre base");
            Console.WriteLine("5. Ajouter une nouvelle ligne aléatoire");
            Console.WriteLine("6. Quitter");
            Console.Write("\nEntrez votre choix : ");

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
                    return true;
                case "2":

                    Console.WriteLine("Vous avez choisi de supprimer des informations.");
                    Console.WriteLine("Donnez la date de la partie:");
                    Console.WriteLine("Format JJ/MM/AAAA");
                    var dateDel = Console.ReadLine();//string requires non nullable

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            // Ouvrir la connexion à la base de données
                            connection.Open();

                            // Commande SQL pour supprimer une ligne dans une table
                            string deleteQuery = "DELETE FROM puntoGame WHERE date="+dateDel;

                            // Création de l'objet SqlCommand
                            using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                            {
                                // Exécution de la commande SQL pour supprimer la ligne
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    Console.WriteLine("La ligne a été supprimée avec succès.");
                                }
                                else
                                {
                                    Console.WriteLine("Aucune ligne n'a été supprimée.");
                                }
                            }
                            connection.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erreur : " + ex.Message);
                        }
                    }
                    Console.ReadLine(); // Attendre une entrée pour revenir au menu
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

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            // Ouvrir la connexion à la base de données
                            connection.Open();

                            // Commande SQL pour mettre à jour une ligne dans une table
                            string updateQuery = "UPDATE puntoGame SET date = "+dateUp+", winners = "+winnersUp+" WHERE date="+dateUpMod;

                            // Création de l'objet SqlCommand
                            using (SqlCommand command = new SqlCommand(updateQuery, connection))
                            {
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    Console.WriteLine("La mise à jour a été effectuée avec succès.");
                                }
                                else
                                {
                                    Console.WriteLine("Aucune ligne n'a été mise à jour.");
                                }
                            }
                            connection.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erreur : " + ex.Message);
                        }
                    }
                    return true;
                case "4":
                    //requete get
                    Console.WriteLine("Vous avez choisi d'obtenir des informations.");
                    Console.WriteLine("Donnez la date de la partie:");
                    Console.WriteLine("Format JJ/MM/AAAA");
                    var dateGet = Console.ReadLine();
                    string selectCommand = "SELECT Colonne2, Colonne3 FROM puntoGame WHERE date = @date";
                    string dateRet = "";
                    string winnerRet = "";
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (var command = new SqlCommand(selectCommand, connection))
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
                    Console.WriteLine("2: Mettre sur SQLite");
                    Console.WriteLine("3: Quitter");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            MongoDB.MongoInsert(dateRet, winnerRet);
                            return true;
                        case "2":
                            MongoDB.SQLiteInsert(dateRet, winnerRet);
                            return true;
                        case "3":
                            return false;
                    }
                    return false;
                case "5":
                    Random random = new Random();
                    int year = random.Next(1900, DateTime.Today.Year + 1);
                    int month = random.Next(1, 13);
                    int daysInMonth = DateTime.DaysInMonth(year, month);
                    int day = random.Next(1, daysInMonth + 1);
                    string formattedDate = $"{day:D2}/{month:D2}/{year}";
                    string[] playerChoose = { "Player1", "Player2", "Player3", "Player4" };
                    string playerNew = playerChoose[random.Next(playerChoose.Length)];
                    MongoDB.SQLinsert(formattedDate, playerNew);
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

        public static void createTablIfNotExist(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string checkTableCommand = "SELECT name FROM sqlite_master WHERE type='table' AND name='puntoGame'";
                using (var checkTable = new SqlCommand(checkTableCommand, connection))
                {
                    var existingTable = checkTable.ExecuteScalar();

                    if (existingTable == null)
                    {
                        string createTableCommand = "CREATE TABLE puntoGame (id INTEGER PRIMARY KEY NOT NULL AUTO_INCREMENT, date TEXT, winners INTEGER)";
                        using (var createTable = new SqlCommand(createTableCommand, connection))
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
