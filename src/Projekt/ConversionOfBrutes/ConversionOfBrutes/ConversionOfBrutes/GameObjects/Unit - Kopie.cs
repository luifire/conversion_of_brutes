/**
 * Author: David Luibrand, Julian Löffler
 * 
 * Unit
 * Missing: 
 * - Patrol Function (probably endless move path?)
 * - AttackMove Function
 **/

using System;
using System.Collections.Generic;
using ConversionOfBrutes.Animation;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Misc;
using ConversionOfBrutes.Sound;
using Microsoft.Xna.Framework;
using RVO;

namespace ConversionOfBrutes.GameObjects
{
	/// <summary>
	/// Unit Class
	/// </summary>
	[Serializable]
	public abstract class Unit : WorldObject
	{
		#region Private Member Variables
		// Stats 
		private int mHealthPoints;
		private int mFaithPoints;
		private bool mVisible;	// Enemies are not always visible


		private readonly int mMaxHealthPoints;
		private readonly int mMaxFaithPoints;

		private readonly int mAttackDamage;
		protected readonly int mAttackRange;	// of Attack
		private readonly float mAttackSpeed;

		private readonly int mSightRange = 75; // of View/Sight 
		private readonly float mSpeed;	// MainMap Einheiten pro 
		private readonly float mRelativeSpeed; // had to be adjusted because of collision handler

		// Attack
		private bool mAttacking;
		private Unit mTargetUnit;
		private TimeSpan mAttackInterval;
		private TimeSpan mLastAttack; // for attack
		private int mTimeOfLastAttack; // for Autoattack.
		private Unit mLastAttacker;
		private Vector2 mLastPositionBeforeAutoAttack; // last position before auto attack
		private bool mAutoAttack; // auto attack mode
		private int mAutoAttackCheck;

		// other
		private int mCurrentSendId = -1; // used for collision
		private bool mDestinationReached = true;
		private LinkedList<Vector2> mPath = new LinkedList<Vector2>();
		private bool mPatrolling;
		private Vector2 mPatrolStart;
		private Vector2 mPatrolDestination;

		// Collision
		public int mCollisionId;
		private int mCollisionUpdate;
		private double mNextMovementCheck;
		private Vector2? mLastMovementCheckPosition;

		private AnimationManager.AnimationModels mCurrentAnimation;

		private String mAiDevInfo = "";

		#endregion

		//Animation
		[NonSerialized]
		public AnimatedModel mAnimatedModel;

		protected Unit(Ident id, Vector2 position, Fraction fraction, float attackSpeed, int healthPoints, int attackDamage,int attackRange, int speed, float areaSize) :
			base(id, position, fraction, areaSize*1.5f)
		{
			mAttackSpeed = attackSpeed;
			mAttackInterval = TimeSpan.FromMilliseconds(1000 / mAttackSpeed);
			mHealthPoints = healthPoints;
			mFaithPoints = healthPoints;

			mMaxHealthPoints = healthPoints;
			mMaxFaithPoints = healthPoints;
			mAttackDamage = attackDamage;

			mAttackRange = attackRange;
			mSpeed = speed;
			mRelativeSpeed = speed / 4f;

			//mVisible = fraction == Fraction.Player;
			mVisible = true;

			// get Animation model
			mAnimatedModel = new AnimatedModel(GameScreen.GraphicsManager.GetAsset(Ident));
			mAnimatedModel.LoadContent(Main.Content);

			// Play idle animation
			mCurrentAnimation = AnimationManager.AnimationModels.Idle;
			GameScreen.GraphicsManager.mAnimationManager.PlayAnimation(this, mCurrentAnimation);

			PulkUpdate();
		}

		/// <summary>
		/// needed by serialization
		/// </summary>
		/// <param name="copyUnit"></param>
		public void CopyFromUnit(Unit copyUnit)
		{
			mHealthPoints = copyUnit.HealthPoints;
			mFaithPoints = copyUnit.mFaithPoints;

			// Attack
			if (copyUnit.mTargetUnit != null)
			{
				mTargetUnit = SaveAndLoad.GetNewUnit(copyUnit.mTargetUnit);
				// TODO why?
				//if (mTargetUnit == null)
					//mTargetUnit = mTargetUnit;
			}

			if (mTargetUnit != null)
			{ 
				mAttacking = copyUnit.mAttacking;
				mAttackInterval = copyUnit.mAttackInterval;
				mLastAttack = copyUnit.mLastAttack;
				mTimeOfLastAttack = copyUnit.mTimeOfLastAttack; // for Autoattack.
				
				mLastPositionBeforeAutoAttack = copyUnit.mLastPositionBeforeAutoAttack; // last position before auto attack
				mAutoAttack = copyUnit.mAutoAttack; // auto attack mode
			}
			mAutoAttackCheck = copyUnit.mAutoAttackCheck;

			if (copyUnit.mLastAttacker != null)
				mLastAttacker = SaveAndLoad.GetNewUnit(copyUnit.mLastAttacker);
			
			// other
			mCurrentSendId = copyUnit.mCurrentSendId; // used for collision
			mDestinationReached = copyUnit.mDestinationReached;
			mPath = copyUnit.mPath;

			// Collision
			//mCollisionUpdate;
			//mNextMovementCheck;
			mLastMovementCheckPosition = copyUnit.mLastMovementCheckPosition;

			StartAnimation(copyUnit.mCurrentAnimation);
			UpdateRotationAndCollision();
		}

		/// <summary>
		/// invoked before save
		/// </summary>
		public void PrepareToBeSaved()
		{
			if(CheckForDead() == false)
				CheckTargetUnit();
		}

		private void StartAnimation(AnimationManager.AnimationModels animation)
		{
			if (animation != mCurrentAnimation)
			{
				GameScreen.GraphicsManager.mAnimationManager.PlayAnimation(this, animation);
				mCurrentAnimation = animation;
			}
		}

		#region Movement

		public void UpdateRotationAndCollision(Vector2? forceDirection = null)
		{
			Vector2 direction;

			if (forceDirection != null)
			{
				direction = (Vector2) forceDirection;
			}
			else
			{ 
				if (mAttacking)
				{
					if(IsInAttackRange(mTargetUnit))
						direction = Vector2.Zero;
					else
					{ 
						direction = mTargetUnit.Position - Position;
						direction.Normalize();
					}
				}
				else
				{
					direction = Direction;
				}
			}

			if (float.IsNaN(direction.X) || float.IsNaN(direction.Y))
				throw new Exception("sags Lui 123");

			// Model Rotation
			if (direction != Vector2.Zero)
				mModelRotation = (float)(Math.Atan2(direction.X, direction.Y) + Math.PI);

			Simulator.Instance.setAgentPrefVelocity(mCollisionId, direction * mRelativeSpeed);
		}

		/// <summary>
		/// Sets all variables to initiate Patrolling
		/// </summary>
		/// <param name="destination"></param>
		public void StartPatrol(Vector2 destination)
		{
			mPatrolStart = mPosition;
			mPatrolDestination = destination;
			mPatrolling = true;
		}

		/// <summary>
		/// Unit walks to newPosition
		/// </summary>
		/// <param name="newPosition"></param>
		/// <param name="sendId"></param>
		public void WalkToPosition(Vector2 newPosition, int sendId)
		{
			WalkToPosition(GameScreen.ObjectManager.Pathfinder.FindPath(Position, newPosition), sendId);
		}

		public void WalkToPosition(LinkedList<Vector2> path, int sendId)
		{
			mCurrentSendId = sendId;
			mPath = path;
			// so it checks the movement immediatly
			mNextMovementCheck = Main.GameTime.TotalGameTime.TotalMilliseconds;

			UpdateRotationAndCollision();

			if (mPath.Count > 0)
			{
				mDestinationReached = false;
				StartAnimation(AnimationManager.AnimationModels.Walking);
			}
			else
			{
				DestinationReached = true;
				StartAnimation(AnimationManager.AnimationModels.Idle);
			}
		}

		/// <summary>
		/// finds a new path to destination
		/// </summary>
		public void FindNewWayToDestination()
		{
			if (mPath.Count > 0)
				WalkToPosition(GameScreen.ObjectManager.Pathfinder.FindPath(Position, mPath.Last.Value), mCurrentSendId);
		}							

		/// <summary>
		/// sometimes the copyUnit doesn't move because of blocking units or buildings
		/// so I check the progress of the movement once in a while and push Units or
		/// do another pathfinding
		/// </summary>
		private void MovementCheck(Vector2 target)
		{
			// happens only once
			if (mLastMovementCheckPosition == null)
			{
				mLastMovementCheckPosition = Position;
				return;
			}
			// something went wront
			if (Vector2.Distance((Vector2) mLastMovementCheckPosition, Position) < 10)
			{
				var collidingObjects = GameScreen.Map.GetObjects(Area.Rectangle);

				foreach (var obj in collidingObjects)
				{
					if (obj is Unit && obj != this)
					{
						Unit annoyingUnit = (Unit) obj;
						if (annoyingUnit.IsMoving == false)
						{
							annoyingUnit.StandingMoveAwayFromUnit(this);
						}
					}
				}
			}

			mLastMovementCheckPosition = Position;
			mNextMovementCheck = Main.GameTime.TotalGameTime.TotalMilliseconds + 700;
		}


		private void MotionUpdate()
		{
			mCollisionUpdate++;

			Vector2? currentTarget = null;

			if (mPath.Count > 0)
				currentTarget = mPath.First.Value;
			else if (mAttacking)
				currentTarget = mTargetUnit.Position;

			if (currentTarget != null)
			{
				if (mCollisionUpdate > Vector2.Distance(Position, (Vector2) currentTarget) / 2)
					UpdateRotationAndCollision();

				if (mNextMovementCheck < Main.GameTime.TotalGameTime.TotalMilliseconds)
					MovementCheck((Vector2) currentTarget);
			}
		}

		/// <summary>
		/// actually sets the copyUnit to this position
		/// invoked by the collision handler
		/// </summary>
		/// <param name="pos"></param>
		public void ChangePosition(Vector2 pos)
		{
			Position = pos;
			if (mPath.Count == 0) return;

			// check if a neighbour reached the same destination
			bool neighbourReachedDest = false;
			if (mPath.Count == 1 && mAttacking == false)
			{
				var neighbours = GameScreen.Map.GetObjects(Area.Rectangle);
				foreach (var obj in neighbours)
				{
					if(obj is Unit && obj != this)
					{ 
						Unit neighbour = (Unit) obj;

						if (neighbour.DestinationReached && neighbour.CurrentSendId == CurrentSendId)
						{
							neighbourReachedDest = true;
							break;
						}
					}
				}
			}

			// Point reached or neighbour reached point
			Vector2 goal = mAttacking ? mTargetUnit.Position : mPath.First.Value;
			if (neighbourReachedDest ||
				Math.Abs(Vector2.Distance(Position, goal)) < 4)
			{
				if (mPath.Count > 0)
					mPath.RemoveFirst();

			    if (mPath.Count == 0 || mAttacking)
			    {
				    if (mPatrolling)
				    {
					    var dmy = mPatrolDestination;
					    mPatrolDestination = mPatrolStart;
					    mPatrolStart = dmy;
						WalkToPosition(mPatrolDestination, GameScreen.ObjectManager.GetFreeSendId());
						PulkUpdate();
				    }
				    else
				    {
						PulkUpdate();
						DestinationReached = true;   
				    }
			    }

				UpdateRotationAndCollision();
			}
		}

		#region Collision

		/// <summary>
		/// this is standing and moves away from the other copyUnit
		/// </summary>
		/// <param name="sendingUnit"></param>
		private void StandingMoveAwayFromUnit(Unit sendingUnit)
		{
			Vector2 direction;
			if (sendingUnit.IsMoving)
			{
				Vector2 sendingDir = sendingUnit.Direction;
				direction = new Vector2(-sendingDir.Y / sendingDir.X, 1) + CollisionHandler.RandomVector2;
				direction.Normalize();

				Vector2 smallMove = Position + 0.01f * direction;
				float startDist = (sendingUnit.Position - Position).Length();
				float afterSmallMoveDistance = (sendingUnit.Position - smallMove).Length();

				// if distance is bigger orthogonalSendDir moves the object near to sendingUnit
				if (startDist > afterSmallMoveDistance)
					direction = -direction;
			}
			else
			{
				direction = CollisionHandler.RandomVector2;
				direction.Normalize();
			}

			WalkToPosition(Position + direction * 8, GameScreen.ObjectManager.GetFreeSendId());
		}

		#endregion // Collision

		#endregion

		#region Attack

		private void AttackUpdate()
		{
			// Auto Attack enemies in sight
			if (mDestinationReached && !mAttacking && mAutoAttackCheck++ > 40)
			{
				mAutoAttackCheck = 0;
				// Performance? by now reasonably good!
				foreach (WorldObject enemyUnit in GameScreen.Map.GetObjects(new Rectangle((int)(Position.X - SightRange),
							(int)(Position.Y - SightRange),
							2 * SightRange,
							2 * SightRange)))
				{
					if (enemyUnit is Unit && mFraction != enemyUnit.Fraction && IsInSight((Unit)enemyUnit))
					{
						mAutoAttack = true;
						mLastPositionBeforeAutoAttack = Position;
						Attack((Unit)enemyUnit);
						break;
					}
				}
			}

			// Update the Attack
			// Attacking			 
			if (mAttacking && (mFraction != mTargetUnit.mFraction || mTargetUnit.IsConverted()))
			{
				// Followed enemies too far in auto attack mode
				if (mAutoAttack && Vector2.Distance(Position, mLastPositionBeforeAutoAttack) > 2 * SightRange)
				{
					mAttacking = false;
					mAutoAttack = false;
					WalkToPosition(mLastPositionBeforeAutoAttack, 0);
				}
				else if (TimeToAttack()) Attack(mTargetUnit);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>false, if dead</returns>
		private void CheckTargetUnit()
		{
			if (mTargetUnit != null)
			{ 
				if (mTargetUnit.IsDead() || mTargetUnit.IsConverted())
				{
					mAutoAttack = false;
					mAttacking = false;
					mDestinationReached = true;
					Stop();
				}
			}
		}

		/// <summary>
		/// Attack an Enemy copyUnit
		/// </summary>
		/// <param name="target"></param>
		public void Attack(Unit target)
		{
			mTargetUnit = target;
			mAttacking = true;
			//UpdateRotationAndCollision();

			if (IsInAttackRange(target))
			{
				mLastAttack = Main.GameTime.TotalGameTime;
				StartAnimation(AnimationManager.AnimationModels.Attack);
				Main.Audio.PlaySound(AudioManager.Sound.SwordAttack);

				mTargetUnit.AbsorbDamage(mAttackDamage, this);
				CheckTargetUnit();
			}
			else
			{
				WalkToPosition(mTargetUnit.Position, mTargetUnit.ObjectNumber);
			}
		}

		/// <summary>
		/// Check if the Unit is Alive
		/// </summary>
		/// <returns></returns>
		public bool IsDead()
		{
			return mHealthPoints <= 0;
		}

		private bool CheckForDead()
		{
			// in Case of death
			if (IsDead())
			{
				StartAnimation(AnimationManager.AnimationModels.Death);
				Main.Audio.PlaySound(AudioManager.Sound.SwordDeath);
				GameScreen.ObjectManager.UnitDied(this);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check if the Unit got Converted
		/// </summary>
		/// <returns></returns>
		public bool IsConverted()
		{
			return mFaithPoints <= 0;
		}
		
		// Method that handles Damage taken
		private void AbsorbDamage(int damage, Unit attackingUnit)
		{
			mTimeOfLastAttack = Main.GameTime.ElapsedGameTime.Milliseconds;
			mLastAttacker = attackingUnit;
			if (attackingUnit.Ident == Ident.Priest || attackingUnit.Ident == Ident.PriestRanged)
			{
				mFaithPoints = (mFaithPoints - damage < 0) ? 0 : mFaithPoints - damage;
			}
			else
			{
				mHealthPoints = (HealthPoints - damage < 0) ? 0 : mHealthPoints - damage;
			}
		}

		// helper method for Attack() to determine the attackRange between copyUnit and target
		private bool IsInAttackRange(Unit attackedUnit)
		{
			// calculate distance between start and target
			var distance = Vector2.Distance(Position, attackedUnit.Position);
			return distance <= mAttackRange;
		}

		// helper method for (auto-)Attack() to determine wether the copyUnit can see the other copyUnit 
		private bool IsInSight(Unit seenUnit)
		{
			// calculate distance between start and target
			var distance = Vector2.Distance(Position, seenUnit.Position);
			return distance <= SightRange;
		}

		// helper Method for Attacks per second.
		private bool TimeToAttack()
		{
			if (mLastAttack + mAttackInterval < Main.GameTime.TotalGameTime)
			{
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// let the Unit stop all current actions.
		/// </summary>
		public void Stop()
		{
			StartAnimation(AnimationManager.AnimationModels.Idle);
			mAttacking = false;
			mAutoAttack = false;
			mPatrolling = false;
			// so the Unit make it's normal way to stop
			WalkToPosition(Position, ObjectNumber);
			UpdateRotationAndCollision();
		}
		#endregion

		#region Update/Draw
		public void Update()
		{
			if (CheckForDead())
				return;

			// in case of conviction
			if (mFraction != Fraction.Player)
			{
				if (IsConverted())
				{
					mFraction = Fraction.Player;
					FowObject.IsVisible = true;
					Main.mGameStatistic.ConvertedUnits++;
					Stop();
				}
			}
			
			MotionUpdate();
			AttackUpdate();

			// Update the models animation if in Viewfield
			if (GameScreen.Camera.VisibleRectangle.Contains(PointPosition))
			{
				mAnimatedModel.Update(Main.GameTime);
			}
		}

		public void RevealSurroundingObjects()
		{
			
		}

		/// <summary>
		/// Draw function
		/// </summary>
		public override void Draw()
		{
			GameScreen.GraphicsManager.DrawUnit(this, Position, mModelRotation,FowObject.IsVisible);

			if(GameScreen.Hud.DeveloperInformation.ShowUnitInfo)
			{
				if (mAiDevInfo.Length > 0)
				{
					GameScreen.GraphicsManager.DrawText(Position, mAiDevInfo, Color.Red);
				}
				else
				{
					//GameScreen.GraphicsManager.DrawText(Position, mObjectNumber + " - " + Area, Fraction == Fraction.Player ? Color.LightGreen : Color.Red);
					//GameScreen.GraphicsManager.DrawText(Position, CurrentSendId.ToString(), DestinationReached ? Color.LightGreen : Color.Red);
					GameScreen.GraphicsManager.DrawText(Position, ToString(), Fraction == Fraction.Player ? Color.LightGreen : Color.Red);
				}
			}
		}
#endregion

		#region Pulk

		private const int PulkAreaSize = 20;
		private static uint sPulkCounter;
		private uint mPulkIndex = sPulkCounter++;
		private HashSet<Unit> mPulkNeighbours = new HashSet<Unit>();

		/*
		 * Units werden weggeschickt gleicher Pulk bekommt neue PulkId
		 * CollisionAvoidance und kein Pfad danach => PulkUpda
		 * DestinationAngekommen => PulkUpda 
		 * 
		 * PulkUpda: alle Nachbarn anschauen und speichern 
		 * min = kleinste Pulk Id
		 * eigene PulkId dabei => spread min
		 * nicht dabei => alten Nachbarn verlassen mitteilen
		 * 
		 * 
		 * nicht machen, falls diese Runde schon upgedated wurde
		 * 
		 * Regel: jeder Nachbar kennt jeden Nachbar
		 */

		public void PulkUpdate()
		{
			HashSet<Unit> newNeighbours = new HashSet<Unit>();
			Rectangle pulkRect = new Rectangle((int)Position.X - PulkAreaSize, (int)Position.Y - PulkAreaSize, 2 * PulkAreaSize, 2 * PulkAreaSize);
			var nearbyObj = GameScreen.Map.GetObjects(pulkRect);
			uint smallestPulkIdx = mPulkIndex;
			bool stillInOwnPulk = false;
			bool othersFound = false;

			foreach (var obj in nearbyObj)
			{
				if (obj != this && obj is Unit)
				{
					Unit unit = (Unit)obj;
					// fraction has to be the same
					if (unit.Fraction == Fraction)
					{
						if (unit.mPulkIndex == mPulkIndex)
							stillInOwnPulk = true;
						else
						{
							othersFound = true;
							if (unit.mPulkIndex < smallestPulkIdx)
								smallestPulkIdx = unit.mPulkIndex;
						}

						newNeighbours.Add(unit);
					}
				}
			}

			bool friendKicked = false;

			// tell old friends, that I'm no longer with them
			foreach (var oldneighbour in mPulkNeighbours)
			{
				if (newNeighbours.Contains(oldneighbour) == false)
				{
					oldneighbour.IamNotYourNeighbourAnymore(this);
					friendKicked = true;
				}
			}

			if (othersFound)
				mPulkIndex = smallestPulkIdx;
			// this one is not in a group anymore
			else if (mPulkNeighbours.Count > 0 && newNeighbours.Count == 0)
				mPulkIndex = sPulkCounter++;

			mPulkNeighbours = newNeighbours;

			if (othersFound)
			{
				foreach (var neighbour in mPulkNeighbours)
				{
					neighbour.IamYourNewNeighbour(this);
				}
				SetNewPulkNumber(mPulkIndex);
			}
			// if one is left it wouldn't change its pulk index
			else if (stillInOwnPulk && friendKicked && mPulkIndex == smallestPulkIdx)
			{
				mPulkIndex = sPulkCounter++;
				SetNewPulkNumber(mPulkIndex);
			}
		}

		/// <summary>
		/// spreads the pulkIndex rekursivly
		/// </summary>
		/// <param name="pulkIndex"></param>
		private void SetNewPulkNumber(uint pulkIndex)
		{
			mPulkIndex = pulkIndex;
			foreach (var neighbour in mPulkNeighbours)
			{
				if (neighbour.mPulkIndex != pulkIndex)
					neighbour.SetNewPulkNumber(pulkIndex);
			}
		}

		private void IamNotYourNeighbourAnymore(Unit unit)
		{
			mPulkNeighbours.Remove(unit);
		}

		private void IamYourNewNeighbour(Unit unit)
		{
			if (mPulkNeighbours.Contains(unit) == false)
				mPulkNeighbours.Add(unit);
		}

		public uint PulkId { get { return mPulkIndex; } }

		#endregion // Pulk

		#region Getter/Setter
		public int HealthPoints { get { return mHealthPoints; } }
		public int MaxHealthPoints { get { return mMaxHealthPoints; } }
		public int FaithPoints { get { return mFaithPoints; } }
		public int MaxFaithPoints { get { return mMaxFaithPoints; } }
		public int Damage { get { return mAttackDamage; } }
		public int AttackRange { get { return mAttackRange; } }
		public float AttackSpeed { get { return mAttackSpeed; } }
		public float Speed { get { return mSpeed; } }
		public bool Visible { get { return mVisible; } set { mVisible = value; } }
		public LinkedList<Vector2> CurrentPath { get { return mPath; } }
		public bool Attacking { get { return mAttacking; } set { mAttacking = value; } }
		public bool AutoAttack { get { return mAutoAttack; } }
		public bool IsMoving { get { return mPath.Count > 0; } }
		public int TimeOfLastAttack { get { return mTimeOfLastAttack; } }
		public Unit LastAttacker { get { return mLastAttacker; } }
		public Unit TargetUnit { get { return mTargetUnit; } }
		public int SightRange { get { return mSightRange; } }
		internal int CurrentSendId { get { return mCurrentSendId; } }
		// Normalized
		private Vector2 Direction { get { return mPath.Count == 0 ? Vector2.Zero : Vector2.Normalize(mPath.First.Value - Position); } }

		public String AiDevInfo
		{
			get { return mAiDevInfo; }
			set { mAiDevInfo = value; }
		}

		// for Collision Handler to decide whether a goal is reached
		public bool DestinationReached
		{
			get { return mDestinationReached; }
			private set
			{
				mDestinationReached = value;
				// If destination is reached
				if (value)
				{
					mPath.Clear();
					StartAnimation(AnimationManager.AnimationModels.Idle);
				}
			}
		}
#endregion

	}
}
