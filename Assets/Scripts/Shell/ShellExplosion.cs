using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;                                 //see which tanks to affect
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;            //life of the cell and after this time shell will disppear       
    public float m_ExplosionRadius = 5f;               


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);                //that is after it comes....if it is still alive after max lifetime....destroy it
    }


    private void OnTriggerEnter(Collider other)                              //comes into picture whenanything comes in contact with it or intersected
    {
        // Find all the tanks in an area around the shell and damage them.
		Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);                  //overlap sphere creates an imaginary sphere...anything that is inside that sphere is collected as collider.........tankmask is used so it only captures the tanks			
		for(int i = 0; i < colliders.Length; i++)                       //now we iterate through each of the colliders
			{
				Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();         //iterating through each collider inside the sphere....now it checks if that collider has a rigid bo

				if(!targetRigidbody)                   //if it doesnt have a rigid body it continues.....though every collider should have one...it is a safety measure in case 
					continue;

				targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);         //this will add force according to three values force of explosion, position of the shell and radius i.e. how far the explosion can reach 
			
				TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();                //if the collider with rigid body is the tank

				if(!targetHealth)                                                                    //if the target is not the tank ....continue  
					continue;

				float damage = CalculateDamage(targetRigidbody.position);                   //now we calculate the damage

				targetHealth.TakeDamage(damage);                                            //calculates the tank's health after the damage
			
			}
			
		m_ExplosionParticles.transform.parent = null;
			
		m_ExplosionParticles.Play ();

	    m_ExplosionAudio.Play ();
		    
		Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
		Destroy (gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
		Vector3 explosionToTarget = targetPosition - transform.position;                  //we are finding the vector between the target position and shell position

		float explosionDistance = explosionToTarget.magnitude;                            //magnitude of the distance

		float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;      //fraction of distance to explosion radius

		float damage = relativeDistance * m_MaxDamage;              //finds out the damage according to the relative distance(ratio) of the explosion distances

		damage = Mathf.Max (0f, damage);                       //if it comes out to be zero.....set it to negative

		return damage;                //there is a case when though the tank is outside the sphere...its collider is still within it...so it will be picked up in colliders array and then gets calculated but we only caught the edge of it so the relative distance will be negative as the centre of the collider is outside
    }
}