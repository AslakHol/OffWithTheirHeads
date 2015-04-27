using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour {

    public float targetSelectTime = 2f;
    public HeadGestureDetect gestureController;

    public List<NodActionInstance> nodActors;

    NodActor selectedTarget;

    NodActor targetPreviousFrame;
    float targetTimer;

    //Public for debug reasons
    public List<float> previousXDeltas;

    public float currentXDeltaSmooth;

    void Start(){
        StartCoroutine(DeltaAngleUpdater());
    }

    void Update(){
        NodActionInstance target = GetCurrentTarget();

        if (target == null || currentXDeltaSmooth > 1f){
            targetPreviousFrame = null;
            targetTimer = 0f;
        }
        else{
            if (target.nodActor != targetPreviousFrame){
                targetTimer = 0f;
            }
            else if (target.nodActor == targetPreviousFrame){
                targetTimer += Time.deltaTime;
                if (targetTimer > targetSelectTime && selectedTarget != target.nodActor){
                    Debug.Log("NEW TARGET " + target.nodActor.name);
                    selectedTarget = target.nodActor;
                }
            }
            targetPreviousFrame = target.nodActor;
        }
    }

	public void HeadFullNod(){
        //This makes the value stay roughly between 0 and 180 degrees

        NodActionInstance target = GetCurrentTarget();
        if (target != null){
            target.nodActor.PerformNodAction(selectedTarget);
        }

//        Debug.Log("NOD at " + currentAngle);
	}

    public NodActionInstance GetCurrentTarget(){
        float currentAngle = gestureController.transform.eulerAngles.y - 90f;
        for (int i = 0; i < nodActors.Count; i++){
            if (currentAngle > nodActors[i].startAngle && currentAngle < nodActors[i].endAngle){
                return nodActors[i];
            }
        }
        return null;
    }

    IEnumerator DeltaAngleUpdater(){
        float previousXAngle = 0f;
        while (true){
            float thisXAngle = gestureController.transform.eulerAngles.x;
            if (thisXAngle > 180f){
                thisXAngle = 360f - thisXAngle;
            }

            if (previousXDeltas.Count > 20){
                previousXDeltas.RemoveAt(0);
            }
            previousXDeltas.Add(Mathf.Abs(thisXAngle - previousXAngle));

            currentXDeltaSmooth = 0f;
            for (int i = 0; i < previousXDeltas.Count; i++){
                currentXDeltaSmooth += previousXDeltas[i];
            }
            currentXDeltaSmooth /= previousXDeltas.Count;

            previousXAngle = thisXAngle;
            yield return new WaitForSeconds(0.05f);
        }
    }

    [System.Serializable]
    public class NodActionInstance{
        public float startAngle;
        public float endAngle;

        public NodActor nodActor;
    }

}
