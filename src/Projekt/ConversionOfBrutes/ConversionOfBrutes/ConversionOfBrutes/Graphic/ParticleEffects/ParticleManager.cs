using System.Collections.Generic;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.ParticleEffects.Particlesystems;
using ConversionOfBrutes.Graphic.Screen;

namespace ConversionOfBrutes.Graphic.ParticleEffects
{
	class ParticleManager
	{
		private int mDrawOrder;
		private int mUpdateOrder;

		private List<RingParticleSystem> mRingParticleSystems;
		private List<MagicAttackParticleSystem> mMagicAttackParticleSystems; 

		public ParticleManager()
		{
			mRingParticleSystems = new List<RingParticleSystem>();
			mMagicAttackParticleSystems = new List<MagicAttackParticleSystem>();
			
		}
		public void Draw()
		{
			foreach (var system in mRingParticleSystems)
			{
				if (GameScreen.Camera.VisibleRectangle.Contains(system.Zone.PointPosition))
				{
					system.SimulationSpeed = 1;
					system.Draw();
				}
				else
				{
					system.SimulationSpeed = 0;
				}
			}
			
			foreach (var system in mMagicAttackParticleSystems)
			{
				if (GameScreen.Camera.VisibleRectangle.Contains(system.Unit.PointPosition))
				{
					system.SimulationSpeed = 1;
					system.Draw();
				}
				else
				{
					system.SimulationSpeed = 0;
				}
			}
		}

		public void Update()
		{

			foreach (var system in mRingParticleSystems)
			{
				if (GameScreen.Camera.VisibleRectangle.Contains(system.Zone.PointPosition))
				{
					//var World = Matrix.Identity * Matrix.CreateTranslation(system.Position);
					system.SetWorldViewProjectionMatrices(system.World, GameScreen.Camera.ViewMatrix, GameScreen.Camera.ProjectionMatrix);
					system.SetCameraPosition(GameScreen.Camera.Position);
					system.Update((float)Main.GameTime.ElapsedGameTime.TotalSeconds);
				}
			}

			foreach (var system in mMagicAttackParticleSystems)
			{
				if (GameScreen.Camera.VisibleRectangle.Contains(system.Unit.PointPosition))
				{
					//var World = Matrix.Identity * Matrix.CreateTranslation(system.Position);
					system.SetWorldViewProjectionMatrices(system.World, GameScreen.Camera.ViewMatrix, GameScreen.Camera.ProjectionMatrix);
					system.SetCameraPosition(GameScreen.Camera.Position);
					system.Update((float)Main.GameTime.ElapsedGameTime.TotalSeconds);
				}
				if (system.Emitter.NumberOfParticlesEmitted < 20)
				{
					continue;
				}
				system.Destroy();
				mMagicAttackParticleSystems.Remove(system);
				return;
			}
		}

		public void AddRingEmitter(Zone zone)
		{
			mDrawOrder += 100;
			mUpdateOrder += 100;
			var particleSystem = new RingParticleSystem(Main.Game, zone);
			particleSystem.DrawOrder = mDrawOrder;
			particleSystem.DrawOrder = mUpdateOrder;
			particleSystem.AutoInitialize(Main.Graphics.GraphicsDevice, Main.Content);

			mRingParticleSystems.Add(particleSystem);
		}

		public void AddMagicEmitter(Unit unit)
		{
			mDrawOrder += 100;
			mUpdateOrder += 100;
			var particleSystem = new MagicAttackParticleSystem(Main.Game, unit);
			particleSystem.DrawOrder = mDrawOrder;
			particleSystem.DrawOrder = mUpdateOrder;
			particleSystem.AutoInitialize(Main.Graphics.GraphicsDevice, Main.Content, GameScreen.GraphicsManager.SpriteBatch);
			mMagicAttackParticleSystems.Add(particleSystem);
		}
		
		public void RemoveRingEmitter(Zone zone)
		{
			foreach (var system in mRingParticleSystems)
			{
				if (system.Zone != zone)
				{
					continue;
				}
				system.Destroy();
				mRingParticleSystems.Remove(system);
				return;
			}
			
		}
	}
}
