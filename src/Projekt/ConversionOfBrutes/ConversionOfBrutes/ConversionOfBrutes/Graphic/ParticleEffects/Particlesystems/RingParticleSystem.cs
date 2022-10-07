#region Using Statements

using System;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace ConversionOfBrutes.Graphic.ParticleEffects.Particlesystems
{
	/// <summary>
	/// Create a new Particle System class that inherits from a Default DPSF Particle System.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	class RingParticleSystem : DefaultTexturedQuadParticleSystem
	{
		private Vector3 mPosition;
		private Zone mZone;
		private Texture2D mPlayerTexture;
		private Texture2D mAiTexture;
		private Texture2D mGaiaTexture;

		private Vector3 Position
		{
			get { return mPosition;}
			set { mPosition = value; }
		}

		public Zone Zone { get { return mZone;} }

		/// <summary>
		/// Constructor
		/// </summary>
		public RingParticleSystem(Game cGame, Zone zone) : base(cGame)
		{
			mZone = zone;
			mPosition.X = zone.Position.X;
			mPosition.Y = 0;
			mPosition.Z = zone.Position.Y;
		}

		//===========================================================
		// Structures and Variables
		//===========================================================
		private bool mBUseAdditiveBlending;
		private SmokeRingParticleSystem mCSmokeParticleSystem;

		//===========================================================
		// Overridden Particle System Functions
		//===========================================================

		protected override void InitializeRenderProperties()
		{
			base.InitializeRenderProperties();
			mBUseAdditiveBlending = false;
			ToggleAdditiveBlending();
		}

		protected override void AfterInitialize()
		{
			mCSmokeParticleSystem = new SmokeRingParticleSystem(Game);

			// Initialize the Smoke Particle System
			mCSmokeParticleSystem.AutoInitialize(GraphicsDevice, ContentManager, null);
			mCSmokeParticleSystem.DrawOrder = 100;
		}

		protected override void AfterDestroy()
		{
			if (mCSmokeParticleSystem != null)
			{
				mCSmokeParticleSystem.Destroy();
				mCSmokeParticleSystem = null;
			}
		}

		protected override void AfterUpdate(float fElapsedTimeInSeconds)
		{
			
			// If the Smoke Particle System is Initialized
			if (mCSmokeParticleSystem.IsInitialized)
			{
				// Update the Smoke Particle System manually
				mCSmokeParticleSystem.CameraPosition = CameraPosition;
				mCSmokeParticleSystem.Update(fElapsedTimeInSeconds);
			}

		}

		protected override void AfterDraw()
		{
			// Set the World, View, and Projection matrices so the Smoke Particle System knows how to draw the particles on screen properly
			mCSmokeParticleSystem.SetWorldViewProjectionMatrices(World, View, Projection);
			// If the Smoke Particle System is Initialized
			if (mCSmokeParticleSystem.IsInitialized)
			{
				// Draw the Smoke Particles manually
				mCSmokeParticleSystem.Draw();
			}

			if (((Zone.Visible || Zone.Seen) && Zone.Fraction != Fraction.Gaia) || GameScreen.FogOfWar == false)
			{
				if (mZone.Fraction == Fraction.Player)
				{
					Texture = mPlayerTexture;
				}
				if (mZone.Fraction == Fraction.Ai)
				{
					Texture = mAiTexture;
				}
			}
			else
			{
				Texture = mGaiaTexture;
			}
		}

		public override int TotalNumberOfActiveParticles { get { return base.TotalNumberOfActiveParticles + mCSmokeParticleSystem.TotalNumberOfActiveParticles; } }
		public override int TotalNumberOfParticlesAllocatedInMemory { get { return base.TotalNumberOfParticlesAllocatedInMemory + mCSmokeParticleSystem.TotalNumberOfParticlesAllocatedInMemory; } }
		public override int TotalNumberOfParticlesBeingDrawn { get { return base.TotalNumberOfParticlesBeingDrawn + mCSmokeParticleSystem.TotalNumberOfParticlesBeingDrawn; } }


	//===========================================================
		// Initialization Functions
		//===========================================================
		public void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager)
		{
			mPlayerTexture = Main.Content.Load<Texture2D>("ParticleEffect/particle");
			mGaiaTexture = Main.Content.Load<Texture2D>("ParticleEffect/greyparticle");
			mAiTexture = Main.Content.Load<Texture2D>("ParticleEffect/redparticle");
			
			
			if (mZone.Fraction == Fraction.Player)
			{
				Texture = mPlayerTexture;
			}
			if (mZone.Fraction == Fraction.Ai)
			{
				Texture = mAiTexture;
			}
			if (mZone.Fraction == Fraction.Gaia)
			{
				Texture = mGaiaTexture;
			}

			InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 100, 200,
												UpdateVertexProperties, Texture);

			Name = "Fire and Smoke";
			LoadFireRingEvents();
			Emitter.ParticlesPerSecond = 40;
			//SetAmountOfSmokeToRelease(0.25f);

			World = Matrix.Identity * Matrix.CreateTranslation(Position);
			UpdatesPerSecond = 20;
		}

		private void LoadFireRingEvents()
		{
			ParticleInitializationFunction = InitializeParticleFireOnHorizontalRing;

			// Set the Events to use
			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
			ParticleEvents.AddEveryTimeEvent(ReduceSizeBasedOnLifetime);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 200);
			ParticleEvents.AddEveryTimeEvent(UpdatePosition);

			Emitter.PositionData.Position = new Vector3(0, 0, 0);

			// Set the Fire Ring Settings
			InitialProperties.LifetimeMin = 0.1f;
			InitialProperties.LifetimeMax = 3.0f;
			InitialProperties.PositionMin = Vector3.Zero;
			InitialProperties.PositionMax = Vector3.Zero;
			InitialProperties.StartSizeMin = 20.0f;
			InitialProperties.StartSizeMax = 40.0f;
			InitialProperties.EndSizeMin = 0.0f;
			InitialProperties.EndSizeMax = 20.0f;
			InitialProperties.StartColorMin = Color.White;
			InitialProperties.StartColorMax = Color.White;
			InitialProperties.EndColorMin = Color.White;
			InitialProperties.EndColorMax = Color.White;
			InitialProperties.InterpolateBetweenMinAndMaxColors = false;
			InitialProperties.RotationMin = Vector3.Zero;
			InitialProperties.RotationMax.Z = MathHelper.TwoPi;
			InitialProperties.VelocityMin = new Vector3(-10, 15, -10);
			InitialProperties.VelocityMax = new Vector3(10, 30, 10);
			InitialProperties.AccelerationMin = Vector3.Zero;
			InitialProperties.AccelerationMax = Vector3.Zero;
			InitialProperties.RotationalVelocityMin.Z = -MathHelper.TwoPi;
			InitialProperties.RotationalVelocityMax.Z = MathHelper.TwoPi;

			mCSmokeParticleSystem.LoadEvents();
		}

		private void InitializeParticleFireOnHorizontalRing(DefaultTexturedQuadParticle cParticle)
		{
			Quaternion cBackup = Emitter.OrientationData.Orientation;
			Emitter.OrientationData.Orientation = Quaternion.Identity;
			InitializeParticleUsingInitialProperties(cParticle);
			Emitter.OrientationData.Orientation = cBackup;

			cParticle.Position = DPSFHelper.PointOnSphere(RandomNumber.Between(0, MathHelper.TwoPi), 0, 100);
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

		private void UpdatePosition(DefaultTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
		{
			Vector3 zonePosition = new Vector3(Zone.Position.X, 0, Zone.Position.Y);
			if (Position != zonePosition)
			{
				Position = zonePosition;
				World = Matrix.Identity*Matrix.CreateTranslation(Position);
			}
		}

		
		//===========================================================
		// Particle System Update Functions
		//===========================================================

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

		

#if (WINDOWS)
		[Serializable]
#endif
		private class SmokeRingParticleSystem : DefaultTexturedQuadParticleSystem
		{
			public SmokeRingParticleSystem(Game cGame) : base(cGame)
			{
				
			}

			public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
			{
				InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 100, 1000,
													UpdateVertexProperties, "ParticleEffect/smoke");
				LoadEvents();
			}

			public void LoadEvents()
			{
				ParticleInitializationFunction = InitializeSmokeRingParticle;

				ParticleEvents.RemoveAllEvents();
				ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
				ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
				ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
				ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 200);
				ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
			}

			// Used to generate smoke coming off the ring of fire
			private void InitializeSmokeRingParticle(DefaultTexturedQuadParticle cParticle)
			{
				cParticle.Lifetime = RandomNumber.Between(1.0f, 5.0f);

				cParticle.Position = new Vector3(0, 10, 0);
				cParticle.StartSize = RandomNumber.Next(10, 40);
				cParticle.EndSize = RandomNumber.Next(20, 60);
				cParticle.Size = cParticle.StartSize;
				cParticle.Color = Color.White;
				cParticle.Orientation = Orientation3D.Rotate(Matrix.CreateRotationZ(RandomNumber.Between(0, MathHelper.TwoPi)), cParticle.Orientation);
				cParticle.Velocity = new Vector3(RandomNumber.Next(0, 30), RandomNumber.Next(10, 30), RandomNumber.Next(-20, 10));
				cParticle.Acceleration = Vector3.Zero;
				cParticle.RotationalVelocity.Z = RandomNumber.Between(-MathHelper.Pi, MathHelper.Pi);
			}
		}
	}
}