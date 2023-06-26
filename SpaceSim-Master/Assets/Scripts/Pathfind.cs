using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace AlwaysEast
{
    public class Node
    {
        public Vector3Int coordinate { get; set; }
        public Vector3 worldPosition { get; set; }
        public bool walkable = false;
        public Node parent;
        public byte distance = 0;

        public int gCost;
        public int hCost;
        public int fCost { get { return gCost + hCost; } }
    }

    public class Pathfind
    {
        public enum Report
        {
            OK,
            ATTEMPTING_TO_MOVE_ON_SELF,
            DESTINATION_IS_OCCUPIED_AND_IS_ADJACENT_TO_PLAYER_CHARACTER,
            NO_ADJACENT_NEIGHBOURS_TO_START_NODE,
            NO_ADJACENT_NEIGHBOURS_TO_END_NODE,
            NO_PATH_TO_FACILITY
        };

        private static Node[,] nodes;

        public static void Occupy( Vector3Int coordinates ) {
            nodes[coordinates.x, coordinates.y].walkable = false;
        }

        public static void Unoccupy( Vector3Int coordinates ) {
            nodes[coordinates.x, coordinates.y].walkable = true;
        }

        public static bool IsWalkable( Vector3Int coordinates ) {

            bool checkXInBounds = coordinates.x >= 0 && coordinates.x < nodes.GetLength(0);
            bool checkYInBounds = coordinates.y >= 0 && coordinates.y < nodes.GetLength(1);

            if( !checkXInBounds || !checkYInBounds || nodes[coordinates.x, coordinates.y] == null )
                return false;

            return nodes[coordinates.x, coordinates.y].walkable;
        }
        public static void Setup( Tilemap floorTileMap ) {
            nodes = new Node[floorTileMap.size.x + 1, floorTileMap.size.y + 1];
            foreach( Vector3Int position in floorTileMap.cellBounds.allPositionsWithin ) {
                nodes[position.x, position.y] = new Node() {
                    walkable = floorTileMap.GetTile( position ) != null,
                    coordinate = position,
                    worldPosition = Helper.CellToWorld( position )
                };
            }
        }

        /// <summary>
        /// Gets a path from Start to Destination. If destination has no neighbours, it is adjusted by Size until a path can be found.
        /// </summary>
        /// <param name="start">The start point</param>
        /// <param name="destination">The end point</param>
        /// <param name="size">The size of the destination should the default have no neighbours</param>
        /// <param name="report">Error handling</param>
        /// <returns>Returns a list of nodes that make up a path from start to finish.</returns>
        public static List<Node> GetPath( Vector3Int start, ref BoundsInt destination, Vector3Int[] applicableNeighbours, Vector2Int[] interactiveSpace, out Report report ) {
            Node startNode = nodes[start.x, start.y];
            Node endNode = nodes[destination.x, destination.y];

            // If destination is equal to start position, forfeit turn
            if( start == destination.position ) {
                report = Report.ATTEMPTING_TO_MOVE_ON_SELF;
                return null;
            }

            int disX = Mathf.Abs(startNode.coordinate.x - endNode.coordinate.x);
            int disY = Mathf.Abs(startNode.coordinate.y - endNode.coordinate.y);
            int distance = disX + disY;

            // If destination is occupied and distance is one tile away, forfeit turn
            if( endNode.walkable == false && distance == 1 ) {
                report = Report.DESTINATION_IS_OCCUPIED_AND_IS_ADJACENT_TO_PLAYER_CHARACTER;
                return null;
            }
            // Now it's established we're moving tiles, establish can we move
            List<Node> startNodeNeighbours = GetNeighbours(startNode);

            // If we can't move because there are no unoccupied neighbours, forfeit turn
            if( startNodeNeighbours.Count == 0 ) {
                report = Report.NO_ADJACENT_NEIGHBOURS_TO_START_NODE;
                return null;
            }

            if( endNode.walkable == false ) {
                // If the end node is occupied, move it to an adjacent tile

                foreach( Vector2Int space in interactiveSpace ) {
                    Vector3Int point = new Vector3Int(endNode.coordinate.x + space.x, endNode.coordinate.y + space.y, 0);
                    Node eNode = new Node { coordinate = point };
                    List<Node> endNodeNeighbours = GetNeighbours(eNode, applicableNeighbours);

                    if( endNodeNeighbours.Count > 0 ) {
                        destination.position = eNode.coordinate;
                        List<Node> neighboursSortedByDistance = SortNearest(startNode, endNodeNeighbours); // new code
                        Node pathToNeighbour = GetPathToNeighbour(startNode, neighboursSortedByDistance);

                        if( pathToNeighbour != null ) {
                            report = Report.OK;
                            return GetPath( startNode, pathToNeighbour );
                        }
                    }
                }

                report = Report.NO_PATH_TO_FACILITY;
                return null;
            }

            report = Report.OK;
            return GetPath( startNode, endNode );
        }

        private static Node GetPathToNeighbour( Node startNode, List<Node> neighboursSortedByDistance ) {
            foreach( Node node in neighboursSortedByDistance ) {
                List<Node> path = GetPath(startNode, node);
                if( path != null )
                    return path[path.Count - 1];
            }

            return null;
        }

        private static List<Node> GetPath( Node startNode, Node endNode ) {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add( startNode );

            while( openSet.Count > 0 ) {
                Node currentNode = openSet[0];

                for( int i = 0; i < openSet.Count; i++ ) {
                    if( openSet[i].fCost < currentNode.fCost ||
                        openSet[i].fCost == currentNode.fCost &&
                        openSet[i].hCost < currentNode.hCost ) {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove( currentNode );
                closedSet.Add( currentNode );

                if( currentNode == endNode ) {
                    return RetracePath( startNode, endNode );
                }

                List<Node> neighbours = GetNeighbours(currentNode); // We'll want to ignore walkables

                foreach( Node neighbour in neighbours ) {
                    if( neighbour.walkable == false || closedSet.Contains( neighbour ) ) {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if( newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains( neighbour ) ) {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance( neighbour, endNode );
                        neighbour.parent = currentNode;

                        if( !openSet.Contains( neighbour ) )
                            openSet.Add( neighbour );
                    }
                }
            }

            return null;
        }

        public static int GetDistance( Node a, Node b ) {
            int disX = Mathf.Abs(a.coordinate.x - b.coordinate.x);
            int disY = Mathf.Abs(a.coordinate.y - b.coordinate.y);
            return disX > disY ? 14 * disY + 10 * ( disX - disY ) : 14 * disX + 10 * ( disY - disX );
        }
        public static int GetDistance( Vector3Int a, Vector3Int b ) => GetDistance( nodes[a.x, a.y], nodes[b.x, b.y] );

        private static List<Node> RetracePath( Node startNode, Node endNode ) {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while( currentNode != startNode ) {
                path.Add( currentNode );
                currentNode = currentNode.parent;
            }

            path.Reverse();
            return path;
        }

        private static List<Node> GetNeighbours( Node node, Vector3Int[] offset = null )
        {
            List<Node> neighbours = new List<Node>();
            
            if(offset == null)
                offset = new Vector3Int[] { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left };

            for( int i = 0; i < offset.Length; i++ ) {
                int checkX = node.coordinate.x + offset[i].x;
                int checkY = node.coordinate.y + offset[i].y;

                bool checkXInBounds = checkX >= 0 && checkX < nodes.GetLength(0);
                bool checkYInBounds = checkY >= 0 && checkY < nodes.GetLength(1);

                if( !checkXInBounds || !checkYInBounds ) {
                    continue;
                }

                if( nodes[checkX, checkY] == null )
                    continue;

                if( nodes[checkX, checkY].walkable == false )
                    continue;

                neighbours.Add( nodes[checkX, checkY] );
            }

            return neighbours;
        }

        private static List<Node> SortNearest( Node startNode, List<Node> endNodes ) {
            foreach( Node node in endNodes ) {
                node.distance = ( byte )GetDistance( startNode, node );
            }

            endNodes.Sort( ( x, y ) => x.distance.CompareTo( y.distance ) );

            return endNodes;
        }

        // Have an entity wander to a random tile around an object
        public static Vector3Int GetRandomTile( Vector3Int origin ) {
            Node focalPoint = new Node() { coordinate = origin };
            var neighbours = GetNeighbours(focalPoint);
            return neighbours[Random.Range( 0, neighbours.Count - 1 )].coordinate;
        }
    }
}