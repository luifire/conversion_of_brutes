/*
 * Author: Pius Meinert 
 * TODO: Being able to delete or change nodes, edges and especially borderEdges and make stamps/predefined nodes for standard worldObjects.
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using ConversionOfBrutes.AI.Pathfinding;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ConversionOfBrutes.Misc
{
	class NavMeshEditor
	{
		private NavigationGraph mNavigationGraph = new NavigationGraph();
		private List<Node> mNodes = new List<Node>();
		private List<Vector2> mBorderCorners = new List<Vector2>();
		private List<Tuple<Vector2, Vector2>> mBorderEdges = new List<Tuple<Vector2, Vector2>>();
		private List<Tuple<Vector2, Vector2, float>> mEdges = new List<Tuple<Vector2, Vector2, float>>();
		private Node mSelectedNode;
		private Vector2 mSelectedCorner = new Vector2(float.MaxValue, float.MaxValue);

        private readonly Model mPoint;
        private Texture2D mTextureWalkable;
        private Texture2D mTextureBorder;
        private Texture2D mTextureSelected;

		private static NavMeshEditor sInstance;

		/// <summary>
		/// Constructor 
		/// </summary>
		public NavMeshEditor()
		{
			sInstance = this;
			mPoint = Main.Content.Load<Model>("Map\\test");
			mTextureWalkable = Main.Content.Load<Texture2D>("Map\\Floor12");
			mTextureBorder = Main.Content.Load<Texture2D>("Map\\Floor13");
			mTextureSelected = Main.Content.Load<Texture2D>("Map\\Floor14");
			CreateFromWorld();
		}

		private void CreateFromWorld()
		{
			foreach (WorldObject obj in GameScreen.ObjectManager.MapObjects)
			{
				if (!(obj is Zone))
				{
					Vector2 leftTop = new Vector2(obj.Rect.X, obj.Rect.Y);
					Vector2 rightTop = new Vector2(obj.Rect.X + obj.Rect.Width, obj.Rect.Y);
					Vector2 rightBottom = new Vector2(obj.Rect.X + obj.Rect.Width, obj.Rect.Y + obj.Rect.Height);
					Vector2 leftBottom = new Vector2(obj.Rect.X, obj.Rect.Y + obj.Rect.Height);

					mBorderCorners.Add(leftTop);
					mBorderCorners.Add(rightTop);
					mBorderCorners.Add(rightBottom);
					mBorderCorners.Add(leftBottom);
					mBorderEdges.Add(new Tuple<Vector2, Vector2>(leftTop, rightTop));
					mBorderEdges.Add(new Tuple<Vector2, Vector2>(rightTop, rightBottom));
					mBorderEdges.Add(new Tuple<Vector2, Vector2>(rightBottom, leftBottom));
					mBorderEdges.Add(new Tuple<Vector2, Vector2>(leftBottom, leftTop));

					Node lTop = new Node
					{
						mPosition = leftTop + new Vector2(-10, -10),
						mNeighbours = new List<Tuple<Node, float>>()
					};
					mNodes.Add(lTop);
					Node rTop = new Node
					{
						mPosition = rightTop + new Vector2(+10, -10),
						mNeighbours = new List<Tuple<Node, float>>()
					};
					mNodes.Add(rTop);
					Node rBottom = new Node
					{
						mPosition = rightBottom + new Vector2(+10, +10),
						mNeighbours = new List<Tuple<Node, float>>()
					};
					mNodes.Add(rBottom);
					Node lBottom = new Node
					{
						mPosition = leftBottom + new Vector2(-10, +10),
						mNeighbours = new List<Tuple<Node, float>>()
					};
					mNodes.Add(lBottom);
					AddEdge(lTop, rTop);
					AddEdge(rTop, rBottom);
					AddEdge(rBottom, lBottom);
					AddEdge(lBottom, lTop);
				}
			}
		}

		private void AddNode(Vector2 position)
		{
			Node node = new Node();
			node.mPosition = position;
			node.mNeighbours = new List<Tuple<Node, float>>();
			mNodes.Add(node);
		}

		private void MoveNode(Node node)
		{
			List<Node> updateNeighbours = new List<Node>();
			foreach (Tuple<Node, float> neighbour in node.mNeighbours)
			{
				updateNeighbours.Add(neighbour.Item1);
				neighbour.Item1.mNeighbours.Remove(new Tuple<Node, float>(node, neighbour.Item2));
			}
			node.mNeighbours.Clear();
			node.mPosition = Calc3DPos(Main.Input.GetCurrentMousePosition());
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

		private void MoveCorner(Vector2 corner)
		{
			List<Vector2> updateCorners = new List<Vector2>();
			foreach (Tuple<Vector2, Vector2> borderEdge in mBorderEdges)
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
			mBorderCorners.Add(Calc3DPos(Main.Input.GetCurrentMousePosition()));
			foreach (Vector2 v in updateCorners)
			{
				mBorderEdges.Remove(new Tuple<Vector2, Vector2>(v, corner));
				mBorderEdges.Remove(new Tuple<Vector2, Vector2>(corner, v));
				mBorderEdges.Add(new Tuple<Vector2, Vector2>(v, Calc3DPos(Main.Input.GetCurrentMousePosition())));
			}
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


        private Vector2 Calc3DPos(Vector2 position)
        {
	        return GameScreen.Camera.CalcWorldPosition(position);
			/*
            var nearScreenPoint = new Vector3(position.X, position.Y, 0);
            var farScreenPoint = new Vector3(position.X, position.Y, 1);
			var nearWorldPoint = Main.Graphics.GraphicsDevice.Viewport.Unproject(nearScreenPoint, GameScreen.Camera.ProjectionMatrix, GameScreen.Camera.ViewMatrix, Matrix.Identity);
			var farWorldPoint = Main.Graphics.GraphicsDevice.Viewport.Unproject(farScreenPoint, GameScreen.Camera.ProjectionMatrix, GameScreen.Camera.ViewMatrix, Matrix.Identity);
            var direction = farWorldPoint - nearWorldPoint;
            var zFactor = -nearWorldPoint.Y / direction.Y;
            var zeroWorldPoint = nearWorldPoint + direction * zFactor;

			return new Vector2(zeroWorldPoint.X, zeroWorldPoint.Z);*/
        }

		/// <summary>
		/// Handle input in order to create new nodes, edges and borders for the navigation graph.
		/// </summary>
		public void Update()
		{
			InputManager input = Main.Input;
			// Add node
			if (input.LeftMouseClicked() && input.KeyboardState.IsKeyUp(Keys.LeftControl) &&
				input.KeyboardState.IsKeyUp((Keys.LeftShift)) && input.KeyboardState.IsKeyUp(Keys.LeftAlt))
			{
				AddNode(Calc3DPos(input.GetCurrentMousePosition()));
			}
			// Move node
			if (input.LeftMouseClicked() && input.KeyboardState.IsKeyUp(Keys.LeftControl) &&
				input.KeyboardState.IsKeyUp((Keys.LeftShift)) && input.KeyboardState.IsKeyDown(Keys.LeftAlt) && mNodes.Count > 0)
			{
				MoveNode(GetNearestNode(Calc3DPos(input.GetCurrentMousePosition())));
			}
			// Remove node
			if (input.LeftMouseClicked() && input.KeyboardState.IsKeyUp(Keys.LeftControl) &&
				input.KeyboardState.IsKeyDown(Keys.LeftShift) && mNodes.Count > 0)
			{
				RemoveNode(GetNearestNode(Calc3DPos(input.GetCurrentMousePosition())));
			}
			// Add corner
			if (input.LeftMouseClicked() && input.KeyboardState.IsKeyDown(Keys.LeftControl) &&
				input.KeyboardState.IsKeyUp(Keys.LeftShift) && input.KeyboardState.IsKeyUp(Keys.LeftAlt))
			{
				mBorderCorners.Add(Calc3DPos(input.GetCurrentMousePosition()));
			}
			// Move corner
			if (input.LeftMouseClicked() && input.KeyboardState.IsKeyDown(Keys.LeftControl) &&
				input.KeyboardState.IsKeyUp(Keys.LeftShift) && input.KeyboardState.IsKeyDown(Keys.LeftAlt) && mBorderCorners.Count > 0)
			{
				MoveCorner(GetNearestCorner(Calc3DPos(input.GetCurrentMousePosition())));
			}
			// Remove corner
			if (input.LeftMouseClicked() && input.KeyboardState.IsKeyDown(Keys.LeftControl) &&
				input.KeyboardState.IsKeyDown(Keys.LeftShift) && mBorderCorners.Count > 0)
			{
				mBorderCorners.Remove(GetNearestCorner(Calc3DPos(input.GetCurrentMousePosition())));
			}
			// Select node 
			if (input.RightMouseClicked() && mNodes.Count > 0 && input.KeyboardState.IsKeyUp(Keys.LeftControl))
			{
				mSelectedNode = GetNearestNode(Calc3DPos(input.GetCurrentMousePosition()));
			}
			// Select corner 
			if (input.RightMouseClicked() && mBorderCorners.Count > 0  && input.KeyboardState.IsKeyDown(Keys.LeftControl))
			{
				mSelectedCorner =
					GetNearestCorner(Calc3DPos(input.GetCurrentMousePosition()));
			}
			// Add walkable edge
			if (input.MiddleMouseClicked() && mSelectedNode != null && input.KeyboardState.IsKeyUp(Keys.LeftControl) &&
				input.KeyboardState.IsKeyUp(Keys.LeftAlt))
			{
				Node nearestNode = GetNearestNode(Calc3DPos(input.GetCurrentMousePosition()));
				if (nearestNode != mSelectedNode)
				{
					AddEdge(mSelectedNode, nearestNode);
				} 
			}
			// Add border edge
			if (input.MiddleMouseClicked() && mSelectedCorner != new Vector2(float.MaxValue, float.MaxValue)  &&
				input.KeyboardState.IsKeyDown(Keys.LeftControl) && input.KeyboardState.IsKeyUp(Keys.LeftAlt))
			{
				Vector2 nearestCorner = GetNearestCorner(Calc3DPos(input.GetCurrentMousePosition()));
				if (nearestCorner != mSelectedCorner)
				{
					mBorderEdges.Add(new Tuple<Vector2, Vector2>(mSelectedCorner, nearestCorner));
				} 
			}
			// Save the current navigation graph.
			if (Main.Input.WasButtonPressed(Keys.F5))
			{
				mNavigationGraph.SetNodes(mNodes);
				mNavigationGraph.SetBorderEdges(mBorderEdges);
				Serialization.SaveNavigationGraph(mNavigationGraph);
			}
			// Load the saved navigation graph.
			if (Main.Input.WasButtonPressed(Keys.F6))
			{
				mNodes.Clear();
				mBorderCorners.Clear();
				mBorderEdges.Clear();
				mEdges.Clear();
				mNavigationGraph = Serialization.LoadNavigationGraphGraph();
				mNodes = mNavigationGraph.GetNodes();
				mBorderEdges = mNavigationGraph.GetBorderEdges();
				foreach (Node node in mNodes)
				{
					foreach (Tuple<Node, float> neighbour in node.mNeighbours)
					{
						mEdges.Add(new Tuple<Vector2, Vector2, float>(node.mPosition, neighbour.Item1.mPosition, neighbour.Item2));
					}	
				}
				foreach (Tuple<Vector2, Vector2> borderEdge in mBorderEdges)
				{
					mBorderCorners.Add(borderEdge.Item1);
					mBorderCorners.Add(borderEdge.Item2);
				}
				mBorderCorners = mBorderCorners.Distinct().ToList();
			}
		}

		/// <summary>
		/// Draws nodes, edges and borders on screen.
		/// </summary>
		public void Draw()
		{
			Vector3 modelPosition = Vector3.Zero;
			foreach (Node node in mNodes)
			{
				//var modelTransforms = new Matrix[mPoint.Bones.Count];
				//mPoint.CopyAbsoluteBoneTransformsTo(modelTransforms);

				modelPosition.X = node.mPosition.X;
				modelPosition.Y = 1;
				modelPosition.Z = node.mPosition.Y;
				foreach (var mesh in mPoint.Meshes)
				{
					foreach (var effect in mesh.Effects.Cast<BasicEffect>())
					{
						effect.EnableDefaultLighting();
						//effect.World = modelTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(0.0075f, 0.005f, 0.01f)
						//			   * Matrix.CreateTranslation(modelPosition);
						effect.World = Matrix.CreateScale(0.0075f, 0.005f, 0.01f)
						               * Matrix.CreateTranslation(modelPosition);
						effect.View = GameScreen.Camera.ViewMatrix;
						effect.Projection = GameScreen.Camera.ProjectionMatrix;
						effect.AmbientLightColor = new Vector3(0.07f, 0.07f, 0.07f);
						if (node == mSelectedNode)
						{
							effect.Texture = mTextureSelected;
						}
						else
						{
							effect.Texture = mTextureWalkable;
						}
						effect.TextureEnabled = true;


					}
					mesh.Draw();
				}
			}

			DrawEdges(mEdges, mTextureWalkable);

			List<Tuple<Vector2, Vector2, float>> newBorderEdges = new List<Tuple<Vector2, Vector2, float>>();
			foreach (Tuple<Vector2, Vector2> edge in mBorderEdges)
			{
				newBorderEdges.Add(new Tuple<Vector2, Vector2, float>(edge.Item1, edge.Item2,
					Vector2.Distance(edge.Item1, edge.Item2)));
			}
			DrawEdges(newBorderEdges, mTextureBorder);
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
						if (edge.Item1.X - edge.Item2.X >= 0)
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

		public static NavMeshEditor Instance { get { return sInstance; } }
		public NavigationGraph NavigationGraph { get { return mNavigationGraph; } }
	}
}

