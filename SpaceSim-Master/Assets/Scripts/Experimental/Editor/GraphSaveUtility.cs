using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AlwaysEast
{
    public class GraphSaveUtility
    {
        private BehaviourGraphView _graphView;
        private EntityStateMachine _EntityStateMachine;

        private List<Edge> Edges => _graphView.edges.ToList();
        private List<InstructionNode> Nodes => _graphView.nodes.ToList().Cast<InstructionNode>().ToList();

        private List<Group> CommentBlocks =>
            _graphView.graphElements.ToList().Where(x => x is Group).Cast<Group>().ToList();


        public static GraphSaveUtility GetInstance(StoryGraphView graphView)
        {
            return new GraphSaveUtility
            {
                _graphView = graphView
            };
        }

        public void SaveGraph(string fileName)
        {
            var EntityStateMachineObject = ScriptableObject.CreateInstance<EntityStateMachine>();
            if (!SaveNodes(fileName, EntityStateMachineObject)) return;
            SaveExposedProperties(EntityStateMachineObject);
            SaveCommentBlocks(EntityStateMachineObject);

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            UnityEngine.Object loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/Resources/{fileName}.asset", typeof(EntityStateMachine));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset)) 
			{
                AssetDatabase.CreateAsset(EntityStateMachineObject, $"Assets/Resources/{fileName}.asset");
            }
            else 
			{
                EntityStateMachine container = loadedAsset as EntityStateMachine;
                container.NodeLinks = EntityStateMachineObject.NodeLinks;
                container.InstructionNodeData = EntityStateMachineObject.InstructionNodeData;
                container.ExposedProperties = EntityStateMachineObject.ExposedProperties;
                container.CommentBlockData = EntityStateMachineObject.CommentBlockData;
                EditorUtility.SetDirty(container);
            }

            AssetDatabase.SaveAssets();
        }

        private bool SaveNodes(string fileName, EntityStateMachine EntityStateMachineObject)
        {
            if (!Edges.Any()) return false;
            var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedSockets.Count(); i++)
            {
                var outputNode = (connectedSockets[i].output.node as InstructionNode);
                var inputNode = (connectedSockets[i].input.node as InstructionNode);
                EntityStateMachineObject.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGUID = outputNode.GUID,
                    PortName = connectedSockets[i].output.portName,
                    TargetNodeGUID = inputNode.GUID
                });
            }

            foreach (var node in Nodes.Where(node => !node.EntyPoint))
            {
                EntityStateMachineObject.InstructionNodeData.Add(new InstructionNodeData
                {
                    NodeGUID = node.GUID,
                    Instruction = node.Instruction,
                    Position = node.GetPosition().position
                });
            }

            return true;
        }

        private void SaveExposedProperties(EntityStateMachine EntityStateMachine)
        {
            EntityStateMachine.ExposedProperties.Clear();
            EntityStateMachine.ExposedProperties.AddRange(_graphView.ExposedProperties);
        }

        private void SaveCommentBlocks(EntityStateMachine EntityStateMachine)
        {
            foreach (var block in CommentBlocks)
            {
                var nodes = block.containedElements.Where(x => x is InstructionNode).Cast<InstructionNode>().Select(x => x.GUID)
                    .ToList();

                EntityStateMachine.CommentBlockData.Add(new CommentBlockData
                {
                    ChildNodes = nodes,
                    Title = block.title,
                    Position = block.GetPosition().position
                });
            }
        }

        public void LoadNarrative(string fileName)
        {
            _EntityStateMachine = Resources.Load<EntityStateMachine>(fileName);
            if (_EntityStateMachine == null)
            {
                EditorUtility.Display("File Not Found", "Target Narrative Data does not exist!", "OK");
                return;
            }

            ClearGraph();
            GenerateInstructionNodes();
            ConnectInstructionNodes();
            AddExposedProperties();
            GenerateCommentBlocks();
        }

        /// <summary>
        /// Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too)
        /// </summary>
        private void ClearGraph()
        {
            Nodes.Find(x => x.EntyPoint).GUID = _EntityStateMachine.NodeLinks[0].BaseNodeGUID;
            foreach (var perNode in Nodes)
            {
                if (perNode.EntyPoint) continue;
                Edges.Where(x => x.input.node == perNode).ToList()
                    .ForEach(edge => _graphView.RemoveElement(edge));
                _graphView.RemoveElement(perNode);
            }
        }

        /// <summary>
        /// Create All serialized nodes and assign their guid and ue text to them
        /// </summary>
        private void GenerateInstructionNodes()
        {
            foreach (var perNode in _EntityStateMachine.InstructionNodeData)
            {
                var tempNode = _graphView.CreateNode(perNode.Instruction, Vector2.zero);
                tempNode.GUID = perNode.NodeGUID;
                _graphView.AddElement(tempNode);

                var nodePorts = _EntityStateMachine.NodeLinks.Where(x => x.BaseNodeGUID == perNode.NodeGUID).ToList();
                nodePorts.ForEach(x => _graphView.AddChoicePort(tempNode, x.PortName));
            }
        }

        private void ConnectInstructionNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var k = i; //Prevent access to modified closure
                var connections = _EntityStateMachine.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[k].GUID).ToList();
                for (var j = 0; j < connections.Count(); j++)
                {
                    var targetNodeGUID = connections[j].TargetNodeGUID;
                    var targetNode = Nodes.First(x => x.GUID == targetNodeGUID);
                    LinkNodesTogether(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);

                    targetNode.SetPosition(new Rect(
                        _EntityStateMachine.InstructionNodeData.First(x => x.NodeGUID == targetNodeGUID).Position,
                        _graphView.DefaultNodeSize));
                }
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _graphView.Add(tempEdge);
        }

        private void AddExposedProperties()
        {
            _graphView.ClearBlackBoardAndExposedProperties();
            foreach (var exposedProperty in _EntityStateMachine.ExposedProperties)
            {
                _graphView.AddPropertyToBlackBoard(exposedProperty);
            }
        }

        private void GenerateCommentBlocks()
        {
            foreach (var commentBlock in CommentBlocks)
            {
                _graphView.RemoveElement(commentBlock);
            }

            foreach (var commentBlockData in _EntityStateMachine.CommentBlockData)
            {
               var block = _graphView.CreateCommentBlock(new Rect(commentBlockData.Position, _graphView.DefaultCommentBlockSize),
                    commentBlockData);
               block.AddElements(Nodes.Where(x=>commentBlockData.ChildNodes.Contains(x.GUID)));
            }
        }
    }
}
