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
        
            Login sqlDat = new Login(); //test con
           
            //if()

            var server = new Server();
            server.Start("http://192.168.2.19:81/"/*, sqlDat*/); //port 81 is open
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

        public async void Start(string listenerPrefix/*, Login sqlDatIN*/)
        {
            Login sqlDatIN = new Login();

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
        public class InFlightCharData
        {

            public bool HasTurn = false;
            public bool MovedAlready = false;
            public int Ability1 = 0;
            public int Ability2 = 0;
            public int RngMinPri;
            public int RngMaxPri;
            public int RngMinSec;
            public int RngMaxSec;
            public int CurW = 1;
            public bool ShowSecondary = false;
            public int AtkBuff = 0;
            public int DefBuff = 0;
            public int MovBuff = 0;
            public int HpBuff = 0;
            public int Atk2 = 0;
            public int Def2 = 0;
            public bool Dead = false;
            public int PosX;
            public int PosY;
            public int team;
            public int Hp;
            public int Atk;
            public int Def;
            public int Mov;
            public int PW;
            public int SW;
            public int CH;
            public int CB;
            public string name;

            public float startRotationX; //eulerAngles
            public float startRotationY; //eulerAngles
            public float startRotationZ; //eulerAngles

        }

        [Serializable]
        public class MapMakerVarsSaveDat
        {

            public int TileId;
            public int CDatX = -1; // -1 is null char
            public int CDatY = -1; // -1 is null char

        }

        [Serializable]
        public class LocalMatchSaveMapData
        {

            ////LMICS vars
            public string SaveMapName; //LCIMS save map name

            public bool HighestHpWin = false;
            public int HighestHpWinTurnLimit = 30;
            public bool MonumentWin = false;
            public int MonumentTurnLimit = 30;

            //store tiles -- with some indiation of char data link
            //store char dat seperately?!?
            public List<InFlightCharData> BlueChar = new List<InFlightCharData>();
            public List<InFlightCharData> RedChar = new List<InFlightCharData>();
            public List<InFlightCharData> GreenChar = new List<InFlightCharData>();
            public List<InFlightCharData> YellowChar = new List<InFlightCharData>();
            public List<InFlightCharData> PurpleChar = new List<InFlightCharData>();
            //compile into dictionary of all chars using a tuple as key for (x,y) when loading to set char in positions

            public MapMakerVarsSaveDat[][] TilesArray; //char pos is where you fetch char

            ////

            //reg vars from local match start
            public int Winner = 0;
            public bool CalcMoveDone = true;
            public int MovePhase = 0;
            public int CXPos = 0;
            public int CYPos = 0;
            public bool currentlyFighting = false;
            public int currentTurn = 0;
            public int CurrentTeamTurn = 0;
            public int TeamCount = 0;
            public int MonumentValBlue = 0;
            public int MonumentValRed = 0;
            public int MonumentValYellow = 0;
            public int MonumentValGreen = 0;
            public int MonumentValPurple = 0;
            public bool PermaHoverInfoTextOff = false;
            public string HoverInfoTMP = "";
            public bool HideTileInfo;
            public bool attackPrep = false;

            //need to convert vars
            public Tuple<int, int> HoverTupleStore; //x y pos for tuples and list
            public Tuple<int, int> SelectedAttackTarget;

            public List<Tuple<int, int>> TmpAbilityTiles = new List<Tuple<int, int>>(); //xy positions of tile to load into dictionary
            public List<Tuple<int, int>> TmpMoveTiles = new List<Tuple<int, int>>();
            public List<Tuple<int, int>> TmpAtkTiles = new List<Tuple<int, int>>();

            public int SelectedCharX; // X position
            public int SelectedCharY; // Y position

            // // is active state bools for game menu stuff
            public bool IsHoverInfoActive;
            public bool IsHoverInfoCloseButtonActive;
            public bool IsActionMenuObjActive;
            public bool IsCloseActionMenuObjActive;
            public bool IsCloseActionAtkObjActive;
            public bool IsActionAbilityInfoObjActive;
            public bool IsCloseActionAbilityObjActive;
            public bool IsAttack1Active;
            public bool AIsttack2Active;
            public bool IsAbility1Active;
            public bool IsAbility2Active;
            public bool IsMoveActive;
            public bool IsTileInfoObjActive;
            public bool IsTileInfoToggleActive;
            public bool IsAttackPrepScreenAttackerActive;
            public bool IsAttackPrepScreenDefenderActive;
            public bool IsStartToAttackButtonActive;
            public bool IsCloseActionRotateObjBackButtonActive;
            public bool IsWinConditionObjActive;
            //anim vars?

        }

        [Serializable]
        public class CharDatL
        {
            public List<CharDat> C = new List<CharDat>();
        }

        class UserSpecificData
        {
            public List<long> TmpMapID = new List<long>();
            public List<long> players = new List<long>();
            public Dictionary<int, long> teamsOrder = new Dictionary<int, long>() { { 0, 0 } };
            public Dictionary<long, int> teamsOrderInv = new Dictionary<long, int>() { { 0, 0 } };
            public int yourOrder = 0;
            public int MatchType = 0;
            public int MapIndex = 0;
            public long paired = 0;
            public long[] pairedL;
            public bool MatchReady = false;
            public bool CharLoadReady = false;
            public CharDatL YourCharArr = new CharDatL();
            public string Username = "";

            public string YourMapName = "";

            public long MapID = 0;
            public string YourMapData = "";
            public bool NeedToSendMapData = false;
            public long TakeMapDataFrom = 0;

            public bool lookingForFriends = false;
        };

        Dictionary<long, UserSpecificData> USR = new Dictionary<long, UserSpecificData>(); //id links to class with stuff

        long NumberOfMapForPID(long ID, MySqlCommand cmd)
        {
            cmd = new MySqlCommand();
            cmd.CommandText = "SELECT COUNT(AuthID) FROM uploadmaps where AuthID = "+ID.ToString();
            cmd.Connection = sqlDat.connection;
            cmd.CommandType = CommandType.Text;

            return (Int64)cmd.ExecuteScalar();
        }

        void uploadMapToSQL(long ID, string MapName, int MapTeamCount, string MapDat, MySqlCommand cmd)
        {

            if (NumberOfMapForPID(ID, cmd) <101 ) { //count AuthID - if maps is less than 100 - you can make - limit 100, 
                cmd = new MySqlCommand();
                cmd.CommandText = "INSERT INTO uploadmaps (AuthID,MapID,MapName,JsonMap, Downloads, PlayerCount, Comment) VALUES ( " + ID.ToString() + "," + "((SELECT COUNT(MapID) FROM uploadmaps AS T))" + ",'" + MapName + "','" + MapDat + "',0,"+ MapTeamCount.ToString()+",'')"; //comment is a json of string array/list
                cmd.Connection = sqlDat.connection;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }

        void deleteMapFromSQLPlayer(long ID, int MapIndex, MySqlCommand cmd)
        {
            USR[ID].MapID = 0;
            //checked for map id and pass map name to add map - also save map id when loaded
            cmd = new MySqlCommand(); //get all maps in mapsavedata 0 ID1, ID2, ID3, ID4, ID5, MapName, JsonString, MapID    -   then send how many there are - resize array on client, then for that range do a send get loop to fill map names on client - select, and then it fetches JsonString from server to load game from 
            cmd.CommandText = "SELECT MapID FROM (SELECT * FROM `uploadmaps` WHERE AuthID = " + ID + ") AS T";
            cmd.Connection = sqlDat.connection;
            cmd.CommandType = CommandType.Text;
            MySqlDataReader sqlReader = cmd.ExecuteReader();

            List<long> MapIDL = new List<long>();

            while (sqlReader.Read()) //iterate rows
            {//Get values
                MapIDL.Add(sqlReader.GetInt64(0)); // map name
            }
            sqlReader.Close();
            try
            {
                long MapID = MapIDL[MapIndex];

                cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE uploadmaps SET JsonMap = '',AuthID = 0, MapName = '', Downloads = 0,PlayerCount = 0, Comment='' where MapID = " + MapID.ToString();
                cmd.Connection = sqlDat.connection;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            catch { }

        }

        void InsertMapIDIntoSaveMapID(long ID, long MapID, MySqlCommand cmd)
        {
            
            List<long> tmpIDs = new List<long> { 0, 0, 0, 0, 0 };
            for (int i = 0; i < USR[ID].players.Count; i++)
            {
                tmpIDs[i] = USR[ID].players[i];
            }
            cmd = new MySqlCommand();
            cmd.CommandText = "INSERT INTO mapsavedata (ID1,ID2,ID3,ID4,ID5,MapName,JsonString,MapID) VALUES ( " + tmpIDs[0].ToString() + "," + tmpIDs[1].ToString() + "," + tmpIDs[2].ToString() + "," + tmpIDs[3].ToString() + "," + tmpIDs[4].ToString() + ",'" + USR[ID].YourMapName + "','" + USR[ID].YourMapData + "'," + USR[ID].MapID + ")";
            cmd.Connection = sqlDat.connection; 
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }

        long GetNextMapID(long ID, MySqlCommand cmd)
        {
            cmd = new MySqlCommand();
            cmd.CommandText = "SELECT COUNT(MapID) FROM mapsavedata";
            cmd.Connection = sqlDat.connection;
            cmd.CommandType = CommandType.Text;
            
            return (Int64)cmd.ExecuteScalar();

        }

        void UpdateMapIDFromSaveMapID(long ID, MySqlCommand cmd)
        {
            cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE mapsavedata SET JsonString = '" + USR[ID].YourMapData + "' WHERE MapID = " + USR[ID].MapID;
            cmd.Connection = sqlDat.connection;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }

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
                bool firstTID = false;

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
                    
                    //first message must be TID

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

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            receiveResult = await webSocket.ReceiveAsync(buf, CancellationToken.None);

                            email = Encoding.UTF8.GetString(buf.Array, 0, receiveResult.Count);

                            //Check if Email is in Sql collumns, if not inside. you continue:


                            cmd = new MySqlCommand();
                            cmd.CommandText = "SELECT COUNT(email) from (SELECT email FROM player WHERE email = '" + email + "') AS T";
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;

                            Int64 result = (Int64)cmd.ExecuteScalar();
                            //Console.WriteLine(result);
                            if (result == 0)
                            {
                                //reader.Close();
                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("Y"));
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                buf = new ArraySegment<byte>(new byte[REGMSG]);
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

                                    buf = new ArraySegment<byte>(new byte[REGMSG]);
                                    receiveResult = await webSocket.ReceiveAsync(buf, CancellationToken.None);
                                    password = Encoding.UTF8.GetString(buf.Array, 0, receiveResult.Count);

                                    // make new account using MAX(ID) - or SELECT COUNT(ID+1)  FROM `player`;

                                    cmd = new MySqlCommand();
                                    cmd.CommandText = "SELECT COUNT(ID) FROM player";
                                    cmd.Connection = sqlDat.connection;
                                    cmd.CommandType = CommandType.Text;
                                    MySqlDataReader reader = cmd.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ID = reader.GetInt64(0);
                                    }
                                    reader.Close();

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
                            firstTID = true;

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
                            Console.WriteLine("send ID");

                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            receiveResult = await webSocket.ReceiveAsync(buf, CancellationToken.None);


                            cmd = new MySqlCommand();
                            cmd.CommandText = "INSERT INTO randomid (id) VALUES ( " + ID.ToString() + ")";
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();

                            USR[ID] = new UserSpecificData();

                            Console.WriteLine("Now We have Rand ID: " + ID.ToString());

                            USR[ID].Username = ID.ToString();

                            USR[ID].teamsOrderInv[ID] = 0;


                            //TODO: at end of day or smthing I clear this random char id thing place
                        }
                        else if ("FID" == resultR)
                        {// login full id

                            string email;
                            string username;
                            string password;

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);
                            
                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            receiveResult = await webSocket.ReceiveAsync(buf, CancellationToken.None);

                            email = Encoding.UTF8.GetString(buf.Array, 0, receiveResult.Count);

                            //Check if Email is in Sql collumns
                            cmd = new MySqlCommand();
                            cmd.CommandText = "SELECT COUNT(email) from (SELECT email FROM player WHERE email = '" + email + "') AS T";
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;
                            
                            Int64 result = (Int64)cmd.ExecuteScalar();

                            if (result != 0)
                            {
                                //reader.Close();
                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("Y"));
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                buf = new ArraySegment<byte>(new byte[REGMSG]);
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

                                    buf = new ArraySegment<byte>(new byte[REGMSG]);
                                    receiveResult = await webSocket.ReceiveAsync(buf, CancellationToken.None);

                                    // make new account using MAX(ID) - or SELECT COUNT(ID+1)  FROM `player`;

                                    cmd = new MySqlCommand();
                                    cmd.CommandText = "SELECT ID from(SELECT id,email,pass FROM player WHERE email = '" + email + "' AND pass = '" + password + "') AS T";
                                    cmd.Connection = sqlDat.connection;
                                    cmd.CommandType = CommandType.Text;
                                    ID = (Int64)cmd.ExecuteScalar();

                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                    buf = new ArraySegment<byte>(new byte[REGMSG]);
                                    await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                                    cmd = new MySqlCommand();
                                    cmd.CommandText = "SELECT username from(SELECT username,email,pass FROM player WHERE email = '" + email + "' AND pass = '" + password + "') AS T";
                                    cmd.Connection = sqlDat.connection;
                                    cmd.CommandType = CommandType.Text;
                                    username = (String)cmd.ExecuteScalar();

                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(username.ToString()));
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                    buf = new ArraySegment<byte>(new byte[REGMSG]);
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

                        else if (resultR == "FMPU")
                        {
                            USR[ID].MapID = 0;
                            //checked for map id and pass map name to add map - also save map id when loaded
                            cmd = new MySqlCommand(); //get all maps in mapsavedata 0 ID1, ID2, ID3, ID4, ID5, MapName, JsonString, MapID    -   then send how many there are - resize array on client, then for that range do a send get loop to fill map names on client - select, and then it fetches JsonString from server to load game from 
                            cmd.CommandText = "SELECT MapName FROM (SELECT * FROM `uploadmaps` WHERE AuthID = " + ID +") AS T";
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;
                            MySqlDataReader sqlReader = cmd.ExecuteReader();

                            //List<string> JsonDat = new List<string>();
                            //List<long> MapID = new List<long>();
                            List<string> MapNameDat = new List<string>();

                            while (sqlReader.Read()) //iterate rows
                            {//Get values
                                //JsonDat.Add(sqlReader.GetString(0)); // Json
                                //MapID.Add(sqlReader.GetInt64(1)); // map id
                                MapNameDat.Add(sqlReader.GetString(0)); // map name
                            }
                            sqlReader.Close();


                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(MapNameDat))); //<List<string>>
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                        }

                        else if (resultR == "FMIP")
                        {
                            USR[ID].MapID = 0;
                            //checked for map id and pass map name to add map - also save map id when loaded
                            cmd = new MySqlCommand(); //get all maps in mapsavedata 0 ID1, ID2, ID3, ID4, ID5, MapName, JsonString, MapID    -   then send how many there are - resize array on client, then for that range do a send get loop to fill map names on client - select, and then it fetches JsonString from server to load game from 
                            cmd.CommandText = "SELECT JsonString, MapID, MapName FROM (SELECT * FROM `mapsavedata` WHERE ID1 = " + ID + " || ID2 = " + ID + " || ID3 = " + ID + " || ID4 = " + ID + " || ID5 = " + ID + ") AS T";
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;
                            MySqlDataReader sqlReader = cmd.ExecuteReader();

                            List<string> JsonDat = new List<string>();
                            List<long> MapID = new List<long>();
                            List<string> MapNameDat = new List<string>();

                            while (sqlReader.Read()) //iterate rows
                            {//Get values
                                JsonDat.Add(sqlReader.GetString(0)); // Json
                                MapID.Add(sqlReader.GetInt64(1)); // map id
                                MapNameDat.Add(sqlReader.GetString(2)); // map name
                            }
                            sqlReader.Close();


                           buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(MapNameDat))); //<List<string>>
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                        }
                        else if (resultR == "SBAGMPU")
                        {
                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("f")); //<List<string>>
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            USR[ID].MapID = 0;
                            //checked for map id and pass map name to add map - also save map id when loaded
                            cmd = new MySqlCommand(); //get all maps in mapsavedata 0 ID1, ID2, ID3, ID4, ID5, MapName, JsonString, MapID    -   then send how many there are - resize array on client, then for that range do a send get loop to fill map names on client - select, and then it fetches JsonString from server to load game from 
                            cmd.CommandText = "SELECT JsonMap, MapID, MapName FROM (SELECT * FROM `uploadmaps` WHERE AuthID = " + ID + ") AS T";
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;
                            MySqlDataReader sqlReader = cmd.ExecuteReader();

                            List<string> JsonDat = new List<string>();
                            List<long> MapID = new List<long>();
                            List<string> MapNameDat = new List<string>();

                            while (sqlReader.Read()) //iterate rows
                            {//Get values
                                JsonDat.Add(sqlReader.GetString(0)); // Json
                                MapID.Add(sqlReader.GetInt64(1)); // map id
                                MapNameDat.Add(sqlReader.GetString(2)); // map name
                            }
                            sqlReader.Close();

                            buf = new ArraySegment<byte>(new byte[REGMSG]);

                            receiveResult = await webSocket.ReceiveAsync(buf, CancellationToken.None); //await webSocket.ReceiveAsync(buf, CancellationToken.None);

                            int MIDTMP = Int32.Parse(Encoding.UTF8.GetString(buf.Array, 0, receiveResult.Count));

                            USR[ID].MapID = MapID[MIDTMP];

                            USR[ID].YourMapData = JsonDat[MIDTMP];

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(USR[ID].YourMapData)); //<List<string>>
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            USR[ID].MatchType = 1;

                            await webSocket.ReceiveAsync(buf, CancellationToken.None);
                        }

                        else if (resultR == "SBAGMIP")
                        {
                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("f")); //<List<string>>
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            USR[ID].MapID = 0;
                            //checked for map id and pass map name to add map - also save map id when loaded
                            cmd = new MySqlCommand(); //get all maps in mapsavedata 0 ID1, ID2, ID3, ID4, ID5, MapName, JsonString, MapID    -   then send how many there are - resize array on client, then for that range do a send get loop to fill map names on client - select, and then it fetches JsonString from server to load game from 
                            cmd.CommandText = "SELECT JsonString, MapID, MapName FROM (SELECT * FROM `mapsavedata` WHERE ID1 = " + ID + " || ID2 = " + ID + " || ID3 = " + ID + " || ID4 = " + ID + " || ID5 = " + ID + ") AS T";
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;
                            MySqlDataReader sqlReader = cmd.ExecuteReader();

                            List<string> JsonDat = new List<string>();
                            List<long> MapID = new List<long>();
                            List<string> MapNameDat = new List<string>();

                            while (sqlReader.Read()) //iterate rows
                            {//Get values
                                JsonDat.Add(sqlReader.GetString(0)); // Json
                                MapID.Add(sqlReader.GetInt64(1)); // map id
                                MapNameDat.Add(sqlReader.GetString(2)); // map name
                            }
                            sqlReader.Close();

                            buf = new ArraySegment<byte>(new byte[REGMSG]);

                            receiveResult = await webSocket.ReceiveAsync(buf, CancellationToken.None); //await webSocket.ReceiveAsync(buf, CancellationToken.None);

                            int MIDTMP = Int32.Parse(Encoding.UTF8.GetString(buf.Array, 0, receiveResult.Count));

                            USR[ID].MapID = MapID[MIDTMP];

                            USR[ID].YourMapData = JsonDat[MIDTMP];

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(USR[ID].YourMapData)); //<List<string>>
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            USR[ID].MatchType = 1;
                            
                            await webSocket.ReceiveAsync(buf, CancellationToken.None);
                        }
                        //Recived text data
                        else if (resultR == "RM")
                        {
                            USR[ID].MapID = 0;
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
                        else if ("GMDF" == resultR)
                        {
                            USR[ID].MapID = 0;
                            USR[ID].MatchReady = false;
                            USR[ID].CharLoadReady = false;

                            USR[ID].players.Clear();
                            USR[ID].teamsOrder.Clear();
                            USR[ID].teamsOrderInv.Clear();
                            USR[ID].MatchType = 1;
                            USR[ID].paired = 0;
                            USR[ID].yourOrder = 0;
                            USR[ID].YourMapData = "";

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            WebSocketReceiveResult RRT = await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            USR[ID].pairedL = new long[Int32.Parse(Encoding.UTF8.GetString(buf.Array, 0, RRT.Count))]; //player count is set
                            Array.Fill(USR[ID].pairedL, 0);
                            USR[ID].pairedL[0] = ID;


                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[BIGBIGMSG]);
                            RRT = await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            int stringSize = Int32.Parse(Encoding.UTF8.GetString(buf.Array, 0, RRT.Count));

                            string tmpString = "";

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            while (tmpString.Length < stringSize)
                            {
                                buf = new ArraySegment<byte>(new byte[BIGBIGMSG]);
                                RRT = await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                                tmpString += Encoding.UTF8.GetString(buf.Array, 0, RRT.Count);
                            }

                            //Console.WriteLine(tmpString);


                            USR[ID].YourMapData = tmpString;

                            USR[ID].lookingForFriends = true; // if looking is true, you try to insert your self into this player hosting ID's
                                                              //USR[ID].pairedL[0]
                            bool tmpDone = true;

                            while (USR[ID].lookingForFriends)
                            {
                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("f"));
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                buf = new ArraySegment<byte>(new byte[REGMSG]);
                                await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                                tmpDone = true;

                                for (int i = 0; i < USR[ID].pairedL.Length; i++)
                                {

                                    //   USR[ID].lookingForFriends = false;

                                    //Console.WriteLine(USR[ID].pairedL[i].ToString());

                                    if (USR[ID].pairedL[i] == 0)
                                    {
                                        tmpDone = false;
                                        //USR[ID].lookingForFriends = true;
                                        i = USR[ID].pairedL.Length;
                                    }
                                }
                                if (tmpDone != false)
                                {
                                    USR[ID].lookingForFriends = false;
                                }
                            }
                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("g"));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            //now main can load map

                            for (int i = 0; i < USR[ID].pairedL.Length; i++)
                            {

                                for (int ii = 0; ii < USR[ID].pairedL.Length; ii++)
                                {
                                    USR[USR[ID].pairedL[i]].players.Add(USR[ID].pairedL[ii]);
                                    //                                    USR[USR[ID].pairedL[i]].pairedL[ii] = USR[ID].pairedL[i]; // teams are now synced
                                }
                            }

                            Random rnd = new Random();
                            int Index = 0;

                            while (USR[ID].teamsOrder.Count < USR[ID].players.Count)
                            {
                                Index = rnd.Next(USR[ID].players.Count);

                                if (!USR[ID].teamsOrder.ContainsKey(Index))
                                {
                                    USR[ID].teamsOrder[Index] = USR[ID].players[Index];
                                    USR[ID].teamsOrderInv[USR[ID].players[Index]] = Index;
                                }

                            }

                            for (int i = 0; i < USR[ID].players.Count(); i++)
                            {
                                USR[USR[ID].players[i]].teamsOrder = USR[ID].teamsOrder; //set team orders
                                USR[USR[ID].players[i]].teamsOrderInv = USR[ID].teamsOrderInv;
                                USR[USR[ID].players[i]].YourMapData = USR[ID].YourMapData;
                            }

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(USR[ID].YourMapData)); //send your map to play with to self... no idea why
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(USR[ID].teamsOrderInv[ID].ToString())); //send your order for team
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            USR[ID].MapID = GetNextMapID(ID, cmd);

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(USR[ID].teamsOrderInv[ID].ToString())); //send your order for team
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            USR[ID].YourMapName = Encoding.UTF8.GetString(buf.Array, 0, receiveResult.Count);

                            InsertMapIDIntoSaveMapID(ID, USR[ID].MapID, cmd);

                            Console.WriteLine("done FR setup");
                        }

                        else if ("JFMS" == resultR)
                        { // join friend match start
                            USR[ID].MapID = 0;
                            USR[ID].MatchReady = false;
                            USR[ID].CharLoadReady = false;

                            USR[ID].players.Clear();
                            USR[ID].teamsOrder.Clear();
                            USR[ID].teamsOrderInv.Clear();
                            USR[ID].MatchType = 1;
                            USR[ID].paired = 0;
                            USR[ID].yourOrder = 0;
                            USR[ID].YourMapData = "";

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            WebSocketReceiveResult RRT = await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data  - you get id of player you want to join

                            long idToJoin = Int32.Parse(Encoding.UTF8.GetString(buf.Array, 0, RRT.Count));

                            //Console.WriteLine(idToJoin.ToString());
                            //Console.WriteLine(USR[idToJoin].lookingForFriends);
                            //Console.WriteLine(USR.ContainsKey(idToJoin));

                            if (USR.ContainsKey(idToJoin) && USR[idToJoin].lookingForFriends)
                            {
                                bool gotToSendID = false;
                                for (int i = 0; i < USR[idToJoin].pairedL.Length; i++)
                                {
                                    if (USR[idToJoin].pairedL[i] == 0)
                                    {
                                        gotToSendID = true;
                                        USR[idToJoin].pairedL[i] = ID; //you are joined in their match now - match starter sets every one at the end to the same pairedL array values
                                        i = USR[idToJoin].pairedL.Length;
                                    }
                                }
                                if (gotToSendID)
                                {
                                    Console.WriteLine("joined friend");
                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("GOOD"));
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                    await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent


                                    while (USR[ID].teamsOrderInv.Count == 0)
                                    {
                                        //Console.WriteLine("waiting");
                                        buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("g"));
                                        await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);
                                        // wait until main player is ready and passed data
                                        await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                                    }
                                    Console.WriteLine("waiting done");

                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("u"));
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                    await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent - for the rand f

                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(USR[ID].YourMapData)); //send your map to play with to self... no idea why
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                    await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(USR[ID].teamsOrderInv[ID].ToString())); //send your order for team
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                                    await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent


                                    //now stall these playes with loop of stuff until main player has enough players 

                                }
                                else
                                {
                                    Console.WriteLine("Can't join friend 2");
                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("BAD"));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Can't join friend 1");
                                buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("BAD"));
                                await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);
                            }



                        }
                        else if ("DUMP" == resultR)
                        {
                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            WebSocketReceiveResult RRT = await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            deleteMapFromSQLPlayer(ID, Int32.Parse(Encoding.UTF8.GetString(buf.Array, 0, RRT.Count)), cmd);

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                        }

                        else if("UPMAP" == resultR)
                        {
                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            WebSocketReceiveResult RRT = await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            string MapNameFull = Encoding.UTF8.GetString(buf.Array, 0, RRT.Count);

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            RRT = await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            int MapTeamCount = Int32.Parse(Encoding.UTF8.GetString(buf.Array, 0, RRT.Count));
                            
                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            RRT = await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            int stringSize = Int32.Parse(Encoding.UTF8.GetString(buf.Array, 0, RRT.Count));

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            string tmpString = "";

                            while (tmpString.Length < stringSize)
                            {

                                bufOfCharDat = new ArraySegment<byte>(new byte[BIGBIGMSG]);
                                RRT = await webSocket.ReceiveAsync(bufOfCharDat, CancellationToken.None); // wait until recive data to comfirm you sent

                                tmpString += Encoding.UTF8.GetString(bufOfCharDat.Array, 0, RRT.Count);
                            }

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);


                            uploadMapToSQL(ID, MapNameFull, MapTeamCount, tmpString, cmd);

                        }

                        else if ("PGDMAP" == resultR)
                        {
                            string SearchString = "";
                            int TeamCount = 0;
                            int SearchType = 0;

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            WebSocketReceiveResult RRT = await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            SearchString = Encoding.UTF8.GetString(buf.Array, 0, RRT.Count);
                            //int stringSize = Int32.Parse(Encoding.UTF8.GetString(bufOfCharDat.Array, 0, RRT.Count));

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            RRT = await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            TeamCount = Int32.Parse(Encoding.UTF8.GetString(buf.Array, 0, RRT.Count));

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            RRT = await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            SearchType = Int32.Parse(Encoding.UTF8.GetString(buf.Array, 0, RRT.Count));

                            cmd = new MySqlCommand(); //get all maps in mapsavedata 0 ID1, ID2, ID3, ID4, ID5, MapName, JsonString, MapID    -   then send how many there are - resize array on client, then for that range do a send get loop to fill map names on client - select, and then it fetches JsonString from server to load game from 

                            if (SearchType == 0)
                            {
                                cmd.CommandText = "SELECT MapName, Downloads, MapID from uploadmaps WHERE MapName like '" + SearchString + "%'  and PlayerCount = "+TeamCount.ToString()+" LIMIT 30";
                            }
                            else if(SearchType == 1)
                            {
                                cmd.CommandText = "SELECT MapName, Downloads, MapID from uploadmaps WHERE MapID = " + SearchString + "  and PlayerCount = " + TeamCount.ToString() + " LIMIT 3";
                            }
                            else if(SearchType == 2)
                            {
                                cmd.CommandText = "SELECT MapName, Downloads, MapID from uploadmaps WHERE AuthID = " + SearchString + "  and PlayerCount = " + TeamCount.ToString() + " LIMIT 100";
                            }
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;
                            MySqlDataReader sqlReader = cmd.ExecuteReader();

                            List<string> MapName = new List<string>();
                            List<long> Downloads = new List<long>();
                            USR[ID].TmpMapID.Clear();

                            while (sqlReader.Read()) //iterate rows
                            {//Get values
                                MapName.Add(sqlReader.GetString(0)); // map name
                                Downloads.Add(sqlReader.GetInt64(1));
                                USR[ID].TmpMapID.Add(sqlReader.GetInt64(2));
                            }
                            sqlReader.Close();


                            for(int i = 0; i < MapName.Count; i++)
                            {
                                MapName[i] += "\nDownloads: " + Downloads[i].ToString();
                            }

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes( JsonConvert.SerializeObject(MapName) ));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            

                        }
                        else if ("GDMMAP" == resultR)
                        {
                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(ID.ToString()));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);

                            buf = new ArraySegment<byte>(new byte[REGMSG]);
                            WebSocketReceiveResult RRT = await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent

                            int selectedI = Int32.Parse(Encoding.UTF8.GetString(buf.Array, 0, RRT.Count));

                            
                            cmd = new MySqlCommand(); //get all maps in mapsavedata 0 ID1, ID2, ID3, ID4, ID5, MapName, JsonString, MapID    -   then send how many there are - resize array on client, then for that range do a send get loop to fill map names on client - select, and then it fetches JsonString from server to load game from 
                            cmd.CommandText = "SELECT jSONmAP FROM uploadmaps WHERE MapID = " + USR[ID].TmpMapID[selectedI].ToString();
                            cmd.Connection = sqlDat.connection;
                            cmd.CommandType = CommandType.Text;
                            MySqlDataReader sqlReader = cmd.ExecuteReader();

                            string MD = ""; 

                            while (sqlReader.Read()) //iterate rows
                            {//Get values
                                MD = sqlReader.GetString(0); // map name
                            }
                            sqlReader.Close();
                            //counts as download - add if below certain count

                            buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes(MD));
                            await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);
                            
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

                            while (tmpString.Length < stringSize)
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

                            if (USR[ID].MapID != 0)
                            {

                                cmd = new MySqlCommand();
                                cmd.CommandText = "SELECT MapID FROM mapsavedata WHERE MapID = +" + USR[ID].MapID.ToString();
                                cmd.Connection = sqlDat.connection;
                                cmd.CommandType = CommandType.Text;
                                long result = (Int64)cmd.ExecuteScalar();

                                if (result == 0)
                                {
                                    LocalMatchSaveMapData NameRead = JsonConvert.DeserializeObject<LocalMatchSaveMapData>(USR[ID].YourMapData); //pass char array data into CharDat[] array {this is same array as in game}
                                    USR[ID].YourMapName = NameRead.SaveMapName;

                                    InsertMapIDIntoSaveMapID(ID, USR[ID].MapID, cmd);
                                }
                                else
                                {
                                    UpdateMapIDFromSaveMapID(ID, cmd);
                                }

                                //      then insert with using map id into mapsavedata the map data
                            }

                        }
                        else if ("h" == resultR && firstTID == true)
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
                                if (USR[ID].MapID == 0)
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
                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes("NTSMD"));
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);


                                    await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent



                                    buf = new ArraySegment<byte>(Encoding.UTF8.GetBytes((USR[USR[ID].TakeMapDataFrom].YourMapData).ToString()));
                                    await webSocket.SendAsync(buf, WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);



                                    await webSocket.ReceiveAsync(buf, CancellationToken.None); // wait until recive data to comfirm you sent


                                    USR[ID].NeedToSendMapData = false;
                                }
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

                try
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "DELETE FROM randommatchpool WHERE ID = " + ID.ToString(); //check for error
                    cmd.Connection = sqlDat.connection;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                catch { }
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
