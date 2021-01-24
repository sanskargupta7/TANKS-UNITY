using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;              
    public GameObject m_TankPrefab;         
    public TankManager[] m_Tanks;           


    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;             
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;       


    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

        StartCoroutine(GameLoop());
    }


    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();          //uses setup function of the tank manager
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()                                        //game loop ---- rounds start -> wait -> round playing -> wait -> round ending......therfore after each step we wait for certain seconds and it is there where routines come into play
    {                                                                     //IEnumerator is just a return type
        yield return StartCoroutine(RoundStarting());                     //first a coroutine starts and when it comes to yield program exits that function goes away and then it waits for some period until the function completes and then after that time it again goes to that function and continues with the coroutine
        yield return StartCoroutine(RoundPlaying());                      //so instead of doing everything in one go......it waits for some time and then continues with the execution.......and this is really useful.....we can use yield in a while loop
        yield return StartCoroutine(RoundEnding());                       //and so we exist in the loop until there is only 1 tank left

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }
	                  //this is how game mangaer and tank manager overplay

    private IEnumerator RoundStarting()
    {
		ResetAllTanks ();
		DisableTankControl ();

		m_CameraControl.SetStartPositionAndSize ();

		m_RoundNumber++;
		m_MessageText.text = "ROUND " + m_RoundNumber;

		yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
		EnableTankControl ();

		m_MessageText.text = string.Empty;

		while (!OneTankLeft ()) 
		{
			yield return null;
		}
    }


    private IEnumerator RoundEnding()
    {
		DisableTankControl ();

		m_RoundWinner = null;

		m_RoundWinner = GetRoundWinner ();

		if (m_RoundWinner != null)
			m_RoundWinner.m_Wins++;

		m_GameWinner = GetGameWinner ();

		string message = EndMessage ();
		m_MessageText.text = message;


		yield return m_EndWait;
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()                        //goes through all tanks and if it gets a tank with active instance.....it returns that tank
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)              //it can return null if no tanks are left in case they both blew each other up
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()                      //check which tank reches the limit for no. of wins first
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";             //wins for a round
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";                              //wins for the game

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}