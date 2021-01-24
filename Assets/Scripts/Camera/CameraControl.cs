using UnityEngine;                  //check the commented script in completed folder

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 //the approximate time that we want for the camera to move to the position it is required to be in
    public float m_ScreenEdgeBuffer = 4f;           //make sure that tanks arent at the edge of the screen
    public float m_MinSize = 6.5f;                  //dont want the camera to really zoom in
    [HideInInspector]public Transform[] m_Targets;        


    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;         //we get the desired zoom position on the tanks by taking out the average of the tanks position or it could zoom to the wrong place where the tanks arent even    


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();     //get component on children, here main camera
    }


    private void FixedUpdate()
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);        // SMOOTHLY moves the camera with a specific speed between the current position and the desired position
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();        // create empty vector
        int numTargets = 0;

		for (int i = 0; i < m_Targets.Length; i++)        //m_Targets is the list that contains the tanks or the players so we iterate through every tank......here 2 players so lenght of the list is 2       
        {
            if (!m_Targets[i].gameObject.activeSelf)      //checks if the tank object is active.....as when a tank dies, we deactivate it.....therefore we zoom only if tank is active 
                continue;

            averagePos += m_Targets[i].position;         //and so when a tank dies only one tank is active then camera automatically zooms on the winner tank which is only active tank
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        averagePos.y = transform.position.y;            //we dont want the y position to change due to average calculation hence we keep the old value of y....

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);      //first find the required size and then smoothly moving towards that size with a specific speed
    }


    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);     //we want to accomodate the tank which is further

        float size = 0f;

		for (int i = 0; i < m_Targets.Length; i++)                        //go through the tanks(targets)
        {
            if (!m_Targets[i].gameObject.activeSelf)                      //if not active then continue
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);          

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;           // we want to do this from the desired position   

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));        //absolute value of y

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);   
        }
        
        size += m_ScreenEdgeBuffer;

        size = Mathf.Max(size, m_MinSize);

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}