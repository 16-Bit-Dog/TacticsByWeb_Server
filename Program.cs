using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Windows;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace HLWSS
{

    


    public class Login
    {
        public MySqlConnection connection;
        public string server;
        public string database;
        public string user;
        public string password;
        public string port;
        public string connectionString;
        public string sslM;

        public Login()
        {

            server = "127.0.0.1";
            database = "tacticsbywebsuper";
            user = "root";
            password = "";
            port = "3306";
            sslM = "none";

            connectionString = String.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5}", server, port, user, password, database, sslM);

            connection = new MySqlConnection(connectionString);
            
            conexion();
        }

        private void conexion()
        {
            try
            {
                connection.Open();

                Console.WriteLine("connected");

                //  connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message + connectionString);
            }
        }
    }


    //https://learnsql.com/cookbook/how-to-find-the-maximum-value-of-a-numeric-column-in-sql/ <-- to make new id - get max from ID in player table and add 1, make new id with that and email, and then add username and password
    public class Program
    {

        static void CleanRandomIds(Login sqlDat)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "DELETE FROM randommatchpool WHERE ID != 0";
            cmd.Connection = sqlDat.connection;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();

            cmd = new MySqlCommand();
            cmd.CommandText = "DELETE FROM randomid WHERE ID != 1000000000";
            cmd.Connection = sqlDat.connection;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }

        static void Main(string[] args)
        {
        
            Login sqlDat = new Login();
           
            var server = new Server();
            server.Start("http://192.168.2.19:81/", sqlDat); //port 81 is open
            Console.WriteLine("Press any key (many times) to exit...");
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();

            CleanRandomIds(sqlDat);
        }

    }
    class Server
    {
    string CasualRandomMapsPath = "D:/WampServer/tactics_by_web perma_files/CasualRandomMap/";

    string[] CasualRandomMapsA;

    void GetAllCasualMaps()
    {
        CasualRandomMapsA = Directory.GetFiles(CasualRandomMapsPath);
    }

    void SendFile(WebSocket webSocket, WebSocketReceiveResult receiveResult, string FilePath)
    {
        ArraySegment<byte> bb = new ArraySegment<byte>(File.ReadAllBytes(FilePath));
        webSocket.SendAsync(bb, WebSocketMessageType.Binary, receiveResult.EndOfMessage, CancellationToken.None);
    }

        private int count = 0;

        public Login sqlDat;

        public async void Start(string listenerPrefix, Login sqlDatIN)
        {
            GetAllCasualMaps();

            sqlDat = sqlDatIN;
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(listenerPrefix);
            listener.Start();
            Console.WriteLine("Listening...");

            while (true)
            {
                HttpListenerContext listenerContext = await listener.GetContextAsync();
                if (listenerContext.Request.IsWebSocketRequest)
                {
                    ProcessRequest(listenerContext);
                }
                else
                {
                    listenerContext.Response.StatusCode = 400;
                    listenerContext.Response.Close();
#if DEBUG
                    Console.WriteLine("connected non web socket - closing");
#endif
                }
            }
        }

        /*
        int tmpIDHandle()
        {

            return 0; //nothing
        }
        int FullIDHandle()
        {

            return 0; //nothing 
        }
        int CreateAccountHandle()
        {

            return 0; //nothing 
        }
        */
        void ReciveMapData()
        {

        }
        void SendMapData()
        {

        }

        const int REGMSG = 1000;
        const int BIGMSG = 16000;
        const int BIGBIGMSG = 50000;



        [Serializable]
        public class CharDat
        {
            public int PosX;
            public int PosY;

            public int team; //based on overlap - // 0 is null, 1 is blue, 2 is red, 3 is yellow, 4 is green, 5 is purple

            public int Hp;
            public int Atk;
            public int Def;
            public int Mov;
            public int PW; //Pri weapon id
            public int SW; //Secondary weapon id
            public int CH; //Cosmetic hat id
            public int CB; //Cosmetic body id

            public string name; //
        }

        [Serializable]
        public class CharDatL
        {
            public List<CharDat> C = new List<CharDat>();
        }

        class UserSpecificData
        {

            public List<long> players = new List<long>();
            public Dictionary<int, long> teamsOrder = new Dictionary<int, long>() { { 0, 0 } };
            public Dictionary<long, int> teamsOrderInv = new Dictionary<long, int>() { { 0, 0 } };
            public int yourOrder = 0;
            public int MatchType = 0;
            public int MapIndex = 0;
            public long paired = 0;
            public bool MatchReady = false;
            public bool CharLoadReady = false;
            public CharDatL YourCharArr = new CharDatL();
            public string Username = "";

            public string YourMapData = "";
            public bool NeedToSendMapData = false;
            public long TakeMapDataFrom = 0;
        };

        Dictionary<long, UserSpecificData> USR = new Dictionary<long, UserSpecificData>(); //id links to class with stuff


        private async void ProcessRequest(HttpListenerContext listenerContext)
        {
            WebSocket webSocket = null;

            long ID = 0; //every instance has server side controlled ID

            try
            {
                WebSocketContext webSocketContext = await listenerContext.AcceptWebSocketAsync(subProtocol: "null");

                Interlocked.Increment(ref count);
                Console.WriteLine("Processed: {0}", count);

                webSocket = webSocketContext.WebSocket;

                ArraySegment<byte> buf;

                ArraySegment<byte> bufOfCharDat;

                MySqlCommand cmd = new MySqlCommand();

                //LOOP LOGIC
                while (webSocket.State == WebSocketState.Open)
                {
                    
                    buf = new ArraySegment<byte>(new byte[REGMSG]);
                    
                    WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(buf, CancellationToken.None); //await webSocket.ReceiveAsync(buf, CancellationToken.None);

                    string resultR = Encoding.UTF8.GetString(buf.Array, 0, receiveResult.Count); //do it like this since I need to know number of bytes to copy, else I get empty string ends

                    //Console.WriteLine(resultR);

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        //Recived closed message to kill websocket - means to return we close without message
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }
                    else if (receiveResult.MessageType == WebSocketMessageType.Text)
                    {
                        ///


                        if ("CID" == resultR)
                        {
                            string email;
                            string username;
                            string password;

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            receiveResult = await webSocket.ReceiveAsync(buf, CancellationToken.None);

                            email = Encoding.UTF8.GetString(buf.Array, 0, receiveResult.Count);

                            //Check if Email is in Sql collumns, if not inside. you continue:
                            cmd = new MySqlCommand();
                            cmd.CommandText = "SELECT COUNT(email) from (SELECT email FROM player WHERE email = '" + email + "') AS T";
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;
                            MySqlDataReader reader;
                            Int64 result = (Int64)cmd.ExecuteScalar();
                            //Console.WriteLine(result);
                            if (result == 0)
                            {
                                //reader.Close();
                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("Y"));
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                receiveResult = await webSocket.ReceiveAsync(buf, CancellationToken.None);
                                username = Encoding.UTF8.GetString(buf.Array, 0, receiveResult.Count);

                                //Check if Username is in Sql collumns, if not inside. you continue:
                                cmd = new MySqlCommand();
                                cmd.CommandText = "SELECT COUNT(username) from(SELECT username FROM player WHERE username = '" + username + "') AS T";
                                cmd.Connection = sqlDat.connection;
                                cmd.CommandType = CommandType.Text;
                                result = (Int64)cmd.ExecuteScalar();
                                Console.WriteLine(username);
                                if (result == 0)
                                {
                                    //    reader.Close();
                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("Y"));
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                    receiveResult = await webSocket.ReceiveAsync(buf, CancellationToken.None);
                                    password = Encoding.UTF8.GetString(buf.Array, 0, receiveResult.Count);

                                    // make new account using MAX(ID) - or SELECT COUNT(ID+1)  FROM `player`;

                                    cmd = new MySqlCommand();
                                    cmd.CommandText = "SELECT COUNT(ID) FROM player";
                                    cmd.Connection = sqlDat.connection;
                                    cmd.CommandType = CommandType.Text;
                                    reader = cmd.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ID = reader.GetInt64(0);
                                    }


                                    cmd = new MySqlCommand();
                                    cmd.CommandText = "INSERT INTO player (ID, email, pass, username) VALUES ( (( " + ID.ToString() + ")) ,'" + email + "', '" + password + "','" + username + "')";
                                    cmd.Connection = sqlDat.connection;
                                    cmd.CommandType = CommandType.Text;
                                    cmd.ExecuteNonQuery();


                                    Console.WriteLine(ID);
                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                    USR[ID] = new UserSpecificData();

                                    USR[ID].teamsOrderInv[ID] = 0;

                                    USR[ID].Username = username;
                                }
                                else
                                {
                                    //  Console.WriteLine(result);

                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("Username already in use"));
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                }
                            }
                            else
                            {
                                //Console.WriteLine(result);

                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("Email already used"));
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            }
                        }
                        ///

                        else if ("TID" == resultR)
                        {// random id from random id table (number not taken yet)


                            cmd = new MySqlCommand();
                            cmd.CommandText = "SELECT COUNT(id) FROM randomid";
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;
                            MySqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                ID = reader.GetInt64(0) + 1000000000; //extra 1 billion for preventing overlap with new accounts
                            }
                            reader.Close();


                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);


                            cmd = new MySqlCommand();
                            cmd.CommandText = "INSERT INTO randomid (id) VALUES ( " + ID.ToString() + ")";
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();

                            USR[ID] = new UserSpecificData();

                            USR[ID].Username = ID.ToString();

                            USR[ID].teamsOrderInv[ID] = 0;
                            //TODO: at end of day or smthing I clear this random char id thing place
                        }
                        else if ("FID" == resultR)
                        {// login full id

                            string email;
                            string username;
                            string password;

                            buf = new ArraySegment<byte>(new byte[8192]);
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            receiveResult = await webSocket.ReceiveAsync(buf, CancellationToken.None);

                            email = Encoding.UTF8.GetString(buf.Array, 0, receiveResult.Count);

                            //Check if Email is in Sql collumns
                            cmd = new MySqlCommand();
                            cmd.CommandText = "SELECT COUNT(email) from (SELECT email FROM player WHERE email = '" + email + "') AS T";
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;
                            MySqlDataReader reader;
                            Int64 result = (Int64)cmd.ExecuteScalar();

                            if (result != 0)
                            {
                                //reader.Close();
                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("Y"));
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                receiveResult = await webSocket.ReceiveAsync(buf, CancellationToken.None);
                                password = Encoding.UTF8.GetString(buf.Array, 0, receiveResult.Count);
                                //Check if Password is in Sql collumn of email, if inside; you continue: 
                                cmd = new MySqlCommand();
                                cmd.CommandText = "SELECT COUNT(email) from(SELECT email,pass FROM player WHERE email = '" + email + "' AND pass = '" + password + "') AS T";
                                cmd.Connection = sqlDat.connection;
                                cmd.CommandType = CommandType.Text;
                                result = (Int64)cmd.ExecuteScalar();

                                if (result != 0)
                                {
                                    //    reader.Close();
                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("Y"));
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                    receiveResult = await webSocket.ReceiveAsync(buf, CancellationToken.None);

                                    // make new account using MAX(ID) - or SELECT COUNT(ID+1)  FROM `player`;

                                    cmd = new MySqlCommand();
                                    cmd.CommandText = "SELECT ID from(SELECT id,email,pass FROM player WHERE email = '" + email + "' AND pass = '" + password + "') AS T";
                                    cmd.Connection = sqlDat.connection;
                                    cmd.CommandType = CommandType.Text;
                                    ID = (Int64)cmd.ExecuteScalar();

                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                    await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                                    cmd = new MySqlCommand();
                                    cmd.CommandText = "SELECT username from(SELECT username,email,pass FROM player WHERE email = '" + email + "' AND pass = '" + password + "') AS T";
                                    cmd.Connection = sqlDat.connection;
                                    cmd.CommandType = CommandType.Text;
                                    username = (String)cmd.ExecuteScalar();

                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(username.ToString()));
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                    await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                                    USR[ID] = new UserSpecificData();
                                    USR[ID].Username = username;
                                    USR[ID].teamsOrderInv[ID] = 0;
                                }
                                else
                                {
                                    //  Console.WriteLine(result);

                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("Password is wrong"));
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                }
                            }

                            else
                            {
                                //Console.WriteLine(result);

                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("Email not in use"));
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            }

                        }
                        //Recived text data
                        else if (resultR == "RM")
                        {
                            USR[ID].MatchReady = false;
                            USR[ID].CharLoadReady = false;

                            USR[ID].players.Clear();
                            USR[ID].teamsOrder.Clear();
                            USR[ID].teamsOrderInv.Clear();
                            USR[ID].MatchType = 1;
                            USR[ID].paired = 0;
                            USR[ID].yourOrder = 0;
                            Object pariedO;
                            //no username in random match

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("f"));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            cmd = new MySqlCommand();
                            cmd.CommandText = "INSERT INTO randommatchpool (id) VALUES ( " + ID.ToString() + ")";
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();

                            await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            pariedO = null;

                            while (USR[ID].paired == ID || USR[ID].paired == 0)
                            {
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                try
                                {
                                    cmd = new MySqlCommand();
                                    cmd.CommandText = "SELECT ID FROM randommatchpool ORDER BY RAND ( ) LIMIT 1";
                                    cmd.Connection = sqlDat.connection;
                                    cmd.CommandType = CommandType.Text;
                                    pariedO = cmd.ExecuteScalar();
                                }
                                catch { }

                                if (pariedO != null)
                                { //- items from yours being chosen, this *is* async

                                    USR[ID].paired = (Int64)pariedO;
                                    //Console.WriteLine((Int64)pariedO);
                                }

                                await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            }

                            USR[USR[ID].paired].paired = ID;

                            cmd = new MySqlCommand();
                            cmd.CommandText = "DELETE FROM randommatchpool WHERE ID = " + ID.ToString() + " OR ID = " + USR[ID].paired.ToString(); //check for error
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                            //"DELETE FROM randommatchpool WHERE ID == "+ID.ToString()+" OR ID == "+paired.ToString()+" "

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("d"));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent


                            Console.WriteLine(USR[ID].paired + " is with " + ID);
                            //this later logic applies to more than 2 players - done for later so I need to think onto how to code less later - so this *will* be a function later
                            USR[ID].players.Add(ID);
                            USR[ID].players.Add(USR[ID].paired);

                            Random rnd = new Random();
                            int Index = 0;

                            while (USR[USR[ID].players.Max()].teamsOrder.Count == 0 && ID != USR[ID].players.Max()) { } //sync players

                            while (USR[ID].teamsOrder.Count < USR[ID].players.Count)
                            {
                                Index = rnd.Next(USR[ID].players.Count);

                                if (!USR[ID].teamsOrder.ContainsKey(Index))
                                {
                                    USR[ID].teamsOrder[Index] = USR[ID].players[Index];
                                    USR[ID].teamsOrderInv[USR[ID].players[Index]] = Index;
                                }

                            }

                            //random map index;

                            USR[ID].MapIndex = rnd.Next(CasualRandomMapsA.Length);

                            for (int i = 0; i < USR[ID].players.Count(); i++)
                            {
                                USR[ID].teamsOrder = USR[USR[ID].players.Max()].teamsOrder; //set team orders
                                USR[ID].MapIndex = USR[USR[ID].players.Max()].MapIndex;
                                USR[ID].teamsOrderInv = USR[USR[ID].players.Max()].teamsOrderInv;
                            }


                            SendFile(webSocket, receiveResult, CasualRandomMapsA[USR[ID].MapIndex]);

                            await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(USR[ID].MatchType.ToString())); //send your order for team
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(USR[ID].teamsOrderInv[ID].ToString())); //send your order for team
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            Console.WriteLine("done RM setup");
                        }
                        else if ("NTSCRR" == resultR)
                        {

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            bufOfCharDat = new ArraySegment<byte>(new byte[REGMSG]);
                            WebSocketReceiveResult RRT = await webSocket.ReceiveAsync(bufOfCharDat, CancellationToken.None); // wait until recive data to comfirm you sent

                            int stringSize = Int32.Parse(Encoding.UTF8.GetString(bufOfCharDat.Array, 0, RRT.Count));

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            string tmpString = "";

                            while(tmpString.Length < stringSize)
                            {

                                bufOfCharDat = new ArraySegment<byte>(new byte[BIGBIGMSG]);
                                RRT = await webSocket.ReceiveAsync(bufOfCharDat, CancellationToken.None); // wait until recive data to comfirm you sent

                                tmpString += Encoding.UTF8.GetString(bufOfCharDat.Array, 0, RRT.Count);
                            }
                            
                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            /*
                            string tmpCharSPath = "D:/SERVERTMPJUNK/" + ID.ToString() + "DCTMP.txt";
                            File.WriteAllBytes(tmpCharSPath, bufOfCharDat.Array);

                            BinaryFormatter bf = new BinaryFormatter();

                            FileStream file = File.Open(tmpCharSPath, FileMode.Open);

                            USR[ID].YourCharArr = (CharDat[])bf.Deserialize(file); //pass char array data into CharDat[] array {this is same array as in game}

                            file.Close();
                            */

                            //bf.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                            //Console.WriteLine(Encoding.UTF8.GetString(bufOfCharDat.Array, 0, RRT.Count));
                            USR[ID].YourCharArr = JsonConvert.DeserializeObject<CharDatL>(tmpString); //pass char array data into CharDat[] array {this is same array as in game}

                            Console.WriteLine("Char Count: " + USR[ID].YourCharArr.C.Count);

                            USR[ID].MatchReady = true;
                            //take char data and compile into 1 array later - then send back to all players and the game loads into the map data with it
                        }
                        else if ("SMDBAT1" == resultR)
                        {
                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);


                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            WebSocketReceiveResult RRT = await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            int stringSize = Int32.Parse(Encoding.UTF8.GetString(buf.Array, 0, RRT.Count));

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            string tmpString = "";

                            while (tmpString.Length < stringSize)
                            {
                                buf = new ArraySegment<byte>(new byte[BIGBIGMSG]);
                                RRT = await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                                tmpString += Encoding.UTF8.GetString(buf.Array, 0, RRT.Count);
                            }

                            USR[ID].YourMapData = tmpString;

                            //Console.WriteLine("\n\n\n\n\n\n"+receiveResult.Count+"\n\n\n\n\n\n");
                            //Console.WriteLine("\n\n\n\n\n\n");

                            //Console.WriteLine("\n\n\n\n\n\n\n" + stringSize.ToString());

                            //Console.WriteLine("\n\n\n\n\n\n\n"+USR[ID].YourMapData.Length.ToString());
                            //Console.WriteLine("\n\n\n\n\n\n" + receiveResult.Count + "\n\n\n\n\n\n");

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("h"));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            for (int i = 0; i < USR[ID].players.Count(); i++)
                            {
                                USR[USR[ID].players[i]].NeedToSendMapData = true;
                                USR[USR[ID].players[i]].TakeMapDataFrom = ID;
                            }

                        }
                        else if ("h" == resultR)
                        {// send id over to not allow cheaters :anger:
                            //Console.WriteLine("YARR"+ID.ToString());

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(USR[ID].MatchType.ToString())); //match type
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(USR[ID].teamsOrderInv[ID].ToString())); //send your order for team
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            if (USR[ID].MatchReady == true)
                            {
                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("MR"));
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);
                                await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                                //TODO: poll to check if anyone is not playing in match - kill wait room and penalize the player who failed the poll - add to new table that they left

                                //TODO: Add code here that checks if everyone is matchReady, if so then add together the arrays of character and send for players to load
                                bool tempValidateP = true;
                                for (int i = 0; i < USR[ID].players.Count(); i++)
                                {
                                    if (USR[USR[ID].players[i]].MatchReady == false)
                                    {
                                        tempValidateP = false;
                                    }
                                }
                                if (tempValidateP == true)
                                {
                                    for (int i = 0; i < USR[ID].players.Count(); i++)
                                    {
                                        USR[USR[ID].players[i]].CharLoadReady = true;
                                        USR[USR[ID].players[i]].MatchReady = false;
                                    }
                                }

                            }
                            else if (USR[ID].CharLoadReady == true)
                            {
                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("CLR"));
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);
                                await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                                CharDatL CharArrPaired = new CharDatL();

                                for (int i = 0; i < USR[ID].players.Count(); i++)
                                {
                                    Console.WriteLine("number of chars from other ID: " + USR[USR[ID].players[i]].YourCharArr.C.Count);

                                    CharArrPaired.C.AddRange(USR[USR[ID].players[i]].YourCharArr.C);

                                    USR[ID].CharLoadReady = false;
                                }

                                string JSONOut = JsonConvert.SerializeObject(CharArrPaired);

                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JSONOut));

                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            }
                            else
                            {
                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("NN"));
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            }

                            if (USR[ID].NeedToSendMapData)
                            {
                               
                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("NTSMD"));
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                
                                await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                                

                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes((USR[USR[ID].TakeMapDataFrom].YourMapData).ToString()));
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                
                                
                                await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                                
                                USR[ID].NeedToSendMapData = false;
                            }
                            else
                            {
                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("NN"));
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            }

                        }
                        else
                        {
                            //Recived binary data


                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("h"));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);




                        }
                    }
                }
            }
            //

            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "DELETE FROM randommatchpool WHERE ID = " + ID.ToString(); //check for error
                cmd.Connection = sqlDat.connection;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

            }

            if (webSocket != null) webSocket.Dispose();
            


        }

    }



    public static class HelperExtensions
    {
        public static Task GetContextAsync(this HttpListener listener)
        {
            return Task.Factory.FromAsync<HttpListenerContext>(listener.BeginGetContext, listener.EndGetContext, TaskCreationOptions.None);
        }
    }
}