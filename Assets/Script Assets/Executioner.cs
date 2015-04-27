using UnityEngine;
using System.Collections;

public class Executioner : NodActor {

    public float axeSpeed = 1f;
    public GameObject axe;

    NodActor currentTarget;
    Transform currentAxe;

    override public void PerformNodAction(NodActor target){
        Debug.Log("Executioner going to murder someone named " + target);
        GameObject currentAxeObject = (GameObject)Instantiate(axe, axe.transform.position, axe.transform.rotation);
        currentAxe = currentAxeObject.transform;
        currentAxe.gameObject.SetActive(true);
        currentTarget = target;
    }

    void Update(){
        if (currentTarget != null){
            if (Vector3.Distance(currentAxe.transform.position, currentTarget.transform.position + Vector3.up) < 0.1f){
                Rigidbody targetRigidbody = currentTarget.GetComponent<Rigidbody>();
                if (targetRigidbody != null){
                    targetRigidbody.isKinematic = false;
                    targetRigidbody.useGravity = true;
                    targetRigidbody.AddForce((Vector3.Normalize(currentTarget.transform.position - transform.position) + Vector3.up/2f) * 1000f);
                }
                currentAxe.parent = currentTarget.transform;
                currentTarget = null;
            }
            else{
                currentAxe.transform.position = Vector3.MoveTowards(currentAxe.transform.position, currentTarget.transform.position + Vector3.up, axeSpeed * Time.deltaTime);
            }
        }

    }

}
