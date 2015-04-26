using UnityEngine;
using System.Collections;

public class Executioner : NodActor {

    override public void PerformNodAction(NodActor target){
        Debug.Log("Executioner going to murder someone named " + target);
    }

}
