using System;
using System.Collections;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            GameManager gm = new GameManager();
            gm.Run();
        }
    }

    class GameManager
    {
        ArrayList Players = new ArrayList();
        
        public void CreatePlayers()
        {
            for (int i = 0; i < 4; i++)
            {
                Players.Add(new Player(10000, Convert.ToString(i)));
            }
        }
        public void Run()
        {
            Database.CreateConnection();
            //   GameBoard.SetupMyGameBoard();
            this.CreatePlayers();
            int n = -1;
            while (true)
            {   n++;
                Player a = (Player)Players[n];
                Console.WriteLine("Player {0}'s Turn", a.PlayerName);
                PlayGame(a);
                if (n > Players.Count - 2) { n = -1; }
               
            }
        }
        public void PlayGame(Player aPlayer)
        {
            int numberOfSpacesToMove = 0;
            Console.WriteLine("Move how many spaces?");
            try { numberOfSpacesToMove = Convert.ToInt16(Console.ReadLine()); }
            catch (Exception e) { Console.WriteLine(e.ToString()); }

            this.AdvancePlayer(numberOfSpacesToMove, aPlayer);
            aPlayer.DisplayStatus();


        }

        public void AdvancePlayer(int MoveHowManySpaces, Player aPlayer)
        {
            aPlayer.CurrentAddress += MoveHowManySpaces;
            Console.WriteLine("{0} is now in space {1}", aPlayer.PlayerName, aPlayer.CurrentAddress);
            Console.WriteLine("This property is owned by : {0} ", Database.GetPropertyOwner(aPlayer.CurrentAddress));
        }
    }

    class Player
    {
        public double CurrentBalance;
        public string PlayerName;
        public int CurrentAddress;
        public ArrayList PropertiesOwned;

        public Player(double InitialBalance, string PName)
        {
            this.CurrentBalance = InitialBalance;
            this.PlayerName = PName;
            this.CurrentAddress = 0;
        }

        public void BuyAProperty(Property aProperty)
        {
            Console.WriteLine("Would you like to buy this property? The cost is {0}", aProperty.PropertyValue);
            string Buy = Console.ReadLine();
            if (Buy.Equals("y")) { this.PerformBuyTransaction(); }
        }

        public void PerformBuyTransaction() { }

        public void PayRent(Player PropertyOwner)
        {

        }

        public void DeclareBankruptcy(Player PlayerToPayTo)
        {

        }

        public void DisplayStatus()
        {
            Console.WriteLine("{0} has a balance of {1} ", this.PlayerName,  this.CurrentBalance);
        }
    }

    class Property
    {
        public Player PropertyOwner = null;
        Random r1 = new Random();
        public double PropertyValue;
        public Property()
        {
            this.PropertyValue = 100 + 700 * r1.NextDouble();
        }
    }

    class GameBoard
    {
        public static ArrayList MyGameBoard = new ArrayList();

        public static void SetupMyGameBoard()
        {   Random r = new Random();

            string RecordToInsert;
            // create 100 Properties on The Board
            for (int i = 0; i < 100; i++)
            {
                // How can I put a INSERT statement to the GameBoard SQL Table here??
                RecordToInsert = "INSERT INTO GameBoard(PropertyID, OwnerName, PropertyValue) VALUES(" + i + ", 'Bank', " + r.Next(100, 900) + ")";
                Database.InsertData(Database.sqlite_conn, RecordToInsert);
            }
        }

        //public static int FindLocationOfProperty(Property PropertyToLookFor)
        //{   // Note that this method is used for the version of the same which used a List Array to store the properties in.
        //    int IndexInArray = 0;
        //    foreach (Property aProperty in MyGameBoard)
        //    {
        //        if (PropertyToLookFor == aProperty)
        //        {
        //            return IndexInArray;
        //        }
        //    }
        //    return -1;
        //}
    }

    class Database
    {
        public static SQLiteConnection sqlite_conn;
      
        public static void CreateConnection()
        {
            sqlite_conn = new SQLiteConnection("Data Source= database.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.ToString()); }
        }

        public static string GetPropertyOwner(int CurrentAddress)
        {
            string Owner = "blank";
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT OwnerName FROM GameBoard where PropertyID="+CurrentAddress;
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                Owner = sqlite_datareader.GetString(0);
            }
            return Owner;
        }

        public static void CreateTable()
        {
            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE GameBoard (PropertyID INT, OwnerName VARCHAR(20), PropertyValue INT)";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        public static void InsertData(SQLiteConnection conn, string sqlStatement)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = sqlStatement;
            sqlite_cmd.ExecuteNonQuery();

        }

        public static void ReadData()
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM GameBoard";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                int propertyID = sqlite_datareader.GetInt16(0);
                string ownerName = sqlite_datareader.GetString(1);
                int propertyValue = sqlite_datareader.GetInt16(2);
            }
        }
    }
}