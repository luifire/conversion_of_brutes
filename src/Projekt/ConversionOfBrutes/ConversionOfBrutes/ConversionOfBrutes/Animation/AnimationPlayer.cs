//ReSharper Disable All 
// part of the external Animationbundle. leads to problems if things get changed

using System.Collections.Generic;
using System.ComponentModel;
using AnimationAux;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.Animation
{
    /// <summary>
    /// Animation clip player. It maps an animation clip onto a model
    /// </summary>
    public class AnimationPlayer
    {
        #region Fields

        /// <summary>
        /// Current position in time in the clip
        /// </summary>
        private float mPosition;

        /// <summary>
        /// The clip we are playing
        /// </summary>
        private readonly AnimationClip mClip;

        /// <summary>
        /// We maintain a BoneInfo class for each bone. This class does
        /// most of the work in playing the animation.
        /// </summary>
        private readonly BoneInfo[] mBoneInfos;

	    /// <summary>
        /// An assigned model
        /// </summary>
        private readonly AnimatedModel mOdel;

        /// <summary>
        /// The looping option
        /// </summary>
        private bool mLooping;

        #endregion

        #region Properties

        /// <summary>
        /// The position in the animation
        /// </summary>
        [Browsable(false)]
        private float Position
        {
            get { return mPosition; }
            set
            {
                if (value > Duration)
                    value = Duration;

                mPosition = value;
                foreach (BoneInfo bone in mBoneInfos)
                {
                    bone.SetPosition(mPosition);
                }
            }
        }

        /// <summary>
        /// The associated animation clip
        /// </summary>
        [Browsable(false)]
        public AnimationClip Clip { get { return mClip; } }

        /// <summary>
        /// The clip duration
        /// </summary>
        [Browsable(false)]
        private float Duration { get { return (float)mClip.mDuration; } }

        /// <summary>
        /// A model this animation is assigned to. It will play on that model.
        /// </summary>
        [Browsable(false)]
        public AnimatedModel Model { get { return mOdel; } }

        /// <summary>
        /// The looping option. Set to true if you want the animation to loop
        /// back at the end
        /// </summary>
        public bool Looping { get { return mLooping; } set { mLooping = value; } }

        #endregion

        #region Construction

	    /// <summary>
	    /// Constructor for the animation player. It makes the 
	    /// association between a clip and a model and sets up for playing
	    /// </summary>
	    /// <param name="clip"></param>
	    /// <param name="model"></param>
	    public AnimationPlayer(AnimationClip clip, AnimatedModel model)
        {
		    mClip = clip;
            mOdel = model;

            // Create the bone information classes
            var boneCnt = clip.Bones.Count;
            mBoneInfos = new BoneInfo[boneCnt];

            for(int b=0;  b<mBoneInfos.Length;  b++)
            {
                // Create it
                mBoneInfos[b] = new BoneInfo(clip.Bones[b]);

                // Assign it to a model bone
                mBoneInfos[b].SetModel(model);
            }

            Rewind();
        }

        #endregion

        #region Update and Transport Controls


        /// <summary>
        /// Reset back to time zero.
        /// </summary>
        private void Rewind()
        {
            Position = 0;
        }

	    /// <summary>
	    /// Update the clip position
	    /// </summary>
	    public void Update(GameTime gameTime)
        {
            Position = Position + (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (mLooping && Position >= Duration)
                Position = 0;
        }

        #endregion

        #region BoneInfo class


        /// <summary>
        /// Information about a bone we are animating. This class connects a bone
        /// in the clip to a bone in the model.
        /// </summary>
        private class BoneInfo
        {
            #region Fields

            /// <summary>
            /// The current keyframe. Our position is a time such that the 
            /// we are greater than or equal to this keyframe's time and less
            /// than the next keyframes time.
            /// </summary>
            private int mCurrentKeyframe;

            /// <summary>
            /// Bone in a model that this keyframe bone is assigned to
            /// </summary>
            private Bone mAssignedBone;

            /// <summary>
            /// We are not valid until the rotation and translation are set.
            /// If there are no keyframes, we will never be valid
            /// </summary>
            //private bool mValid;

            /// <summary>
            /// Current animation rotation
            /// </summary>
            private Quaternion mRotation;

            /// <summary>
            /// Current animation translation
            /// </summary>
            private Vector3 mTranslation;

            /// <summary>
            /// We are at a location between Keyframe1 and Keyframe2 such 
            /// that Keyframe1's time is less than or equal to the current position
            /// </summary>
            private AnimationClip.Keyframe mKeyframe1;

            /// <summary>
            /// Second keyframe value
            /// </summary>
            private AnimationClip.Keyframe mKeyframe2;

            #endregion

            #region Properties

            /// <summary>
            /// The bone in the actual animation clip
            /// </summary>
            private AnimationClip.Bone ClipBone { get; set; }

            /// <summary>
            /// The bone this animation bone is assigned to in the model
            /// </summary>
            public Bone ModelBone { get { return mAssignedBone; } }
            
            #endregion

            #region Constructor

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bone"></param>
            public BoneInfo(AnimationClip.Bone bone)
            {
                ClipBone = bone;
                SetKeyframes();
                SetPosition(0);
            }


            #endregion

            #region Position and Keyframes

            /// <summary>
            /// Set the bone based on the supplied position value
            /// </summary>
            /// <param name="position"></param>
            public void SetPosition(float position)
            {
                List<AnimationClip.Keyframe> keyframes = ClipBone.Keyframes;
                if (keyframes.Count == 0)
                    return;

                // If our current position is less that the first keyframe
                // we move the position backward until we get to the right keyframe
                while (position < mKeyframe1.mTime && mCurrentKeyframe > 0)
                {
                    // We need to move backwards in time
                    mCurrentKeyframe--;
                    SetKeyframes();
                }

                // If our current position is greater than the second keyframe
                // we move the position forward until we get to the right keyframe
                while (position >= mKeyframe2.mTime && mCurrentKeyframe < ClipBone.Keyframes.Count - 2)
                {
                    // We need to move forwards in time
                    mCurrentKeyframe++;
                    SetKeyframes();
                }

                if (mKeyframe1 == mKeyframe2)
                {
                    // Keyframes are equal
                    mRotation = mKeyframe1.mRotation;
                    mTranslation = mKeyframe1.mTranslation;
                }
                else
                {
                    // Interpolate between keyframes
                    float t = (float)((position - mKeyframe1.mTime) / (mKeyframe2.mTime - mKeyframe1.mTime));
                    mRotation = Quaternion.Slerp(mKeyframe1.mRotation, mKeyframe2.mRotation, t);
                    mTranslation = Vector3.Lerp(mKeyframe1.mTranslation, mKeyframe2.mTranslation, t);
                }

                //mValid = true;
                if (mAssignedBone != null)
                {
                    // Send to the model
                    // Make it a matrix first
                    Matrix m = Matrix.CreateFromQuaternion(mRotation);
                    m.Translation = mTranslation;
                    mAssignedBone.SetCompleteTransform(m);
                }
            }



            /// <summary>
            /// Set the keyframes to a valid value relative to 
            /// the current keyframe
            /// </summary>
            private void SetKeyframes()
            {
                if (ClipBone.Keyframes.Count > 0)
                {
                    mKeyframe1 = ClipBone.Keyframes[mCurrentKeyframe];
                    if (mCurrentKeyframe == ClipBone.Keyframes.Count - 1)
                        mKeyframe2 = mKeyframe1;
                    else
                        mKeyframe2 = ClipBone.Keyframes[mCurrentKeyframe + 1];
                }
                else
                {
                    // If there are no keyframes, set both to null
                    mKeyframe1 = null;
                    mKeyframe2 = null;
                }
            }

            /// <summary>
            /// Assign this bone to the correct bone in the model
            /// </summary>
            /// <param name="model"></param>
            public void SetModel(AnimatedModel model)
            {
                // Find this bone
                mAssignedBone = model.FindBone(ClipBone.Name);

            }

            #endregion
        }

        #endregion

    }
}
