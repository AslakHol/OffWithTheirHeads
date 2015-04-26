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



    void Update(){
        NodActionInstance target = GetCurrentTarget();

        if (target == null || GetNodAngleDelta() > 1f){
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

    float GetNodAngleDelta(){
        return 0f;
    }

    [System.Serializable]
    public class NodActionInstance{
        public float startAngle;
        public float endAngle;

        public NodActor nodActor;
    }

}
