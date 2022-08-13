using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using Random = UnityEngine.Random;


public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text ScoreBestText;
    public GameObject GameOverText;
    private bool m_Started = false;
    private int m_Points;
    private string playerName;
    private bool m_GameOver = false;
    private int CurrentBestPoint;
    private string CurrentNameBestPoint;
    public GameObject[] scores;
    private SaveData data = new SaveData();
    private string[] tempBestNamePoint = new string[4];
    private int[] tempBestPoint = new int[4];

    private void Awake()
    {
        LoadScore();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerName = MenuManager.Instance.NameText;
        ScoreText.text = "Score " + playerName + " : "   + " " + m_Points;
        ScoreBestText.text = "Best Score es de " + CurrentNameBestPoint + " : " + CurrentBestPoint;
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        Debug.Log(data.BestScore);
        Debug.Log(data.nameBestScore);
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
       //ScoreText.text = $"Score : {m_Points}";
       ScoreText.text = "Score " + playerName + " : "   + " " + m_Points;
        ChangeBestScore();
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        SaveScore();
        ShowTablePoints();
    }

    public void ChangeBestScore()
    {
        if (m_Points > CurrentBestPoint)
        {
            Debug.Log("Entrando en changeBestScore");
            CurrentBestPoint = m_Points;
            CurrentNameBestPoint = MenuManager.Instance.NameText;
            data.BestScore = m_Points;
            data.nameBestScore = CurrentNameBestPoint;
            ScoreBestText.text = "Best Score es de " + data.nameBestScore + " : " + data.BestScore;
        }
        
        for (int i = 0; i < tempBestPoint.Length; i++)
        {
            if (m_Points > tempBestPoint[i])
            {
                tempBestPoint[i] = m_Points;
                tempBestNamePoint[i] = MenuManager.Instance.NameText;
                break;
            }
        }
    }
    
    [System.Serializable]
    class  SaveData
    {
        public int BestScore;
        public string nameBestScore;
        public int[] bestscores = new int[4];
        public string[] nameBestScores = new string[4];
    }
    public void SaveScore()
    {
        for (int i = 0; i < tempBestPoint.Length; i++)
            
        {
                data.nameBestScores[i] = tempBestNamePoint[i];
                data.bestscores[i] = tempBestPoint[i];
        }
        data.BestScore = CurrentBestPoint;
        data.nameBestScore = CurrentNameBestPoint;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);  
        

    }
    public void LoadScore()
    {
        
        Debug.Log("Entrando en LoadScore");
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            CurrentNameBestPoint = data.nameBestScore;    
            CurrentBestPoint = data.BestScore;
            for (int i = 0; i < data.bestscores.Length; i++)
            {
                tempBestPoint[i] = data.bestscores[i];
                tempBestNamePoint[i] = data.nameBestScores[i];
            }
        }
    }

    public void ShowTablePoints()
    {
        for (int i = 0; i < tempBestPoint.Length; i++)
        {
            scores[i].GetComponent<Text>().text = " El player " + tempBestNamePoint[i] + " tiene " + 
                                                  tempBestPoint[i]; 
        }
    }
    public void Exit()
    {
        SaveScore();
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #else
        Application.Quit();
        #endif
    }
}
