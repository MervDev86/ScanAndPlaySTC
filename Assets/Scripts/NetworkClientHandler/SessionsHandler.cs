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
        public static string IS_PLAYING = "/isPlaying";
    }

    public class SessionsHandler : MonoBehaviour
    {

        //public static SessionsHandler instance;

        public static Action<bool> OnStartPlaying;
        
        //MERVIN ADDED
        public static Action<int> OnInitializeGame; //Set how many players

        #region PLAYER_Movement
        public static Action<int> OnMovePlayer1;
        public static Action<string> OnPlayer1SetName;
        
        public static Action<int> OnMovePlayer2;
        public static Action<string> OnPlayer2SetName;


        #endregion PLAYER_Movement


        private static string DB_SESSIONS = "stcrunner_sessions";
        private static string DB_REGISTRATIONS = "stcrunner_registrations";

        private static string REF_SESSION_ISPLAYING = "/isPlaying";
        private static string REF_SESSION_ISMULTIPLAYER = "/isMultiplayer";

        private static string REF_SESSION_PLAYER1 = "/player1";
        private static string REF_SESSION_PLAYER2 = "/player2";

        private FirebaseApp m_App;
        private FirebaseDatabase m_DB;

        private DatabaseReference m_sessionsDB;
        private DatabaseReference m_registrationsDB;
        private DatabaseReference m_refCurrentPosP1;
        private DatabaseReference m_refUuidPosP1;
        private DatabaseReference m_refNameP1;

        private bool m_isPlaying = false;
        private bool m_isMultiplayer = false;


        //private DatabaseReference m_player1;

        DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        protected bool isFirebaseInitialized = false;


        private static string m_namePlayer1 = "";
        public static string GetPlayerName1()
        {
            return m_namePlayer1;
        }

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

            m_sessionsDB = m_DB.GetReference(DB_SESSIONS);
            m_sessionsDB.ChildAdded += HandleSessionsChildAdded;
            m_sessionsDB.ChildChanged += HandleSessionsChildChanged;

            m_registrationsDB = m_DB.GetReference(DB_REGISTRATIONS);

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
                switch (args.Snapshot.Key)
                {
                    case "player1":
                        //Debug.Log("[PLAYER_1 :: added] uuid = " + args.Snapshot.Child("uuid").Value);
                        string uuidP1 = (string) args.Snapshot.Child("uuid").Value;
                        m_registrationsDB.Child(uuidP1).GetValueAsync().ContinueWithOnMainThread(task =>
                        {
                            if (task.IsCompleted)
                            {
                                string player1Name = (string)task.Result.Child("firstname").Value;
                                OnPlayer1SetName?.Invoke(player1Name); //Initialize Player when name is added
                            }
                        });
                        break;

                    case "player2":
                        Debug.Log("[PLAYER_2 :: added] uuid = " + args.Snapshot.Child("uuid").Value);
                        string uuidP2 = (string) args.Snapshot.Child("uuid").Value;
                        m_registrationsDB.Child(uuidP2).GetValueAsync().ContinueWithOnMainThread(task =>
                        {
                            if (task.IsCompleted)
                            {
                                string player2Name = (string)task.Result.Child("firstname").Value;
                                OnPlayer2SetName?.Invoke(player2Name); //Initialize Player when name is added
                            }
                        });
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
                GameState state = GameManager.instance.GetCurrentGameState();
                if (args.Snapshot.Key == "isPlaying")
                {
                    //Debug.Log("[ChildChanged] isPlaying = " + args.Snapshot.Value);
                    m_isPlaying = (bool) args.Snapshot.Value;
                    if (m_isPlaying) OnStartPlaying?.Invoke(false);
                }
                if (args.Snapshot.Key == "player1")
                {
                    if (!m_isPlaying && state.Equals(GameState.GAME_OVER))
                    {
                        m_sessionsDB.ChildChanged -= HandleSessionsChildChanged;
                        OnMovePlayer1?.Invoke(1);
                    } 
                    else
                    {
                        if (args.Snapshot.Child("currentPosition").Key == "currentPosition")
                        {
                            //Debug.Log("[ChildChanged] player1.currentPosition = " + args.Snapshot.Child("currentPosition").Value);
                            var position = int.Parse(args.Snapshot.Child("currentPosition").Value.ToString());
                            if (position == -1) OnMovePlayer1?.Invoke(0);
                            else if (position == 1) OnMovePlayer1?.Invoke(2);
                            else OnMovePlayer1?.Invoke(1);  // (position == 0)
                        }
                    }
                }
            }
        }
        #endregion Listeners for database table: stcrunner_sessions

        private void OnGameStateChanged(GameState p_gameState)
        {
            if (p_gameState.Equals(GameState.GAME_OVER))
            {
                m_isPlaying = false;
                m_sessionsDB.ChildChanged -= HandleSessionsChildChanged;
                Debug.Log("SessionClientHandler::--> " + p_gameState);
                m_sessionsDB.Child(SessionsDataRef.IS_PLAYING).SetValueAsync(false).ContinueWithOnMainThread(task =>
                {

                });

                OnMovePlayer1?.Invoke(1);
                return;
            }
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

