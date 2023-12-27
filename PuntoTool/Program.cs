// See https://aka.ms/new-console-template for more information
using System;
using Microsoft.Data.SqlClient;
Console.WriteLine("Hello, World!");
PuntoTool.Program.Main([]);


namespace PuntoTool
{
    class Program
    {
        public static void Main(string[] args)
        {
            bool showMenu = true;

            while (showMenu)
            {
                showMenu = DBMainMenu();
            }
        }

        private static bool DBMainMenu()
        {
            Console.Clear();
            Console.WriteLine("Bienvenue dans l'application de gestion des BDDs du Punto:");
            Console.WriteLine("Choisissez la BDD à utiliser:");
            Console.WriteLine("1. SQL");
            Console.WriteLine("2. SQLite");
            Console.WriteLine("3. MongoDB");
            Console.WriteLine("4. Neo4J");
            Console.WriteLine("5. Quitter");
            Console.Write("\nEntrez votre choix : ");
            switch (Console.ReadLine())
            {
                case "1":
                    SQL.MySQL();
                    return true;
                case "2":
                    SQLite.Sqlite();
                    return true;
                case "3":
                    MongoDB.MenuMongodDB();
                    return true;
                case "4":
                    Neo4J.MenuNeo4J();
                    return true;
                case "5":
                    // Quitter l'application
                    return false;
                default:
                    Console.WriteLine("Choix invalide. Veuillez choisir une option valide.");
                    Console.ReadLine(); // Attendre une entrée pour revenir au menu
                    return true;

            }
        }
    }
}
