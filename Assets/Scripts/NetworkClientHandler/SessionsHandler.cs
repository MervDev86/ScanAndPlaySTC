using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkClientHandler
{
    public class SessionsHandler : MonoBehaviour
    {

        //public static SessionsHandler instance;

        public static Action<bool> OnStartPlaying;
        public static Action<int> OnPlayer1Move;
        

        public static Action OnPlayer1MoveRight;
        public static Action OnPlayer1MoveLeft;

        private static string DB_SESSIONS = "stcrunner_sessions";
        private static string DB_REGISTRATIONS = "stcrunner_registrations";

        private static string REF_SESSION_ISPLAYING = "stcrunner_sessions/isPlaying";
        private static string REF_SESSION_ISMULTIPLAYER = "stcrunner_sessions/isMultiplayer";

        private static string REF_SESSION_PLAYER1 = "stcrunner_sessions/player1";
        private static string REF_SESSION_PLAYER2 = "stcrunner_sessions/player2";

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
            //instance = this;

            GameManager.instance.onChangedGameState += HandleGameStateChanged;

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
            m_registrationsDB = m_DB.GetReference(DB_REGISTRATIONS);

            LoadServerListeners();

            isFirebaseInitialized = true;
        }

        protected void LoadServerListeners()
        {
            m_sessionsDB.ValueChanged += HandleSessionStatusChanged;
        }

        private void HandleGameStateChanged(GameState p_gameState)
        {
            Debug.Log("SessionClientHandler::--> " + p_gameState);
            if (p_gameState.Equals(GameState.GAME_END))
            {
                m_isPlaying = false;

                var newData = new Dictionary<string, object>();
                newData["/isPlaying"] = false;

                m_sessionsDB.UpdateChildrenAsync(newData);

                OnPlayer1Move?.Invoke(1);
                m_refCurrentPosP1.ValueChanged -= OnP1CurrentPositionChanged;
                return;
            }

            
        }

        private void HandleSessionStatusChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            if (args.Snapshot != null)
            {
                foreach (var childSnapshot in args.Snapshot.Children)
                {
                    switch(childSnapshot.Key)
                    {
                        case "isPlaying":
                            m_isPlaying = (bool) childSnapshot.Value;
                            Debug.Log("[isPlaying] " + m_isPlaying);
                            if (m_isPlaying)
                            {
                                OnStartPlaying?.Invoke(false);
                            }
                            break;

                        case "isMultiplayer":
                            m_isMultiplayer = (bool) childSnapshot.Value;
                            //Debug.Log("[isMultiplayer] " + childSnapshot.Value);
                            break;

                        case "player1":
                            if (GameManager.instance.GetCurrentGameState().Equals(GameState.GAME_END)) return;

                            if (m_isPlaying)
                            {
                                var playerRef = m_DB.GetReference(REF_SESSION_PLAYER1);
                                m_refCurrentPosP1 = playerRef.Child("/currentPosition");
                                m_refCurrentPosP1.ValueChanged += OnP1CurrentPositionChanged;

                                m_refUuidPosP1 = playerRef.Child("/uuid");
                                m_refUuidPosP1.ValueChanged += OnP1UuidChanged;

                                /*
                                var uuid = (string) playerRef.Child("/uuid").GetValueAsync().Result.Value;
                                m_namePlayer1 = (string) m_registrationsDB.Child("/"+ uuid + "/firstname").GetValueAsync().Result.Value;
                                Debug.Log("playerName = " + m_namePlayer1);
                                */
                            }
                            else
                            {
                                m_refUuidPosP1.ValueChanged -= OnP1UuidChanged;
                            }
                            break;
                        //case "player2":
                        //    if (m_isPlaying && m_isMultiplayer)
                        //    {
                        //        m_DB.GetReference(REF_SESSION_PLAYER2 + "/moveTo").ValueChanged += OnPlayer2SessionDataChanged;
                        //        //m_DB.GetReference(REF_SESSION_PLAYER2).KeepSynced(true);
                        //    }
                        //    break;
                    }
                }

            }
        }

        private void OnP1UuidChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            if (args.Snapshot != null && args.Snapshot.Value != null)
            {
                //Debug.Log(args.Snapshot.Key + " = " + args.Snapshot.Value);
                m_refNameP1 = m_registrationsDB.Child("/" + args.Snapshot.Value.ToString() + "/firstname");
                m_refNameP1.ValueChanged += OnP1CurrentNameValueChanged;
            }
        }

        private void OnP1CurrentNameValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            if (args.Snapshot != null && args.Snapshot.Value != null)
            {
                //Debug.Log(args.Snapshot.Key + " = " + args.Snapshot.Value);
                m_namePlayer1 = args.Snapshot.Value.ToString();
            }
        }


        private void OnP1CurrentPositionChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            if (args.Snapshot != null && args.Snapshot.Value != null)
            {
                //Debug.Log(args.Snapshot.Key + " = " + args.Snapshot.Value);
                var position = int.Parse(args.Snapshot.Value.ToString());
                if (position == -1)
                {
                    OnPlayer1Move?.Invoke(0);
                }
                if (position == 0)
                {
                    OnPlayer1Move?.Invoke(1);
                }
                if (position == 1)
                {
                    OnPlayer1Move?.Invoke(2);
                }
            } 
            else
            {
                //OnStartPlaying?.Invoke(false);
                OnPlayer1Move?.Invoke(1);
                m_refCurrentPosP1.ValueChanged -= OnP1CurrentPositionChanged;
                m_refUuidPosP1.ValueChanged -= OnP1UuidChanged;
            }
        }

        private void OnPlayer2SessionDataChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            if (args.Snapshot != null)
            {
                foreach (var data in args.Snapshot.Children)
                {
                    if (data.Key == "currentPosition")
                    {
                        //Debug.Log("[player_2] currPosition = " + data.Value);
                    }
                }
            }
        }

        void OnDestroy()
        {
            //m_DB.GetReference(REF_SESSION_PLAYER1 + "/currentPosition").ValueChanged -= OnPlayer1SessionDataChanged;
            if (m_refCurrentPosP1 != null) m_refCurrentPosP1.ValueChanged -= OnP1CurrentPositionChanged;
            if (m_refUuidPosP1 != null) m_refUuidPosP1.ValueChanged -= OnP1UuidChanged;

            m_sessionsDB.ValueChanged -= HandleSessionStatusChanged;

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

