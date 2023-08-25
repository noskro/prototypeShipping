using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBattleManager : MonoBehaviour
{
    private int shipBattlePhase;
    private float shipBattleTimer;
    private ShipStats attackingShip;
    private ShipStats defendingShip;

    private float attackingDamage = 0f;
    private float defendingDamage = 0f;
    private int deltaDurability;
    private int deltaCrew;

    public bool battleInProgress = false;

    public AudioSource audioSourceBattle;
    public AudioClip audioClipCanonHit;
    public AudioClip audioClipCanonShot;
    public AudioClip audioClipCanonMiss;

    // Start is called before the first frame update
    void Start()
    {
        shipBattlePhase = 0;
        shipBattleTimer = 0f;
        battleInProgress = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (battleInProgress)
        {
            if (shipBattleTimer > 0)
            {
                shipBattleTimer -= Time.deltaTime;
                if (shipBattlePhase == 1)
                {
                    //if (attackingDamage > 0)
                    //{
                        Color c = attackingShip.spriteCanonFireSmoke.color;
                        c.a = Mathf.Lerp(0, 1, shipBattleTimer);
                        attackingShip.spriteCanonFireSmoke.color = c;
                    //}
                }
                if (shipBattlePhase == 2)
                {
                    if (attackingDamage > 0)
                    {
                        Color c = defendingShip.spriteCanonFireHit.color;
                        c.a = Mathf.Lerp(0, 1, shipBattleTimer);
                        defendingShip.spriteCanonFireHit.color = c;
                    }
                }
                if (shipBattlePhase == 3)
                {
                    // animation fire back
                    //if (defendingDamage > 0)
                    //{
                        Color c = defendingShip.spriteCanonFireSmoke.color;
                        c.a = Mathf.Lerp(0, 1, shipBattleTimer);
                        defendingShip.spriteCanonFireSmoke.color = c;
                    //}
                }
                if (shipBattlePhase == 4)
                {
                    // animation fire back
                    if (defendingDamage > 0)
                    {
                        Color c = attackingShip.spriteCanonFireHit.color;
                        c.a = Mathf.Lerp(0, 1, shipBattleTimer);
                        attackingShip.spriteCanonFireHit.color = c;
                    }
                }
            }
            else if (shipBattleTimer < 0)
            {
                shipBattlePhase++;
                DoBattleActionPhase(shipBattlePhase);
                shipBattleTimer = 1f;
            }
        }
    }

    private void DoBattleActionPhase(int shipBattlePhase)
    {
        if (shipBattlePhase == 1)
        {
            float attackMinDamage = 0; // Math.Min(attackingShip.GetCurrentMaxCanons(), attackingShip.CrewCount);
            float attackMaxDamage = Math.Min(attackingShip.GetCurrentMaxCanons(), attackingShip.CrewCount); // modifiers???
            attackingDamage = UnityEngine.Random.Range(attackMinDamage, attackMaxDamage) * 2;

            float percentageCrewLostAttack = UnityEngine.Random.Range(0f, 1f);
            int oldDurability = Mathf.FloorToInt(defendingShip.ShipDurability);
            defendingShip.ShipDurability -= attackingDamage * (1 - percentageCrewLostAttack);
            deltaDurability = Mathf.FloorToInt(defendingShip.ShipDurability) - oldDurability;

            deltaCrew = -1 * Mathf.FloorToInt(attackingDamage * percentageCrewLostAttack);
            defendingShip.CrewCount += deltaCrew;

            audioSourceBattle.PlayOneShot(audioClipCanonShot);
        }
        if (shipBattlePhase == 2)
        {
            // wait
            if (attackingDamage > 0)
            {
                audioSourceBattle.PlayOneShot(audioClipCanonHit);

                if (DemoController.Instance.shipController.shipStats.Equals(defendingShip))
                {
                    DemoController.Instance.shipController.shipStatusUI.ShowStatChange(deltaDurability, deltaCrew, 0, 0, 0, 0);
                    DemoController.Instance.shipController.shipStats.TriggerShipUpdated();
                }
            }
            else
            {
                audioSourceBattle.PlayOneShot(audioClipCanonMiss);
            }

        }
        if (shipBattlePhase == 3)
        {
            if (defendingShip.ShipDurability > 0 && defendingShip.CrewCount > 0)
            {
                float defendingMinDamage = 0; // Math.Min(attackingShip.GetCurrentMaxCanons(), attackingShip.CrewCount);
                float defendingxDamage = Math.Min(defendingShip.GetCurrentMaxCanons(), defendingShip.CrewCount); // modifiers???
                defendingDamage = UnityEngine.Random.Range(defendingMinDamage, defendingxDamage) * 2;

                float percentageCrewLostDefense = UnityEngine.Random.Range(0f, 1f);
                attackingShip.ShipDurability -= defendingDamage * (1 - percentageCrewLostDefense);
                attackingShip.CrewCount -= Mathf.FloorToInt(defendingDamage * percentageCrewLostDefense);

                audioSourceBattle.PlayOneShot(audioClipCanonShot);
            }
            else
            {
                // ship sinks
                shipBattlePhase = 6; 

                if (DemoController.Instance.shipController.shipStats.Equals(attackingShip))
                {
                    // player is attacker = victory
                    DemoController.Instance.gameMapHandler.SetPirateShipLost(defendingShip);
                }
                else
                {
                    DemoController.Instance.SetGameState(EnumGameStates.ShipLost);
                    DemoController.Instance.shipController.shipStats.TriggerShipUpdated();
                }
            }
        }
        if (shipBattlePhase == 4)
        {
            // wait
            if (defendingDamage > 0)
            {
                audioSourceBattle.PlayOneShot(audioClipCanonHit);

                if (DemoController.Instance.shipController.shipStats.Equals(attackingShip))
                {
                    DemoController.Instance.shipController.shipStatusUI.ShowStatChange(deltaDurability, deltaCrew, 0, 0, 0, 0);
                    DemoController.Instance.shipController.shipStats.TriggerShipUpdated();
                }
            }
            else
            {
                audioSourceBattle.PlayOneShot(audioClipCanonMiss);
            }
        }
        if (shipBattlePhase == 5)
        {
            if (attackingShip.ShipDurability <= 0 && attackingShip.CrewCount <= 0)
            {
                if (DemoController.Instance.shipController.shipStats.Equals(attackingShip))
                {
                    // player is attacker = defeat
                    DemoController.Instance.SetGameState(EnumGameStates.ShipLost);
                    DemoController.Instance.shipController.shipStats.TriggerShipUpdated();
                }
                else
                {
                    DemoController.Instance.gameMapHandler.SetPirateShipLost(defendingShip);
                }
            }
            shipBattlePhase = 6;
        }
        if (shipBattlePhase >= 6)
        {
            battleInProgress = false;
            this.shipBattlePhase = 0;
            shipBattleTimer = 0f;
            if (DemoController.Instance.GameState.Equals(EnumGameStates.InBattle))
            {
                DemoController.Instance.SetGameState(EnumGameStates.ShipIdle);
            }
        }
    }

    internal void StartShipBattle(ShipStats attackingShip, ShipStats defendingShip)
    {
        DemoController.Instance.SetGameState(EnumGameStates.InBattle);
        this.attackingShip = attackingShip;
        this.defendingShip = defendingShip;

        shipBattleTimer = -1f;// will trigger next phase if < 0
        battleInProgress = true;
    }
}
