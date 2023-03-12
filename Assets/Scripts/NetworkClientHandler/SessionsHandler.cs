using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NetworkClientHandler
{
    public class SessionsDataRef
    {
        public const string PLAYER_1 = "player1";
        public const string PLAYER_2 = "player2";
        public const string GAME_STATE = "gameState";
        public const string PLAYER_COUNT = "playerCount";
    }

    public class SessionsHandler : MonoBehaviour
    {

        [SerializeField] string m_dbSession = "stcrunner_sessions";

        DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        protected bool isFirebaseInitialized = false;

        //public static Action<bool> OnStartPlaying;

        //MERVIN ADDED
        public static Action<int> OnInitializeGame;     //Set how many players
        public static Action OnStartGame;               // Set to GameState.PLAYING
        public static Action OnRestartGame;               // Set to GameState.IDLE

        #region PLAYER_Movement
        public static Action<int> OnMovePlayer1;
        public static Action<string> OnPlayer1SetName;

        public static Action<int> OnMovePlayer2;
        public static Action<string> OnPlayer2SetName;
        #endregion PLAYER_Movement

        private FirebaseApp m_App;
        private FirebaseDatabase m_DB;
        private DatabaseReference m_sessionsDB;

        private int m_playerCount = 0;

        private GameState m_currentGameState = GameState.MAIN_MENU;

        // Start is called before the first frame update
        void Start()
        {
            GameManager.instance.onChangedGameState += OnGameStateChanged;

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }

        // Initialize the Firebase database:
        protected void InitializeFirebase()
        {
            m_App = FirebaseApp.DefaultInstance;
            m_DB = FirebaseDatabase.DefaultInstance;

            m_sessionsDB = m_DB.GetReference(m_dbSession);
            m_sessionsDB.ChildAdded += HandleSessionsChildAdded;
            m_sessionsDB.ChildChanged += HandleSessionsChildChanged;

            //m_registrationsDB = m_DB.GetReference(DB_REGISTRATIONS);

            isFirebaseInitialized = true;
        }

        #region Listeners for database table: stcrunner_sessions
        private void HandleSessionsChildAdded(object sender, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            if (args.Snapshot != null)
            {
                Debug.Log("dbsessions.onChildAdded = " + args.Snapshot.Key);
                switch (args.Snapshot.Key)
                {
                    case SessionsDataRef.PLAYER_1:
                        //Debug.Log("[PLAYER_1 :: added] uuid = " + args.Snapshot.Child("name").Value);

                        string player1Name = args.Snapshot.Child("name").Value.ToString();
                        OnPlayer1SetName?.Invoke(player1Name); //Initialize Player when name is added
                        break;

                    case SessionsDataRef.PLAYER_2:
                        //Debug.Log("[PLAYER_2 :: added] uuid = " + args.Snapshot.Child("name").Value);

                        string player2Name = args.Snapshot.Child("name").Value.ToString();
                        OnPlayer2SetName?.Invoke(player2Name); //Initialize Player when name is added
                        break;
                }
            }
        }
        private void HandleSessionsChildChanged(object sender, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            if (args.Snapshot != null)
            {
                //Debug.Log("dbsessions.onChildChanged = " + args.Snapshot.Key + " | " + args.Snapshot.Value);
                switch (args.Snapshot.Key)
                {
                    case SessionsDataRef.PLAYER_COUNT:
                        {
                            m_playerCount = int.Parse(args.Snapshot.Value.ToString());
                            if (m_playerCount > 0)
                            {
                                OnInitializeGame?.Invoke(m_playerCount);
                            }
                            break;
                        }

                    case SessionsDataRef.GAME_STATE:
                        {
                            string gameState = (string)args.Snapshot.Value;
                            switch (gameState)
                            {
                                case "WAITING":
                                    break;

                                case "PLAYING":
                                    OnStartGame?.Invoke();
                                    break;

                                case "END":
                                    break;

                                case "IDLE":
                                    if (m_currentGameState == GameState.GAME_PLAYING)
                                    {
                                        OnRestartGame?.Invoke();
                                    }
                                    break;
                            }
                            break;
                        }

                    case SessionsDataRef.PLAYER_1:
                        {
                            OnMovePlayer1?.Invoke(int.Parse(args.Snapshot.Child("currentPosition").Value.ToString()));
                            break;
                        }
                    case SessionsDataRef.PLAYER_2:
                        {
                            OnMovePlayer2?.Invoke(int.Parse(args.Snapshot.Child("currentPosition").Value.ToString()));
                            break;
                        }
                }
            }
        }
        #endregion Listeners for database table: stcrunner_sessions

        private void OnGameStateChanged(GameState pState)
        {
            //Debug.Log("SessionClientHandler.OnGameOver() ::--> " + pState);
            m_currentGameState = pState;

            if (m_currentGameState == GameState.GAME_OVER)
            {
                // Set database to gameState = END
                m_sessionsDB.Child(SessionsDataRef.GAME_STATE).SetValueAsync("END");
            }
            else if (m_currentGameState == GameState.LEADERBOARD)
            {
                // Set database to gameState = IDLE
                m_sessionsDB.Child(SessionsDataRef.GAME_STATE).SetValueAsync("IDLE");
            }
            return;
        }

        void OnDestroy()
        {
            m_sessionsDB.ChildAdded -= HandleSessionsChildAdded;
            m_sessionsDB.ChildChanged -= HandleSessionsChildChanged;

            m_DB.GoOffline();
            m_App.Dispose();
        }
    }

    [System.Serializable]
    public class PlayerSessionData
    {
        public string path;
        public string uuid;
        public int currentPosition;
        public string moveTo;

        public PlayerSessionData(string p_sessionPath, string p_uuid, int p_currentPos, string p_moveTo)
        {
            path = p_sessionPath;
            uuid = p_uuid;
            currentPosition = p_currentPos;
            moveTo = p_moveTo;
        }

        public static PlayerSessionData CreateFromJson(string json_string)
        {
            return JsonUtility.FromJson<PlayerSessionData>(json_string);
        }
    }
}

