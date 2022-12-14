/*
 * Author: Pius Meinert 
 * ? 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using ConversionOfBrutes.AI.Pathfinding;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ConversionOfBrutes.Map
{
	sealed class NavigationMeshEditor
	{
		#region Private member variables
		private List<Node> mNodes = new List<Node>();
		private readonly List<Tuple<Vector2, Vector2, float>> mEdges = new List<Tuple<Vector2, Vector2, float>>();
		private List<Vector2> mBorderCorners = new List<Vector2>();
		private List<Tuple<Vector2, Vector2, float>> mBorderEdges = new List<Tuple<Vector2, Vector2, float>>();
		private Node mSelectedNode;
		private Vector2 mSelectedCorner = new Vector2(float.MaxValue, float.MaxValue);

        private readonly Model mPoint;
        private readonly Texture2D mRed;
        private readonly Texture2D mBlue;
        private readonly Texture2D mYellow;

		private readonly MapEditor mMapEditor;

		private static NavigationMeshEditor sInstance;
#endregion

		/// <summary>
		/// Constructor 
		/// </summary>
		public NavigationMeshEditor(MapEditor mapEditor)
		{
			mMapEditor = mapEditor;

			sInstance = this;
			mPoint = Main.Content.Load<Model>("Map\\test");
			mRed = Main.Content.Load<Texture2D>("Map\\Floor12");
			mBlue = Main.Content.Load<Texture2D>("Map\\Floor13");
			mYellow = Main.Content.Load<Texture2D>("Map\\Floor14");
		}

		/// <summary>
		/// Surround the object with connected Nodes and BorderCorners.
		/// </summary>
		/// <param name="obj"></param>
		private void EmbedWorldObject(WorldObject obj)
		{
			if (!IsNearEnough(obj.Position))
			{
				return;
			}
			Vector2 leftTop = new Vector2(obj.Rect.X, obj.Rect.Y);
			Vector2 rightTop = new Vector2(obj.Rect.X + obj.Rect.Width, obj.Rect.Y);
			Vector2 rightBottom = new Vector2(obj.Rect.X + obj.Rect.Width, obj.Rect.Y + obj.Rect.Height);
			Vector2 leftBottom = new Vector2(obj.Rect.X, obj.Rect.Y + obj.Rect.Height);
			if (mBorderCorners.Any(c => c == rightBottom))
			{
				return;
			}

			mBorderCorners.Add(leftTop);
			mBorderCorners.Add(rightTop);
			mBorderCorners.Add(rightBottom);
			mBorderCorners.Add(leftBottom);
			mBorderEdges.Add(new Tuple<Vector2, Vector2, float>(leftTop, rightTop, Vector2.Distance(leftTop, rightTop)));
			mBorderEdges.Add(new Tuple<Vector2, Vector2, float>(rightTop, rightBottom, Vector2.Distance(rightTop, rightBottom)));
			mBorderEdges.Add(new Tuple<Vector2, Vector2, float>(rightBottom, leftBottom, Vector2.Distance(rightBottom, leftBottom)));
			mBorderEdges.Add(new Tuple<Vector2, Vector2, float>(leftBottom, leftTop, Vector2.Distance(leftBottom, leftTop)));

			AddNode(leftTop + new Vector2(-10, -10));
			AddNode(rightTop + new Vector2(+10, -10));
			AddNode(rightBottom + new Vector2(+10, +10));
			AddNode(leftBottom + new Vector2(-10, +10));
			AddEdge(GetNearestNode(leftTop), GetNearestNode(rightTop));
			AddEdge(GetNearestNode(rightTop), GetNearestNode(rightBottom));
			AddEdge(GetNearestNode(rightBottom), GetNearestNode(leftBottom));
			AddEdge(GetNearestNode(leftBottom), GetNearestNode(leftTop));
		}

		#region Nodes/Edges
		private void AddNode(Vector2 position)
		{
			if (mNodes.Any(n => position == n.mPosition))
			{
				return;
			}
			Node node = new Node
			{
				mPosition = position,
				mNeighbours = new List<Tuple<Node, float>>()
			};
			mNodes.Add(node);
		}

		private void MoveNode(Node node)
		{
			if (!IsNearEnough(node.mPosition))
			{
				return;
			}
			if (mNodes.Any(n => Calc3DPos(Main.Input.GetCurrentMousePosition) == n.mPosition))
			{
				return;
			}
			List<Node> updateNeighbours = new List<Node>();
			foreach (Tuple<Node, float> neighbour in node.mNeighbours)
			{
				updateNeighbours.Add(neighbour.Item1);
				neighbour.Item1.mNeighbours.Remove(new Tuple<Node, float>(node, neighbour.Item2));
			}
			node.mNeighbours.Clear();
			node.mPosition = Calc3DPos(Main.Input.GetCurrentMousePosition);
			foreach (Node neighbour in updateNeighbours)
			{
				neighbour.mNeighbours.Add(new Tuple<Node, float>(node,
					(float) Math.Sqrt(Math.Pow(Math.Abs(neighbour.mPosition.X - node.mPosition.X), 2) + Math.Pow(Math.Abs(neighbour.mPosition.Y - node.mPosition.Y), 2))));	
				node.mNeighbours.Add(new Tuple<Node, float>(neighbour,
					(float) Math.Sqrt(Math.Pow(Math.Abs(neighbour.mPosition.X - node.mPosition.X), 2) + Math.Pow(Math.Abs(neighbour.mPosition.Y - node.mPosition.Y), 2))));
			}
			mEdges.Clear();
			foreach (Node n in mNodes)
			{
				foreach (Tuple<Node, float> neighbour in n.mNeighbours)
				{
					mEdges.Add(new Tuple<Vector2, Vector2, float>(n.mPosition, neighbour.Item1.mPosition, neighbour.Item2));
				}	
			}
		}

		private void RemoveNode(Node node)
		{
			if (!IsNearEnough(node.mPosition))
			{
				return;
			}
			foreach (Tuple<Node, float> neighbour in node.mNeighbours)
			{
				neighbour.Item1.mNeighbours.Remove(new Tuple<Node, float>(node, neighbour.Item2));
				mEdges.Remove(new Tuple<Vector2, Vector2, float>(node.mPosition, neighbour.Item1.mPosition, neighbour.Item2));
				mEdges.Remove(new Tuple<Vector2, Vector2, float>(neighbour.Item1.mPosition, node.mPosition, neighbour.Item2));
			}
			mNodes.Remove(node);
		}

		private void AddEdge(Node node1, Node node2)
		{
			float distance = (float) Math.Sqrt(Math.Pow(Math.Abs(node1.mPosition.X - node2.mPosition.X), 2) +
			                                   Math.Pow(Math.Abs(node1.mPosition.Y - node2.mPosition.Y), 2));
			node1.mNeighbours.Add(new Tuple<Node, float>(node2, distance));
			node2.mNeighbours.Add(new Tuple<Node, float>(node1, distance));
			mEdges.Add(new Tuple<Vector2, Vector2, float>(node1.mPosition, node2.mPosition, distance));
		}
		private Node GetNearestNode(Vector2 position)
		{
			float closestDistance = float.MaxValue;
			Node nearestNode = mNodes[0];
			foreach (Node node in mNodes)
			{
				float distance = (float) Math.Sqrt(Math.Pow(Math.Abs(position.X - node.mPosition.X), 2) +
				                                   Math.Pow(Math.Abs(position.Y - node.mPosition.Y), 2));
				if (distance < closestDistance)
				{
					closestDistance = distance;
					nearestNode = node;
				}	
			}
			return nearestNode;
		}
#endregion

		#region BorderCorners
		private void MoveBorderCorner(Vector2 corner)
		{
			if (!IsNearEnough(corner))
			{
				return;
			}
			if (mBorderCorners.Any(c => Calc3DPos(Main.Input.GetCurrentMousePosition) == c))
			{
				return;
			}
			List<Vector2> updateCorners = new List<Vector2>();
			foreach (Tuple<Vector2, Vector2, float> borderEdge in mBorderEdges)
			{
				if (borderEdge.Item1 == corner)
				{
					updateCorners.Add(borderEdge.Item2);
				}
				if (borderEdge.Item2 == corner)
				{
					updateCorners.Add(borderEdge.Item1);
				}
			}
			mBorderCorners.Remove(corner);
			mBorderCorners.Add(Calc3DPos(Main.Input.GetCurrentMousePosition));
			foreach (Vector2 v in updateCorners)
			{
				mBorderEdges.Remove(new Tuple<Vector2, Vector2, float>(v, corner, Vector2.Distance(v, corner)));
				mBorderEdges.Remove(new Tuple<Vector2, Vector2, float>(corner, v, Vector2.Distance(corner, v)));
				Vector2 newPosition = Calc3DPos(Main.Input.GetCurrentMousePosition);
				mBorderEdges.Add(new Tuple<Vector2, Vector2, float>(v, newPosition, Vector2.Distance(v, newPosition)));
			}
		}

		private void RemoveBorderCorner(Vector2 corner)
		{
			if (!IsNearEnough(corner))
			{
				return;
			}
			mBorderCorners.Remove(corner);
			List<Tuple<Vector2, Vector2, float>> borderEdgesToRemove = mBorderEdges.Where(borderEdge => borderEdge.Item1 == corner || borderEdge.Item2 == corner).ToList();
			foreach (Tuple<Vector2, Vector2, float> removeBorderEdge in borderEdgesToRemove)
			{
				mBorderEdges.Remove(removeBorderEdge);
			}

		}

		private Vector2 GetNearestCorner(Vector2 position)
		{
			float closestDistance = float.MaxValue;
			Vector2 nearestCorner = mBorderCorners[0];
			foreach (Vector2 v in mBorderCorners)
			{
				float distance = (float) Math.Sqrt(Math.Pow(Math.Abs(position.X - v.X), 2) +
				                                   Math.Pow(Math.Abs(position.Y - v.Y), 2));
				if (distance < closestDistance)
				{
					closestDistance = distance;
					nearestCorner = v;
				}	
			}
			return nearestCorner;
		}
#endregion

		private static bool IsNearEnough(Vector2 position)
		{
			return !(Vector2.Distance(Calc3DPos(Main.Input.GetCurrentMousePosition), position) > 30);
		}

		internal static Vector2 Calc3DPos(Vector2 position)
        {
	        return GameScreen.Camera.CalcWorldPosition(position);
        }

		/// <summary>
		/// Handle input in order to create new nodes, edges and borders for the navigation graph.
		/// </summary>
		public void Update()
		{
			#region Nodes/Edges
			if (Main.Input.KeyboardState.IsKeyUp(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorAlt)))
			{
				if (Main.Input.LeftMouseClicked)
				{
					// Add node
					if (Main.Input.KeyboardState.IsKeyUp(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorMove)) &&
					    Main.Input.KeyboardState.IsKeyUp(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorDelete)))
					{
						AddNode(Calc3DPos(Main.Input.GetCurrentMousePosition));
					}
					// Remove node
					else if (Main.Input.KeyboardState.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorDelete)) && mNodes.Count > 0)
					{
						RemoveNode(GetNearestNode(Calc3DPos(Main.Input.GetCurrentMousePosition)));
					}
				}
				// Add edge
				else if (Main.Input.MiddleMouseClicked && mSelectedNode != null)
				{
					Node nearestNode = GetNearestNode(Calc3DPos(Main.Input.GetCurrentMousePosition));
					if (nearestNode != mSelectedNode && IsNearEnough(nearestNode.mPosition))
					{
						AddEdge(mSelectedNode, nearestNode);
					}

				}
				else if (mNodes.Count > 0)
				{
					// Select node 
					if (Main.Input.RightMouseClicked)
					{
						Node nearestNode = GetNearestNode(Calc3DPos(Main.Input.GetCurrentMousePosition));
						if (IsNearEnough(nearestNode.mPosition))
						{
							mSelectedNode = nearestNode;
						}
					}
					// Move node
					else if (Main.Input.MouseState.LeftButton == ButtonState.Pressed &&
					         Main.Input.KeyboardState.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorMove)))
					{
						MoveNode(GetNearestNode(Calc3DPos(Main.Input.GetCurrentMousePosition)));
					}
				}
			}
			#endregion
			#region BorderCorners/BorderEdges
			else
			{
				if (Main.Input.LeftMouseClicked)
				{
					// Add borderCorner
					if (Main.Input.KeyboardState.IsKeyUp(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorMove)) &&
					    Main.Input.KeyboardState.IsKeyUp(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorDelete)))
					{
						if (mBorderCorners.All(c => Calc3DPos(Main.Input.GetCurrentMousePosition) != c))
						{
							mBorderCorners.Add(Calc3DPos(Main.Input.GetCurrentMousePosition));
						}
					}
					// Remove borderCorner 
					else if (Main.Input.KeyboardState.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorDelete)) && mBorderCorners.Count > 0)
					{
						RemoveBorderCorner(GetNearestCorner(Calc3DPos(Main.Input.GetCurrentMousePosition)));
					}
				}
				// Add borderEdge
				else if (Main.Input.MiddleMouseClicked && mSelectedCorner != new Vector2(float.MaxValue, float.MaxValue))
				{
					Vector2 nearestCorner = GetNearestCorner(Calc3DPos(Main.Input.GetCurrentMousePosition));
					if (nearestCorner != mSelectedCorner && IsNearEnough(nearestCorner))
					{
						mBorderEdges.Add(new Tuple<Vector2, Vector2, float>(mSelectedCorner,
							nearestCorner,
							Vector2.Distance(mSelectedCorner, nearestCorner)));
					}
				}
				else if (mBorderCorners.Count > 0)
				{
					// Select borderCorner 
					if (Main.Input.RightMouseClicked)
					{
						Vector2 nearestCorner = GetNearestCorner(Calc3DPos(Main.Input.GetCurrentMousePosition));
						if (IsNearEnough(nearestCorner))
						{
							mSelectedCorner = nearestCorner;
						}
					}
					// Move node
					else if (Main.Input.MouseState.LeftButton == ButtonState.Pressed &&
					         Main.Input.KeyboardState.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorMove)))
					{
						MoveBorderCorner(GetNearestCorner(Calc3DPos(Main.Input.GetCurrentMousePosition)));
					}
				}
			}
			#endregion

			// Embed an object in the navigation graph
			if (Main.Input.KeyboardState.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorMove)) &&
				Main.Input.MiddleMouseClicked && GameScreen.ObjectManager.MapObjects.Count > 0)
			{
				EmbedWorldObject(mMapEditor.GetNearestObject(Calc3DPos(Main.Input.GetCurrentMousePosition)));
			}

			// Hide WorldObjects
			if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.HideObjects)))
			{
				GameScreen.WorldObjectsHidden = !GameScreen.WorldObjectsHidden;
			}

			if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorDelete)))
			{
				if (Main.Input.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorSafety)))
				{
					if (!Main.Input.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorAlt)))
					{
						mEdges.Clear();
						mNodes.Clear();
					}
					else
					{
						mBorderEdges.Clear();
						mBorderCorners.Clear();
					}
				}
			}
		}

		#region Draw
		/// <summary>
		/// Draws nodes, edges and borders on screen.
		/// </summary>
		public void Draw()
		{
			foreach (Node node in mNodes)
			{
				DrawPoint(node.mPosition,
					mSelectedNode == null ? new Vector2(float.MaxValue, float.MaxValue) : mSelectedNode.mPosition,
					mRed);
			}
			foreach (Vector2 corner in mBorderCorners)
			{
				DrawPoint(corner, mSelectedCorner, mBlue);
			}

			DrawEdges(mEdges, mRed);
			DrawEdges(mBorderEdges, mBlue);
		}

		private void DrawPoint(Vector2 position, Vector2 selectedPosition, Texture2D texture)
		{
			Vector3 modelPosition = Vector3.Zero;
			modelPosition.X = position.X;
			modelPosition.Y = 1;
			modelPosition.Z = position.Y;
			foreach (var mesh in mPoint.Meshes)
			{
				foreach (var effect in mesh.Effects.Cast<BasicEffect>())
				{
					effect.EnableDefaultLighting();
					effect.World = Matrix.CreateScale(0.0025f, 0.00125f, 0.00375f)
					               * Matrix.CreateTranslation(modelPosition);
					effect.View = GameScreen.Camera.ViewMatrix;
					effect.Projection = GameScreen.Camera.ProjectionMatrix;
					effect.AmbientLightColor = new Vector3(0.07f, 0.07f, 0.07f);
					effect.Texture = position == selectedPosition ? mYellow : texture;
					effect.TextureEnabled = true;


				}
				mesh.Draw();
			}
		}

		private void DrawEdges(List<Tuple<Vector2, Vector2, float>> edges, Texture2D texture)
		{
			Vector3 modelPosition = Vector3.Zero;
			foreach (Tuple<Vector2, Vector2, float> edge in edges)
			{
				if (edge.Item1.X - edge.Item2.X <= 0)
				{
					modelPosition.X = (edge.Item1.X);
					modelPosition.Z = (edge.Item1.Y);
				}
				else
				{
					modelPosition.X = (edge.Item2.X);
					modelPosition.Z = (edge.Item2.Y);
				
				}
				modelPosition.Y = 1;
				foreach (var mesh in mPoint.Meshes)
				{
					foreach (var effect in mesh.Effects.Cast<BasicEffect>())
					{
						effect.EnableDefaultLighting();
						int invertAngle = 1;
						if (edge.Item1.X - edge.Item2.X > 0)
						{
							invertAngle = -1;
						}
						effect.World = Matrix.CreateScale(edge.Item3 * 0.00050f, 0.001f, 0.001f) *
						               Matrix.CreateRotationY(MathHelper.ToRadians(
								               (float) (invertAngle * 100 * Math.Tan((edge.Item1.Y - edge.Item2.Y) / edge.Item3) / 2)))*
						               Matrix.CreateTranslation(modelPosition);
						effect.View = GameScreen.Camera.ViewMatrix;
						effect.Projection = GameScreen.Camera.ProjectionMatrix;
						effect.AmbientLightColor = new Vector3(0.07f, 0.07f, 0.07f);
						effect.Texture = texture;
						effect.TextureEnabled = true;
					}
					mesh.Draw();
				}
			}
		}
		#endregion
		public static NavigationMeshEditor Instance { get { return sInstance; } }
		public NavigationGraph GetNavigationGraphSaveInfo()
		{
			NavigationGraph navigationGraph = new NavigationGraph();
			navigationGraph.SetNodes(mNodes);
			navigationGraph.SetBorderEdges(mBorderEdges);
			return navigationGraph;
		}

		/// <summary>
		/// Load new Navigation Graph in the editor, to be invoked from the ingameMenuLoad (i guess?!)
		/// </summary>
		public void SetNavigationGraphInfo()
		{
			NavigationGraph navigationGraph = SaveAndLoad.NavigationGraph;

			mNodes.Clear();
			mBorderCorners.Clear();
			mBorderEdges.Clear();
			mEdges.Clear();
			mNodes = navigationGraph.GetNodes();
			mBorderEdges = navigationGraph.GetBorderEdges();

			foreach (Node node in mNodes)
			{
				foreach (Tuple<Node, float> neighbour in node.mNeighbours)
				{
					mEdges.Add(new Tuple<Vector2, Vector2, float>(node.mPosition, neighbour.Item1.mPosition, neighbour.Item2));
				}	
			}
			foreach (Tuple<Vector2, Vector2, float> borderEdge in mBorderEdges)
			{
				mBorderCorners.Add(borderEdge.Item1);
				mBorderCorners.Add(borderEdge.Item2);
			}
			mBorderCorners = mBorderCorners.Distinct().ToList();
		}
	}
}

