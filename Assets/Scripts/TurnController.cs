using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    private int s_Turn = 0;
    public int Turn { get; }
    private enum Faction { MYTHICAL, MUTANTS, EVOLVED, ZOMBIES, ROBOTS };
    private Faction currFaction;

    private UnitManager unitManager;
    // Start is called before the first frame update
    void Start()
    {
        unitManager = GameObject.FindWithTag("UnitManager").GetComponent<UnitManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            StartNextTurn();
        }
    }

    void OnEnable()
    {
        EventManager.StartListening("TurnCompleteEvent", TriggerNextFaction);
    }

    void OnDisable()
    {
        EventManager.StopListening("TurnCompleteEvent", TriggerNextFaction);
    }

    private void StartNextTurn()
    {
        unitManager.GatherActiveUnits();
        currFaction = Faction.MYTHICAL;
    }

    private void TriggerNextFaction()
    {
        currFaction++;
        Debug.Log("We are on Faction " + currFaction);
        if (currFaction == Faction.MYTHICAL)
        {
            StartNextTurn();
        }
        else
        {
            // Tell the next faction to start doing stuff
        }
    }
}
