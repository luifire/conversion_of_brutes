// ReSharper Disable All 
// because the resharper wants to change important variables.
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.Animation
{
    /// <summary>
    /// Bones in this model are represented by this class, which
    /// allows a bone to have more detail associatd with it.
    /// 
    /// This class allows you to manipulate the local coordinate system
    /// for objects by changing the scaling, translation, and rotation.
    /// These are indepenent of the bind transformation originally supplied
    /// for the model. So, the actual transformation for a bone is
    /// the product of the:
    /// 
    /// Scaling
    /// Bind scaling (scaling removed from the bind transform)
    /// Rotation
    /// Translation
    /// Bind Transformation
    /// Parent Absolute Transformation
    /// 
    /// </summary>
	public class Bone
    {
        #region Fields

        /// <summary>
        /// Any parent for this bone
        /// </summary>
        private readonly Bone mParent;

        /// <summary>
        /// The children of this bone
        /// </summary>
        private readonly List<Bone> mChildren = new List<Bone>();

        /// <summary>
        /// The bind transform is the transform for this bone
        /// as loaded from the original model. It's the base pose.
        /// I do remove any scaling, though.
        /// </summary>
        private readonly Matrix mBindTransform;

        /// <summary>
        /// The bind scaling component extracted from the bind transform
        /// </summary>
        private readonly Vector3 mBindScale;

        /// <summary>
        /// Any translation applied to the bone
        /// </summary>
        private Vector3 mTranslation = Vector3.Zero;

        /// <summary>
        /// Any rotation applied to the bone
        /// </summary>
        private Quaternion mRotation = Quaternion.Identity;

        /// <summary>
        /// Any scaling applied to the bone
        /// </summary>
        private Vector3 mScale = Vector3.One;

        #endregion 

        #region Properties

        /// <summary>
        /// The bone name
        /// </summary>
        private readonly string mName;

        /// <summary>
        /// The bone bind transform
        /// </summary>
        private Matrix BindTransform { get {return mBindTransform;} }

        /// <summary>
        /// Inverse of absolute bind transform for skinnning
        /// </summary>
        public Matrix SkinTransform { get; private set; }

        /// <summary>
        /// Bone rotation
        /// </summary>
        private Quaternion Rotation {get {return mRotation;} set {mRotation = value;}}

        /// <summary>
        /// Any translations
        /// </summary>
        private Vector3 Translation {get {return mTranslation;} set {mTranslation = value;}}

        /// <summary>
        /// Any scaling
        /// </summary>
        private Vector3 Scale { get { return mScale; } }

        /// <summary>
        /// The parent bone or null for the root bone
        /// </summary>
        private Bone Parent { get { return mParent; } }

        /// <summary>
        /// The children of this bone
        /// </summary>
        public List<Bone> Children { get { return mChildren; } }

        /// <summary>
        /// The bone absolute transform
        /// </summary>
        public Matrix mAbsoluteTransform = Matrix.Identity;

	    private Matrix mBoneScale;

        #endregion

        #region Operations

        /// <summary>
        /// Constructor for a bone object
        /// </summary>
        /// <param name="name">The name of the bone</param>
        /// <param name="bindTransform">The initial bind transform for the bone</param>
        /// <param name="parent">A parent for this bone</param>
        public Bone(string name, Matrix bindTransform, Bone parent)
        {
            mName = name;
            mParent = parent;
            if (parent != null)
                parent.mChildren.Add(this);

            // I am not supporting scaling in animation in this
            // example, so I extract the bind scaling from the 
            // bind transform and save it. 

            mBindScale = new Vector3(bindTransform.Right.Length(),
                bindTransform.Up.Length(), bindTransform.Backward.Length());

            bindTransform.Right = bindTransform.Right / mBindScale.X;
            bindTransform.Up = bindTransform.Up / mBindScale.Y;
            bindTransform.Backward = bindTransform.Backward / mBindScale.Y;
            mBindTransform = bindTransform;

            // Set the skinning bind transform
            // That is the inverse of the absolute transform in the bind pose
	        mBoneScale = Matrix.CreateScale(Scale * mBindScale);
            ComputeAbsoluteTransform();
            SkinTransform = Matrix.Invert(mAbsoluteTransform);

        }

        /// <summary>
        /// Compute the absolute transformation for this bone.
        /// </summary>
        public void ComputeAbsoluteTransform()
        {
            Matrix transform = mBoneScale *
                Matrix.CreateFromQuaternion(Rotation) *
                Matrix.CreateTranslation(Translation) *
                mBindTransform;

            if (Parent != null)
            {
                // This bone has a parent bone
                mAbsoluteTransform = transform * Parent.mAbsoluteTransform;
            }
            else
            {   // The root bone
                mAbsoluteTransform = transform;
            }
        }

	    private static Dictionary<Matrix, Matrix> sMatrix = new Dictionary<Matrix, Matrix>(); 
        /// <summary>
        /// This sets the rotation and translation such that the
        /// rotation times the translation times the bind after set
        /// equals this matrix. This is used to set animation values.
        /// </summary>
        /// <param name="m">A matrix include translation and rotation</param>
        public void SetCompleteTransform(Matrix m)
        {
	        Matrix setTo;
	        Matrix inverted;

			if (sMatrix.TryGetValue(mBindTransform, out inverted) == false)
	        {
		        Matrix a = Matrix.Invert(mBindTransform);
				sMatrix.Add(mBindTransform, a);
		        setTo = m * a;
	        }
			else
				setTo = m * inverted;
			
            Translation = setTo.Translation;
            Rotation = Quaternion.CreateFromRotationMatrix(setTo);
        }

        #endregion

    }
}
