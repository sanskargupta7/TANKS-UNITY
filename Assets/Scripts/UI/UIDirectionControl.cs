using UnityEngine;

public class UIDirectionControl : MonoBehaviour                   //Makes sure the UI rotation stays the same when it was at start
{
    public bool m_UseRelativeRotation = true;  


    private Quaternion m_RelativeRotation;     


    private void Start()
    {
        m_RelativeRotation = transform.parent.localRotation;       //finds the rotation of th canvas
    }


    private void Update()
    {
        if (m_UseRelativeRotation)
			transform.rotation = m_RelativeRotation;          //Update current rotation to local rotation i.e the health wheel doesnt rotate with tank's rotation but be fixed..... 
    }
}
