using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkClientHandler
{

    public class SessionsHandler : MonoBehaviour
    {
        private static string DB_SESSIONS = "stcrunner_sessions";
        private static string REF_SESSION_ISPLAYING = "stcrunner_sessions/isPlaying";
        private static string REF_SESSION_ISMULTIPLAYER = "stcrunner_sessions/isMultiplayer";

        private static string REF_SESSION_PLAYER1 = "stcrunner_sessions/player1";
        private static string REF_SESSION_PLAYER2 = "stcrunner_sessions/player2";

        private FirebaseApp m_App;
        private FirebaseDatabase m_DB;

        private DatabaseReference m_sessionsDB;

        private bool m_isPlaying = false;
        private bool m_isMultiplayer = false;

        //private DatabaseReference m_player1;

        DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        protected bool isFirebaseInitialized = false;


        private List<PlayerSessionData> m_playersData = new List<PlayerSessionData>();

        // Start is called before the first frame update
        void Start()
        {
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
        protected virtual void InitializeFirebase()
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
                            if (m_isPlaying)
                            {
                                Debug.Log("[isPlaying] " + childSnapshot.Value);
                            }
                            break;

                        case "isMultiplayer":
                            m_isMultiplayer = (bool) childSnapshot.Value;
                            if (m_isMultiplayer)
                            {
                                Debug.Log("[isMultiplayer] " + childSnapshot.Value);
                            }
                            break;

                        default:
                            // DO NOTHING FOR NOW
                            if (m_isPlaying)
                            {
                                //Debug.Log(childSnapshot.Key);
                                m_playersData.Add(PlayerSessionData.CreateFromJson(childSnapshot.GetRawJsonValue()));
                                //m_playersData.Add(new PlayerSessionData(childSnapshot.Child("uuid").Value.ToString(), childSnapshot.Child("current_position")));
                            }
                            break;
                    }
                }

                if (m_playersData.Count != 0)
                {
                    for (int i = 0; i < m_playersData.Count; i++)
                    {
                        PlayerSessionData data = m_playersData[i];
                        Debug.Log("[" + data.path + "] " + data.uuid);
                    }
                }
            }
        }

        void OnDestroy()
        {
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

        PlayerSessionData(string p_sessionPath, string p_uuid, int p_currentPos)
        {
            path = p_sessionPath;
            uuid = p_uuid;
            currentPosition = p_currentPos;
        }

        public static PlayerSessionData CreateFromJson(string json_string)
        {
            return JsonUtility.FromJson<PlayerSessionData>(json_string);
        }
    }
}

