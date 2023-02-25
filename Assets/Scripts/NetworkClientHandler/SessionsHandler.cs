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
        public static Action<int> OnPlayer1Move;
        public static Action OnPlayer1MoveRight;
        public static Action OnPlayer1MoveLeft;

        private static string DB_SESSIONS = "stcrunner_sessions";
        private static string REF_SESSION_ISPLAYING = "stcrunner_sessions/isPlaying";
        private static string REF_SESSION_ISMULTIPLAYER = "stcrunner_sessions/isMultiplayer";

        private static string REF_SESSION_PLAYER1 = "stcrunner_sessions/player1";
        private static string REF_SESSION_PLAYER2 = "stcrunner_sessions/player2";

        private FirebaseApp m_App;
        private FirebaseDatabase m_DB;

        private DatabaseReference m_sessionsDB;
        private DatabaseReference m_sessionPlayer1;

        private bool m_isPlaying = false;
        private bool m_isMultiplayer = false;

        //private DatabaseReference m_player1;

        DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        protected bool isFirebaseInitialized = false;



        // Start is called before the first frame update
        void Start()
        {
            //instance = this;

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

            LoadServerListeners();

            isFirebaseInitialized = true;
        }

        protected void LoadServerListeners()
        {
            m_sessionsDB.ValueChanged += HandleSessionStatusChanged;
            //m_sessionsDB.ChildAdded += HandlePlayerSessionDataAdded;
            //m_sessionsDB.ChildRemoved += HandlePlayerSessionDataRemoved;
        }

        //private void HandlePlayerSessionDataAdded(object sender, ChildChangedEventArgs args)
        //{
        //    if (args.DatabaseError != null)
        //    {
        //        Debug.LogError(args.DatabaseError.Message);
        //        return;
        //    }

        //    if (args.Snapshot != null)
        //    {
        //        Debug.Log("dataadded::--> " + args.Snapshot.Key);
        //        if (args.Snapshot.Key == "playe1")
        //        {
        //            m_DB.GetReference(REF_SESSION_PLAYER1 + "/currentPosition").ValueChanged += OnPlayer1SessionDataChanged;
        //        }
        //    }
        //}

        //private void HandlePlayerSessionDataRemoved(object sender, ChildChangedEventArgs args)
        //{
        //    if (args.DatabaseError != null)
        //    {
        //        Debug.LogError(args.DatabaseError.Message);
        //        return;
        //    }

        //    if (args.Snapshot != null)
        //    {
        //        if (args.Snapshot.Key == "playe1")
        //        {
        //            m_DB.GetReference(REF_SESSION_PLAYER1 + "/currentPosition").ValueChanged -= OnPlayer1SessionDataChanged;
        //        }
        //    }
        //}

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
                            //Debug.Log("[isPlaying] " + m_isPlaying);
                            break;

                        case "isMultiplayer":
                            m_isMultiplayer = (bool) childSnapshot.Value;
                            //Debug.Log("[isMultiplayer] " + childSnapshot.Value);
                            break;

                        case "player1":
                            if (m_isPlaying)
                            {
                                m_sessionPlayer1 = m_DB.GetReference(REF_SESSION_PLAYER1 + "/currentPosition");
                                m_sessionPlayer1.ValueChanged += OnPlayer1SessionDataChanged;
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

        private void OnPlayer1SessionDataChanged(object sender, ValueChangedEventArgs args)
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
                OnPlayer1Move?.Invoke(1);
                m_sessionPlayer1.ValueChanged -= OnPlayer1SessionDataChanged;
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
            if (m_sessionPlayer1 != null) m_sessionPlayer1.ValueChanged -= OnPlayer1SessionDataChanged;

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

        PlayerSessionData(string p_sessionPath, string p_uuid, int p_currentPos, string p_moveTo)
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

