using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlwaysEast
{
    public class AutonomousBehaviour : MonoBehaviour
    {
        EntityStateMachine entityStateMachine;

        private void Start()
        {
            entityStateMachine = Resources.Load("LogicGraph") as EntityStateMachine;
            var instruction = entityStateMachine.NodeLinks.First();
            HandleState( instruction.TargetNodeGUID );
        }

        private void HandleState( string nodeGUID )
        {
            string instruction = entityStateMachine.Advance(nodeGUID, out IEnumerable<NodeLinkData> nodeLinkData);

            // perform instruction
        }
    }
}
