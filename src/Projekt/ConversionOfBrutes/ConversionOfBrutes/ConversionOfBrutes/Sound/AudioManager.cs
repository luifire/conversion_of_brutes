/*
 * Author: Pius Meinert 
 * ? 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using ConversionOfBrutes.Graphic.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace ConversionOfBrutes.Sound
{
    /// <summary>
    /// This is the Audio which is responsible for
    ///  audio playback and control.
    /// </summary>
    public sealed class AudioManager
	{
		#region Private member variables
		AudioEngine mAudioEngine;
        SoundBank mSoundBank;
		// Have to be disable since they are needed but it has been
		// suggested to delete them though this would result in
		// an error (cue/sound not found).
	    // ReSharper disable once NotAccessedField.Local
        WaveBank mInMemoryWave;
	    // ReSharper disable once NotAccessedField.Local
        WaveBank mStreamWave;
	    private float mCurrentMusicVolume;
		private float mCurrentSoundVolume;

	    private bool mSoundOn = true;
	    private bool mMusicOn = true;
	    private bool mMainMenuMusicPlaying;
	    private bool mInGameMusicPlaying;
	    private bool mPauseMusicPlaying;

        List<Cue> mActiveCues = new List<Cue>(); 
        private string[] mSoundNames;
#endregion

		public enum Category 
		{
			Music,
			MainMenuMusic,
			CreditsMusic,
			InGameMusic,
			PauseMusic,
			Units
		}
	    private string[] mCategories;

	    public enum Sound
        {
			// Music
			MainMenuMusic,
			CreditsMusic, 
			InGameMusic,
            PauseMusic,

			ArrowShot,
			ArrowDeath,
			SwordAttack,
			SwordDeath,
			HorseDeath,
            Convert,
			Converted,
            Taunt, // ShieldGuard - T
			AiZoneCaptured,
			PlayerZoneCaptured,
			Click1,
			Click2,
			Click3
        };

        public AudioManager()
        {
            Initialize();
        }

        /// <summary>
        /// Initialisation of the AudioEngine, Sound Banks, etc. necessary for
        /// audio playback.
        /// </summary>
        private void Initialize()
        {
            mAudioEngine = new AudioEngine("Content\\Audio\\XACTAudioProject.xgs");
            mSoundBank = new SoundBank(mAudioEngine, "Content\\Audio\\SoundBank.xsb");
            mInMemoryWave = new WaveBank(mAudioEngine, "Content\\Audio\\InMemoryWave.xwb");
            mStreamWave = new WaveBank(mAudioEngine, "Content\\Audio\\StreamWave.xwb", 0, 4);
	        mCurrentMusicVolume = (float) Math.Round(0.5f);
			mCurrentSoundVolume = (float)Math.Round(0.5f);

			#region Sound initialization
			mSoundNames = new string[Enum.GetNames(typeof(Sound)).Length];
			// Music
			mSoundNames[(int) Sound.MainMenuMusic] = "MainMenuMusic";
	        mSoundNames[(int) Sound.CreditsMusic] = "CreditsMusic";
	        mSoundNames[(int) Sound.InGameMusic] = "InGameMusic";
	        mSoundNames[(int) Sound.PauseMusic] = "PauseMusic";
			// Sounds
	        mSoundNames[(int) Sound.ArrowShot] = "ArrowShot";
	        mSoundNames[(int) Sound.ArrowDeath] = "ArrowDeath";
	        mSoundNames[(int) Sound.SwordAttack] = "SwordAttack";
	        mSoundNames[(int) Sound.SwordDeath] = "SwordDeath";
	        mSoundNames[(int) Sound.HorseDeath] = "HorseDeath";
	        mSoundNames[(int) Sound.Convert] = "Convert";
	        mSoundNames[(int) Sound.Converted] = "Converted";
	        mSoundNames[(int) Sound.Taunt] = "Taunt";
	        mSoundNames[(int) Sound.AiZoneCaptured] = "40 Enemy";
	        mSoundNames[(int) Sound.PlayerZoneCaptured] = "39 Ally";
	        mSoundNames[(int) Sound.Click1] = "1_click";
	        mSoundNames[(int) Sound.Click2] = "2_click";
	        mSoundNames[(int) Sound.Click3] = "3_click";
#endregion
			#region Category initialization
			mCategories = new string[Enum.GetNames(typeof(Category)).Length];
	        mCategories[(int) Category.Music] = "Music";
	        mCategories[(int) Category.MainMenuMusic] = "MainMenuMusic";
	        mCategories[(int) Category.CreditsMusic] = "CreditsMusic";
	        mCategories[(int) Category.InGameMusic] = "InGameMusic";
	        mCategories[(int) Category.PauseMusic] = "PauseMusic";
	        mCategories[(int) Category.Units] = "Units";
			//mSoundNames = Enum.GetNames(typeof(Sounds));
			mAudioEngine.Update();
#endregion
        }
   
		/// <summary>
		/// Controls (eg. pause, volume) the given audio, 
        /// in particular the given audio categories. 
		/// </summary>
        public void Update()
        {
            mAudioEngine.Update();

			if (mInGameMusicPlaying && !mActiveCues.Exists(cue => cue.Name == "InGameMusic"))
			{
				//SwitchPauseBool(Sound.InGameMusic);
				PlaySound(Sound.InGameMusic);
				ResumeSound(Sound.InGameMusic);
			}

            for (int i = 0; i < mActiveCues.Count; i++)
            {
                if (mActiveCues[i].IsStopped)
                {
                    mActiveCues.Remove(mActiveCues[i]);
                }
            }
        }

        /// <summary>
        /// Regular playback of a cue using XACT. 
        /// </summary>
        /// <param name="soundId"></param>
        public void PlaySound(Sound soundId)
        {

            var cue = mSoundBank.GetCue((mSoundNames[(int)soundId]));
	        if (!cue.Name.Contains("Music") && !mSoundOn)
	        {
		        return;
	        }

            if (IsSoundPlaying(cue) && cue.Name != "3_click" && cue.Name != "ArrowShot" &&
				cue.Name != "SwordAttack")
            {
                return;
            }
	        if (!(!mMusicOn && (soundId == Sound.MainMenuMusic || soundId == Sound.InGameMusic || soundId == Sound.PauseMusic)))
	        {
		        cue.Play();
		        mActiveCues.Add(cue);
	        }

	        if (!mMusicOn &&
	            (soundId == Sound.MainMenuMusic || soundId == Sound.InGameMusic || soundId == Sound.PauseMusic))
	        {
		        cue.Pause();
	        }
			SetPlaying(soundId, true);
        }

		/// <summary>
		/// Play sound for units. But only if visible in camera.
		/// </summary>
		/// <param name="soundId"></param>
		/// <param name="position"></param>
	    public void PlayUnitSound(Sound soundId, Point position)
	    {
	        switch (soundId)
	        {
		        case Sound.SwordAttack:
			        if (mActiveCues.FindAll(c => c.Name == "SwordAttack").Count > 1)
				        return;
			        break;
		        case Sound.ArrowShot:
			        if (mActiveCues.FindAll(c => c.Name == "ArrowShot").Count > 2)
				        return;
			        break;
	        }

		    if (GameScreen.Camera.VisibleRectangle.Contains(position))
		    {
			    PlaySound(soundId);
		    }
	    }

		/// <summary>
		/// Pause given music cue.
		/// </summary>
		/// <param name="soundId"></param>
	    public void PauseSound(Sound soundId)
	    {
		    if (mActiveCues.FirstOrDefault(cue => cue.Name == soundId.ToString()) == null)
		    {
				SetPlaying(soundId, false);
			    return;
		    }

		    mActiveCues.First(cue => cue.Name == soundId.ToString()).Pause();
			//if (!mMusicOn)
			//{
			//	return;
			//}

			SetPlaying(soundId, false);
	    }

		/// <summary>
		/// Resume given music cue. Play if not already playing.
		/// </summary>
		/// <param name="soundId"></param>
		/// <returns></returns>
	    public bool ResumeSound(Sound soundId)
	    {
		    if (mActiveCues.FirstOrDefault(cue => cue.Name == soundId.ToString()) == null)
		    {
				StopSound(soundId);
				PlaySound(soundId);
				SetPlaying(soundId, true);
			    return false;
		    }

		    if (mMusicOn)
		    {
				mActiveCues.First(cue => cue.Name == soundId.ToString()).Resume();
		    }

			SetPlaying(soundId, true);
		    return true;
	    }

		/// <summary>
		/// Stop current cue, removes it from list of active cues.
		/// </summary>
		/// <param name="soundId"></param>
	    public void StopSound(Sound soundId)
	    {
			SetPlaying(soundId, false);

		    if (mActiveCues.FirstOrDefault(cue => cue.Name == soundId.ToString()) == null)
		    {
			    return;
		    }

			mActiveCues.First(cue => cue.Name == soundId.ToString()).Stop(AudioStopOptions.AsAuthored);
		    mActiveCues.Remove(mActiveCues.First(cue => cue.Name == soundId.ToString()));
	    }

		/// <summary>
		/// Turn Music on or off.
		/// </summary>
		/// <param name="musicOn"></param>
	    public void ToggleMusic(bool musicOn)
	    {
		    mMusicOn = musicOn;

		    if (!mMusicOn)
		    {
			    bool dummy = MainMenuMusicPlaying;
			    PauseSound(Sound.MainMenuMusic);
				SetPlaying(Sound.MainMenuMusic, dummy);
			    dummy = InGameMusicPlaying;
				PauseSound(Sound.InGameMusic);
				SetPlaying(Sound.InGameMusic, dummy);
			    dummy = PauseMusicPlaying;
				PauseSound(Sound.PauseMusic);
				SetPlaying(Sound.PauseMusic, dummy);
		    }
		    else
		    {
			    if (MainMenuMusicPlaying)
			    {
				    if (!ResumeSound(Sound.MainMenuMusic))
				    {
					    PlaySound(Sound.MainMenuMusic);
				    }
			    }
			    if (InGameMusicPlaying)
			    {
				    if (!ResumeSound(Sound.InGameMusic))
				    {
					    PlaySound(Sound.InGameMusic);
				    }
			    }
			    if (PauseMusicPlaying)
			    {
				    if (!ResumeSound(Sound.PauseMusic))
				    {
					    PlaySound(Sound.PauseMusic);
				    }
			    }
		    }
	    }

		/// <summary>
		/// Turn Sound on or off.
		/// </summary>
		/// <param name="soundOn"></param>
	    public void ToogleSound(bool soundOn)
	    {
		    mSoundOn = soundOn;
		    if (mSoundOn)
		    {
			   PauseSound(Category.Units); 
			}
		    else
		    {
			    ResumeSound(Category.Units);
		    }
	    }

		/// <summary>
		/// Pause all sounds of given audio category ('Music', 'Units').
		/// </summary>
		/// <param name="categoryIdentifier"></param>
		private void PauseSound(Category categoryIdentifier)
	    {
			AudioCategory audioCategory = mAudioEngine.GetCategory(mCategories[(int)categoryIdentifier]);
			audioCategory.Pause();
	    }

		/// <summary>
		/// Resume all sounds of given audio category ('Music', 'Units').
		/// </summary>
		/// <param name="categoryIdentifier"></param>
		private void ResumeSound(Category categoryIdentifier)
	    {
			AudioCategory audioCategory = mAudioEngine.GetCategory(mCategories[(int) categoryIdentifier]);
			audioCategory.Resume();
	    }

	    private void SetPlaying(Sound soundId, bool playing)
	    {
	        switch (soundId)
	        {
		        case Sound.MainMenuMusic:
			        mMainMenuMusicPlaying = playing;
			        break;
		        case Sound.InGameMusic:
			        mInGameMusicPlaying = playing;
			        break;
		        case Sound.PauseMusic: 
			        mPauseMusicPlaying = playing;
			        break;
	        }
	    }

		/// <summary>
		/// Set Volume of available ('Music', 'Units') audio categories. Ranges from 0 to 1 (and beyond).
		/// </summary>
		/// <param name="categoryIdentifier"></param>
		/// <param name="volume"></param>
	    public void SetVolume(Category categoryIdentifier, float volume)
		{

			if (categoryIdentifier == Category.Units) { mCurrentSoundVolume = volume; }
			if(categoryIdentifier == Category.Music) {mCurrentMusicVolume = volume;}
			AudioCategory audioCategory = mAudioEngine.GetCategory(mCategories[(int)categoryIdentifier]);
		    if (volume >= 0)
		    {
			    audioCategory.SetVolume(volume);
		    }

	    }

        private bool IsSoundPlaying(Cue cue)
        {
	        return mActiveCues.Any(t => cue.Name == t.Name && t.IsPlaying);
        }

		#region Getter/Setter
		public float CurrentMusicVolume { get { return mCurrentMusicVolume; }}
		public float CurrentSoundVolume { get { return mCurrentSoundVolume; } }
		public bool IsMusicOn { get { return mMusicOn; } }
		public bool IsSoundOn { get { return mSoundOn; } }
	    private bool MainMenuMusicPlaying { get { return mMainMenuMusicPlaying; } }
	    private bool InGameMusicPlaying { get { return mInGameMusicPlaying; } }
	    private bool PauseMusicPlaying { get { return mPauseMusicPlaying; } }
#endregion
    }
}
