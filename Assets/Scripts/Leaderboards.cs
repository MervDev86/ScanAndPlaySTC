using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;
using System.IO;

public class Leaderboards : MonoBehaviour
{
    struct lbItem
    {
        public string playerName;
        //public string playerEmail;
        public float playerID;
        public int score;
    }

    Dictionary<lbItem, float> m_leaderboardDic = new Dictionary<lbItem, float>();
    //[SerializeField] Transform m_closeBtn;
    [SerializeField] Transform m_grid;
    [SerializeField] Transform m_leaderboardItem;
    [SerializeField] int m_maxItemToShow = 3;
    [SerializeField] string m_leaderboardName = "HARD";

    void Start()
    {
        //InitLeaderboard();
    }

    public void InitLeaderboard()
    {
        m_leaderboardDic = new Dictionary<lbItem, float>();
        string docPath = Application.dataPath + "/StreamingAssets/";
        string[] m_playerList = System.IO.File.ReadAllLines(docPath + m_leaderboardName + ".csv");

        for (int i = 0; i < m_playerList.Length; i++)
        {
            string[] playerItem = m_playerList[i].Split(',');
            lbItem item = new lbItem();
            item.playerName = playerItem[0];
            //item.playerEmail = playerItem[1];
            item.playerID = UnityEngine.Random.Range(1000, 99999);
            item.score = int.Parse(playerItem[2]);
            m_leaderboardDic.Add(item, float.Parse(playerItem[1]));
        }

        var sortedScore = from entry in m_leaderboardDic orderby entry.Key.score descending select entry;
        var sortedDict = from entry in sortedScore orderby entry.Value ascending select entry;

        foreach (Transform child in m_grid)
            Destroy(child.gameObject);

        int index = 0;
        foreach (KeyValuePair<lbItem, float> val in sortedDict)
        {
            //Debug.Log(val.Value);
            Transform leaderboardItem = Instantiate(m_leaderboardItem, m_grid);
            leaderboardItem.Find("PlaceVal").GetComponent<TextMeshProUGUI>().text = ++index + "";
            leaderboardItem.Find("NameVal").GetComponent<TextMeshProUGUI>().text = val.Key.playerName.ToUpper();
            leaderboardItem.Find("ScoreVal").GetComponent<TextMeshProUGUI>().text = val.Key.score.ToString();
            leaderboardItem.Find("TimeVal").GetComponent<TextMeshProUGUI>().text = val.Value.ToString("00.00")+ "s";

            if (index >= m_maxItemToShow)
                break;
        }
    }

    public void SaveScore(string p_playerName, string p_time, string p_score)
    {

        string docPath = Application.dataPath + "/StreamingAssets/";

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath + m_leaderboardName + ".csv"), true))
        {
            outputFile.WriteLine($"{p_playerName},{p_time},{p_score}");
        }

        Debug.Log("Save dataPlayer" + " : " + docPath);
    }
}
