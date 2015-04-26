using UnityEngine;
using System.Collections;

public class Announcer : NodActor {

    override public void PerformNodAction(NodActor target){
        Debug.Log("Announcer targeted " + target);
    }

}
