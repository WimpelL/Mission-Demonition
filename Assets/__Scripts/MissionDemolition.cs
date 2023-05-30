using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameMode
{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S;

    [Header ("Set in Inspector")]
    public TextMeshProUGUI uitLevel;
    public TextMeshProUGUI uitShots;
    public TextMeshProUGUI uitButton;
    public TextMeshProUGUI recordToken;
    public Vector3 castlePos;
    public GameObject[] castles;


    [Header("Set Dynamically")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";   
    static public int recTok;

   
    void Start()
    {
        S = this;
        level = 0;
        levelMax = castles.Length;

        if(PlayerPrefs.HasKey("recTok"))
        {
            recTok = PlayerPrefs.GetInt("recTok");
        }
        PlayerPrefs.SetInt ("recTok", recTok);


        
        StartLevel();  

    }
    void StartLevel()
    {
        
        if(castle != null)
        {
            Destroy (castle);
        }
        GameObject[] gos = GameObject.FindGameObjectsWithTag ("Projectile");
        foreach (GameObject pTemp in gos)
        {
            DestroyImmediate(pTemp);
        }

        castle = Instantiate<GameObject>(castles[level] );
        castle.transform.position = castlePos;
        while(Slingshot.S.projectileLines.Count > 0)
        {
            Slingshot.S.DestroyProjectileLines();
        }
        shotsTaken = 0;
        SwitchView("Show Both");
        ProjectileLine.S.Clear();
        Goal.goalMet = false; 
        UpdateGUI();
        mode = GameMode.playing;

    }

    void UpdateGUI()
    {
        uitLevel.text = "Level: " + ( level + 1 ) + " of " + levelMax;

        uitShots.text = "Shot Taken: " +shotsTaken;
        recordToken.text = "Used Takens: " + recTok;
        if(recTok > PlayerPrefs.GetInt("recTok"))
        {
            PlayerPrefs.SetInt( "recTok", recTok);
        }

    }
    void Update()
    {
        UpdateGUI();
        if ((mode == GameMode.playing) && Goal.goalMet)
        {
            mode = GameMode.levelEnd;
            SwitchView("Show Both");
            Invoke("NextLevel", 2f);
        }
    }
    void NextLevel()
    {
        level++;
        if(level == levelMax)
        {
            level = 0;
        }
        StartLevel();
    }
    public void SwitchView (string eView = "")
    {
        if(eView == "")
        {
            eView = uitButton.text;
        }
        showing = eView;
        switch (showing)
        {
            case "Show Slingshot":
            FollowCam.POI = null;
            uitButton.text = "Show Castle";
            break;

            case "Show Castle":
            FollowCam.POI = S.castle;
            uitButton.text = "Show Both";
            break;

            case "Show Both":
            FollowCam.POI = GameObject.Find("ViewBoth");
            uitButton.text = "Show Slingshot";
            break;
        }
    }
    public static void ShotFired()
    {
        S.shotsTaken++;
        if(S.shotsTaken > recTok)
        {
            recTok = S.shotsTaken;
        }
    }

}

