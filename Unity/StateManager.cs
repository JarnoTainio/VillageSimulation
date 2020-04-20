using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    int state;
    Dummy dummy;
    public CameraController cameraController;
    public PlayerController player;

    public GameObject[] simulationObjects;
    public GameObject[] adventureObjects;

    // Start is called before the first frame update
    void Start()
    {
        dummy = Dummy.instance;
        SimulationState();
    }

    public void NextState()
    {
        if (state == 0)
        {
            AdventureState();
        }
        else
        {
            SimulationState();
        }
    }

    private void SimulationState()
    {
        state = 0;
        dummy.autoSimulation = true;
        cameraController.enabled = true;

        foreach(GameObject g in simulationObjects)
        {
            g.SetActive(true);
        }
        foreach (GameObject g in adventureObjects)
        {
            g.SetActive(false);
        }
    }

    private void AdventureState()
    {
        state = 1;
        dummy.autoSimulation = false;
        cameraController.enabled = false;

        foreach (GameObject g in simulationObjects)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in adventureObjects)
        {
            g.SetActive(true);
        }

        player.FocusCamera();
    }


}
