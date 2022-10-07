#region Using Statements

using ConversionOfBrutes.GameObjects;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace ConversionOfBrutes.Graphic.ParticleEffects.Particlesystems
{
	class MagicAttackParticleSystem : DefaultTexturedQuadParticleSystem
	{
		private Vector3 mPosition;
		private Vector3 mTargetPosition;
		private Unit mUnit;
		private Vector3 Position { get { return mPosition; } }
		public Unit Unit { get { return mUnit; } }

		/// <summary>
		/// Constructor
		/// </summary>
		public MagicAttackParticleSystem(Game cGame, Unit unit)
			: base(cGame)
		{
			mUnit = unit;

			mPosition.X = unit.Position.X;
			mPosition.Y = 0;
			mPosition.Z = unit.Position.Y;


			mTargetPosition.X = unit.TargetUnit.Position.X;
			mTargetPosition.Y = 0;
			mTargetPosition.Z = unit.TargetUnit.Position.Y;
			

		}

		//===========================================================
		// Structures and Variables
		//===========================================================
		private bool mBUseAdditiveBlending;

		//===========================================================
		// Overridden Particle System Functions
		//===========================================================

		protected override void InitializeRenderProperties()
		{
			base.InitializeRenderProperties();
			mBUseAdditiveBlending = false;
			ToggleAdditiveBlending();
		}

		//===========================================================
		// Initialization Functions
		//===========================================================
		public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
		{
			Texture = Main.Content.Load<Texture2D>("ParticleEffect/particle");
			
			InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 100, 200,
												UpdateVertexProperties, Texture);

			Name = "Fire and Smoke";
			LoadFireRingEvents();
			Emitter.ParticlesPerSecond = 40;
			//SetAmountOfSmokeToRelease(0.25f);

			World = Matrix.Identity * Matrix.CreateTranslation(Position);
			UpdatesPerSecond = 80;
		}

		private void LoadFireRingEvents()
		{
			ParticleInitializationFunction = InitializeParticleFireOnVerticalRing;

			// Set the Events to use
			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
			//ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
			ParticleEvents.AddEveryTimeEvent(ReduceSizeBasedOnLifetime);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 200);
			ParticleEvents.AddEveryTimeEvent(UpdatePosition);
			//ParticleEvents.AddNormalizedTimedEvent(0.5f, GenerateSmokeParticle);

			Emitter.PositionData.Position = new Vector3(0, 0, 0);

			// Set the Fire Ring Settings
			InitialProperties.LifetimeMin = 0.1f;
			InitialProperties.LifetimeMax = 2.0f;
			InitialProperties.PositionMin = Vector3.Zero;
			InitialProperties.PositionMax = Vector3.Zero;
			InitialProperties.StartSizeMin = 10.0f;
			InitialProperties.StartSizeMax = 20.0f;
			InitialProperties.EndSizeMin = 0.0f;
			InitialProperties.EndSizeMax = 10.0f;
			InitialProperties.StartColorMin = Color.White;
			InitialProperties.StartColorMax = Color.White;
			InitialProperties.EndColorMin = Color.LightBlue;
			InitialProperties.EndColorMax = Color.LightCyan;
			InitialProperties.InterpolateBetweenMinAndMaxColors = false;
			InitialProperties.RotationMin = Vector3.Zero;
			InitialProperties.RotationMax.Z = MathHelper.TwoPi;
			InitialProperties.VelocityMin = Vector3.Forward;
			InitialProperties.VelocityMax = new Vector3(10, 20, 10);
			InitialProperties.AccelerationMin = Vector3.Zero;
			InitialProperties.AccelerationMax = Vector3.Zero;
			InitialProperties.RotationalVelocityMin.Z = -MathHelper.TwoPi;
			InitialProperties.RotationalVelocityMax.Z = MathHelper.TwoPi;

		}

		private void InitializeParticleFireOnVerticalRing(DefaultTexturedQuadParticle cParticle)
		{
			Quaternion cBackup = Emitter.OrientationData.Orientation;
			Emitter.OrientationData.Orientation = Quaternion.Identity;
			InitializeParticleUsingInitialProperties(cParticle);
			Emitter.OrientationData.Orientation = cBackup;

			cParticle.Position = Vector3.Transform(cParticle.Position, Emitter.OrientationData.Orientation);
			cParticle.Position += Emitter.PositionData.Position;
		}
		
		//===========================================================
		// Particle Update Functions
		//===========================================================
		private void ReduceSizeBasedOnLifetime(DefaultTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
		{
			cParticle.Size = ((1.0f - cParticle.NormalizedElapsedTime) / 1.0f) * cParticle.StartSize;
		}
		

		//===========================================================
		// Particle System Update Functions
		//===========================================================

		private void UpdatePosition(DefaultTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
		{
			var direction = Vector3.Normalize(mTargetPosition - Position);
			Emitter.PositionData.Position = Vector3.Transform(Emitter.PositionData.Position, Matrix.CreateTranslation(direction));
			
		}

		//===========================================================
		// Other Particle System Functions
		//===========================================================

		private void ToggleAdditiveBlending()
		{
			// Toggle Additive Blending on/off
			mBUseAdditiveBlending = !mBUseAdditiveBlending;

			// If Additive Blending should be used
			if (mBUseAdditiveBlending)
			{
				// Turn it on
				RenderProperties.BlendState = BlendState.Additive;
			}
			else
			{
				// Turn off Additive Blending
				RenderProperties.BlendState = BlendState.AlphaBlend;
			}
		}
	}
}