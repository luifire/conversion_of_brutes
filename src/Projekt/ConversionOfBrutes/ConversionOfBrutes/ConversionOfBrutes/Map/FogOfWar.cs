using System.Linq;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Map
{
	class FogOfWar
	{
		private Model mFowCircle;
		private Model mFogOfWar;
		private Texture2D mFogOfWarTexture;
		Matrix[] mFowTransforms;
		Matrix mFowTranslation;

		public FogOfWar()
		{
			mFowCircle = Main.Content.Load<Model>("Map\\fowcircle");
			mFogOfWar = Main.Content.Load<Model>("Map\\fow");
			mFogOfWarTexture = Main.Content.Load<Texture2D>("Map\\FowTexture");
			mFowTransforms = new Matrix[mFogOfWar.Bones.Count];
			mFogOfWar.CopyAbsoluteBoneTransformsTo(mFowTransforms);
			mFowTranslation = Matrix.CreateTranslation(0, 50, 0);
		}

		private void DrawPlayerSight()
		{
			var objects = GameScreen.ObjectManager.SelectableObjects;
			foreach (var obj in objects)
			{
				if (!GameScreen.Camera.FogOfWarVisibleRectangle.Contains(obj.PointPosition)) continue;
				if (obj.Fraction != Fraction.Player) continue;
				

				Main.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
				Main.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
				Main.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

				int offset = 8;
				

				Vector3 modelPosition;
				modelPosition.X = obj.Position.X + offset;
				modelPosition.Y = 50;
				modelPosition.Z = obj.Position.Y + offset;
				Model model = mFowCircle;

				// Draw the Model
				var modelTransforms = new Matrix[model.Bones.Count];
				model.CopyAbsoluteBoneTransformsTo(modelTransforms);

				foreach (var mesh in model.Meshes)
				{
					foreach (var effect in mesh.Effects.Cast<BasicEffect>())
					{
						effect.EnableDefaultLighting();
						if (obj.Ident == Ident.HomeZone) effect.World = modelTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(2f) * Matrix.CreateTranslation(modelPosition);
						else effect.World = modelTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(modelPosition);

						effect.View = GameScreen.Camera.ViewMatrix;
						effect.Projection = GameScreen.Camera.ProjectionMatrix;
						effect.Alpha = 0.0f;

					}
					mesh.Draw();
				}
				Main.Graphics.GraphicsDevice.BlendState = BlendState.Opaque;
			}
		}

		/// <summary>
		/// Draw the Fog of War
		/// </summary>
		public void DrawFow()
		{
			DrawPlayerSight();
			Main.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
			Main.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			Main.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
			
			Model model = mFogOfWar;

			// Draw the Model
			foreach (var mesh in model.Meshes)
			{
				foreach (var effect in mesh.Effects.Cast<BasicEffect>())
				{
					effect.EnableDefaultLighting();
					effect.World = mFowTransforms[mesh.ParentBone.Index] * mFowTranslation;
					effect.View = GameScreen.Camera.ViewMatrix;
					effect.Projection = GameScreen.Camera.ProjectionMatrix;
					effect.TextureEnabled = true;
					effect.Texture = mFogOfWarTexture;
					effect.Alpha = 0.5f;
				}
				mesh.Draw();
			}
			Main.Graphics.GraphicsDevice.BlendState = BlendState.Opaque;

		}
	}
}
