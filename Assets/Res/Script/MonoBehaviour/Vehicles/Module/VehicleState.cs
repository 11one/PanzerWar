using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleState : MonoBehaviour {
    public EngineSoundModule engineSoundModule;

    public TankTracksController tankTracksController;

    public int currentHealth;
	
	void Update () {
        engineSoundModule.AccelG = tankTracksController.accelG;
	}
}
