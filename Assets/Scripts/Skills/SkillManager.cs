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
    #endregion

}