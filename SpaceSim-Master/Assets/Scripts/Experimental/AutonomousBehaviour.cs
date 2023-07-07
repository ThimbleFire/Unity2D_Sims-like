using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace AlwaysEast
{
    public class AutonomousBehaviour : MonoBehaviour
    {
        EntityStateMachine entityStateMachine;

        private void Start()
        {
            entityStateMachine = Resources.Load("LogicGraph") as EntityStateMachine;
            string instruction = entityStateMachine.NodeLink.First();
            HandleState( instruction );
        }

        private void HandleState( string nodeGUID )
        {
            string instruction = entityStateMachine.NodeData.Find( x => x.NodeGUID == nodeGUID).Instruction;
            //IEnumerable<NodeLinkData> solutions = entityStateMachine.NodeLinks.Where( x => x.BaseNodeGUID == nodeGUID );

            // perform instruction
        }
    }
}
