using System;
using System.Linq;
using ConversionOfBrutes.Animation;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Library;
using ConversionOfBrutes.Map;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*
 * Author: Julian Bürklin, julian Löffler
 * 
 * Graphics Manager Class
 * Usage: drawing and loading graphics
 */


namespace ConversionOfBrutes.Graphic
{

	public class GraphicsManager
	{
		private Model[] mModels;
		private string[] mModelAssets;
		public AnimationManager mAnimationManager;
		private readonly SpriteBatch mSpriteBatch;
		private Texture2D mWhitePixel;
		private SpriteFont mFont;
		private Texture2D mSelectionButtons;

		private bool mDrawHealthBar = true;
		// Map
		private Model mMap;
		private Texture2D mMapTexture;

		// fog of war
		private FogOfWar mFogOfWar;


		//precalulated matrices
		private Matrix mEliteBarbarianTransform;
		private Matrix mUnitTransform;
		
		/// <summary>
		/// Constructor
		/// </summary>
		public GraphicsManager()
		{
			Initialize();
			mSpriteBatch = new SpriteBatch(Main.Graphics.GraphicsDevice);
		}

		private void Initialize()
		{
			// Non Animated Worldobjects

			mModels = new Model[Enum.GetNames(typeof(Ident)).Length + 1];
			mModels[(int)Ident.Castle] = Main.Content.Load<Model>("Map\\Objects\\Castle\\casteBig");
			mModels[(int)Ident.Tree] = Main.Content.Load<Model>("Map\\Objects\\Tree\\trees\\tree");
			mModels[(int)Ident.Tree2] = Main.Content.Load<Model>("Map\\Objects\\Tree\\trees\\tree");
			mModels[(int)Ident.Spawnzone] = Main.Content.Load<Model>("Map\\Objects\\Castle\\castle");
			mModels[(int)Ident.Zone] = Main.Content.Load<Model>("Map\\Objects\\House\\Village");
			mModels[(int)Ident.MountainSmall] = Main.Content.Load<Model>("Map\\Objects\\Rock\\berg_klein");
			mModels[(int)Ident.Mountain] = Main.Content.Load<Model>("Map\\Objects\\Rock\\berg");
			mModels[(int)Ident.MountainBig] = Main.Content.Load<Model>("Map\\Objects\\Rock\\berg_groß");
			mModels[(int)Ident.Pond] = Main.Content.Load<Model>("Map\\Objects\\Pond\\pond");
			mModels[(int)Ident.SelectionCircle] = Main.Content.Load<Model>("HUD\\BlueCircle");
			mModels[(int)Ident.ZoneCircle] = Main.Content.Load<Model>("Map\\Objects\\Zone\\ZoneCircle");
			mModels[(int)Ident.HomeZone] = Main.Content.Load<Model>("Map\\Objects\\Castle\\casteBig");
			mModels[(int)Ident.Waypoint] = Main.Content.Load<Model>("HUD\\Waypoint");
		
			mAnimationManager = new AnimationManager();

			// Animated Models
			mModelAssets = new string[Enum.GetNames(typeof(Ident)).Length];
			mModelAssets[(int)Ident.EliteBarbarian] = "Units\\EliteBarbarian\\Warrior-tpose";
			mModelAssets[(int)Ident.Axeman] = "Units\\Axeman\\Axeman-tpose";
			mModelAssets[(int)Ident.Archer] = "Units\\Archer\\Archer-tpose";
			mModelAssets[(int)Ident.ArcherMounted] = "Units\\ArcherMounted\\ArcherMounted-Tpose";
			mModelAssets[(int)Ident.Knight] = "Units\\Knight\\Axeman-tpose";
			mModelAssets[(int)Ident.Beast] = "Units\\Beast\\Beast-Tpose";
			mModelAssets[(int)Ident.EliteAtlantic] = "Units\\EliteAtlantic\\EliteAtlantic-tpose";
			mModelAssets[(int)Ident.Priest] = "Units\\Priest\\Priest-tpose";
			mModelAssets[(int)Ident.PriestRanged] = "Units\\PriestRanged\\PriestRanged-tpose";
			mModelAssets[(int)Ident.ShieldGuard] = "Units\\ShieldGuard\\ShieldGuard-tpose";
			mModelAssets[(int)Ident.Horse] = "Units\\Horse\\Horse-tpose";
			mModelAssets[(int)Ident.Witch] = "Units\\Witch\\Witch-tpose";
			mModelAssets[(int)Ident.TechdemoAtlantic] = "Units\\Techdemo\\Atlantic\\EliteAtlantic-tpose";
			mModelAssets[(int)Ident.TechdemoBarb] = "Units\\Techdemo\\Axeman\\Axeman-tpose";
			
			mWhitePixel = new Texture2D(Main.Graphics.GraphicsDevice, 1, 1);
			mWhitePixel.SetData(new[] { Color.BlueViolet });

			// Map
			mMap = Main.Content.Load<Model>(SaveAndLoad.MapModellName);
			mMapTexture = Main.Content.Load<Texture2D>(SaveAndLoad.MapTextureName);

			// Fog of War 
			mFogOfWar = new FogOfWar();

			// Font
			mFont = Main.Content.Load<SpriteFont>("Fonts\\Helvetica");

			// spriteSheet
			mSelectionButtons = Main.Content.Load<Texture2D>("Button//SelectionButtons");

			// precalculated matrices
			mEliteBarbarianTransform = Matrix.CreateScale(0.1f) * Matrix.CreateRotationX(-MathHelper.PiOver2);
			mUnitTransform = Matrix.CreateScale(0.1f);
			
		}
		
		/// <summary>
		/// returns asset name of a unit model
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string GetAsset(Ident id)
		{
			return mModelAssets[(int)id];
		}

		/// <summary>
		/// Draws the specified model at the given position
		/// </summary>
		/// <param name="modelIdentifier"></param>
		/// <param name="position"></param>
		/// <param name="fraction"></param>
		/// <param name="seen"></param>
		/// <param name="selected"></param>
		/// <returns></returns>
		public void DrawWorldObject(Ident modelIdentifier, Vector2 position,Fraction fraction = Fraction.Gaia, bool seen = false, bool selected = false)
		{
			Main.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
			Main.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			Main.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

			Vector3 modelPosition;
			modelPosition.X = position.X;
			modelPosition.Y = 1;
			modelPosition.Z = position.Y;

			Model model = mModels[(int)modelIdentifier];
			
			if ((modelIdentifier == Ident.Spawnzone || modelIdentifier == Ident.Zone))
			{
				DrawWorldObject(Ident.ZoneCircle, position,fraction,seen,selected);
			}

			// Draw the Model 
			// TODO: Precalc for every Static model
			var modelTransforms = new Matrix[model.Bones.Count];
			model.CopyAbsoluteBoneTransformsTo(modelTransforms);
			
			foreach (var mesh in model.Meshes)
			{ 
				
				foreach (var effect in mesh.Effects.Cast<BasicEffect>())
				{
					
					effect.EnableDefaultLighting();
					effect.World = modelTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(modelPosition);
					effect.View = GameScreen.Camera.ViewMatrix;
					effect.Projection = GameScreen.Camera.ProjectionMatrix;
					
					//effect.SpecularColor = Color.Yellow.ToVector3();
					if (selected)
					{
						Color color;
						switch (fraction)
						{
							case Fraction.Player:
								color = Color.DeepSkyBlue;
								break;
							case Fraction.Ai:
								color = Color.Red;
								break;
							default:
								color = Color.GhostWhite;
								break;
						}

						effect.AmbientLightColor = color.ToVector3();
					}

					// ZoneCircle
					if (modelIdentifier == Ident.ZoneCircle)
					{
						//return true;
						effect.FogEnabled = true;
						effect.FogColor = Color.White.ToVector3();
						

						// FOG OF WAR
						if (GameScreen.FogOfWar)
						{
							if (seen && fraction != Fraction.Gaia)
							{
								if (fraction == Fraction.Player)
								{
									effect.FogColor = selected ? Color.LightBlue.ToVector3() : Color.Blue.ToVector3();
								}
								else if (fraction == Fraction.Ai)
								{
									effect.FogColor = selected ? Color.Red.ToVector3() : Color.DarkRed.ToVector3();
								}
							}
						}
						else
						{
							if (fraction == Fraction.Player) effect.FogColor = Color.Blue.ToVector3();
							else if (fraction == Fraction.Ai) effect.FogColor = Color.Red.ToVector3();
						}

					}

					// SelectionCircle
					if (modelIdentifier == Ident.SelectionCircle)
					{
						effect.FogEnabled = true;
						if (fraction == Fraction.Player) effect.FogColor = Color.LightBlue.ToVector3();
						else if (fraction == Fraction.Ai) effect.FogColor = Color.Red.ToVector3();
					}

				}
				mesh.Draw();
			}

		}

		/// <summary>
		/// Draws a healthbar
		/// optional Spritebatch is needed for HUD
		/// </summary>
		/// <param name="bar"></param>
		/// <param name="optionalBatch">Optional: Give the SpriteBatch that should draw the bar.</param>
		public void DrawHealthBar(Healthbar bar, SpriteBatch optionalBatch = null)
		{
			if (mDrawHealthBar == false) return;

			bool useMemberBatch = optionalBatch == null;
			if (useMemberBatch)
				mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			SpriteBatch dmyBatch = useMemberBatch ? mSpriteBatch : optionalBatch;

			Color backgroundColor = new Color(0, 0, 0, 200);
			Color barColor = bar.ShowsFp ? Color.DeepSkyBlue : new Color(0, 255, 0, 200);

			Rectangle backgroundRectangle = new Rectangle
			{
				Width = (int) bar.ScaledDimension.X,
				Height = (int) bar.ScaledDimension.Y,
				X = (int) bar.Position.X,
				Y = (int) bar.Position.Y
			};

			dmyBatch.Draw(mSelectionButtons, backgroundRectangle, bar.BackSourceRect, backgroundColor);

			backgroundRectangle.Width = (int)(bar.ScaledDimension.X * 0.9);
			backgroundRectangle.Height = (int)(bar.ScaledDimension.Y * 0.5);
			backgroundRectangle.X = (int)bar.Position.X + (int)(bar.ScaledDimension.X * 0.05);
			backgroundRectangle.Y = (int)bar.Position.Y + (int)(bar.ScaledDimension.Y * 0.25);

			dmyBatch.Draw(mSelectionButtons, backgroundRectangle, (bar.RepresentedObject is Zone) ? new Rectangle(535, 934, 1, 1) : bar.BackSourceRect, (bar.RepresentedObject is Zone) ? Color.DarkRed : barColor);

			backgroundRectangle.Width = (int)(bar.ScaledDimension.X * 0.9 * bar.Percent);
			backgroundRectangle.Height = (int)(bar.ScaledDimension.Y * 0.5);
			backgroundRectangle.X = (int)bar.Position.X + (int)(bar.ScaledDimension.X * 0.05);
			backgroundRectangle.Y = (int)bar.Position.Y + (int)(bar.ScaledDimension.Y * 0.25);

			dmyBatch.Draw(mSelectionButtons, backgroundRectangle, bar.BarSourceRect, barColor);

			if (useMemberBatch)
				mSpriteBatch.End();
		}


		public void DrawUnit(AnimatedModel animatedModel, Ident ident, Vector2 position, float modelRotation)
		{
			Main.Graphics.GraphicsDevice.BlendState = BlendState.Opaque;
			Main.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			Main.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
			AnimatedModel model = animatedModel;
			Vector3 modelpos;
			modelpos.X = position.X;
			modelpos.Y = 10;
			modelpos.Z = position.Y;

			//
			// Compute all of the bone absolute transforms
			//
			Matrix[] boneTransforms = new Matrix[model.mBones.Count];

			for (int i = 0; i < model.mBones.Count; i++)
			{
				Bone bone = model.mBones[i];
				bone.ComputeAbsoluteTransform();

				boneTransforms[i] = bone.mAbsoluteTransform;
			}

			//
			// Determine the skin transforms from the skeleton
			//

			Matrix[] skeleton = new Matrix[model.mModelExtra.Skeleton.Count];
			for (int s = 0; s < model.mModelExtra.Skeleton.Count; s++)
			{
				Bone bone = model.mBones[model.mModelExtra.Skeleton[s]];
				skeleton[s] = bone.SkinTransform * bone.mAbsoluteTransform;
			}

			// Kngiht and Mounted archers have to be higher due the horse
			if (ident == Ident.Knight || ident == Ident.ArcherMounted)
			{
				modelpos.Y = 20;
			}
			// Kngiht and Mounted archers Horse has to be higher.
			if (ident == Ident.Horse)
			{
				modelpos.Y = 16;
			}

			// Elitebarbarian model is rotated, rotate it in the right direction.
			Matrix worldTransformation;
			if (ident == Ident.EliteBarbarian)
			{
				worldTransformation = mEliteBarbarianTransform * Matrix.CreateRotationY(MathHelper.PiOver2 + modelRotation) * Matrix.CreateTranslation(modelpos);
			}
			
			// normal
			else
			{
				worldTransformation = mUnitTransform * Matrix.CreateRotationY(MathHelper.Pi + modelRotation) * Matrix.CreateTranslation(modelpos);
			}

			// Draw the model.
			foreach (ModelMesh modelMesh in model.mOdel.Meshes)
			{
				foreach (Effect effect in modelMesh.Effects)
				{
					SkinnedEffect seffect = effect as SkinnedEffect;
					if (seffect != null)
					{
						seffect.World = boneTransforms[modelMesh.ParentBone.Index] * worldTransformation;
						seffect.View = GameScreen.Camera.ViewMatrix;
						seffect.Projection = GameScreen.Camera.ProjectionMatrix;
						seffect.SetBoneTransforms(skeleton);
					}
				}

				modelMesh.Draw();
			}
		}

		/// <summary>
		/// ok not really the best position for this function
		/// draws a rectangle
		/// </summary>
		/// <param name="area"></param>
		public void DrawArea(Area area)
		{
			Rectangle[] rects = area.GetRectanglesBordersAsRectangleLines();

			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			foreach (var rec in rects)
			{
				mSpriteBatch.Draw(mWhitePixel, rec, Color.White);
			}
			mSpriteBatch.End();
		}

		public void DrawText(Vector2 position, String text, Color color)
		{
			Vector3 fontPos;

			fontPos.X = position.X;
			fontPos.Y = 10;
			fontPos.Z = position.Y;
			
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			
			Vector3 projectedPosition = Main.Graphics.GraphicsDevice.Viewport.Project(fontPos, GameScreen.Camera.ProjectionMatrix, GameScreen.Camera.ViewMatrix, Matrix.Identity);
			Vector2 screenPosition = new Vector2(projectedPosition.X, projectedPosition.Y);

			// Draw the string
			mSpriteBatch.DrawString(mFont, text, screenPosition, color);
			mSpriteBatch.End();
		}

		/// <summary>
		/// Draws the Map
		/// </summary>
		public void DrawMap()
		{

			Main.Graphics.GraphicsDevice.BlendState = BlendState.Opaque;
			Main.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			Main.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

			var modelTransforms = new Matrix[mMap.Bones.Count];
			mMap.CopyAbsoluteBoneTransformsTo(modelTransforms);
			Vector3 ambiLight = new Vector3(0.07f, 0.07f, 0.07f);

			foreach (var mesh in mMap.Meshes)
			{
				foreach (var effect in mesh.Effects.Cast<BasicEffect>())
				{
					effect.EnableDefaultLighting();
					effect.World = modelTransforms[mesh.ParentBone.Index]; // * Matrix.Identity
					effect.View = GameScreen.Camera.ViewMatrix;
					effect.Projection = GameScreen.Camera.ProjectionMatrix;
					effect.AmbientLightColor = ambiLight;
					effect.Texture = mMapTexture;
					effect.TextureEnabled = true;
				}
				mesh.Draw();
			}
		}

		public void DrawFogOfWar()
		{
			mFogOfWar.DrawFow();
		}

		public SpriteBatch SpriteBatch
		{
			get { return mSpriteBatch; }
		}
		public void ToggleHealthBar()
		{
			mDrawHealthBar = !mDrawHealthBar;
		}
	}
}
