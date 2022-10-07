// ReSharper Disable All 

/*
 * Made Private for Resharper
 * 
 *	public int Count
	public bool IsEmptyLeaf
	internal void GetAllObjects(ref List<T> results)
 */

/* NOTES:
 * ------
 * This quad tree was developed as a generically typed quad tree for use with
 * the Microsoft XNA framework. To that end, it references
 * Microsoft.Xna.Framework.Rectangle to supply the functionality for defining a
 * rectangle as well as providing the Contains and Intersects methods used for
 * determining what is in a quad or not.
 * 
 * This code can quite easily be modified to remove the dependence on the XNA
 * framework by removing the reference and updating anywhere that the rectangle
 * is used. The rest should function as is.
 */

using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.Map
{
	/// <summary>
	/// Interface to define Rect, so that QuadTree knows how to store the object.
	/// </summary>
	public interface IQuadStorable
	{
		/// <summary>
		/// The rectangle that defines the object's boundaries.
		/// </summary>
		Rectangle Rect { get; }
		
		/// <summary>
		/// This should return True if the object has moved during the last update, false otherwise
		/// </summary>
		bool HasMoved { get; }
	}

	/// <summary>
	/// Used internally to attach an Owner to each object stored in the QuadTree
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class QuadTreeObject<T> where T : IQuadStorable //, IComparable<QuadTreeObject<T>>
	{
		/// <summary>
		/// The wrapped data value
		/// </summary>
		public T Data
		{
			get;
			private set;
		}

		/// <summary>
		/// The QuadTreeNode that owns this object
		/// </summary>
		internal QuadTreeNode<T> Owner
		{
			get;
			set;
		}

		/// <summary>
		/// Wraps the data value
		/// </summary>
		/// <param name="data">The data value to wrap</param>
		public QuadTreeObject(T data)
		{
			Data = data;
		}


		//public int CompareTo(QuadTreeObject<T> other)
		//{
		//    return (int)(Data.Rect.Y + Data.Rect.Height) - (int)(other.Data.Rect.Y + other.Data.Rect.Height);
		//}
	}

	/// <summary>
	/// A QuadTree Object that provides fast and efficient storage of objects in a world space.
	/// </summary>
	/// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
	public class QuadTree<T> : ICollection<T> where T : IQuadStorable
	{
		#region Private Members

		private readonly Dictionary<T, QuadTreeObject<T>> mWrappedDictionary = new Dictionary<T, QuadTreeObject<T>>();

		// Alternate method, use Parallel arrays
		//private List<T> m_rawObjects = new List<T>();       // The unwrapped objects in this QuadTree
		//private List<QuadTreeObject<T>> m_wrappedObjects = new List<QuadTreeObject<T>>();       // The wrapped objects in this QuadTree

		// The root of this quad tree
		private readonly QuadTreeNode<T> mQuadTreeRoot;

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a QuadTree for the specified area.
		/// </summary>
		/// <param name="rect">The area this QuadTree object will encompass.</param>
		public QuadTree(Rectangle rect)
		{
			mQuadTreeRoot = new QuadTreeNode<T>(rect);
		}


		/// <summary>
		/// Creates a QuadTree for the specified area.
		/// </summary>
		/// <param name="x">The top-left position of the area rectangle.</param>
		/// <param name="y">The top-right position of the area rectangle.</param>
		/// <param name="width">The width of the area rectangle.</param>
		/// <param name="height">The height of the area rectangle.</param>
		public QuadTree(int x, int y, int width, int height)
		{
			mQuadTreeRoot = new QuadTreeNode<T>(new Rectangle(x, y, width, height));
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the rectangle that bounds this QuadTree
		/// </summary>
		public Rectangle QuadRect
		{
			get { return mQuadTreeRoot.QuadRect; }
		}

		/// <summary>
		/// Get the objects in this tree that intersect with the specified rectangle.
		/// </summary>
		/// <param name="rect">The rectangle to find objects in.</param>
		public List<T> GetObjects(Rectangle rect)
		{
			return mQuadTreeRoot.GetObjects(rect);
		}


		/// <summary>
		/// Get the objects in this tree that intersect with the specified rectangle.
		/// </summary>
		/// <param name="rect">The rectangle to find objects in.</param>
		/// <param name="results">A reference to a list that will be populated with the results.</param>
		public void GetObjects(Rectangle rect, ref List<T> results)
		{
			mQuadTreeRoot.GetObjects(rect, ref results);
		}


		/// <summary>
		/// Get all objects in this Quad, and it's children.
		/// </summary>
		public List<T> GetAllObjects()
		{
			return new List<T>(mWrappedDictionary.Keys);
			//quadTreeRoot.GetAllObjects(ref results);
		}


		/// <summary>
		/// Moves the object in the tree
		/// </summary>
		/// <param name="item">The item that has moved</param>
		public bool Move(T item)
		{
			if (Contains(item))
			{
				mQuadTreeRoot.Move(mWrappedDictionary[item]);
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region ICollection<T> Members

		///<summary>
		///Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
		///</summary>
		///
		///<param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
		public void Add(T item)
		{
			QuadTreeObject<T> wrappedObject = new QuadTreeObject<T>(item);
			mWrappedDictionary.Add(item, wrappedObject);
			mQuadTreeRoot.Insert(wrappedObject);
		}


		///<summary>
		///Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
		///</summary>
		///
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
		public void Clear()
		{
			mWrappedDictionary.Clear();
			mQuadTreeRoot.Clear();
		}


		///<summary>
		///Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
		///</summary>
		///
		///<returns>
		///true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
		///</returns>
		///
		///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
		public bool Contains(T item)
		{
			return mWrappedDictionary.ContainsKey(item);
		}


		///<summary>
		///Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
		///</summary>
		///
		///<param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		///<param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		///<exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null.</exception>
		///<exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than 0.</exception>
		///<exception cref="T:System.ArgumentException"><paramref name="array" /> is multidimensional.-or-<paramref name="arrayIndex" /> is equal to or greater than the length of <paramref name="array" />.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.-or-Type cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
		public void CopyTo(T[] array, int arrayIndex)
		{
			mWrappedDictionary.Keys.CopyTo(array, arrayIndex);
		}

		///<summary>
		///Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		///</summary>
		///<returns>
		///The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		///</returns>
		public int Count
		{
			get { return mWrappedDictionary.Count; }
		}

		///<summary>
		///Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
		///</summary>
		///
		///<returns>
		///true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
		///</returns>
		///
		public bool IsReadOnly
		{
			get { return false; }
		}

		///<summary>
		///Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
		///</summary>
		///
		///<returns>
		///true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
		///</returns>
		///
		///<param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
		public bool Remove(T item)
		{
			if (Contains(item))
			{
				mQuadTreeRoot.Delete(mWrappedDictionary[item], true);
				mWrappedDictionary.Remove(item);
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region IEnumerable<T> and IEnumerable Members

		///<summary>
		///Returns an enumerator that iterates through the collection.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>1</filterpriority>
		public IEnumerator<T> GetEnumerator()
		{
			return mWrappedDictionary.Keys.GetEnumerator();
		}


		///<summary>
		///Returns an enumerator that iterates through a collection.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion


		/// <summary>
		/// The top left child for this QuadTree, only usable in debug mode
		/// </summary>
		public QuadTreeNode<T> RootQuad
		{
			get { return mQuadTreeRoot; }
		}
	}

	/// <summary>
	/// A QuadTree Object that provides fast and efficient storage of objects in a world space.
	/// </summary>
	/// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
	public class QuadTreeNode<T> where T : IQuadStorable
	{
		#region Constants

		// How many objects can exist in a QuadTree before it sub divides itself
		private const int MaxObjectsPerNode = 2;

		#endregion

		#region Private Members

		//private List<T> m_objects = null;       // The objects in this QuadTree
		private List<QuadTreeObject<T>> mObjects;
		private Rectangle mRect; // The area this QuadTree represents

		private QuadTreeNode<T> mParent; // The parent of this quad

		private QuadTreeNode<T> mChildTl; // Top Left Child
		private QuadTreeNode<T> mChildTr; // Top Right Child
		private QuadTreeNode<T> mChildBl; // Bottom Left Child
		private QuadTreeNode<T> mChildBr; // Bottom Right Child

		#endregion

		#region Public Properties

		/// <summary>
		/// The area this QuadTree represents.
		/// </summary>
		public Rectangle QuadRect
		{
			get { return mRect; }
		}

		/// <summary>
		/// The top left child for this QuadTree
		/// </summary>
		public QuadTreeNode<T> TopLeftChild
		{
			get { return mChildTl; }
		}

		/// <summary>
		/// The top right child for this QuadTree
		/// </summary>
		public QuadTreeNode<T> TopRightChild
		{
			get { return mChildTr; }
		}

		/// <summary>
		/// The bottom left child for this QuadTree
		/// </summary>
		public QuadTreeNode<T> BottomLeftChild
		{
			get { return mChildBl; }
		}

		/// <summary>
		/// The bottom right child for this QuadTree
		/// </summary>
		public QuadTreeNode<T> BottomRightChild
		{
			get { return mChildBr; }
		}

		/// <summary>
		/// This QuadTree's parent
		/// </summary>
		public QuadTreeNode<T> Parent
		{
			get { return mParent; }
		}

		/// <summary>
		/// The objects contained in this QuadTree at it's level (ie, excludes children)
		/// </summary>
		//public List<T> Objects { get { return m_objects; } }
		internal List<QuadTreeObject<T>> Objects
		{
			get { return mObjects; }
		}

		/// <summary>
		/// How many total objects are contained within this QuadTree (ie, includes children)
		/// </summary>
		private int Count
		{
			get { return ObjectCount(); }
		}

		/// <summary>
		/// Returns true if this is a empty leaf node
		/// </summary>
		private bool IsEmptyLeaf
		{
			get { return Count == 0 && mChildTl == null; }
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a QuadTree for the specified area.
		/// </summary>
		/// <param name="rect">The area this QuadTree object will encompass.</param>
		public QuadTreeNode(Rectangle rect)
		{
			mRect = rect;
		}


		/// <summary>
		/// Creates a QuadTree for the specified area.
		/// </summary>
		/// <param name="x">The top-left position of the area rectangle.</param>
		/// <param name="y">The top-right position of the area rectangle.</param>
		/// <param name="width">The width of the area rectangle.</param>
		/// <param name="height">The height of the area rectangle.</param>
		public QuadTreeNode(int x, int y, int width, int height)
		{
			mRect = new Rectangle(x, y, width, height);
		}


		private QuadTreeNode(QuadTreeNode<T> parent, Rectangle rect)
			: this(rect)
		{
			mParent = parent;
		}

		#endregion

		#region Private Members

		/// <summary>
		/// Add an item to the object list.
		/// </summary>
		/// <param name="item">The item to add.</param>
		private void Add(QuadTreeObject<T> item)
		{
			if (mObjects == null)
			{
				//m_objects = new List<T>();
				mObjects = new List<QuadTreeObject<T>>();
			}

			item.Owner = this;
			mObjects.Add(item);
		}


		/// <summary>
		/// Remove an item from the object list.
		/// </summary>
		/// <param name="item">The object to remove.</param>
		private void Remove(QuadTreeObject<T> item)
		{
			if (mObjects != null)
			{
				int removeIndex = mObjects.IndexOf(item);
				if (removeIndex >= 0)
				{
					mObjects[removeIndex] = mObjects[mObjects.Count - 1];
					mObjects.RemoveAt(mObjects.Count - 1);
				}
			}
		}


		/// <summary>
		/// Get the total for all objects in this QuadTree, including children.
		/// </summary>
		/// <returns>The number of objects contained within this QuadTree and its children.</returns>
		private int ObjectCount()
		{
			int count = 0;

			// Add the objects at this level
			if (mObjects != null)
			{
				count += mObjects.Count;
			}

			// Add the objects that are contained in the children
			if (mChildTl != null)
			{
				count += mChildTl.ObjectCount();
				count += mChildTr.ObjectCount();
				count += mChildBl.ObjectCount();
				count += mChildBr.ObjectCount();
			}

			return count;
		}


		/// <summary>
		/// Subdivide this QuadTree and move it's children into the appropriate Quads where applicable.
		/// </summary>
		private void Subdivide()
		{
			// We've reached capacity, subdivide...
			Point size = new Point(mRect.Width / 2, mRect.Height / 2);
			Point mid = new Point(mRect.X + size.X, mRect.Y + size.Y);

			mChildTl = new QuadTreeNode<T>(this, new Rectangle(mRect.Left, mRect.Top, size.X, size.Y));
			mChildTr = new QuadTreeNode<T>(this, new Rectangle(mid.X, mRect.Top, size.X, size.Y));
			mChildBl = new QuadTreeNode<T>(this, new Rectangle(mRect.Left, mid.Y, size.X, size.Y));
			mChildBr = new QuadTreeNode<T>(this, new Rectangle(mid.X, mid.Y, size.X, size.Y));

			// If they're completely contained by the quad, bump objects down
			for (int i = 0; i < mObjects.Count; i++)
			{
				QuadTreeNode<T> destTree = GetDestinationTree(mObjects[i]);

				if (destTree != this)
				{
					// Insert to the appropriate tree, remove the object, and back up one in the loop
					destTree.Insert(mObjects[i]);
					Remove(mObjects[i]);
					i--;
				}
			}
		}


		/// <summary>
		/// Get the child Quad that would contain an object.
		/// </summary>
		/// <param name="item">The object to get a child for.</param>
		/// <returns></returns>
		private QuadTreeNode<T> GetDestinationTree(QuadTreeObject<T> item)
		{
			// If a child can't contain an object, it will live in this Quad
			QuadTreeNode<T> destTree = this;

			if (mChildTl.QuadRect.Contains(item.Data.Rect))
			{
				destTree = mChildTl;
			}
			else if (mChildTr.QuadRect.Contains(item.Data.Rect))
			{
				destTree = mChildTr;
			}
			else if (mChildBl.QuadRect.Contains(item.Data.Rect))
			{
				destTree = mChildBl;
			}
			else if (mChildBr.QuadRect.Contains(item.Data.Rect))
			{
				destTree = mChildBr;
			}

			return destTree;
		}


		private void Relocate(QuadTreeObject<T> item)
		{
			// Are we still inside our parent?
			if (QuadRect.Contains(item.Data.Rect))
			{
				// Good, have we moved inside any of our children?
				if (mChildTl != null)
				{
					QuadTreeNode<T> dest = GetDestinationTree(item);
					if (item.Owner != dest)
					{
						// Delete the item from this quad and add it to our child
						// Note: Do NOT clean during this call, it can potentially delete our destination quad
						QuadTreeNode<T> formerOwner = item.Owner;
						Delete(item, false);
						dest.Insert(item);

						// Clean up ourselves
						formerOwner.CleanUpwards();
					}
				}
			}
			else
			{
				// We don't fit here anymore, move up, if we can
				if (mParent != null)
				{
					mParent.Relocate(item);
				}
			}
		}


		private void CleanUpwards()
		{
			if (mChildTl != null)
			{
				// If all the children are empty leaves, delete all the children
				if (mChildTl.IsEmptyLeaf &&
					mChildTr.IsEmptyLeaf &&
					mChildBl.IsEmptyLeaf &&
					mChildBr.IsEmptyLeaf)
				{
					mChildTl = null;
					mChildTr = null;
					mChildBl = null;
					mChildBr = null;

					if (mParent != null && Count == 0)
					{
						mParent.CleanUpwards();
					}
				}
			}
			else
			{
				// I could be one of 4 empty leaves, tell my parent to clean up
				if (mParent != null && Count == 0)
				{
					mParent.CleanUpwards();
				}
			}
		}

		#endregion

		#region Internal Methods

		/// <summary>
		/// Clears the QuadTree of all objects, including any objects living in its children.
		/// </summary>
		internal void Clear()
		{
			// Clear out the children, if we have any
			if (mChildTl != null)
			{
				mChildTl.Clear();
				mChildTr.Clear();
				mChildBl.Clear();
				mChildBr.Clear();
			}

			// Clear any objects at this level
			if (mObjects != null)
			{
				mObjects.Clear();
				mObjects = null;
			}

			// Set the children to null
			mChildTl = null;
			mChildTr = null;
			mChildBl = null;
			mChildBr = null;
		}


		/// <summary>
		/// Deletes an item from this QuadTree. If the object is removed causes this Quad to have no objects in its children, it's children will be removed as well.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		/// <param name="clean">Whether or not to clean the tree</param>
		internal void Delete(QuadTreeObject<T> item, bool clean)
		{
			if (item.Owner != null)
			{
				if (item.Owner == this)
				{
					Remove(item);
					if (clean)
					{
						CleanUpwards();
					}
				}
				else
				{
					item.Owner.Delete(item, clean);
				}
			}
		}



		/// <summary>
		/// Insert an item into this QuadTree object.
		/// </summary>
		/// <param name="item">The item to insert.</param>
		internal void Insert(QuadTreeObject<T> item)
		{
			// If this quad doesn't contain the items rectangle, do nothing, unless we are the root
			if (!mRect.Contains(item.Data.Rect))
			{
				System.Diagnostics.Debug.Assert(mParent == null, "We are not the root, and this object doesn't fit here. How did we get here?");
				if (mParent == null)
				{
					// This object is outside of the QuadTree bounds, we should add it at the root level
					Add(item);
				}
				else
				{
					return;
				}
			}

			if (mObjects == null ||
				(mChildTl == null && mObjects.Count + 1 <= MaxObjectsPerNode))
			{
				// If there's room to add the object, just add it
				Add(item);
			}
			else
			{
				// No quads, create them and bump objects down where appropriate
				if (mChildTl == null)
				{
					Subdivide();
				}

				// Find out which tree this object should go in and add it there
				QuadTreeNode<T> destTree = GetDestinationTree(item);
				if (destTree == this)
				{
					Add(item);
				}
				else
				{
					destTree.Insert(item);
				}
			}
		}


		/// <summary>
		/// Get the objects in this tree that intersect with the specified rectangle.
		/// </summary>
		/// <param name="searchRect">The rectangle to find objects in.</param>
		internal List<T> GetObjects(Rectangle searchRect)
		{
			List<T> results = new List<T>();
			GetObjects(searchRect, ref results);
			return results;
		}


		/// <summary>
		/// Get the objects in this tree that intersect with the specified rectangle.
		/// </summary>
		/// <param name="searchRect">The rectangle to find objects in.</param>
		/// <param name="results">A reference to a list that will be populated with the results.</param>
		internal void GetObjects(Rectangle searchRect, ref List<T> results)
		{
			// We can't do anything if the results list doesn't exist
			if (results != null)
			{
				if (searchRect.Contains(mRect))
				{
					// If the search area completely contains this quad, just get every object this quad and all it's children have
					GetAllObjects(ref results);
				}
				else if (searchRect.Intersects(mRect))
				{
					// Otherwise, if the quad isn't fully contained, only add objects that intersect with the search rectangle
					if (mObjects != null)
					{
						for (int i = 0; i < mObjects.Count; i++)
						{
							if (searchRect.Intersects(mObjects[i].Data.Rect))
							{
								results.Add(mObjects[i].Data);
							}
						}
					}

					// Get the objects for the search rectangle from the children
					if (mChildTl != null)
					{
						mChildTl.GetObjects(searchRect, ref results);
						mChildTr.GetObjects(searchRect, ref results);
						mChildBl.GetObjects(searchRect, ref results);
						mChildBr.GetObjects(searchRect, ref results);
					}
				}
			}
		}


		/// <summary>
		/// Get all objects in this Quad, and it's children.
		/// </summary>
		/// <param name="results">A reference to a list in which to store the objects.</param>
		private void GetAllObjects(ref List<T> results)
		{
			// If this Quad has objects, add them
			if (mObjects != null)
			{
				foreach (QuadTreeObject<T> qto in mObjects)
				{
					results.Add(qto.Data);
				}
			}

			// If we have children, get their objects too
			if (mChildTl != null)
			{
				mChildTl.GetAllObjects(ref results);
				mChildTr.GetAllObjects(ref results);
				mChildBl.GetAllObjects(ref results);
				mChildBr.GetAllObjects(ref results);
			}
		}


		/// <summary>
		/// Moves the QuadTree object in the tree
		/// </summary>
		/// <param name="item">The item that has moved</param>
		internal void Move(QuadTreeObject<T> item)
		{
			if (item.Owner != null)
			{
				item.Owner.Relocate(item);
			}
			else
			{
				Relocate(item);
			}
		}

		#endregion
	}
}