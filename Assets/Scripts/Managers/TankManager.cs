using System;
using UnityEngine;
                                         //not monobehaviour
[Serializable]                           
public class TankManager
{
    public Color m_PlayerColor;                                       //which player it is
    public Transform m_SpawnPoint;         
    [HideInInspector] public int m_PlayerNumber;             
    [HideInInspector] public string m_ColoredPlayerText;              //text color of the player....each player has its own color
    [HideInInspector] public GameObject m_Instance;                   //stores instance of the tanks
    [HideInInspector] public int m_Wins;                              //wins the tank has


    private TankMovement m_Movement;       
    private TankShooting m_Shooting;                                 //these three are there so that they can be turned on/off when required
    private GameObject m_CanvasGameObject;


    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";     //adding bits to a string

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();             //mesh renderers are things thatshow your 3d objects on your scree

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }


    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);                       //turns off all the tanks
        m_Instance.SetActive(true);                        //turns on the tanks showing only the tank that won
    }
}
