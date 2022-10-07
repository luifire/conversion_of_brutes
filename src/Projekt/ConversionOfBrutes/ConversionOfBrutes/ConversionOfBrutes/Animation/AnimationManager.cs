using System;
using System.Collections.Generic;
using AnimationAux;
using ConversionOfBrutes.GameObjects;

/*
 * Author: julian Löffler
 * 
 * Animation Manager Class
 * Usage: loading Animations, playing animations
 */

namespace ConversionOfBrutes.Animation
{   
	[Serializable]
	public class AnimationManager
	{
		private Dictionary<Ident, AnimatedModel[]> mAnimationDict;
		
		[Serializable]
		public enum AnimationModels
		{
			Tpose,
			Attack,
			Idle,
			Walking,
			Death
		}
		public AnimationManager()
		{
			mAnimationDict = new Dictionary<Ident, AnimatedModel[]>();
			LoadAnimations();
		}
		#region Loadanimations
		private void LoadAnimations()
		{
			Dictionary<Ident, string> pathDictionary = new Dictionary<Ident, string>();
			pathDictionary.Add(Ident.EliteBarbarian, "Units\\EliteBarbarian\\Warrior");
			pathDictionary.Add(Ident.ShieldGuard, "Units\\ShieldGuard\\ShieldGuard");
			pathDictionary.Add(Ident.PriestRanged, "Units\\PriestRanged\\PriestRanged");
			pathDictionary.Add(Ident.Axeman, "Units\\Axeman\\Axeman");
			pathDictionary.Add(Ident.ArcherMounted, "Units\\ArcherMounted\\ArcherMounted");
			pathDictionary.Add(Ident.Archer, "Units\\Archer\\Archer");
			pathDictionary.Add(Ident.Priest, "Units\\Priest\\Priest");
			pathDictionary.Add(Ident.Knight, "Units\\Knight\\Axeman");
			pathDictionary.Add(Ident.EliteAtlantic, "Units\\EliteAtlantic\\EliteAtlantic");
			pathDictionary.Add(Ident.Beast, "Units\\Beast\\Beast");
			pathDictionary.Add(Ident.Horse, "Units\\Horse\\Horse");
			pathDictionary.Add(Ident.Witch, "Units\\Witch\\Witch");
			pathDictionary.Add(Ident.TechdemoAtlantic, "Units\\Techdemo\\Atlantic\\EliteAtlantic");
			pathDictionary.Add(Ident.TechdemoBarb, "Units\\Techdemo\\Axeman\\Axeman");

			foreach (KeyValuePair<Ident, string> var in pathDictionary)
			{
				string path = pathDictionary[var.Key];
				var model = new AnimatedModel(path + "-Tpose");
				model.LoadContent(Main.Content);

				var attack = new AnimatedModel(path + "-attack");
				attack.LoadContent(Main.Content);

				var idle = new AnimatedModel(path + "-idle");
				idle.LoadContent(Main.Content);

				var walking = new AnimatedModel(path + "-walking");
				walking.LoadContent(Main.Content);

				var death = new AnimatedModel(path + "-Death");
				death.LoadContent(Main.Content);

				var animations = new AnimatedModel[Enum.GetNames(typeof(AnimationModels)).Length];
				animations[(int)AnimationModels.Tpose] = model;
				animations[(int)AnimationModels.Attack] = attack;
				animations[(int)AnimationModels.Idle] = idle;
				animations[(int)AnimationModels.Walking] = walking;
				animations[(int)AnimationModels.Death] = death;

				AnimatedModel[] tmp = animations;
				mAnimationDict.Add(var.Key, tmp);
			}

		}
		#endregion
		public void PlayAnimation(Unit unit, AnimationModels animation)
		{
			AnimationClip clip = mAnimationDict[unit.Ident][(int)animation].Clips[0];

			unit.mAnimatedModel.mPlayer = new AnimationPlayer(clip, unit.mAnimatedModel);
			// death animation shall be played only once. looping = false
			unit.mAnimatedModel.mPlayer.Looping = animation != AnimationModels.Death;
		}
		public void PlayHorseAnimation(Horse horse, AnimationModels animation)
		{
			AnimationClip clip = mAnimationDict[Ident.Horse][(int)animation].Clips[0];

			horse.mAnimatedModel.mPlayer = new AnimationPlayer(clip, horse.mAnimatedModel);
			// death animation shall be played only once. looping = false
			horse.mAnimatedModel.mPlayer.Looping = animation != AnimationModels.Death;
		}
	}
}
