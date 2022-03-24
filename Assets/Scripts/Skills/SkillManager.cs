using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SkillManager : NetworkBehaviour
{

    [SerializeField] private GenericTileMap TMS;
    [SyncVar]
    public GameObject selectedUnit;
    public GameManager gameManager;
    [SyncVar]
    public bool NoSkillRanger = false;




    // Start is called before the first frame update
    public void Start()
    {
    }

    // Update is called once per frame
    public void Update()
    {

    }

    /*public void SkillActivation()
    {
        if (!TMS.skillUsed)
        {
            selectedUnit = TMS.selectedUnit;
            string unit = selectedUnit.GetComponent<GenericUnit>().unitName;
            switch (unit)
            {
                case "Archer":
                    TMS.DisableHighlightUnitRange();
                    CmdArcherSkill();
                    TMS.HighlightUnitAttackOptionsFromPosition();
                    break;

                case "Captain":
                    TMS.DisableHighlightUnitRange();
                    CmdCaptainSkill();
                    TMS.HighlightUnitRange();
                    break;


                default:
                    break;

            }
        }
    }*/

    /*
    public void SkillDeactivation()
    {
        if (TMS.skillUsed)
        {
            selectedUnit = TMS.selectedUnit;
            string unit = selectedUnit.GetComponent<GenericUnit>().unitName;
            switch (unit)
            {
                case "Archer":
                    CmdArcherSkillOff();
                    break;

                case "Captain":
                    CmdCaptainSkillOff();
                    break;


                default:
                    break;

            }
        }
    }*/

    //Calls Rpc version of this function
    [Command(requiresAuthority = false)]
    public void CmdSetUnit(GameObject selectedUnit)
    {
        RpcSetUnit(selectedUnit);
    }

    //Tells both clients which unit should be the "selected one"
    [ClientRpc]
    public void RpcSetUnit(GameObject selectedUnit)
    {
        this.selectedUnit = selectedUnit;
    }


    //Checks if the player can use their skill, then calls the setting function, and uses the name of the unit to call the
    //Skill that matches the unit
    public void SkillActivation()
    {
        if (!TMS.skillUsed)
        {
            selectedUnit = TMS.selectedUnit;
            CmdSetUnit(selectedUnit);
            string unit = selectedUnit.GetComponent<GenericUnit>().unitName;
            //Saves having a massive if statement or a switch that will keep growing
            //ie. One line regardless of how many units there are
            Invoke("Cmd" + unit + "Skill", 0);

        }
    }

    public void SkillDeactivation()
    {
        if (TMS.skillUsed)
        {
            selectedUnit = TMS.selectedUnit;
            CmdSetUnit(selectedUnit);
            string unit = selectedUnit.GetComponent<GenericUnit>().unitName;
            Invoke("Cmd" + unit + "SkillOff", 0);

        }
    }

    //All the functions in this region just call the Rpc version so both clients can run it
    #region Cmd Skill
    [Command(requiresAuthority = false)]
    public void CmdArcherSkill()
    {
        RpcArcherSkill();
    }

    [Command(requiresAuthority = false)]
    public void CmdCaptainSkill()
    {

        RpcCaptainSkill();
    }

    [Command (requiresAuthority =false)]
    public void CmdKnightSkill()
    { 
        RpcKnightSkill();
    }

    [Command (requiresAuthority =false)]
    public void CmdWarriorSkill()
    {
        RpcWarriorSkill();
    }

    [Command (requiresAuthority =false)]
    public void CmdArcanistSkill()
    {
        RpcArcanistSkill();
    }

    [Command (requiresAuthority =false)]
    public void CmdRangerSkill()
    {
        RpcRangerSkill();
    }


    #endregion
    //All the functions here are the actual skills, they change stats/various things to change how the unit behaves
    #region RpcSkill
    [ClientRpc]
    public void RpcArcherSkill()
    {
        TMS.skillUsed = true;
        TMS.DisableHighlightUnitRange();
        selectedUnit.GetComponent<GenericUnit>().range += 1;
        TMS.HighlightUnitAttackOptionsFromPosition();
    }
    [ClientRpc]
    public void RpcCaptainSkill()
    {

        TMS.skillUsed = true;
        TMS.DisableHighlightUnitRange();
        selectedUnit.GetComponent<GenericUnit>().move += 1;
        if (TMS.selectedUnit != null)
        {
            TMS.HighlightUnitRange();
        }
    }
    [ClientRpc]
    public void RpcKnightSkill()
    {
        TMS.skillUsed = true;
        selectedUnit.GetComponent<PhysicalUnit>().strength += 1;
    }

    [ClientRpc]
    public void RpcWarriorSkill()
    {
        TMS.skillUsed = true;
        selectedUnit.GetComponent<PhysicalUnit>().strength += 2;
        selectedUnit.GetComponent<PhysicalUnit>().currentHealth -= 2;
    }

    [ClientRpc]
    public void RpcArcanistSkill()
    {
        TMS.skillUsed = true;
        TMS.DisableHighlightUnitRange();
        selectedUnit.GetComponent<MagicalUnit>().range += 3;
        selectedUnit.GetComponent<MagicalUnit>().maxHealth = 1;
        selectedUnit.GetComponent<MagicalUnit>().currentHealth = 1;
        TMS.HighlightUnitAttackOptionsFromPosition();
    }

    [ClientRpc]
    public void RpcRangerSkill()
    {
        print("RPCRangerBeforeIf");
        if (!NoSkillRanger)
        {
            NoSkillRanger = true;
            print("RPCRangerIf");
            TMS.skillUsed = true;
        }
    }
    #endregion
    //All the following call the Rpc version again
    #region Cmd Skill Off
    [Command(requiresAuthority = false)]
    public void CmdArcherSkillOff()
    {
        RpcArcherSkillOff();
    }


    [Command(requiresAuthority = false)]
    public void CmdCaptainSkillOff()
    {
        RpcCaptainSkillOff();
    }

    [Command (requiresAuthority =false)]
    public void CmdKnightSkillOff()
    {
        RpcKnightSkillOff();
    }


    [Command(requiresAuthority = false)]
    public void CmdWarriorSkillOff()
    {
        RpcWarriorSkillOff();
    }

    [Command (requiresAuthority =false)]
    public void CmdArcanistSkillOff()
    {
        RpcArcanistSkillOff();
    }

    [Command (requiresAuthority =false)]
    public void CmdRangerSkillOff()
    {
        RpcRangerSkillOff();
    }
    #endregion
    //All the following reset whatever stat was changed in the activation
    //They happen whenever the player deselects a unit, or when a unit is set to wait (finished for the turn)
    #region Rpc Skill Off
    [ClientRpc]
    public void RpcArcherSkillOff()
    {
        TMS.skillUsed = false;
        selectedUnit.GetComponent<GenericUnit>().range -= 1;
    }

    [ClientRpc]
    public void RpcCaptainSkillOff()
    {
        TMS.skillUsed = false;
        selectedUnit.GetComponent<GenericUnit>().move -= 1;
    }

    [ClientRpc]
    public void RpcKnightSkillOff()
    {
        TMS.skillUsed = false;
        selectedUnit.GetComponent<PhysicalUnit>().strength -= 1;
    }
    [ClientRpc]
    public void RpcWarriorSkillOff()
    {
        TMS.skillUsed = false;
        selectedUnit.GetComponent<PhysicalUnit>().strength -= 1;
    }

    [ClientRpc]
    public void RpcArcanistSkillOff()
    {
        TMS.skillUsed = false;
    }
    [ClientRpc]
    public void RpcRangerSkillOff()
    {
        TMS.skillUsed = false;
        Invoke("RangerReset", 1f);
        NoSkillRanger = true;
    }
    #endregion
    public void RangerReset()
    {
        selectedUnit.GetComponent<GenericUnit>().SetMovementState(0);
        selectedUnit.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
}