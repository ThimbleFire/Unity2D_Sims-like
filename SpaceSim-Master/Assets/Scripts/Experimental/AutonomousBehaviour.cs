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
            string instruction = entityStateMachine.NodeLinks.First();
            HandleState( instruction );
        }

        private void HandleState( string nodeGUID )
        {
            string instruction = entityStateMachine.Advance(nodeGUID, out List<NodeLinkData> nodeLinkData);

            // perform instruction
        }
    }
}
