﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace SampleAStar
{
  public struct NodePosition
  {
    public int x;
    public int y;

    public NodePosition(int x, int y)
    {
      this.x = x;
      this.y = y;
    }
  }


  public class Path : List<NodePosition>
  {
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < Count; ++i)
      {
        sb.Append(string.Format("Node {0}: {1}, {2}", i, this[i].x, this[i].y));

        if (i < Count - 1)
        {
          sb.Append(" - ");
        }
      }

      return sb.ToString();
    }
  }


  public class Map
  {
    public int MAP_WIDTH = 20;
    public int MAP_HEIGHT = 20;

    private int[] map;

    private static readonly Map instance = new Map();
    public static Map Instance {
      get {
        return instance;
      }
    }

    public Map() {
    }

    public int GetMap(int x, int y)
    {
      if (x < 0 || x >= MAP_WIDTH || y < 0 || y >= MAP_HEIGHT)
      {
        return 9;
      }

      return map[(y * MAP_WIDTH) + x];
    }

    public void SetMap(Point dimension, int[] newMap) {
      map = newMap;
      MAP_WIDTH = (int)dimension.X;
      MAP_HEIGHT = (int)dimension.Y;
    }
  }



  public class MapSearchNode
  {
    public NodePosition position;
    AStarPathfinder pathfinder = null;


    public MapSearchNode(AStarPathfinder _pathfinder)
    {
      position = new NodePosition(0, 0);
      pathfinder = _pathfinder;
    }

    public MapSearchNode(NodePosition pos, AStarPathfinder _pathfinder)
    {
      position = new NodePosition(pos.x, pos.y);
      pathfinder = _pathfinder;
    }

    // Here's the heuristic function that estimates the distance from a Node
    // to the Goal. 
    public float GoalDistanceEstimate(MapSearchNode nodeGoal)
    {
      double X = (double)position.x - (double)nodeGoal.position.x;
      double Y = (double)position.y - (double)nodeGoal.position.y;
      return ((float)System.Math.Sqrt((X * X) + (Y * Y)));
    }

    public bool IsGoal(MapSearchNode nodeGoal)
    {
      return (position.x == nodeGoal.position.x && position.y == nodeGoal.position.y);
    }

    public bool ValidNeigbour(int xOffset, int yOffset)
    {
      // Return true if the node is navigable and within grid bounds
      return (Map.Instance.GetMap(position.x + xOffset, position.y + yOffset) < 9);
    }

    void AddNeighbourNode(int xOffset, int yOffset, NodePosition parentPos, AStarPathfinder aStarSearch)
    {
      if (ValidNeigbour(xOffset, yOffset) &&
        !(parentPos.x == position.x + xOffset && parentPos.y == position.y + yOffset))
      {
        NodePosition neighbourPos = new NodePosition(position.x + xOffset, position.y + yOffset);
        MapSearchNode newNode = pathfinder.AllocateMapSearchNode(neighbourPos);
        aStarSearch.AddSuccessor(newNode);
      }
    }

    // This generates the successors to the given Node. It uses a helper function called
    // AddSuccessor to give the successors to the AStar class. The A* specific initialisation
    // is done for each node internally, so here you just set the state information that
    // is specific to the application
    public bool GetSuccessors(AStarPathfinder aStarSearch, MapSearchNode parentNode)
    {
      NodePosition parentPos = new NodePosition(0, 0);

      if (parentNode != null)
      {
        parentPos = parentNode.position;
      }

      // push each possible move except allowing the search to go backwards
      AddNeighbourNode(-1, 0, parentPos, aStarSearch);
      AddNeighbourNode(0, -1, parentPos, aStarSearch);
      AddNeighbourNode(1, 0, parentPos, aStarSearch);
      AddNeighbourNode(0, 1, parentPos, aStarSearch);

      return true;
    }

    // given this node, what does it cost to move to successor. In the case
    // of our map the answer is the map terrain value at this node since that is 
    // conceptually where we're moving
    public float GetCost(MapSearchNode successor)
    {
      // Implementation specific
      return Map.Instance.GetMap(successor.position.x, successor.position.y);
    }

    public bool IsSameState(MapSearchNode rhs)
    {
      return (position.x == rhs.position.x &&
          position.y == rhs.position.y);


    }
  }


  public class AStarExample
  {
    static FrameworkElement _visualGrid;

    public static void Start(FrameworkElement fe, Point end)
    {
      if (_visualGrid == null) _visualGrid = fe;
      AStarPathfinder pathfinder = new AStarPathfinder();
      Pathfind(new NodePosition(0, 0), new NodePosition((int)end.X, (int)end.Y), pathfinder);
    }

    static bool Pathfind(NodePosition startPos, NodePosition goalPos, AStarPathfinder pathfinder)
    {
      // Reset the allocated MapSearchNode pointer
      pathfinder.InitiatePathfind();

      // Create a start state
      MapSearchNode nodeStart = pathfinder.AllocateMapSearchNode(startPos);

      // Define the goal state
      MapSearchNode nodeEnd = pathfinder.AllocateMapSearchNode(goalPos);

      // Set Start and goal states
      pathfinder.SetStartAndGoalStates(nodeStart, nodeEnd);

      // Set state to Searching and perform the search
      AStarPathfinder.SearchState searchState = AStarPathfinder.SearchState.Searching;
      uint searchSteps = 0;

      do
      {
        searchState = pathfinder.SearchStep();
        searchSteps++;
      }
      while (searchState == AStarPathfinder.SearchState.Searching);

      // Search complete
      bool pathfindSucceeded = (searchState == AStarPathfinder.SearchState.Succeeded);

      if (pathfindSucceeded)
      {
        // Success
        Path newPath = new Path();
        int numSolutionNodes = 0; // Don't count the starting cell in the path length

        // Get the start node
        MapSearchNode node = pathfinder.GetSolutionStart();
        newPath.Add(node.position);
        ++numSolutionNodes;

        // Get all remaining solution nodes
        for (; ; )
        {
          node = pathfinder.GetSolutionNext();

          if (node == null)
          {
            break;
          }

          ++numSolutionNodes;
          newPath.Add(node.position);

          // DRAW PATH TO GOAL
          var vnName = $"vnx{node.position.x}y{node.position.y}";
          VisualNode foundVisualNode = _visualGrid.FindName(vnName) as VisualNode;
          foundVisualNode?.SetDot(true);
        };

        // Once you're done with the solution we can free the nodes up
        pathfinder.FreeSolutionNodes();


        Debug.WriteLine("Solution path length: " + numSolutionNodes);
        Debug.WriteLine("Solution: " + newPath.ToString());
      }
      else if (searchState == AStarPathfinder.SearchState.Failed)
      {
        // FAILED, no path to goal
        Debug.WriteLine("Pathfind FAILED!");
      }

      return pathfindSucceeded;
    }
  }
}