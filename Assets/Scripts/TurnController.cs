using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    [SerializeField]
    private int _turn;
    public int turn { get; set; }
    private Faction currFaction;

    private UnitManager unitManager;
    // Start is called before the first frame update
    void Start()
    {
        unitManager = GameObject.FindWithTag("UnitManager").GetComponent<UnitManager>();
        turn = 0;
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
        Debug.Log("Turn: " + turn);
        EventManager.TriggerEvent("TurnStartEvent");
    }

    private void TriggerNextFaction()
    {
        if (currFaction == Faction.ROBOT)
        {
            currFaction = Faction.MYTHICAL;
        }
        else
        {
            currFaction++;
        }

        if (currFaction == Faction.MYTHICAL)
        {
            turn++;
            StartNextTurn();
        }
        else
        {
            // TEMPORARY CODE
            // Eventually we will tell the next faction to start doing stuff
            // But since there are no other factions, we're just gonna do this
            EventManager.TriggerEvent("TurnCompleteEvent");
        }
    }
}
