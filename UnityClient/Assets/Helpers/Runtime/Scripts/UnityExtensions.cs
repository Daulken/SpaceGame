using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// Helper extensions to built-in Unity objects
/// </summary>
public static class UnityExtensions
{

	/// <summary>
	/// Sets all child GameObjects of this transform to the given active state, while not touching this transform itself
	/// </summary>
	public static void SetChildrenActive(this Transform transform, bool active)
	{
		int childCount = transform.childCount;
		for (int childIndex = 0; childIndex < childCount; ++childIndex)
		{
			Transform child = transform.GetChild(childIndex);
			child.gameObject.SetActive(active);
		}
	}

	/// <summary>
	/// Destroy all child GameObjects of this transform, while not destroying the transform itself
	/// </summary>
	public static void DestroyChildren(this Transform transform)
	{
		int childCount = transform.childCount;
		for (int childIndex = childCount - 1; childIndex >= 0; --childIndex)
		{
			Transform child = transform.GetChild(childIndex);
			GameObject.Destroy(child.gameObject);
		}
	}

	//---------------------------------------------------

	/// <summary>
	/// Get the first component of a particular type on the Transform's children, or null if none found
	/// </summary>
	public static T GetComponentInChildren<T>(this Transform transform, bool includeInactive) where T : Component
	{
		T[] components = transform.GetComponentsInChildren<T>(includeInactive);
		if ((components == null) || (components.Length == 0))
			return null;
		return components[0];
	}

	/// <summary>
	/// Get the first component of a particular type on the Transform's children, or null if none found
	/// </summary>
	public static Component GetComponentInChildren(this Transform transform, Type t, bool includeInactive)
	{
		Component[] components = transform.GetComponentsInChildren(t, includeInactive);
		if ((components == null) || (components.Length == 0))
			return null;
		return components[0];
	}

	/// <summary>
	/// Get the first component of a particular type on the Component's children, or null if none found
	/// </summary>
	public static T GetComponentInChildren<T>(this Component component, bool includeInactive) where T : Component
	{
		return component.transform.GetComponentInChildren<T>(includeInactive);
	}

	/// <summary>
	/// Get the first component of a particular type on the Component's children, or null if none found
	/// </summary>
	public static Component GetComponentInChildren(this Component component, Type t, bool includeInactive)
	{
		return component.transform.GetComponentInChildren(t, includeInactive);
	}

	/// <summary>
	/// Get the first component of a particular type on the GameObject's children, or null if none found
	/// </summary>
	public static T GetComponentInChildren<T>(this GameObject gameObject, bool includeInactive) where T : Component
	{
		return gameObject.transform.GetComponentInChildren<T>(includeInactive);
	}

	/// <summary>
	/// Get the first component of a particular type on the GameObject's children, or null if none found
	/// </summary>
	public static Component GetComponentInChildren(this GameObject gameObject, Type t, bool includeInactive)
	{
		return gameObject.transform.GetComponentInChildren(t, includeInactive);
	}

	//---------------------------------------------------

	/// <summary>
	/// Get the first ancestor component of a particular type on the Transform's parents, or null if none found
	/// </summary>
	public static T GetComponentInAncestors<T>(this Transform transform) where T : Component
	{
		transform = transform.parent;
		while (transform != null)
		{
			T component = transform.GetComponent<T>();
			if (component != null)
				return component;
			transform = transform.parent;
		}
		return null;
	}

	/// <summary>
	/// Get the first ancestor component of a particular type on the Transform's parents, or null if none found
	/// </summary>
	public static Component GetComponentInAncestors(this Transform transform, Type t)
	{
		transform = transform.parent;
		while (transform != null)
		{
			Component component = transform.GetComponent(t);
			if (component != null)
				return component;
			transform = transform.parent;
		}
		return null;
	}
	
	/// <summary>
	/// Get the first ancestor component of a particular type on the Component's parents, or null if none found
	/// </summary>
	public static T GetComponentInAncestors<T>(this Component component) where T : Component
	{
		return component.transform.GetComponentInAncestors<T>();
	}

	/// <summary>
	/// Get the first ancestor component of a particular type on the Component's parents, or null if none found
	/// </summary>
	public static Component GetComponentInAncestors(this Component component, Type t)
	{
		return component.transform.GetComponentInAncestors(t);
	}

	/// <summary>
	/// Get the first ancestor component of a particular type on the GameObject's parents, or null if none found
	/// </summary>
	public static T GetComponentInAncestors<T>(this GameObject gameObject) where T : Component
	{
		return gameObject.transform.GetComponentInAncestors<T>();
	}

	/// <summary>
	/// Get the first ancestor component of a particular type on the GameObject's parents, or null if none found
	/// </summary>
	public static Component GetComponentInAncestors(this GameObject gameObject, Type t)
	{
		return gameObject.transform.GetComponentInAncestors(t);
	}

	//---------------------------------------------------

	/// <summary>
	/// Gets or add a component. Usage example:
	/// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
	/// </summary>
	public static T GetOrAddComponent<T>(this Component child) where T : Component
	{
		T result = child.GetComponent<T>();
		if (result == null)
			result = child.gameObject.AddComponent<T>();
		return result;
	}

	//---------------------------------------------------

	/// <summary>
	/// Check if the given GameObject is a parent of the current transform
	/// </summary>
	public static bool IsParentOf(this Transform transform, GameObject possibleParent)
	{
		if (transform.gameObject == possibleParent)
			return true;
		Transform parentTransform = transform.parent;
		while (parentTransform != null)
		{
			if (parentTransform.gameObject == possibleParent)
				return true;
			parentTransform = parentTransform.parent;
		}
		return false;
	}

	/// <summary>
	/// Check if the given Transform is a parent of the current transform
	/// </summary>
	public static bool IsParentOf(this Transform transform, Transform possibleParent)
	{
		if (transform == possibleParent)
			return true;
		Transform parentTransform = transform.parent;
		while (parentTransform != null)
		{
			if (parentTransform == possibleParent)
				return true;
			parentTransform = parentTransform.parent;
		}
		return false;
	}
	
	/// <summary>
	/// Check if the given GameObject is a parent of the current component
	/// </summary>
	public static bool IsParentOf(this Component component, GameObject possibleParent)
	{
		if (component.gameObject == possibleParent)
			return true;
		Transform parentTransform = component.transform.parent;
		while (parentTransform != null)
		{
			if (parentTransform.gameObject == possibleParent)
				return true;
			parentTransform = parentTransform.parent;
		}
		return false;
	}

	/// <summary>
	/// Check if the given Transform is a parent of the current component
	/// </summary>
	public static bool IsParentOf(this Component component, Transform possibleParent)
	{
		if (component.transform == possibleParent)
			return true;
		Transform parentTransform = component.transform.parent;
		while (parentTransform != null)
		{
			if (parentTransform == possibleParent)
				return true;
			parentTransform = parentTransform.parent;
		}
		return false;
	}

	
	/// <summary>
	/// Check if the given GameObject is a parent of the current GameObject
	/// </summary>
	public static bool IsParentOf(this GameObject gameObject, GameObject possibleParent)
	{
		if (gameObject == possibleParent)
			return true;
		Transform parentTransform = gameObject.transform.parent;
		while (parentTransform != null)
		{
			if (parentTransform.gameObject == possibleParent)
				return true;
			parentTransform = parentTransform.parent;
		}
		return false;
	}

	/// <summary>
	/// Check if the given Transform is a parent of the current GameObject
	/// </summary>
	public static bool IsParentOf(this GameObject gameObject, Transform possibleParent)
	{
		if (gameObject.transform == possibleParent)
			return true;
		Transform parentTransform = gameObject.transform.parent;
		while (parentTransform != null)
		{
			if (parentTransform == possibleParent)
				return true;
			parentTransform = parentTransform.parent;
		}
		return false;
	}
	
	//---------------------------------------------------

	/// <summary>
	/// Find a child GameObject of a given name
	/// </summary>
	public static Transform FindChildRecursive(this Transform transform, string name)
	{
		int childCount = transform.childCount;
		for (int childIndex = 0; childIndex < childCount; ++childIndex)
		{
			Transform child = transform.GetChild(childIndex);
			if (child.name == name)
				return child;
			Transform foundChild = FindChildRecursive(child, name);
			if (foundChild != null)
				return foundChild;
		}
		return null;
	}

	//---------------------------------------------------

	/// <summary>
	/// Set the current colour to a given RGBA value, and return the new value
	/// </summary>
	public static Color FromRGBA(this Color color, byte red, byte green, byte blue, byte alpha)
	{
		color.r = red / 255.0f;
		color.g = green / 255.0f;
		color.b = blue / 255.0f;
		color.a = alpha / 255.0f;
		return color;
	}

	/// <summary>
	/// Set the current colour to a given RGB value, and full alpha, and return the new value
	/// </summary>
	public static Color FromRGB(this Color color, byte red, byte green, byte blue)
	{
		color.r = red / 255.0f;
		color.g = green / 255.0f;
		color.b = blue / 255.0f;
		color.a = 1.0f;
		return color;
	}

	/// <summary>
	/// Get the floating point RGBA distance between this colour and another
	/// </summary>
	public static float Distance(this Color color, Color compare)
	{
		Vector4 offset;
		offset.x = Mathf.Abs(color.r - compare.r);
		offset.y = Mathf.Abs(color.g - compare.g);
		offset.z = Mathf.Abs(color.b - compare.b);
		offset.w = Mathf.Abs(color.a - compare.a);
		return Vector4.Magnitude(offset);
	}

	/// <summary>
	/// Get the floating point RGBA magnitude of this colour
	/// </summary>
	public static float Magnitude(this Color color)
	{
		Vector4 offset;
		offset.x = Mathf.Abs(color.r);
		offset.y = Mathf.Abs(color.g);
		offset.z = Mathf.Abs(color.b);
		offset.w = Mathf.Abs(color.a);
		return Vector4.Magnitude(offset);
	}

	/// <summary>
	/// Convert this color to RGBA32 integer format.
	/// </summary>
	public static int ToInt(this Color color)
	{
		int retVal = 0;
		retVal |= Mathf.RoundToInt(color.r * 255.0f) << 24;
		retVal |= Mathf.RoundToInt(color.g * 255.0f) << 16;
		retVal |= Mathf.RoundToInt(color.b * 255.0f) << 8;
		retVal |= Mathf.RoundToInt(color.a * 255.0f);
		return retVal;
	}

	/// <summary>
	/// Read color from the specified RGBA32 integer.
	/// </summary>
	public static void FromInt(this Color color, int val)
	{
		float inv = 1.0f / 255.0f;
		color.r = inv * ((val >> 24) & 0xFF);
		color.g = inv * ((val >> 16) & 0xFF);
		color.b = inv * ((val >> 8) & 0xFF);
		color.a = inv * (val & 0xFF);
	}

	//---------------------------------------------------

	/// <summary>
	/// Get the absolute Y angle between two vectors
	/// </summary>
	public static float AbsoluteYAngle(this Vector3 vector, Vector3 target)
	{
		/// Get the Y angle difference between these vectors, in degrees
		float angleThis = Mathf.Atan2(vector.z, vector.x);
		float angleTarget = Mathf.Atan2(target.z, target.x);
		float angleDiff = (angleThis - angleTarget) * Mathf.Rad2Deg;

		/// Return this angle
		return angleDiff;
	}

	/// <summary>
	/// Get the current vector rotated around an axis by an angle (in degrees)
	/// </summary>
	public static Vector3 GetRotatedAroundAxis(this Vector3 vector, float degrees, Vector3 axis)
	{
		Quaternion rotationQuat = Quaternion.AngleAxis(degrees, axis);
		return rotationQuat * vector;
	}

	/// <summary>
	/// Rotate the current vector around an axis by an angle (in degrees)
	/// </summary>
	public static void RotateAroundAxis(this Vector3 vector, float degrees, Vector3 axis)
	{
		Quaternion rotationQuat = Quaternion.AngleAxis(degrees, axis);
		vector = rotationQuat * vector;
	}

	/// <summary>
	/// Get the current vector rotated around the X axis by an angle (in radians)
	/// </summary>
	public static Vector3 GetRotatedX(this Vector3 vector, float radians)
	{
		float sin = Mathf.Sin(radians);
		float cos = Mathf.Cos(radians);
		return new Vector3(vector.x, (cos * vector.y) - (sin * vector.z), (cos * vector.z) + (sin * vector.y));
	}
   
	/// <summary>
	/// Rotate the current vector around the X axis by an angle (in radians)
	/// </summary>
	public static void RotateX(this Vector3 vector, float radians)
	{
		float sin = Mathf.Sin(radians);
		float cos = Mathf.Cos(radians);
		float ty = vector.y;
		float tz = vector.z;
		vector.y = (cos * ty) - (sin * tz);
		vector.z = (cos * tz) + (sin * ty);
	}
   
	/// <summary>
	/// Get the current vector rotated around the Y axis by an angle (in radians)
	/// </summary>
	public static Vector3 GetRotatedY(this Vector3 vector, float radians)
	{
		float sin = Mathf.Sin(radians);
		float cos = Mathf.Cos(radians);
		return new Vector3((cos * vector.x) + (sin * vector.z), vector.y, (cos * vector.z) - (sin * vector.x));
	}
 
	/// <summary>
	/// Rotate the current vector around the Y axis by an angle (in radians)
	/// </summary>
	public static void RotateY(this Vector3 vector, float radians)
	{
		float sin = Mathf.Sin(radians);
		float cos = Mathf.Cos(radians);
		float tx = vector.x;
		float tz = vector.z;
		vector.x = (cos * tx) + (sin * tz);
		vector.z = (cos * tz) - (sin * tx);
	}
 
	/// <summary>
	/// Get the current vector rotated around the Z axis by an angle (in radians)
	/// </summary>
	public static Vector3 GetRotatedZ(this Vector3 vector, float radians)
	{
		float sin = Mathf.Sin(radians);
		float cos = Mathf.Cos(radians);
		return new Vector3((cos * vector.x) - (sin * vector.y), (cos * vector.y) + (sin * vector.x), vector.z);
	}

	/// <summary>
	/// Rotate the current vector around the Z axis by an angle (in radians)
	/// </summary>
	public static void RotateZ(this Vector3 vector, float radians)
	{
		float sin = Mathf.Sin(radians);
		float cos = Mathf.Cos(radians);
		float tx = vector.x;
		float ty = vector.y;
		vector.x = (cos * tx) - (sin * ty);
		vector.y = (cos * ty) + (sin * tx);
	}

	//---------------------------------------------------

	/// <summary>
	/// Read all available bytes from a reader into a big buffer
	/// </summary>
	public static byte[] ReadAllBytes(this BinaryReader reader)
	{
		const int bufferSize = 4096;
		using (var ms = new MemoryStream())
		{
			byte[] buffer = new byte[bufferSize];
			int count;
			while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
				ms.Write(buffer, 0, count);
			return ms.ToArray();
		}
	}
	
	//---------------------------------------------------

	/// <summary>
	/// Encode a texture into a JPEG
	/// </summary>
	public static byte[] EncodeToJPG(this Texture2D texture, float quality)
	{
		/// Encode the image as a JPEG - this blocks until done
		JPEGEncoder encoder = new JPEGEncoder(new JPEGEncoder.BitmapData(texture), quality);

		/// Return the encoded JPEG data
		return encoder.GetBytes();
	}

	/// <summary>
	/// Read the pixels in a render target into a texture
	/// </summary>
	public static Texture2D ToTexture(this RenderTexture renderTarget, Rect uvRect)
	{
		/// Get the render target size, assuming screen if no target given
		int fullWidth = (renderTarget != null) ? renderTarget.width : Screen.width;
		int fullHeight = (renderTarget != null) ? renderTarget.height : Screen.height;

		/// Get the source rectangle to read pixels from
		Rect copyRect = new Rect(
							Mathf.RoundToInt(uvRect.x * fullWidth),
							Mathf.RoundToInt(uvRect.y * fullHeight),
							Mathf.RoundToInt(uvRect.width * fullWidth),
							Mathf.RoundToInt(uvRect.height * fullHeight)
						);
		
		/// Create a new texture of exactly the right size
		Texture2D texture = new Texture2D((int)copyRect.width, (int)copyRect.height, TextureFormat.RGB24, false);
		
		/// Read the render target pixels into the texture
		RenderTexture oldTarget = RenderTexture.active;
		RenderTexture.active = renderTarget;
		texture.ReadPixels(copyRect, 0, 0);
		texture.Apply();
		RenderTexture.active = oldTarget;

		/// Return this texture
		return texture;
	}

	//---------------------------------------------------

	/// <summary>
	/// Get the last N characters from a string. Copes with null strings by returning an empty string
	/// </summary>
	public static string Tail(this string source, int length)
	{
		if (string.IsNullOrEmpty(source))
			return String.Empty;
		return source.Substring(Mathf.Max(0, source.Length - length));
	}
	
	//---------------------------------------------------

	/// <summary>
	/// Get the full path name of this GameObject
	/// </summary>
	public static string GetNameWithPath(this GameObject gameObject)
	{
		Stack<string> list = new Stack<string>();
		Transform t = gameObject.transform;
		while (t != null)
		{
			list.Push(t.name);
			t = t.parent;
		}

		StringBuilder sb = new StringBuilder();
		while (list.Count > 0)
			sb.AppendFormat("/{0}", list.Pop());
		return sb.ToString();
	}

	//---------------------------------------------------

	/// <summary>
	/// Adds an item to a list only if it is not already there
	/// </summary>
	/// <returns>
	/// Returns true if the item was added, false otherwise
	/// </returns>
	public static bool AddExclusive<T>(this List<T> list, T item)
	{
		if (!list.Contains(item))
		{
			list.Add(item);
			return true;
		}
		
		return false;
	}

	//---------------------------------------------------

	/// <summary>
	/// Allow enumerable classes to be zipped together, like zip() does in Python
	/// </summary>
	public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
	{
		if (first == null)
			throw new ArgumentNullException("first");
		if (second == null)
			throw new ArgumentNullException("second");
		if (resultSelector == null)
			throw new ArgumentNullException("resultSelector");
		return ZipIterator(first, second, resultSelector);
	}

	private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
	{
		using (IEnumerator<TFirst> e1 = first.GetEnumerator())
		{
			using (IEnumerator<TSecond> e2 = second.GetEnumerator())
			{
				while (e1.MoveNext() && e2.MoveNext())
					yield return resultSelector(e1.Current, e2.Current);
			}
		}
	}	
	
	//---------------------------------------------------

	/// <summary>
	/// Finds the index of the first item matching an expression in an enumerable.
	/// </summary>
	/// <param name="items">
	/// The enumerable to search.
	/// </param>
	/// <param name="predicate">
	/// The expression to test the items against.
	/// </param>
	/// <returns>
	/// The index of the first matching item, or -1 if no items match.
	/// </returns>
	public static int FindIndexMatching<T>(this IEnumerable<T> items, Func<T, bool> predicate)
	{
		if (items == null)
			throw new ArgumentNullException("items");
		if (predicate == null)
			throw new ArgumentNullException("predicate");
		
		int retVal = 0;
		foreach (var item in items)
		{
			if (predicate(item))
				return retVal;
			retVal++;
		}

		return -1;
	}	
}
