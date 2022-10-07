
using System.Collections.Generic;
using AnimationAux;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Animation
{
    /// <summary>
    /// An encloser for an XNA model that we will use that includes support for
    /// bones, animation, and some manipulations.
    /// </summary>
    public class AnimatedModel
    {
        #region Fields

        /// <summary>
        /// The actual underlying XNA model
        /// </summary>
        public Model mOdel;

        /// <summary>
        /// Extra data associated with the XNA model
        /// </summary>
        public ModelExtra mModelExtra;

        /// <summary>
        /// The model bones
        /// </summary>
        public readonly List<Bone> mBones = new List<Bone>();
		private readonly Dictionary<string, Bone> mBonerFinder = new Dictionary<string, Bone>();

        /// <summary>
        /// The model asset name
        /// </summary>
        private readonly string mAssetName;

        /// <summary>
        /// An associated animation clip player
        /// </summary>
        public AnimationPlayer mPlayer;

        #endregion

       #region Properties


        /// <summary>
        /// The model animation clips
        /// </summary>
        public List<AnimationClip> Clips { get { return mModelExtra.Clips; } }

        #endregion

        #region Construction and Loading

        /// <summary>
        /// Constructor. Creates the model from an XNA model
        /// </summary>
        /// <param name="assetName">The name of the asset for this model</param>
        public AnimatedModel(string assetName)
        {
            mAssetName = assetName;

        }

        /// <summary>
        /// Load the model asset from content
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content)
        {
            mOdel = content.Load<Model>(mAssetName);
            mModelExtra = mOdel.Tag as ModelExtra;
            System.Diagnostics.Debug.Assert(mModelExtra != null);

            ObtainBones();
        }


        #endregion

        #region Bones Management

        /// <summary>
        /// Get the bones from the model and create a bone class object for
        /// each bone. We use our bone class to do the real animated bone work.
        /// </summary>
        private void ObtainBones()
        {
            mBones.Clear();
            foreach (ModelBone bone in mOdel.Bones)
            {
                // Create the bone object and add to the heirarchy
                Bone newBone = new Bone(bone.Name, bone.Transform, bone.Parent != null ? mBones[bone.Parent.Index] : null);

                // Add to the bones for this model
                mBones.Add(newBone);
				mBonerFinder.Add(bone.Name, newBone);
            }
        }

        /// <summary>
        /// Find a bone in this model by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bone FindBone(string name)
        {
            /*foreach(Bone bone in Bones)
            {
                if (bone.mName == name)
                    return bone;
            }

            return null;*/
	        Bone found;
	        return (mBonerFinder.TryGetValue(name, out found)) ? found : null;
        }

        #endregion
		
        #region Updating

        /// <summary>
        /// Update animation for the model.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (mPlayer != null)
            {
                mPlayer.Update(gameTime);
            }
        }

        #endregion
        

    }
}
