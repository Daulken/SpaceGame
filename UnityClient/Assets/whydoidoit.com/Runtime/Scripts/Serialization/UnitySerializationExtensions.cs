using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Serialization;
using System.Linq;
using System.Reflection;

[Serializer(typeof(Vector3))]
public class SerializeVector3 : SerializerExtensionBase<Vector3>
{
	public override IEnumerable<object> Save (Vector3 target)
	{
		return new object[] { target.x, target.y, target.z};
	}
	
	public override object Load (object[] data, object instance)
	{
#if USE_LOGGING
		RadicalLogger.DeferredLog ("Vector3: {0},{1},{2}", data [0], data [1], data [2]);
#endif
		return new UnitySerializer.DeferredSetter (d => {
			if(!float.IsNaN((float)data[0]) && !float.IsNaN((float)data[1]) &&  !float.IsNaN((float)data[2]))
				return new Vector3 ((float)data [0], (float)data [1], (float)data [2]);
			else
				return Vector3.zero;
		}
		);
	}
}

[Serializer(typeof(Color))]
public class SerializeColor : SerializerExtensionBase<Color>
{
	public override IEnumerable<object> Save (Color target)
	{
		return new object[] { target.r, target.g, target.b, target.a};
	}
	
	public override object Load (object[] data, object instance)
	{
#if USE_LOGGING
		RadicalLogger.DeferredLog ("Vector3: {0},{1},{2}", data [0], data [1], data [2]);
#endif
		return new Color((float)data[0],(float)data[1],(float)data[2],(float)data[3]);
	}
}

[Serializer(typeof(Vector4))]
public class SerializeVector4 : SerializerExtensionBase<Vector4>
{
	public override IEnumerable<object> Save (Vector4 target)
	{
		return new object[] { target.x, target.y, target.z, target.w};
	}
	
	public override object Load (object[] data, object instance)
	{
#if USE_LOGGING
		RadicalLogger.DeferredLog ("Vector3: {0},{1},{2}", data [0], data [1], data [2]);
#endif
			if(!float.IsNaN((float)data[0]) && !float.IsNaN((float)data[1]) &&  !float.IsNaN((float)data[2]) && !float.IsNaN((float)data[3]))
				return new Vector4 ((float)data [0], (float)data [1], (float)data [2], (float)data[3]);
			else
				return Vector4.zero;
	}
}

[Serializer(typeof(Vector2))]
public class SerializeVector2 : SerializerExtensionBase<Vector2>
{
	public override IEnumerable<object> Save (Vector2 target)
	{
		return new object[] { target.x, target.y};
	}
	
	public override object Load (object[] data, object instance)
	{
#if USE_LOGGING
		RadicalLogger.DeferredLog ("Vector3: {0},{1},{2}", data [0], data [1], data [2]);
#endif
			return new Vector2 ((float)data [0], (float)data [1]);
		
	}
}

[Serializer(typeof(Quaternion))]
public class SerializeQuaternion : SerializerExtensionBase<Quaternion>
{
	public override IEnumerable<object> Save (Quaternion target)
	{
		
		return new object[] { target.x, target.y, target.z, target.w};
	}
	
	public override object Load (object[] data, object instance)
	{
		return new UnitySerializer.DeferredSetter (d => new Quaternion ((float)data [0], (float)data [1], (float)data [2], (float)data [3]));
	}
}

[Serializer(typeof(Bounds))]
public class SerializeBounds : SerializerExtensionBase<Bounds>
{
	public override IEnumerable<object> Save (Bounds target)
	{
		return new object[] { target.center.x, target.center.y, target.center.z, target.size.x, target.size.y, target.size.z };
	}

	public override object Load (object[] data, object instance)
	{
		return new Bounds (
				new Vector3 ((float)data [0], (float)data [1], (float)data [2]),
			    new Vector3 ((float)data [3], (float)data [4], (float)data [5]));
	}
}

public class SerializerExtensionBase<T> : ISerializeObjectEx
{
	#region ISerializeObject implementation
	public object[] Serialize (object target)
	{
		return Save ((T)target).ToArray ();
	}

	public object Deserialize (object[] data, object instance)
	{
		return Load (data, instance);
	}
	#endregion
	
	public virtual IEnumerable<object> Save (T target)
	{
		return new object[0];
	}
	
	public virtual object Load (object[] data, object instance)
	{
		return null;
	}
	
	#region ISerializeObjectEx implementation
	public bool CanSerialize (Type targetType, object instance)
	{
		if (instance == null)
			return true;
		return CanBeSerialized (targetType, instance);
	}
	#endregion
	
	public virtual bool CanBeSerialized (Type targetType, object instance)
	{
		return true;
	}
}
