// Copyright (c) 2011 Bob Berkebile (pixelplacment)
// Please direct any bugs/comments/suggestions to http://pixelplacement.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// -----------------------------------------------------------------------------------
// 
// TERMS OF USE - EASING EQUATIONS
// Open source under the BSD License.
// Copyright (c)2001 Robert Penner
// All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// Neither the name of the author nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// -----------------------------------------------------------------------------------
// 
// This has further been rewritten at a fundamental level by Chris Fry (2014) of Radiant Worlds.
// The basic concepts and behaviours are still the same, hence keeping the same name and licenses.


// Disable this if your project does not use physics, in order to decrease the cost of iTween on mobile by ~20%
//#define ITWEEN_AFFECTS_PHYSICS
// Disable this if you don't have NGUI
//#define ITWEEN_SUPPORTS_NGUI

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


[AddComponentMenu("")]
public class iTween : MonoBehaviour
{
	#region Types

	/// <summary>
	/// The type of easing to use based on Robert Penner's open source easing equations (http://www.robertpenner.com/easing_terms_of_use.html).
	/// </summary>
	public enum EaseType
	{
		easeInQuad,
		easeOutQuad,
		easeInOutQuad,
		easeInCubic,
		easeOutCubic,
		easeInOutCubic,
		easeInQuart,
		easeOutQuart,
		easeInOutQuart,
		easeInQuint,
		easeOutQuint,
		easeInOutQuint,
		easeInSine,
		easeOutSine,
		easeInOutSine,
		easeInExpo,
		easeOutExpo,
		easeInOutExpo,
		easeInCirc,
		easeOutCirc,
		easeInOutCirc,
		linear,
		spring,
		easeInBounce,
		easeOutBounce,
		easeInOutBounce,
		easeInBack,
		easeOutBack,
		easeInOutBack,
		easeInElastic,
		easeOutElastic,
		easeInOutElastic,
		punch,
	}
	
	/// <summary>
	/// The type of easing to use based on Robert Penner's open source easing equations (http://www.robertpenner.com/easing_terms_of_use.html).
	/// </summary>
	public enum ShakeType
	{
		shakeRandom,
		shakeSine,
	}
	
	/// <summary>
	/// The type of loop (if any) to use.  
	/// </summary>
	public enum LoopType
	{
		/// <summary>
		/// Do not loop.
		/// </summary>
		none,
		/// <summary>
		/// Rewind and replay.
		/// </summary>
		loop,
		/// <summary>
		/// Ping pong the animation back and forth.
		/// </summary>
		pingPong,
	}

	/// <summary>
	/// The argument data type passed around iTween
	/// </summary>
	public class ArgumentData
	{
		private static Type ms_typeofString = typeof(string);
		private static Type ms_typeofDouble = typeof(Double);
		private static Type ms_typeofSingle = typeof(Single);
		private static Type ms_typeofInt32 = typeof(Int32);
		
		private Dictionary<string, object> m_objectValues = new Dictionary<string, object>();

		/// <summary>
		/// Creates an ArgumentData from a paired set of parameters
		/// </summary>
		public ArgumentData(params object[] args)
		{
			// Validate arguments
			if ((args.Length % 2) != 0)
			{
				Debug.LogError("iTween Error: ArgumentData requires an even number of arguments!"); 
				return;
			}
			for (int i = 0; i < (args.Length - 1); i += 2)
			{
				if (args[i].GetType() != ms_typeofString)
				{
					Debug.LogError("iTween Error: Hash requires the first of each argument pair to be a string!"); 
					return;
				}
			}
			
			for (int i = 0; i < (args.Length - 1); i += 2)
			{
				string key = (string)args[i];
				Type valueType = args[i + 1].GetType();

				if (valueType == ms_typeofDouble)
					m_objectValues[key] = (float)((double)args[i + 1]);
				else
					m_objectValues[key] = args[i + 1];
			}
		}
		
		/// <summary>
		/// Sets a data value
		/// </summary>
		public void Set(string name, object newValue)
		{
			m_objectValues[name] = newValue;
		}
		
		/// <summary>
		/// Gets a data value. Causes an exception if not found
		/// </summary>
		public object Get(string name)
		{
			return m_objectValues[name];
		}

		/// <summary>
		/// Gets a data value cast to a type. Causes an exception if not found, or the type is incompatible
		/// </summary>
		public T Get<T>(string name)
		{
			object dataValue = m_objectValues[name];
			return (T)dataValue;
		}

		/// <summary>
		/// Gets a data value, or default if not found
		/// </summary>
		public T GetWithDefault<T>(string name, T defaultValue)
		{
			object dataValue;
			if (!m_objectValues.TryGetValue(name, out dataValue))
				return defaultValue;

			// Check that the type is the same
			Type dataType = dataValue.GetType();
			if (typeof(T) != dataType)
			{
				// Allow casting of ints to floats
				if ((typeof(T) == ms_typeofSingle) && (dataType == ms_typeofInt32))
					return (T)dataValue;
				return defaultValue;
			}
			
			return (T)dataValue;
		}

		/// <summary>
		/// Gets an enum data value, or default if not found. Allows enum to be specified by string as well as type
		/// </summary>
		public T GetEnum<T>(string name, T defaultValue) where T : struct, IConvertible
		{
			// If the argument is invalid, return the default value
			if (string.IsNullOrEmpty(name))
				return defaultValue;

			// Lookup the argument data
			object dataValue;
			if (!m_objectValues.TryGetValue(name, out dataValue))
				return defaultValue;
			
			// If the type is specified directly (preferred method), simply cast and return
			if (dataValue.GetType() == typeof(T))
				return (T)dataValue;
	
			// Try to parse the enum out by string (for Javascript compatibility)
			try
			{
				T value = (T)Enum.Parse(typeof(T), (string)dataValue, true);
				return value;
			}
			catch
			{
				Debug.LogWarning("iTween Warning: Cannot parse argument \"" + dataValue.ToString() + "\" to type \"" + typeof(T).ToString() + "\"! Default will be used.");
				return defaultValue;
			}
		}
		
		/// <summary>
		/// Remove a data value if it exists, regardless of type
		/// </summary>
		public bool Remove(string name)
		{
			return m_objectValues.Remove(name);
		}
		
		/// <summary>
		/// Query if a data value exists, regardless of type
		/// </summary>
		public bool Contains(string name)
		{
			return m_objectValues.ContainsKey(name);
		}
		
		/// <summary>
		/// Query the type of a data value
		/// </summary>
		public Type GetType(string name)
		{
			object dataValue;
			if (!m_objectValues.TryGetValue(name, out dataValue))
				return null;
			return dataValue.GetType();
		}

		/// <summary>
		/// Query whether the given argument data is the same as this one
		/// </summary>
		public bool IsEqual(ArgumentData compareData)
		{
			// Go through all object arguments. Cannot just do a simple compare, because "includechildren" needs to be ignored :/
			foreach (KeyValuePair<string, object> kvp in m_objectValues)
			{
				if (kvp.Key == "includechildren")
					continue;
				if (!compareData.m_objectValues.ContainsKey(kvp.Key))
					return false;
				if (!compareData.m_objectValues[kvp.Key].Equals(kvp.Value))
					return false;
			}

			// Now need to go through compare object arguments and do an opposing check. Don't need to do
			// a value comparison because the above loop checked values for all values that we have. Any
			// extra value in compareData will fail this loop anyway, so won't need checking.
			foreach (KeyValuePair<string, object> kvp in compareData.m_objectValues)
			{
				if (kvp.Key == "includechildren")
					continue;
				if (!m_objectValues.ContainsKey(kvp.Key))
					return false;
			}

			// Must be identical
			return true;
		}
	}

	#endregion

	#region Private Variables
	
	private delegate float EasingFunction(float start, float end, float value);
	private delegate void ApplyTween();
	private delegate void CallUpdate();

	// Private members initialized from arguments
	private ArgumentData m_argData;
	private UInt32 m_uniqueID;
	private string m_method;
	private string m_type;
	private bool m_isChild;
    private bool m_useRealTime;
	private float m_startDelay;
	private float m_duration;
	private LoopType m_loopType;
	private int m_loopCount;
	private List<string> m_tagList = null;
	private EaseType m_easeType;
	private ShakeType m_shakeType;
	private float m_shakeFrequency;
	private EasingFunction m_easingFunction;
	private ApplyTween m_applyTweenFunction;
	private CallUpdate m_callUpdateFunction;
	private string m_namedMaterialColourConstant;
	private bool m_allowOnDisabled;
	private List<iTween> m_sameTargetTweens;

	// Static private working state
	private static ArgumentData ms_argDataToUse = null;
	private static List<iTween> ms_tweens = new List<iTween>(100);
	private static Dictionary<GameObject, List<iTween>> ms_tweensPerObject = new Dictionary<GameObject, List<iTween>>(80);
	private static bool ms_isChild = false;
	private static UInt32 ms_nextID = 0;
	private static Type ms_typeofTransform = typeof(Transform);
	private static Type ms_typeofVector3 = typeof(Vector3);
	private static Type ms_typeofVector2 = typeof(Vector2);
	private static Type ms_typeofRect = typeof(Rect);
	private static Type ms_typeofSingle = typeof(Single);
	private static Type ms_typeofColor = typeof(Color);
	private static Type ms_typeofAction = typeof(Action);
	private static Type ms_typeofActionVector3 = typeof(Action<Vector3>);
	private static Type ms_typeofActionVector2 = typeof(Action<Vector2>);
	private static Type ms_typeofActionRect = typeof(Action<Rect>);
	private static Type ms_typeofActionSingle = typeof(Action<Single>);
	private static Type ms_typeofActionColor = typeof(Action<Color>);

	// Private working state
	private bool[] m_validColorChannels;
    private float m_lastRealTime;
 	private float m_runningTime;
	private float m_percentage;
	private float m_delayStartTime;
	private bool m_isLocalSpace;
	private bool m_hasLooped;
	private bool m_isReversed;
	private bool m_isPaused;
	private bool m_isRunning;
	private Vector3[] m_workingVector3State;
	private Vector2[] m_workingVector2State;
	private Color[,] m_workingColourState;
	private int m_noofColoursInState;
	private float[] m_workingFloatState;
	private Rect[] m_workingRectState;
#if ITWEEN_AFFECTS_PHYSICS
	private Rigidbody m_rigidBody = null;
#endif
	private Transform m_transform = null;
#if ITWEEN_SUPPORTS_NGUI
	private UIWidget m_uiWidget = null;
#endif
	private Material[] m_rendererMaterials;
	private Light m_light = null;
	private Material m_guiTextMaterial = null;
	private GUITexture m_guiTexture = null;
	private float m_pauseTimeSeconds = -1.0f;

	#endregion
	
	#region Defaults
	
	/// <summary>
	/// A collection of baseline presets that iTween needs and utilizes if certain parameters are not provided. 
	/// </summary>
	public static class Defaults
	{
		//general defaults:
		public static float m_duration = 1.0f;
		public static float m_startDelay = 0.0f;
        public static bool m_useRealTime = false;
		public static bool m_allowOnDisabled = false;

		public static bool m_isLocalSpace = false;
		
		public static EaseType m_easeType = EaseType.linear;
		public static ShakeType m_shakeType = ShakeType.shakeRandom;
		public static float m_shakeFrequency = 1.0f;

		public static LoopType m_loopType = LoopType.none;
		public static int m_loopCount = -1;

		public static string m_materialColourConstant = "_Color";
	}
	
	#endregion
	
	#region Static Creation

	// Add a new iTween to the given GameObject, with a behaviour defined by the given arguments
	private static bool Launch(GameObject target, ArgumentData args, bool isChild)
	{
		// If there isn't a target, do nothing
		if (target == null)
		{
			Debug.LogError("iTween Error: No target specified!");
			return false;
		}

		// If the target isn't active, then do nothing, unless there is an option to do otherwise!!
		if (!target.activeInHierarchy && !args.GetWithDefault<bool>("allowondisabled", false))
			return false;

		// Check that any present OnPercent callback is valid
		if (args.Contains("onpercent") && !isChild)
		{
			// If the callback argument is not a delegate, taking a float
			if (args.GetType("onpercent") != ms_typeofActionSingle)
			{
				// Callback is bad. Inform the user, and delete the callback so it doesn't process it all of the time
				Debug.LogError("iTween Error: OnPercent method references must be passed as an Action<float>!. Removing callback");
				args.Remove("onpercent");
			}
		}

		// Check that any present OnComplete callback is valid
		if (args.Contains("oncomplete") && !isChild)
		{
			// If the callback argument is not a delegate, taking no parameter
			if (args.GetType("oncomplete") != ms_typeofAction)
			{
				// Callback is bad. Inform the user, and delete the callback so it doesn't process it all of the time
				Debug.LogError("iTween Error: OnComplete method references must be passed as an Action!. Removing callback");
				args.Remove("oncomplete");
			}
		}

		// Set this argument as the one to use in the new iTween
		ms_argDataToUse = args;
		ms_isChild = isChild;

		// Add a new iTween to the given target
		target.AddComponent<iTween>();

		// Reset the hash to use
		ms_argDataToUse = null;
		ms_isChild = false;

		// Launched successfully
		return true;
	}		

	/// <summary>
	/// Returns a value to an 'oncallback' method interpolated between the supplied 'from' and 'to' values for application as desired.  Requires an 'onupdate' callback that accepts the same type as the supplied 'from' and 'to' properties.
	/// </summary>
	/// <param name="from">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> or <see cref="Vector3"/> or <see cref="Vector2"/> or <see cref="Color"/> or <see cref="Rect"/> for the starting value.
	/// </param> 
	/// <param name="to">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> or <see cref="Vector3"/> or <see cref="Vector2"/> or <see cref="Color"/> or <see cref="Rect"/> for the ending value.
	/// </param> 
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="speed">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of duration to allow animation based on speed (only works with Vector2, Vector3, and Floats)
	/// </param>	
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="easetype">
	/// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
	/// </param>   
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onupdate"> 
	/// A <see cref="Action<T>"/> for the delegate to launch on every step of the tween, with T being the type of value.
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool ValueTo(GameObject target, ArgumentData args)
	{
		// Check that we were given necessary arguments
		if (!args.Contains("onupdate") || !args.Contains("from") || !args.Contains("to"))
		{
			Debug.LogError("iTween Error: ValueTo() requires an 'onupdate' callback function and a 'from' and 'to' property.  The supplied 'onupdate' callback must accept a single argument that is the same type as the supplied 'from' and 'to' properties!");
			return false;
		}

		// Set up tween type and method depending on 'from' type
		args.Set("type", "value");
		Type fromType = args.GetType("from");
		if (fromType == ms_typeofVector2)
			args.Set("method", "vector2");
		else if (fromType == ms_typeofVector3)
			args.Set("method", "vector3");
		else if (fromType == ms_typeofRect)
			args.Set("method", "rect");
		else if (fromType == ms_typeofSingle)
			args.Set("method", "float");
		else if (fromType == ms_typeofColor)
			args.Set("method", "color");
		else
		{
			Debug.LogError("iTween Error: ValueTo() only works with interpolating Vector3s, Vector2s, Singles, Rects and Colors!");
			return false;
		}

		// Create a tween with these arguments
		return Launch(target, args, false);
	}

	/// <summary>
	/// Changes a GameObject's color values instantly then returns them to the provided properties over time with MINIMUM customization options.  If a GUIText or GUITexture component is attached, it will become the target of the animation.
	/// </summary>
	/// <param name="color">
	/// A <see cref="Color"/> to change the GameObject's color to.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool ColorFrom(GameObject target, Color color, float duration)
	{
		return ColorFrom(target, new ArgumentData("color", color, "duration", duration));
	}
	
	/// <summary>
	/// Changes a GameObject's color values instantly then returns them to the provided properties over time with FULL customization options.  If a GUIText or GUITexture component is attached, it will become the target of the animation.
	/// </summary>
	/// <param name="color">
	/// A <see cref="Color"/> to change the GameObject's color to.
	/// </param>
	/// <param name="r">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color red.
	/// </param>
	/// <param name="g">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
	/// </param>
	/// <param name="b">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
	/// </param>
	/// <param name="a">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the alpha.
	/// </param> 
	/// <param name="materialColourConstant">
	/// A <see cref="string"/> or <see cref="System.String"/> for the individual setting of the alpha.
	/// </param> 
	/// <param name="includechildren">
	/// A <see cref="System.Boolean"/> for whether or not to include children of this GameObject. True by default.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="easetype">
	/// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
	/// </param>   
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool ColorFrom(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "color");
		args.Set("method", "from");
		
		// Create a tween with these arguments
		if (!Launch(target, args, false))
			return false;
		
		// Propagate the tween to children if requested
		bool success = true;
		if ((target != null) && args.GetWithDefault<bool>("includechildren", false))
		{
			Transform[] children = target.GetComponentsInChildren<Transform>(true);
			for (int childIndex = 0; childIndex < children.Length; ++childIndex)
			{
				// Unity includes the parent as well, so ignore this
				if (children[childIndex].gameObject == target)
					continue;

				// Create a child tween with these arguments
				if (!Launch(children[childIndex].gameObject, args, true))
					success = false;
			}
		}
		return success;
	}
	
	/// <summary>
	/// Changes a GameObject's color values over time with MINIMUM customization options.  If a GUIText or GUITexture component is attached, they will become the target of the animation.
	/// </summary>
	/// <param name="color">
	/// A <see cref="Color"/> to change the GameObject's color to.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool ColorTo(GameObject target, Color color, float duration)
	{
		return ColorTo(target, new ArgumentData("color", color, "duration", duration));
	}
	
	/// <summary>
	/// Changes a GameObject's color values over time with FULL customization options.  If a GUIText or GUITexture component is attached, they will become the target of the animation.
	/// </summary>
	/// <param name="color">
	/// A <see cref="Color"/> to change the GameObject's color to.
	/// </param>
	/// <param name="r">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color red.
	/// </param>
	/// <param name="g">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
	/// </param>
	/// <param name="b">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
	/// </param>
	/// <param name="a">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the alpha.
	/// </param> 
	/// <param name="materialColourConstant">
	/// A <see cref="string"/> or <see cref="System.String"/> for the individual setting of the alpha.
	/// </param> 
	/// <param name="includechildren">
	/// A <see cref="System.Boolean"/> for whether or not to include children of this GameObject. True by default.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="easetype">
	/// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
	/// </param>   
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool ColorTo(GameObject target, ArgumentData args)
	{	
		// Set up tween type and method
		args.Set("type", "color");
		args.Set("method", "to");
		
		// Create a tween with these arguments
		if (!Launch(target, args, false))
			return false;
		
		// Propagate the tween to children if requested
		bool success = true;
		if ((target != null) && args.GetWithDefault<bool>("includechildren", false))
		{
			Transform[] children = target.GetComponentsInChildren<Transform>(true);
			for (int childIndex = 0; childIndex < children.Length; ++childIndex)
			{
				// Unity includes the parent as well, so ignore this
				if (children[childIndex].gameObject == target)
					continue;

				// Create a child tween with these arguments
				if (!Launch(children[childIndex].gameObject, args, true))
					success = false;
			}
		}
		return success;
	}	

	/// <summary>
	/// Changes a GameObject's position over time to a supplied destination with MINIMUM customization options.
	/// </summary>
	/// <param name="position">
	/// A <see cref="Vector3"/> for the destination Vector3.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool MoveTo(GameObject target, Vector3 position, float duration)
	{
		return MoveTo(target, new ArgumentData("position", position, "duration", duration));
	}	
		
	/// <summary>
	/// Changes a GameObject's position over time to a supplied destination with FULL customization options.
	/// </summary>
	/// <param name="position">
	/// A <see cref="Transform"/> or <see cref="Vector3"/> for a point in space the GameObject will animate to.
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
	/// </param>
	/// <param name="islocal">
	/// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="speed">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of duration to allow animation based on speed
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="easetype">
	/// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
	/// </param>   
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool MoveTo(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "move");
		args.Set("method", "to");

		// Create a tween with these arguments
		return Launch(target, args, false);
	}
		
	/// <summary>
	/// Instantly changes a GameObject's position to a supplied destination then returns it to it's starting position over time with MINIMUM customization options.
	/// </summary>
	/// <param name="position">
	/// A <see cref="Vector3"/> for the destination Vector3.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool MoveFrom(GameObject target, Vector3 position, float duration)
	{
		return MoveFrom(target, new ArgumentData("position", position, "duration", duration));
	}		
	
	/// <summary>
	/// Instantly changes a GameObject's position to a supplied destination then returns it to it's starting position over time with FULL customization options.
	/// </summary>
	/// <param name="position">
	/// A <see cref="Transform"/> or <see cref="Vector3"/> for a point in space the GameObject will animate to.
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
	/// </param>
	/// <param name="islocal">
	/// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="speed">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of duration to allow animation based on speed
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="easetype">
	/// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
	/// </param>   
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool MoveFrom(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "move");
		args.Set("method", "from");

		// Create a tween with these arguments
		return Launch(target, args, false);
	}
		
	/// <summary>
	/// Adds the supplied coordinates to a GameObject's postion with MINIMUM customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the amount of change in position to move the GameObject.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool MoveBy(GameObject target, Vector3 amount, float duration)
	{
		return MoveBy(target, new ArgumentData("amount", amount, "duration", duration));
	}
	
	/// <summary>
	/// Adds the supplied coordinates to a GameObject's position with FULL customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the amount of change in position to move the GameObject.
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
	/// </param>
	/// <param name="islocal">
	/// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="speed">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of duration to allow animation based on speed
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="easetype">
	/// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
	/// </param>   
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool MoveBy(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "move");
		args.Set("method", "by");

		// Create a tween with these arguments
		return Launch(target, args, false);
	}
	
	/// <summary>
	/// Changes a GameObject's scale over time with MINIMUM customization options.
	/// </summary>
	/// <param name="scale">
	/// A <see cref="Vector3"/> for the final scale.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool ScaleTo(GameObject target, Vector3 scale, float duration)
	{
		return ScaleTo(target, new ArgumentData("scale", scale, "duration", duration));
	}
	
	/// <summary>
	/// Changes a GameObject's scale over time with FULL customization options.
	/// </summary>
	/// <param name="scale">
	/// A <see cref="Transform"/> or <see cref="Vector3"/> for the final scale.
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="speed">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of duration to allow animation based on speed
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="easetype">
	/// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
	/// </param>   
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool ScaleTo(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "scale");
		args.Set("method", "to");

		// Create a tween with these arguments
		return Launch(target, args, false);
	}
	
	/// <summary>
	/// Instantly changes a GameObject's scale then returns it to it's starting scale over time with MINIMUM customization options.
	/// </summary>
	/// <param name="scale">
	/// A <see cref="Vector3"/> for the final scale.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool ScaleFrom(GameObject target, Vector3 scale, float duration)
	{
		return ScaleFrom(target, new ArgumentData("scale", scale, "duration", duration));
	}
	
	/// <summary>
	/// Instantly changes a GameObject's scale then returns it to it's starting scale over time with FULL customization options.
	/// </summary>
	/// <param name="scale">
	/// A <see cref="Transform"/> or <see cref="Vector3"/> for the final scale.
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="speed">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of duration to allow animation based on speed
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="easetype">
	/// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
	/// </param>   
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool ScaleFrom(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "scale");
		args.Set("method", "from");

		// Create a tween with these arguments
		return Launch(target, args, false);
	}
	
	/// <summary>
	/// Adds to a GameObject's scale over time with FULL customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the amount of scale to be added to the GameObject's current scale.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool ScaleAdd(GameObject target, Vector3 amount, float duration)
	{
		return ScaleAdd(target, new ArgumentData("amount", amount, "duration", duration));
	}
	
	/// <summary>
	/// Adds to a GameObject's scale over time with FULL customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the amount to be added to the GameObject's current scale.
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="speed">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of duration to allow animation based on speed
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="easetype">
	/// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
	/// </param>   
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool ScaleAdd(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "scale");
		args.Set("method", "add");

		// Create a tween with these arguments
		return Launch(target, args, false);
	}
	
	/// <summary>
	/// Multiplies a GameObject's scale over time with MINIMUM customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the amount of scale to be multiplied by the GameObject's current scale.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool ScaleBy(GameObject target, Vector3 amount, float duration)
	{
		return ScaleBy(target, new ArgumentData("amount", amount, "duration", duration));
	}
	
	/// <summary>
	/// Multiplies a GameObject's scale over time with FULL customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the amount to be multiplied to the GameObject's current scale.
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="speed">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of duration to allow animation based on speed
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="easetype">
	/// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
	/// </param>   
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool ScaleBy(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "scale");
		args.Set("method", "by");

		// Create a tween with these arguments
		return Launch(target, args, false);
	}
	
	/// <summary>
	/// Rotates a GameObject to the supplied Euler angles in degrees over time with MINIMUM customization options.
	/// </summary>
	/// <param name="rotation">
	/// A <see cref="Vector3"/> for the target Euler angles in degrees to rotate to.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool RotateTo(GameObject target, Vector3 rotation, float duration)
	{
		return RotateTo(target, new ArgumentData("rotation", rotation, "duration", duration));
	}
	
	/// <summary>
	/// Rotates a GameObject to the supplied Euler angles in degrees over time with FULL customization options.
	/// </summary>
	/// <param name="rotation">
	/// A <see cref="Transform"/> or <see cref="Vector3"/> for the target Euler angles in degrees to rotate to.
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
	/// </param>
	/// <param name="islocal">
	/// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="speed">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of duration to allow animation based on speed
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="easetype">
	/// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
	/// </param>   
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool RotateTo(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "rotate");
		args.Set("method", "to");

		// Create a tween with these arguments
		return Launch(target, args, false);
	}	
	
	/// <summary>
	/// Instantly changes a GameObject's Euler angles in degrees then returns it to it's starting rotation over time (if allowed) with MINIMUM customization options.
	/// </summary>
	/// <param name="rotation">
	/// A <see cref="Vector3"/> for the target Euler angles in degrees to rotate from.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool RotateFrom(GameObject target, Vector3 rotation, float duration)
	{
		return RotateFrom(target, new ArgumentData("rotation", rotation, "duration", duration));
	}
	
	/// <summary>
	/// Instantly changes a GameObject's Euler angles in degrees then returns it to it's starting rotation over time (if allowed) with FULL customization options.
	/// </summary>
	/// <param name="rotation">
	/// A <see cref="Transform"/> or <see cref="Vector3"/> for the target Euler angles in degrees to rotate to.
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
	/// </param>
	/// <param name="islocal">
	/// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="speed">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of duration to allow animation based on speed
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="easetype">
	/// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
	/// </param>   
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool RotateFrom(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "rotate");
		args.Set("method", "from");

		// Create a tween with these arguments
		return Launch(target, args, false);
	}	
	
	/// <summary>
	/// Adds supplied Euler angles in degrees to a GameObject's rotation over time with MINIMUM customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the amount of Euler angles in degrees to add to the current rotation of the GameObject.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool RotateAdd(GameObject target, Vector3 amount, float duration)
	{
		return RotateAdd(target, new ArgumentData("amount", amount, "duration", duration));
	}
	
	/// <summary>
	/// Adds supplied Euler angles in degrees to a GameObject's rotation over time with FULL customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the amount of Euler angles in degrees to add to the current rotation of the GameObject.
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
	/// </param>
	/// <param name="islocal">
	/// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="speed">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of duration to allow animation based on speed
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="easetype">
	/// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
	/// </param>   
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool RotateAdd(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "rotate");
		args.Set("method", "add");

		// Create a tween with these arguments
		return Launch(target, args, false);
	}
	
	/// <summary>
	/// Rotates a GameObject by calculated amount over time with MINIMUM customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the amount to rotate the GameObject.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool RotateBy(GameObject target, Vector3 amount, float duration)
	{
		return RotateBy(target, new ArgumentData("amount", amount, "duration", duration));
	}
	
	/// <summary>
	/// Rotates a GameObject by calculated amount over time with FULL customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the amount to rotate the GameObject.
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
	/// </param>
	/// <param name="islocal">
	/// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="speed">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of duration to allow animation based on speed
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="easetype">
	/// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
	/// </param>   
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool RotateBy(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "rotate");
		args.Set("method", "by");

		// Create a tween with these arguments
		return Launch(target, args, false);
	}		
	
	/// <summary>
	/// Randomly shakes a GameObject's position by a diminishing amount over time with MINIMUM customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the magnitude of shake.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool ShakePosition(GameObject target, Vector3 amount, float duration)
	{
		return ShakePosition(target, new ArgumentData("amount", amount, "duration", duration));
	}
	
	/// <summary>
	/// Randomly shakes a GameObject's position by a diminishing amount over time with FULL customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the magnitude of shake.
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
	/// </param>
	/// <param name="islocal">
	/// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>  
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with shakes)
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool ShakePosition(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "move");
		args.Set("method", "shake");

		// Create a tween with these arguments
		return Launch(target, args, false);
	}		
	
	/// <summary>
	/// Randomly shakes a GameObject's scale by a diminishing amount over time with MINIMUM customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the magnitude of shake.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool ShakeScale(GameObject target, Vector3 amount, float duration)
	{
		return ShakeScale(target, new ArgumentData("amount", amount, "duration", duration));
	}
	
	/// <summary>
	/// Randomly shakes a GameObject's scale by a diminishing amount over time with FULL customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the magnitude of shake.
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with shakes)
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool ShakeScale(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "scale");
		args.Set("method", "shake");

		// Create a tween with these arguments
		return Launch(target, args, false);
	}		
	
	/// <summary>
	/// Randomly shakes a GameObject's rotation by a diminishing amount over time with MINIMUM customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the magnitude of shake.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
	/// </param>
	public static bool ShakeRotation(GameObject target, Vector3 amount, float duration)
	{
		return ShakeRotation(target, new ArgumentData("amount", amount, "duration", duration));
	}
	
	/// <summary>
	/// Randomly shakes a GameObject's rotation by a diminishing amount over time with FULL customization options.
	/// </summary>
	/// <param name="amount">
	/// A <see cref="Vector3"/> for the magnitude of shake.
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
	/// </param>
	/// <param name="islocal">
	/// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
	/// </param>
	/// <param name="duration">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
	/// </param>
	/// <param name="delay">
	/// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
	/// </param>
	/// <param name="looptype">
	/// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with shakes)
	/// </param>
	/// <param name="loopcount">
	/// A <see cref="System.Int"/> for the number of loops to apply once the animation has completed (<0 for infinite loop).
	/// </param>
	/// <param name="onpercent">
	/// A <see cref="Action<float>"/> for the delegate to launch every update while active.
	/// </param>
	/// <param name="oncomplete">
	/// A <see cref="Action"/> for the delegate to launch at the end of the animation.
	/// </param>
	public static bool ShakeRotation(GameObject target, ArgumentData args)
	{
		// Set up tween type and method
		args.Set("type", "rotate");
		args.Set("method", "shake");

		// Create a tween with these arguments
		return Launch(target, args, false);
	}			
	
	#endregion
	
	#region Internal Generation and Application
	
	//call correct set target method and set tween application delegate:
	private void GenerateTargets(bool looped)
	{
		switch (m_type)
		{
		case "value":
			switch (m_method)
			{
			case "rect":
				GenerateRectTargets(looped);
				break;
			case "color":
				GenerateColorTargets(looped);
				break;
			case "vector3":
				GenerateVector3Targets(looped);
				break;
			case "vector2":
				GenerateVector2Targets(looped);
				break;
			case "float":
				GenerateFloatTargets(looped);
				break;
			}
			break;

		case "color":
			switch (m_method)
			{
			case "from":
				GenerateColorFromTargets(looped);
				break;
			case "to":
				GenerateColorToTargets(looped);
				break;
			}
			break;

		case "move":
			switch (m_method)
			{
			case "from":
				GenerateMoveFromTargets(looped);
				break;
			case "to":
				GenerateMoveToTargets(looped);
				break;
			case "by":
				GenerateMoveByTargets(looped);
				break;
			case "shake":
				GenerateMoveShakeTargets(looped);
				break;		
			}
			break;

		case "scale":
			switch (m_method)
			{
			case "from":
				GenerateScaleFromTargets(looped);
				break;
			case "to":
				GenerateScaleToTargets(looped);
				break;
			case "by":
				GenerateScaleByTargets(looped);
				break;
			case "add":
				GenerateScaleAddTargets(looped);
				break;
			case "shake":
				GenerateScaleShakeTargets(looped);
				break;
			}
			break;

		case "rotate":
			switch (m_method)
			{
			case "from":
				GenerateRotateFromTargets(looped);
				break;
			case "to":
				GenerateRotateToTargets(looped);
				break;
			case "add":
				GenerateRotateAddTargets(looped);
				break;
			case "by":
				GenerateRotateByTargets(looped);
				break;				
			case "shake":
				GenerateRotateShakeTargets(looped);
				break;
			}
			break;
		}
	}

	// --------------------------------

	private void GenerateRectTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyRectTargets;
			
			// Values holder [0] from, [1] to, [2] current
			m_workingRectState = new Rect[3];

			// Check that any present OnUpdate callback is valid, and initialise the update delegate
			if (m_argData.Contains("onupdate") && !m_isChild)
			{
				// If the callback argument is not a delegate, taking the correct parameter
				if (m_argData.GetType("onupdate") != ms_typeofActionRect)
				{
					// Callback is bad. Inform the user, and delete the callback so it doesn't process it all of the time
					Debug.LogError("iTween Error: OnUpdate method references must be passed as an Action<Rect>!. Removing callback");
					m_argData.Remove("onupdate");
				}
				else
				{
					m_callUpdateFunction = CallRectUpdate;
				}
			}
		}
		
		// From and to values
		m_workingRectState[0] = m_argData.Get<Rect>("from");
		m_workingRectState[1] = m_argData.Get<Rect>("to");
	}		
	
	private void ApplyRectTargets()
	{
		// Calculate
		m_workingRectState[2].x = m_easingFunction(m_workingRectState[0].x, m_workingRectState[1].x, m_percentage);
		m_workingRectState[2].y = m_easingFunction(m_workingRectState[0].y, m_workingRectState[1].y, m_percentage);
		m_workingRectState[2].width = m_easingFunction(m_workingRectState[0].width, m_workingRectState[1].width, m_percentage);
		m_workingRectState[2].height = m_easingFunction(m_workingRectState[0].height, m_workingRectState[1].height, m_percentage);
	}		

	private void CallRectUpdate()
	{
		m_argData.Get<Action<Rect>>("onupdate").Invoke(m_workingRectState[2]);
	}

	// --------------------------------

	private void GenerateColorTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyColorTargets;
			
			// Values for tween: from[0,0], to[0,1], current[0,2]
			m_workingColourState = new Color[1, 3];

			// Check that any present OnUpdate callback is valid, and initialise the update delegate
			if (m_argData.Contains("onupdate") && !m_isChild)
			{
				// If the callback argument is not a delegate, taking the correct parameter
				if (m_argData.GetType("onupdate") != ms_typeofActionColor)
				{
					// Callback is bad. Inform the user, and delete the callback so it doesn't process it all of the time
					Debug.LogError("iTween Error: OnUpdate method references must be passed as an Action<Color>!. Removing callback");
					m_argData.Remove("onupdate");
				}
				else
				{
					m_callUpdateFunction = CallColorUpdate;
				}
			}
		}

		m_noofColoursInState = 1;
		
		// From and to values
		m_workingColourState[0, 0] = m_argData.Get<Color>("from");
		m_workingColourState[0, 1] = m_argData.Get<Color>("to");

		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			float distance = m_workingColourState[0, 0].Distance(m_workingColourState[0, 1]);
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}
	
	private void ApplyColorTargets()
	{
		// Calculate the current value
		m_workingColourState[0, 2].r = m_easingFunction(m_workingColourState[0, 0].r, m_workingColourState[0, 1].r, m_percentage);
		m_workingColourState[0, 2].g = m_easingFunction(m_workingColourState[0, 0].g, m_workingColourState[0, 1].g, m_percentage);
		m_workingColourState[0, 2].b = m_easingFunction(m_workingColourState[0, 0].b, m_workingColourState[0, 1].b, m_percentage);
		m_workingColourState[0, 2].a = m_easingFunction(m_workingColourState[0, 0].a, m_workingColourState[0, 1].a, m_percentage);
	}	

	private void CallColorUpdate()
	{
		m_argData.Get<Action<Color>>("onupdate").Invoke(m_workingColourState[0, 2]);
	}

	// --------------------------------

	private void GenerateVector3Targets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyVector3Targets;
			
			// Values for tween: from[0], to[1], current[2]
			m_workingVector3State = new Vector3[3];

			// Check that any present OnUpdate callback is valid, and initialise the update delegate
			if (m_argData.Contains("onupdate") && !m_isChild)
			{
				// If the callback argument is not a delegate, taking the correct parameter
				if (m_argData.GetType("onupdate") != ms_typeofActionVector3)
				{
					// Callback is bad. Inform the user, and delete the callback so it doesn't process it all of the time
					Debug.LogError("iTween Error: OnUpdate method references must be passed as an Action<Vector3>!. Removing callback");
					m_argData.Remove("onupdate");
				}
				else
				{
					m_callUpdateFunction = CallVector3Update;
				}
			}
		}
		
		// From and to values
		m_workingVector3State[0] = m_argData.Get<Vector3>("from");
		m_workingVector3State[1] = m_argData.Get<Vector3>("to");
		
		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			float distance = Vector3.Distance(m_workingVector3State[0], m_workingVector3State[1]);
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}
	
	private void ApplyVector3Targets()
	{
		// Calculate the current value
		m_workingVector3State[2].x = m_easingFunction(m_workingVector3State[0].x, m_workingVector3State[1].x, m_percentage);
		m_workingVector3State[2].y = m_easingFunction(m_workingVector3State[0].y, m_workingVector3State[1].y, m_percentage);
		m_workingVector3State[2].z = m_easingFunction(m_workingVector3State[0].z, m_workingVector3State[1].z, m_percentage);
	}

	private void CallVector3Update()
	{
		m_argData.Get<Action<Vector3>>("onupdate").Invoke(m_workingVector3State[2]);
	}
	
	// --------------------------------

	private void GenerateVector2Targets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyVector2Targets;
			
			// Values for tween: from[0], to[1], current[2]
			m_workingVector2State = new Vector2[3];

			// Check that any present OnUpdate callback is valid, and initialise the update delegate
			if (m_argData.Contains("onupdate") && !m_isChild)
			{
				// If the callback argument is not a delegate, taking the correct parameter
				if (m_argData.GetType("onupdate") != ms_typeofActionVector2)
				{
					// Callback is bad. Inform the user, and delete the callback so it doesn't process it all of the time
					Debug.LogError("iTween Error: OnUpdate method references must be passed as an Action<Vector2>!. Removing callback");
					m_argData.Remove("onupdate");
				}
				else
				{
					m_callUpdateFunction = CallVector2Update;
				}
			}
		}
		
		// From and to values
		m_workingVector2State[0] = m_argData.Get<Vector2>("from");
		m_workingVector2State[1] = m_argData.Get<Vector2>("to");
		
		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			Vector3 fromV3 = new Vector3(m_workingVector2State[0].x, m_workingVector2State[0].y, 0.0f);
			Vector3 toV3 = new Vector3(m_workingVector2State[1].x, m_workingVector2State[1].y, 0.0f);
			float distance = Vector3.Distance(fromV3, toV3);
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}
	
	private void ApplyVector2Targets()
	{
		// Calculate the current value
		m_workingVector2State[2].x = m_easingFunction(m_workingVector2State[0].x, m_workingVector2State[1].x, m_percentage);
		m_workingVector2State[2].y = m_easingFunction(m_workingVector2State[0].y, m_workingVector2State[1].y, m_percentage);
	}

	private void CallVector2Update()
	{
		m_argData.Get<Action<Vector2>>("onupdate").Invoke(m_workingVector2State[2]);
	}

	// --------------------------------

	private void GenerateFloatTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyFloatTargets;
			
			// Values for tween: from[0], to[1], current[2]
			m_workingFloatState = new float[3];

			// Check that any present OnUpdate callback is valid, and initialise the update delegate
			if (m_argData.Contains("onupdate") && !m_isChild)
			{
				// If the callback argument is not a delegate, taking the correct parameter
				if (m_argData.GetType("onupdate") != ms_typeofActionSingle)
				{
					// Callback is bad. Inform the user, and delete the callback so it doesn't process it all of the time
					Debug.LogError("iTween Error: OnUpdate method references must be passed as an Action<float>!. Removing callback");
					m_argData.Remove("onupdate");
				}
				else
				{
					m_callUpdateFunction = CallFloatUpdate;
				}
			}
		}
		
		// From and to values
		m_workingFloatState[0] = m_argData.Get<float>("from");
		m_workingFloatState[1] = m_argData.Get<float>("to");
		
		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			float distance = Math.Abs(m_workingFloatState[0] - m_workingFloatState[1]);
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}
	
	private void ApplyFloatTargets()
	{
		// Calculate the current value
		m_workingFloatState[2] = m_easingFunction(m_workingFloatState[0], m_workingFloatState[1], m_percentage);
	}	

	private void CallFloatUpdate()
	{
		m_argData.Get<Action<float>>("onupdate").Invoke(m_workingFloatState[2]);
	}

	// --------------------------------

	private void GenerateColorFromTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyColorToTargets;

			// Default colour channels are invalid
			m_validColorChannels = new bool[4] { false, false, false, false };
		}
		
		// Values for tween: from[0], to[1], current[2]
		// The current state is added here, so we don't have to worry about array size when applying (expensive!)
		
		// From and initial to values
#if ITWEEN_SUPPORTS_NGUI
		if (m_uiWidget != null)
		{
			if (!looped)
				m_workingColourState = new Color[1, 3];
			m_noofColoursInState = 1;
			m_workingColourState[0, 0] = m_uiWidget.color;
			m_workingColourState[0, 1] = m_uiWidget.color;
		}
		else
#endif
		if (m_rendererMaterials != null)
		{
			m_namedMaterialColourConstant = m_argData.GetWithDefault<string>("materialColourConstant", Defaults.m_materialColourConstant);
			if (!looped)
				m_workingColourState = new Color[m_rendererMaterials.Length, 3];
			m_noofColoursInState = m_rendererMaterials.Length;
			for (int i = 0; i < m_noofColoursInState; ++i)
			{
				m_workingColourState[i, 0] = m_rendererMaterials[i].GetColor(m_namedMaterialColourConstant);
				m_workingColourState[i, 1] = m_rendererMaterials[i].GetColor(m_namedMaterialColourConstant);
			}
		}
		else if (m_light != null)
		{
			if (!looped)
				m_workingColourState = new Color[1, 3];
			m_noofColoursInState = 1;
			m_workingColourState[0, 0] = m_workingColourState[0, 1] = m_light.color;	
		}
		else if (m_guiTextMaterial != null)
		{
			if (!looped)
				m_workingColourState = new Color[1, 3];
			m_noofColoursInState = 1;
			m_workingColourState[0, 0] = m_workingColourState[0, 1] = m_guiTextMaterial.color;
		}
		else if (m_guiTexture != null)
		{
			if (!looped)
				m_workingColourState = new Color[1, 3];
			m_noofColoursInState = 1;
			m_workingColourState[0, 0] = m_workingColourState[0, 1] = m_guiTexture.color;
		}
		else
		{
			// Empty placeholder incase the GO is perhaps an empty holder or something similar
			if (!looped)
				m_workingColourState = new Color[1, 3];
			m_noofColoursInState = 0;
		}

		// Update from values, and work out valid colour channels
		if (m_argData.Contains("color"))
		{
			Color dataValue = m_argData.Get<Color>("color");
			for (int i = 0; i < m_noofColoursInState; ++i)
				m_workingColourState[i, 0] = dataValue;
			m_validColorChannels[0] = true;
			m_validColorChannels[1] = true;
			m_validColorChannels[2] = true;
			m_validColorChannels[3] = true;
		}
		else
		{
			if (m_argData.Contains("r"))
			{
				float dataValue = m_argData.Get<float>("r");
				for (int i = 0; i < m_noofColoursInState; ++i)
					m_workingColourState[i, 0].r = dataValue;
				m_validColorChannels[0] = true;
			}
			if (m_argData.Contains("g"))
			{
				float dataValue = m_argData.Get<float>("g");
				for (int i = 0; i < m_noofColoursInState; ++i)
					m_workingColourState[i, 0].g = dataValue;
				m_validColorChannels[1] = true;
			}
			if (m_argData.Contains("b"))
			{
				float dataValue = m_argData.Get<float>("b");
				for (int i = 0; i < m_noofColoursInState; ++i)
					m_workingColourState[i, 0].b = dataValue;
				m_validColorChannels[2] = true;
			}
			if (m_argData.Contains("a"))
			{
				float dataValue = m_argData.Get<float>("a");
				for (int i = 0; i < m_noofColoursInState; ++i)
					m_workingColourState[i, 0].a = dataValue;
				m_validColorChannels[3] = true;
			}
		}

		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			Color offset = Color.black;
			offset.a = 0.0f;
			for (int channelIndex = 0; channelIndex < 3; ++channelIndex)
			{
				if (!m_validColorChannels[channelIndex])
					continue;
				offset[channelIndex] = Mathf.Abs(m_workingColourState[0, 0][channelIndex] - m_workingColourState[0, 1][channelIndex]);
			}
			float distance = offset.Magnitude();
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}

	private void GenerateColorToTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyColorToTargets;

			// Default colour channels are invalid
			m_validColorChannels = new bool[4] { false, false, false, false };
		}
		
		// Values for tween: from[0], to[1], current[2]
		// The current state is added here, so we don't have to worry about array size when applying (expensive!)
		
		// From and initial to values
#if ITWEEN_SUPPORTS_NGUI
		if (m_uiWidget != null)
		{
			if (!looped)
				m_workingColourState = new Color[1, 3];
			m_noofColoursInState = 1;
			m_workingColourState[0, 0] = m_uiWidget.color;
			m_workingColourState[0, 1] = m_uiWidget.color;
		}
		else
#endif
		if (m_rendererMaterials != null)
		{
			m_namedMaterialColourConstant = m_argData.GetWithDefault<string>("materialColourConstant", Defaults.m_materialColourConstant);
			if (!looped)
				m_workingColourState = new Color[m_rendererMaterials.Length, 3];
			m_noofColoursInState = m_rendererMaterials.Length;
			for (int i = 0; i < m_noofColoursInState; ++i)
			{
				m_workingColourState[i, 0] = m_rendererMaterials[i].GetColor(m_namedMaterialColourConstant);
				m_workingColourState[i, 1] = m_rendererMaterials[i].GetColor(m_namedMaterialColourConstant);
			}
		}
		else if (m_light != null)
		{
			if (!looped)
				m_workingColourState = new Color[1, 3];
			m_noofColoursInState = 1;
			m_workingColourState[0, 0] = m_workingColourState[0, 1] = m_light.color;	
		}
		else if (m_guiTextMaterial != null)
		{
			if (!looped)
				m_workingColourState = new Color[1, 3];
			m_noofColoursInState = 1;
			m_workingColourState[0, 0] = m_workingColourState[0, 1] = m_guiTextMaterial.color;
		}
		else if (m_guiTexture != null)
		{
			if (!looped)
				m_workingColourState = new Color[1, 3];
			m_noofColoursInState = 1;
			m_workingColourState[0, 0] = m_workingColourState[0, 1] = m_guiTexture.color;
		}
		else
		{
			// Empty placeholder incase the GO is perhaps an empty holder or something similar
			if (!looped)
				m_workingColourState = new Color[1, 3];
			m_noofColoursInState = 0;
		}

		// Update to values, and work out valid colour channels
		if (m_argData.Contains("color"))
		{
			Color dataValue = m_argData.Get<Color>("color");
			for (int i = 0; i < m_noofColoursInState; ++i)
				m_workingColourState[i, 1] = dataValue;
			m_validColorChannels[0] = true;
			m_validColorChannels[1] = true;
			m_validColorChannels[2] = true;
			m_validColorChannels[3] = true;
		}
		else
		{
			if (m_argData.Contains("r"))
			{
				float dataValue = m_argData.Get<float>("r");
				for (int i = 0; i < m_noofColoursInState; ++i)
					m_workingColourState[i, 1].r = dataValue;
				m_validColorChannels[0] = true;
			}
			if (m_argData.Contains("g"))
			{
				float dataValue = m_argData.Get<float>("g");
				for (int i = 0; i < m_noofColoursInState; ++i)
					m_workingColourState[i, 1].g = dataValue;
				m_validColorChannels[1] = true;
			}
			if (m_argData.Contains("b"))
			{
				float dataValue = m_argData.Get<float>("b");
				for (int i = 0; i < m_noofColoursInState; ++i)
					m_workingColourState[i, 1].b = dataValue;
				m_validColorChannels[2] = true;
			}
			if (m_argData.Contains("a"))
			{
				float dataValue = m_argData.Get<float>("a");
				for (int i = 0; i < m_noofColoursInState; ++i)
					m_workingColourState[i, 1].a = dataValue;
				m_validColorChannels[3] = true;
			}
		}

		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			Color offset = Color.black;
			offset.a = 0.0f;
			for (int channelIndex = 0; channelIndex < 3; ++channelIndex)
			{
				if (!m_validColorChannels[channelIndex])
					continue;
				offset[channelIndex] = Mathf.Abs(m_workingColourState[0, 0][channelIndex] - m_workingColourState[0, 1][channelIndex]);
			}
			float distance = offset.Magnitude();
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}

	private Color ApplyColourWithChannel(Color destColor, Color srcColour)
	{
		Color colour = destColor;
		if (m_validColorChannels[0])
			colour.r = srcColour.r;
		if (m_validColorChannels[1])
			colour.g = srcColour.g;
		if (m_validColorChannels[2])
			colour.b = srcColour.b;
		if (m_validColorChannels[3])
			colour.a = srcColour.a;
		return colour;
	}
	
	private void ApplyColorToTargets()
	{
		// Calculate the current value
		for (int i = 0; i < m_noofColoursInState; ++i)
		{
			m_workingColourState[i, 2].r = m_easingFunction(m_workingColourState[i, 0].r, m_workingColourState[i, 1].r, m_percentage);
			m_workingColourState[i, 2].g = m_easingFunction(m_workingColourState[i, 0].g, m_workingColourState[i, 1].g, m_percentage);
			m_workingColourState[i, 2].b = m_easingFunction(m_workingColourState[i, 0].b, m_workingColourState[i, 1].b, m_percentage);
			m_workingColourState[i, 2].a = m_easingFunction(m_workingColourState[i, 0].a, m_workingColourState[i, 1].a, m_percentage);
		}

		// Apply the current value
#if ITWEEN_SUPPORTS_NGUI
		if (m_uiWidget != null)
		{
			m_uiWidget.color = ApplyColourWithChannel(m_uiWidget.color, m_workingColourState[0, 2]);
		}
		else
#endif
		if (m_rendererMaterials != null)
		{
			for (int i = 0; i < m_noofColoursInState; ++i)
			{
				Color oldColour = m_rendererMaterials[i].GetColor(m_namedMaterialColourConstant);
				Color newColour = ApplyColourWithChannel(oldColour, m_workingColourState[i, 2]);
				if ((oldColour.a != newColour.a) || (oldColour.r != newColour.r) || (oldColour.g != newColour.g) || (oldColour.b != newColour.b))
					m_rendererMaterials[i].SetColor(m_namedMaterialColourConstant, newColour);
			}
		}
		else if (m_light != null)
		{
			m_light.color = ApplyColourWithChannel(m_light.color, m_workingColourState[0, 2]);
		}
		else if (m_guiTextMaterial != null)
		{
			m_guiTextMaterial.color = ApplyColourWithChannel(m_guiTextMaterial.color, m_workingColourState[0, 2]);
		}
		else if (m_guiTexture != null)
		{
			m_guiTexture.color = ApplyColourWithChannel(m_guiTexture.color, m_workingColourState[0, 2]);
		}
	}

	// --------------------------------

	private void GenerateMoveFromTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyMoveToTargets;
		
			// Values for tween: from[0], to[1], current[2]
			m_workingVector3State = new Vector3[3];
		}
		
		// To and initial from values
		if (m_isLocalSpace)
			m_workingVector3State[0] = m_workingVector3State[1] = m_transform.localPosition;				
		else
			m_workingVector3State[0] = m_workingVector3State[1] = m_transform.position;
		
		// Update from values
		if (m_argData.Contains("position"))
		{
			Type dataType = m_argData.GetType("position");
			if (dataType == ms_typeofVector3)
			{
				m_workingVector3State[0] = m_argData.Get<Vector3>("position");
			}
			else if (dataType == ms_typeofTransform)
			{
				Transform trans = m_argData.Get<Transform>("position");
				if (m_isLocalSpace)
					m_workingVector3State[0] = trans.localPosition;			
				else
					m_workingVector3State[0] = trans.position;			
			}
		}
		else
		{
			if (m_argData.Contains("x"))
				m_workingVector3State[0].x = m_argData.Get<float>("x");
			if (m_argData.Contains("y"))
				m_workingVector3State[0].y = m_argData.Get<float>("y");
			if (m_argData.Contains("z"))
				m_workingVector3State[0].z = m_argData.Get<float>("z");
		}
		
		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			float distance = Vector3.Distance(m_workingVector3State[0], m_workingVector3State[1]);
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}

	private void GenerateMoveToTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyMoveToTargets;

			// Values for tween: from[0], to[1], current[2]
			m_workingVector3State = new Vector3[3];
		}
		
		// From and initial to values
		if (m_isLocalSpace)
			m_workingVector3State[0] = m_workingVector3State[1] = m_transform.localPosition;				
		else
			m_workingVector3State[0] = m_workingVector3State[1] = m_transform.position;
		
		// Update to values
		if (m_argData.Contains("position"))
		{
			Type dataType = m_argData.GetType("position");
			if (dataType == ms_typeofVector3)
			{
				m_workingVector3State[1] = m_argData.Get<Vector3>("position");
			}
			else if (dataType == ms_typeofTransform)
			{
				Transform trans = m_argData.Get<Transform>("position");
				if (m_isLocalSpace)
					m_workingVector3State[1] = trans.localPosition;			
				else
					m_workingVector3State[1] = trans.position;			
			}
		}
		else
		{
			if (m_argData.Contains("x"))
				m_workingVector3State[1].x = m_argData.Get<float>("x");
			if (m_argData.Contains("y"))
				m_workingVector3State[1].y = m_argData.Get<float>("y");
			if (m_argData.Contains("z"))
				m_workingVector3State[1].z = m_argData.Get<float>("z");
		}
		
		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			float distance = Vector3.Distance(m_workingVector3State[0], m_workingVector3State[1]);
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}

	private void ApplyMoveToTargets()
	{
#if ITWEEN_AFFECTS_PHYSICS
		// Record current state for physics
		Vector3 preUpdate = m_transform.position;
#endif

		// Calculate the current value
		m_workingVector3State[2].x = m_easingFunction(m_workingVector3State[0].x, m_workingVector3State[1].x, m_percentage);
		m_workingVector3State[2].y = m_easingFunction(m_workingVector3State[0].y, m_workingVector3State[1].y, m_percentage);
		m_workingVector3State[2].z = m_easingFunction(m_workingVector3State[0].z, m_workingVector3State[1].z, m_percentage);
		
		// Apply the current value
		if (m_isLocalSpace)
			m_transform.localPosition = m_workingVector3State[2];
		else
			m_transform.position = m_workingVector3State[2];

#if ITWEEN_AFFECTS_PHYSICS
		// Should this update by done by physics
		if (m_rigidBody != null)
		{
			Vector3 postUpdate = m_transform.position;
			m_transform.position = preUpdate;
			m_rigidBody.MovePosition(postUpdate);
		}
#endif
	}	
	
	// --------------------------------

	private void GenerateMoveByTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyMoveByTargets;
			
			// Values for tween: from[0], to[1], previous value to allow Space utilization[2], current[3]
			m_workingVector3State = new Vector3[4];
		}
		
		// From and initial previous values
		m_workingVector3State[1] = Vector3.zero;
		if (m_isLocalSpace)
			m_workingVector3State[0] = m_workingVector3State[2] = m_transform.localPosition;
		else
			m_workingVector3State[0] = m_workingVector3State[2] = m_transform.position;
				
		// To values
		if (m_argData.Contains("amount"))
		{
			m_workingVector3State[1] = m_argData.Get<Vector3>("amount");
		}
		else
		{
			if (m_argData.Contains("x"))
				m_workingVector3State[1].x = m_argData.Get<float>("x");
			if (m_argData.Contains("y"))
				m_workingVector3State[1].y = m_argData.Get<float>("y");
			if (m_argData.Contains("z"))
				m_workingVector3State[1].z = m_argData.Get<float>("z");
		}	
		
		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			float distance = Vector3.Magnitude(m_workingVector3State[1]);
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}

	private void ApplyMoveByTargets()
	{	
#if ITWEEN_AFFECTS_PHYSICS
		// Record current state for physics
		Vector3 preUpdate = m_transform.position;
#endif

		// Calculate the current value
		m_workingVector3State[3].x = m_workingVector3State[0].x + m_easingFunction(0.0f, m_workingVector3State[1].x, m_percentage);
		m_workingVector3State[3].y = m_workingVector3State[0].y + m_easingFunction(0.0f, m_workingVector3State[1].y, m_percentage);
		m_workingVector3State[3].z = m_workingVector3State[0].z + m_easingFunction(0.0f, m_workingVector3State[1].z, m_percentage);

		// Apply the current value
		if (m_isLocalSpace)
			m_transform.localPosition += m_workingVector3State[3] - m_workingVector3State[2];
		else
			m_transform.position += m_workingVector3State[3] - m_workingVector3State[2];

		// Store the new previous value
		m_workingVector3State[2] = m_workingVector3State[3];
		
#if ITWEEN_AFFECTS_PHYSICS
		// Should this update by done by physics
		if (m_rigidBody != null)
		{
			Vector3 postUpdate = m_transform.position;
			m_transform.position = preUpdate;
			m_rigidBody.MovePosition(postUpdate);
		}
#endif
	}

	// --------------------------------

	private void GenerateMoveShakeTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyMoveShakeTargets;
			
			// Values for tween: from[0], amount[1], current[2]
			m_workingVector3State = new Vector3[3];
		}
		
		// Initial value
		m_workingVector3State[0] = m_transform.position;
		
		// Amount of shake
		if (m_argData.Contains("amount"))
		{
			m_workingVector3State[1] = m_argData.Get<Vector3>("amount");
		}
		else
		{
			if (m_argData.Contains("x"))
				m_workingVector3State[1].x = m_argData.Get<float>("x");
			if (m_argData.Contains("y"))
				m_workingVector3State[1].y = m_argData.Get<float>("y");
			if (m_argData.Contains("z"))
				m_workingVector3State[1].z = m_argData.Get<float>("z");
		}
	}		

	private void ApplyMoveShakeTargets()
	{
#if ITWEEN_AFFECTS_PHYSICS
		// Record current state for physics
		Vector3 preUpdate = m_transform.position;
#endif

		// Set the transform back to initial state
		if (m_isLocalSpace)
			m_transform.localPosition = m_workingVector3State[0];
		else
			m_transform.position = m_workingVector3State[0];
		
		// Calculate the current state
		m_workingVector3State[2].x = m_easingFunction(m_shakeFrequency, m_workingVector3State[1].x, m_percentage);
		m_workingVector3State[2].y = m_easingFunction(m_shakeFrequency, m_workingVector3State[1].y, m_percentage);
		m_workingVector3State[2].z = m_easingFunction(m_shakeFrequency, m_workingVector3State[1].z, m_percentage);

		// Apply the current state
		if (m_isLocalSpace)
			m_transform.localPosition += m_workingVector3State[2];
		else
			m_transform.position += m_workingVector3State[2];
		
#if ITWEEN_AFFECTS_PHYSICS
		// Should this update be done by physics
		if (m_rigidBody != null)
		{
			Vector3 postUpdate = m_transform.position;
			m_transform.position = preUpdate;
			m_rigidBody.MovePosition(postUpdate);
		}
#endif
	}	
	
	// --------------------------------

	private void GenerateScaleFromTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyScaleToTargets;
			
			// Values for tween: from[0], to[1], current[2]
			m_workingVector3State = new Vector3[3];
		}
		
		// To and initial from values
		m_workingVector3State[0] = m_workingVector3State[1] = m_transform.localScale;				

		// Update from values
		if (m_argData.Contains("scale"))
		{
			Type dataType = m_argData.GetType("scale");
			if (dataType == ms_typeofVector3)
			{
				m_workingVector3State[0] = m_argData.Get<Vector3>("scale");
			}
			else if (dataType == ms_typeofTransform)
			{
				m_workingVector3State[0] = m_argData.Get<Transform>("scale").localScale;
			}
		}
		else
		{
			if (m_argData.Contains("x"))
				m_workingVector3State[0].x = m_argData.Get<float>("x");
			if (m_argData.Contains("y"))
				m_workingVector3State[0].y = m_argData.Get<float>("y");
			if (m_argData.Contains("z"))
				m_workingVector3State[0].z = m_argData.Get<float>("z");
		} 
		
		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			float distance = Vector3.Distance(m_workingVector3State[0], m_workingVector3State[1]);
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}

	private void GenerateScaleToTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyScaleToTargets;
			
			// Values for tween: from[0], to[1], current[2]
			m_workingVector3State = new Vector3[3];
		}
		
		// From and initial to values
		m_workingVector3State[0] = m_workingVector3State[1] = m_transform.localScale;				

		// Update to values
		if (m_argData.Contains("scale"))
		{
			Type dataType = m_argData.GetType("scale");
			if (dataType == ms_typeofVector3)
			{
				m_workingVector3State[1] = m_argData.Get<Vector3>("scale");
			}
			else if (dataType == ms_typeofTransform)
			{
				m_workingVector3State[1] = m_argData.Get<Transform>("scale").localScale;
			}
		}
		else
		{
			if (m_argData.Contains("x"))
				m_workingVector3State[1].x = m_argData.Get<float>("x");
			if (m_argData.Contains("y"))
				m_workingVector3State[1].y = m_argData.Get<float>("y");
			if (m_argData.Contains("z"))
				m_workingVector3State[1].z = m_argData.Get<float>("z");
		} 
		
		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			float distance = Vector3.Distance(m_workingVector3State[0], m_workingVector3State[1]);
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}

	private void GenerateScaleByTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyScaleToTargets;
			
			// Values for tween: from[0], to[1], current[2]
			m_workingVector3State = new Vector3[3];
		}
		
		// From and initial to values
		m_workingVector3State[0] = m_workingVector3State[1] = m_transform.localScale;				

		// Update to values
		if (m_argData.Contains("amount"))
		{
			m_workingVector3State[1] = Vector3.Scale(m_workingVector3State[1], m_argData.Get<Vector3>("amount"));
		}
		else
		{
			if (m_argData.Contains("x"))
				m_workingVector3State[1].x *= m_argData.Get<float>("x");
			if (m_argData.Contains("y"))
				m_workingVector3State[1].y *= m_argData.Get<float>("y");
			if (m_argData.Contains("z"))
				m_workingVector3State[1].z *= m_argData.Get<float>("z");
		} 
		
		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			float distance = Vector3.Distance(m_workingVector3State[0], m_workingVector3State[1]);
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}
	
	private void GenerateScaleAddTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyScaleToTargets;
			
			// Values for tween: from[0], to[1], current[2]
			m_workingVector3State = new Vector3[3];
		}
		
		// From and initial to values
		m_workingVector3State[0] = m_workingVector3State[1] = m_transform.localScale;				

		// Update to values
		if (m_argData.Contains("amount"))
		{
			m_workingVector3State[1] += m_argData.Get<Vector3>("amount");
		}
		else
		{
			if (m_argData.Contains("x"))
				m_workingVector3State[1].x += m_argData.Get<float>("x");
			if (m_argData.Contains("y"))
				m_workingVector3State[1].y += m_argData.Get<float>("y");
			if (m_argData.Contains("z"))
				m_workingVector3State[1].z += m_argData.Get<float>("z");
		}

		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			float distance = Vector3.Distance(m_workingVector3State[0], m_workingVector3State[1]);
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}

	private void ApplyScaleToTargets()
	{
		// Calculate the current value
		m_workingVector3State[2].x = m_easingFunction(m_workingVector3State[0].x, m_workingVector3State[1].x, m_percentage);
		m_workingVector3State[2].y = m_easingFunction(m_workingVector3State[0].y, m_workingVector3State[1].y, m_percentage);
		m_workingVector3State[2].z = m_easingFunction(m_workingVector3State[0].z, m_workingVector3State[1].z, m_percentage);
		
		// Apply the current value
		m_transform.localScale = m_workingVector3State[2];	
	}
	
	// --------------------------------

	private void GenerateScaleShakeTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyScaleShakeTargets;
			
			// Values for tween: from[0], amount[1], current[2]
			m_workingVector3State = new Vector3[3];
		}
		
		// Initial value
		m_workingVector3State[0] = m_transform.localScale;
		
		// Amount of shake
		if (m_argData.Contains("amount"))
		{
			m_workingVector3State[1] = m_argData.Get<Vector3>("amount");
		}
		else
		{
			if (m_argData.Contains("x"))
				m_workingVector3State[1].x = m_argData.Get<float>("x");
			if (m_argData.Contains("y"))
				m_workingVector3State[1].y = m_argData.Get<float>("y");
			if (m_argData.Contains("z"))
				m_workingVector3State[1].z = m_argData.Get<float>("z");
		}
	}		
	
	private void ApplyScaleShakeTargets()
	{
		// Set the transform back to initial state
		m_transform.localScale = m_workingVector3State[0];
		
		// Calculate the current state
		m_workingVector3State[2].x = m_easingFunction(m_shakeFrequency, m_workingVector3State[1].x, m_percentage);
		m_workingVector3State[2].y = m_easingFunction(m_shakeFrequency, m_workingVector3State[1].y, m_percentage);
		m_workingVector3State[2].z = m_easingFunction(m_shakeFrequency, m_workingVector3State[1].z, m_percentage);

		// Apply the current state
		m_transform.localScale += m_workingVector3State[2];
	}		
	
	// --------------------------------

	private void GenerateRotateFromTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyRotateToTargets;
			
			// Values for tween: from[0], to[1], current[2]
			m_workingVector3State = new Vector3[3];
		}
		
		// To and initial from values
		if (m_isLocalSpace)
			m_workingVector3State[0] = m_workingVector3State[1] = m_transform.localEulerAngles;				
		else
			m_workingVector3State[0] = m_workingVector3State[1] = m_transform.eulerAngles;
		
		// Update from values
		if (m_argData.Contains("rotation"))
		{
			Type dataType = m_argData.GetType("rotation");
			if (dataType == ms_typeofVector3)
			{
				m_workingVector3State[0] = m_argData.Get<Vector3>("rotation");
			}
			else if (dataType == ms_typeofTransform)
			{
				Transform trans = m_argData.Get<Transform>("rotation");
				if (m_isLocalSpace)
					m_workingVector3State[0] = trans.localEulerAngles;			
				else
					m_workingVector3State[0] = trans.eulerAngles;			
			}
		}
		else
		{
			if (m_argData.Contains("x"))
				m_workingVector3State[0].x = m_argData.Get<float>("x");
			if (m_argData.Contains("y"))
				m_workingVector3State[0].y = m_argData.Get<float>("y");
			if (m_argData.Contains("z"))
				m_workingVector3State[0].z = m_argData.Get<float>("z");
		}

		// Update the to value with the shortest rotational distance
		m_workingVector3State[0].x = EasingFunction_Clerp(m_workingVector3State[1].x, m_workingVector3State[0].x, 1.0f);
		m_workingVector3State[0].y = EasingFunction_Clerp(m_workingVector3State[1].y, m_workingVector3State[0].y, 1.0f);
		m_workingVector3State[0].z = EasingFunction_Clerp(m_workingVector3State[1].z, m_workingVector3State[0].z, 1.0f);
		
		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			float distance = Vector3.Distance(m_workingVector3State[0], m_workingVector3State[1]);
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}

	private void GenerateRotateToTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyRotateToTargets;
			
			// Values for tween: from[0], to[1], current[2]
			m_workingVector3State = new Vector3[3];
		}
		
		// From and initial to values
		if (m_isLocalSpace)
			m_workingVector3State[0] = m_workingVector3State[1] = m_transform.localEulerAngles;				
		else
			m_workingVector3State[0] = m_workingVector3State[1] = m_transform.eulerAngles;
		
		// Update to values
		if (m_argData.Contains("rotation"))
		{
			Type dataType = m_argData.GetType("rotation");
			if (dataType == ms_typeofVector3)
			{
				m_workingVector3State[1] = m_argData.Get<Vector3>("rotation");
			}
			else if (dataType == ms_typeofTransform)
			{
				Transform trans = m_argData.Get<Transform>("rotation");
				if (m_isLocalSpace)
					m_workingVector3State[1] = trans.localEulerAngles;			
				else
					m_workingVector3State[1] = trans.eulerAngles;			
			}
		}
		else
		{
			if (m_argData.Contains("x"))
				m_workingVector3State[1].x = m_argData.Get<float>("x");
			if (m_argData.Contains("y"))
				m_workingVector3State[1].y = m_argData.Get<float>("y");
			if (m_argData.Contains("z"))
				m_workingVector3State[1].z = m_argData.Get<float>("z");
		}

		// Update the to value with the shortest rotational distance
		m_workingVector3State[1].x = EasingFunction_Clerp(m_workingVector3State[0].x, m_workingVector3State[1].x, 1.0f);
		m_workingVector3State[1].y = EasingFunction_Clerp(m_workingVector3State[0].y, m_workingVector3State[1].y, 1.0f);
		m_workingVector3State[1].z = EasingFunction_Clerp(m_workingVector3State[0].z, m_workingVector3State[1].z, 1.0f);
		
		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			float distance = Vector3.Distance(m_workingVector3State[0], m_workingVector3State[1]);
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}

	private void ApplyRotateToTargets()
	{
#if ITWEEN_AFFECTS_PHYSICS
		// Record current state for physics
		Vector3 preUpdate = m_transform.eulerAngles;
#endif

		// Calculate the current value
		m_workingVector3State[2].x = m_easingFunction(m_workingVector3State[0].x, m_workingVector3State[1].x, m_percentage);
		m_workingVector3State[2].y = m_easingFunction(m_workingVector3State[0].y, m_workingVector3State[1].y, m_percentage);
		m_workingVector3State[2].z = m_easingFunction(m_workingVector3State[0].z, m_workingVector3State[1].z, m_percentage);
		
		// Apply the current value
		if (m_isLocalSpace)
			m_transform.localRotation = Quaternion.Euler(m_workingVector3State[2]);
		else
			m_transform.rotation = Quaternion.Euler(m_workingVector3State[2]);

#if ITWEEN_AFFECTS_PHYSICS
		// Should this update be done by physics
		if (m_rigidBody != null)
		{
			Vector3 postUpdate = m_transform.eulerAngles;
			m_transform.eulerAngles = preUpdate;
			m_rigidBody.MoveRotation(Quaternion.Euler(postUpdate));
		}
#endif
	}
	
	// --------------------------------

	private void GenerateRotateAddTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyRotateAddTargets;
			
			// Values for tween: from[0], to[1], previous value to allow Space utilization[2], current[3]
			m_workingVector3State = new Vector3[4];
		}
		
		// From and initial previous values
		if (m_isLocalSpace)
			m_workingVector3State[0] = m_workingVector3State[1] = m_workingVector3State[2] = m_transform.localEulerAngles;
		else
			m_workingVector3State[0] = m_workingVector3State[1] = m_workingVector3State[2] = m_transform.eulerAngles;
		
		// Update to values
		if (m_argData.Contains("amount"))
		{
			m_workingVector3State[1] += m_argData.Get<Vector3>("amount");
		}
		else
		{
			if (m_argData.Contains("x"))
				m_workingVector3State[1].x += m_argData.Get<float>("x");
			if (m_argData.Contains("y"))
				m_workingVector3State[1].y += m_argData.Get<float>("y");
			if (m_argData.Contains("z"))
				m_workingVector3State[1].z += m_argData.Get<float>("z");
		}
		
		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			float distance = Vector3.Distance(m_workingVector3State[0], m_workingVector3State[1]);
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}		
	
	private void GenerateRotateByTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyRotateAddTargets;
			
			// Values for tween: from[0], to[1], previous value to allow Space utilization[2], current[3]
			m_workingVector3State = new Vector3[4];
		}
		
		// From and initial previous values
		if (m_isLocalSpace)
			m_workingVector3State[0] = m_workingVector3State[1] = m_workingVector3State[2] = m_transform.localEulerAngles;
		else
			m_workingVector3State[0] = m_workingVector3State[1] = m_workingVector3State[2] = m_transform.eulerAngles;
		
		// Update to values
		if (m_argData.Contains("amount"))
		{
			m_workingVector3State[1] += m_argData.Get<Vector3>("amount");
		}
		else
		{
			if (m_argData.Contains("x"))
				m_workingVector3State[1].x += m_argData.Get<float>("x");
			if (m_argData.Contains("y"))
				m_workingVector3State[1].y += m_argData.Get<float>("y");
			if (m_argData.Contains("z"))
				m_workingVector3State[1].z += m_argData.Get<float>("z");
		}
		
		// Should speed, rather than duration be used?
		if (m_argData.Contains("speed"))
		{
			float distance = Math.Abs(Vector3.Distance(m_workingVector3State[0], m_workingVector3State[1]));
			m_duration = distance / m_argData.Get<float>("speed");
		}
	}
	
	private void ApplyRotateAddTargets()
	{
#if ITWEEN_AFFECTS_PHYSICS
		// Record current state for physics
		Vector3 preUpdate = m_transform.eulerAngles;
#endif

		// Calculate the current value
		m_workingVector3State[3].x = m_easingFunction(m_workingVector3State[0].x, m_workingVector3State[1].x, m_percentage);
		m_workingVector3State[3].y = m_easingFunction(m_workingVector3State[0].y, m_workingVector3State[1].y, m_percentage);
		m_workingVector3State[3].z = m_easingFunction(m_workingVector3State[0].z, m_workingVector3State[1].z, m_percentage);
		
		// Apply the current value
		m_transform.Rotate(m_workingVector3State[3] - m_workingVector3State[2], m_isLocalSpace ? Space.Self : Space.World);

		// Store the new previous value
		m_workingVector3State[2] = m_workingVector3State[3];	
		
#if ITWEEN_AFFECTS_PHYSICS
		// Should this update be done by physics
		if (m_rigidBody != null)
		{
			Vector3 postUpdate = m_transform.eulerAngles;
			m_transform.eulerAngles = preUpdate;
			m_rigidBody.MoveRotation(Quaternion.Euler(postUpdate));
		}
#endif
	}
	
	// --------------------------------

	private void GenerateRotateShakeTargets(bool looped)
	{
		if (!looped)
		{
			// Set the apply function
			m_applyTweenFunction = ApplyRotateShakeTargets;
			
			// Values for tween: from[0], amount[1], current[2]
			m_workingVector3State = new Vector3[3];
		}
		
		// Initial value
		m_workingVector3State[0] = m_transform.eulerAngles;
		
		// Amount of shake
		if (m_argData.Contains("amount"))
		{
			m_workingVector3State[1] = m_argData.Get<Vector3>("amount");
		}
		else
		{
			if (m_argData.Contains("x"))
				m_workingVector3State[1].x = m_argData.Get<float>("x");
			if (m_argData.Contains("y"))
				m_workingVector3State[1].y = m_argData.Get<float>("y");
			if (m_argData.Contains("z"))
				m_workingVector3State[1].z = m_argData.Get<float>("z");
		}
	}	
	
	private void ApplyRotateShakeTargets()
	{
#if ITWEEN_AFFECTS_PHYSICS
		// Record current state for physics
		Vector3 preUpdate = m_transform.eulerAngles;
#endif

		// Set the transform back to initial state
		m_transform.eulerAngles = m_workingVector3State[0];
		
		// Calculate the current state
		m_workingVector3State[2].x = m_easingFunction(m_shakeFrequency, m_workingVector3State[1].x, m_percentage);
		m_workingVector3State[2].y = m_easingFunction(m_shakeFrequency, m_workingVector3State[1].y, m_percentage);
		m_workingVector3State[2].z = m_easingFunction(m_shakeFrequency, m_workingVector3State[1].z, m_percentage);

		// Apply the current state
		m_transform.Rotate(m_workingVector3State[2], m_isLocalSpace ? Space.Self : Space.World);
		
#if ITWEEN_AFFECTS_PHYSICS
		// Should this update be done by physics
		if (m_rigidBody != null)
		{
			Vector3 postUpdate = m_transform.eulerAngles;
			m_transform.eulerAngles = preUpdate;
			m_rigidBody.MoveRotation(Quaternion.Euler(postUpdate));
		}
#endif
	}
	
	#endregion	
	
	#region Main Functionality

	// Called whenever the application is paused or unpaused due to alt-tab or other switching behaviour
	protected virtual void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			m_pauseTimeSeconds = Time.realtimeSinceStartup;
		}
		else if (m_pauseTimeSeconds >= 0.0f)
		{
			float timePaused = Time.realtimeSinceStartup - m_pauseTimeSeconds;
			m_pauseTimeSeconds = -1.0f;
			if (m_useRealTime)
			{
				m_lastRealTime += timePaused;
				m_delayStartTime += timePaused;
			}
		}
	}
	
	// Delays tween startup by any necessary amount before unpausing and starting the tween
	private IEnumerator TweenDelay()
	{
		// If there is delay remaining, wait for it to finish
		if (m_startDelay > 0.0f)
		{
			if (m_useRealTime)
			{
				m_delayStartTime = Time.realtimeSinceStartup;
				while (Time.realtimeSinceStartup < (m_delayStartTime + m_startDelay))
					yield return null;
			}
			else
			{
				m_delayStartTime = Time.time;
				yield return new WaitForSeconds(m_startDelay);
			}
		}
		
		// Start the tween after the delay
		TweenStart();	
	}	

	// Starts a tween
	private void TweenStart()
	{
        m_lastRealTime = Time.realtimeSinceStartup;

		// We need to generate targets at the start of a loop, or if it is a 'by' tween, since this is a relative change!
		if (!m_hasLooped || ((m_method == "by") && (m_loopType != LoopType.pingPong)))
			GenerateTargets(m_hasLooped);

		// If this is not a loop, check for conflicts with current iTweens.
		// Do this AFTER generating the targets, so we have valid colour channels to check
		if (!m_hasLooped)
		{
			// Loop through all current iTweens on the same target. Use our own cached list, rather than GetComponent
			for (int itemIndex = 0, itemCount = m_sameTargetTweens.Count; itemIndex < itemCount; ++itemIndex)
			{
				// Cache the iTween
				iTween item = m_sameTargetTweens[itemIndex];
	
				// Ignore ourself, as this will always conflict!
				if (item.m_uniqueID == m_uniqueID)
					continue;
	
				// Ignore value-changing tweens, as these don't update the gameobject, and can be on any value.
				// It therefore makes it impossible to tell if a conflict occurs, because it's the OnUpdate
				// call that performs an action when updated, and we can't know what this does.
				if (item.m_type == "value")
					continue;
				
				// If this iTween isn't running yet, or it's of a different type, ignore it as it isn't relevant
				if (!item.m_isRunning || (item.m_type != m_type))
					continue;
	
				// Colour types are special, in that they can alter channels separately, and independantly of method
				if (item.m_type == "color")
				{
					// If the colour channels don't overlap, it cannot be a conflict
					if (!(m_validColorChannels[0] && item.m_validColorChannels[0]) &&
						!(m_validColorChannels[1] && item.m_validColorChannels[1]) &&
						!(m_validColorChannels[2] && item.m_validColorChannels[2]) &&
						!(m_validColorChannels[3] && item.m_validColorChannels[3]))
						continue;
				}
				
				// Any other type of iTween will change the same Transform data if it matches the type
				// (eg, all 'move' types will update position, all 'scale' types will update scale).
				// Therefore, if we got here, the test iTween must be changing the same values as this
				// one, so we want to remove the old item.
				Destroy(item);
			}
		}
		
		// This tween is now running
		m_isRunning = true;
	}
	
	// Delays tween restart by any necessary amount before unpausing and restarting the tween
	private IEnumerator TweenRestart()
	{
		// If there is delay remaining, wait for it to finish
		if (m_startDelay > 0.0f)
		{
			if (m_useRealTime)
			{
				m_delayStartTime = Time.realtimeSinceStartup;
				while (Time.realtimeSinceStartup < (m_delayStartTime + m_startDelay))
					yield return null;
			}
			else
			{
				m_delayStartTime = Time.time;
				yield return new WaitForSeconds(m_startDelay);
			}
		}

		// Set the loop flag
		m_hasLooped = true;
		
		// Start the tween after the delay
		TweenStart();	
	}

	// Updates the tween
	private void TweenUpdate()
	{
		m_applyTweenFunction();
		if (m_callUpdateFunction != null)
			m_callUpdateFunction();
		UpdatePercentage();		
	}

	// Completes a tween
	private void TweenComplete(bool forceDestroy)
	{
		// No longer running
		m_isRunning = false;
		
		// Clamp percentage to 1 or 0 for final run
		m_percentage = m_isReversed ? 0.0f : 1.0f;

		// Apply final value, in case we under- or over- shot.
		// Need to generate targets if we don't have them, since we could be cancelling the tween before a start delay has finished
		if (m_applyTweenFunction == null)
			GenerateTargets(false);
		m_applyTweenFunction();

		// CallBack run for ValueTo since it only calculates and applies in the update callback
		if (m_callUpdateFunction != null)
			m_callUpdateFunction();

		// Loop or destroy the tween as required
		if ((m_loopType == LoopType.none) || (m_loopCount == 0) || forceDestroy)
			Destroy(this);
		else
			TweenLoop();

		// If the OnComplete callback is present, and it's not a cloned child (which would duplicate the callback), call it
		if (!m_isChild && m_argData.Contains("oncomplete"))
			m_argData.Get<Action>("oncomplete").Invoke();
	}

	// Triggers the start of another loop for the tween
	private void TweenLoop()
	{
		switch (m_loopType)
		{
		case LoopType.loop:
			// Rewind to start
			m_runningTime = 0.0f;
			m_percentage = 0.0f;
	        m_lastRealTime = Time.realtimeSinceStartup;

			// Only reset to the beginning if not a 'by' relative loop
			if (m_method != "by")
				m_applyTweenFunction();

			// Restart the tween
			StartCoroutine("TweenRestart");
			break;

		case LoopType.pingPong:
			// Reverse direction of play
			m_isReversed = !m_isReversed;
			m_runningTime = 0.0f;
			m_percentage = m_isReversed ? 1.0f : 0.0f;
	        m_lastRealTime = Time.realtimeSinceStartup;
		
			// Restart the tween
			StartCoroutine("TweenRestart");
			break;
		}

		// Decrease the loop count if not infinite
		if (m_loopCount > 0)
			m_loopCount--;
	}	

	// Awake is called at game startup, regardless of enable state
	protected virtual void Awake()
	{
		// Add ourself to the tween lists. Also cache off the list used for the current target,
		// so we can efficiently search other iTweens on the same target withou using GetComponent
		ms_tweens.Add(this);
		GameObject target = gameObject;
		if (!ms_tweensPerObject.TryGetValue(target, out m_sameTargetTweens))
		{
			m_sameTargetTweens = new List<iTween>(4);
			ms_tweensPerObject[target] = m_sameTargetTweens;
		}
		m_sameTargetTweens.Add(this);
		
		// Get the arguments that this tween uses. This is set by Launch, which creates the iTween component
		m_argData = ms_argDataToUse;

		// Always force an iTween to have a new, unique ID
		m_uniqueID = ms_nextID++;

		// Set whether this is a child
		m_isChild = ms_isChild;

		// Store off the tags if present
		m_tagList = m_argData.GetWithDefault<List<string>>("tags", null);
		
		// Store off the type and method
		m_type = m_argData.Get<string>("type");
		m_method = m_argData.Get<string>("method");

		// Cache off the various used components, for speed
		// Only query these if required, as they are slow
		m_transform = null;
		if ((m_type == "move") || (m_type == "rotate") || (m_type == "scale"))
			m_transform = transform;
#if ITWEEN_AFFECTS_PHYSICS
		m_rigidBody = null;
		if ((m_type == "move") || (m_type == "rotate"))
			m_rigidBody = GetComponent<Rigidbody>();
#endif
#if ITWEEN_SUPPORTS_NGUI
		m_uiWidget = null;
#endif
		m_rendererMaterials = null;
		m_light = null;
		m_guiTextMaterial = null;
		m_guiTexture = null;
		if (m_type == "color")
		{
#if ITWEEN_SUPPORTS_NGUI
			m_uiWidget = GetComponent<UIWidget>();
			if (m_uiWidget == null)
#endif
			{
				Renderer cachedRenderer = GetComponent<Renderer>();
				m_rendererMaterials = (cachedRenderer != null) ? cachedRenderer.materials : null;
				if (m_rendererMaterials == null)
				{
					m_light = GetComponent<Light>();
					if (m_light == null)
					{
						GUIText cachedText = GetComponent<GUIText>();
						m_guiTextMaterial = (cachedText != null) ? cachedText.material : null;
						if (m_guiTextMaterial == null)
						{
							m_guiTexture = GetComponent<GUITexture>();
						}
					}
				}
			}
		}

		// Get timing variables
		m_duration = m_argData.GetWithDefault<float>("duration", Defaults.m_duration);
		m_startDelay = m_argData.GetWithDefault<float>("delay", Defaults.m_startDelay);
		m_useRealTime = m_argData.GetWithDefault<bool>("ignoretimescale", Defaults.m_useRealTime);
		m_allowOnDisabled = m_argData.GetWithDefault<bool>("allowondisabled", Defaults.m_allowOnDisabled);

		// Loop count is special, in order to behave nicely. Count of -1 is infinite. Count of 0 is the same as no loop type.
		// After that, it's the number of EXTRA loops to perform. For ping-ping, it's the number of pings OR pongs to add.
		// For example, a ping-pong loop count of 0 will just ping, and a loop count of 4 will give A->B->A->B->A->B
		m_loopType = m_argData.GetEnum<LoopType>("looptype", Defaults.m_loopType);
		m_loopCount = m_argData.GetWithDefault<int>("loopcount", Defaults.m_loopCount);

		// Work out whether to change local space coordinates, or world space
		m_isLocalSpace = m_argData.GetWithDefault<bool>("islocal", Defaults.m_isLocalSpace);

		// If this is a shake
		if (m_method == "shake")
		{
			// Get the frequency of the shake
			m_shakeFrequency = m_argData.GetWithDefault<float>("frequency", Defaults.m_shakeFrequency);

			// Get the shake type to use, and instantiate the ease equation delegate
			m_shakeType = (m_duration <= 0.0f) ? ShakeType.shakeRandom : m_argData.GetEnum<ShakeType>("shaketype", Defaults.m_shakeType);
			switch (m_shakeType)
			{
			case ShakeType.shakeRandom:
				m_easingFunction = EasingFunction_ShakeRandom;
				break;
			case ShakeType.shakeSine:
				m_easingFunction = EasingFunction_ShakeSine;
				break;
			}
		}
		// Any other method
		else
		{
			// Get the ease type to use, and instantiate the ease equation delegate
			m_easeType = (m_duration <= 0.0f) ? EaseType.linear : m_argData.GetEnum<EaseType>("easetype", Defaults.m_easeType);
			switch (m_easeType)
			{
			case EaseType.easeInQuad:
				m_easingFunction = EasingFunction_EaseInQuad;
				break;
			case EaseType.easeOutQuad:
				m_easingFunction = EasingFunction_EaseOutQuad;
				break;
			case EaseType.easeInOutQuad:
				m_easingFunction = EasingFunction_EaseInOutQuad;
				break;
			case EaseType.easeInCubic:
				m_easingFunction = EasingFunction_EaseInCubic;
				break;
			case EaseType.easeOutCubic:
				m_easingFunction = EasingFunction_EaseOutCubic;
				break;
			case EaseType.easeInOutCubic:
				m_easingFunction = EasingFunction_EaseInOutCubic;
				break;
			case EaseType.easeInQuart:
				m_easingFunction = EasingFunction_EaseInQuart;
				break;
			case EaseType.easeOutQuart:
				m_easingFunction = EasingFunction_EaseOutQuart;
				break;
			case EaseType.easeInOutQuart:
				m_easingFunction = EasingFunction_EaseInOutQuart;
				break;
			case EaseType.easeInQuint:
				m_easingFunction = EasingFunction_EaseInQuint;
				break;
			case EaseType.easeOutQuint:
				m_easingFunction = EasingFunction_EaseOutQuint;
				break;
			case EaseType.easeInOutQuint:
				m_easingFunction = EasingFunction_EaseInOutQuint;
				break;
			case EaseType.easeInSine:
				m_easingFunction = EasingFunction_EaseInSine;
				break;
			case EaseType.easeOutSine:
				m_easingFunction = EasingFunction_EaseOutSine;
				break;
			case EaseType.easeInOutSine:
				m_easingFunction = EasingFunction_EaseInOutSine;
				break;
			case EaseType.easeInExpo:
				m_easingFunction = EasingFunction_EaseInExpo;
				break;
			case EaseType.easeOutExpo:
				m_easingFunction = EasingFunction_EaseOutExpo;
				break;
			case EaseType.easeInOutExpo:
				m_easingFunction = EasingFunction_EaseInOutExpo;
				break;
			case EaseType.easeInCirc:
				m_easingFunction = EasingFunction_EaseInCirc;
				break;
			case EaseType.easeOutCirc:
				m_easingFunction = EasingFunction_EaseOutCirc;
				break;
			case EaseType.easeInOutCirc:
				m_easingFunction = EasingFunction_EaseInOutCirc;
				break;
			case EaseType.linear:
				m_easingFunction = EasingFunction_Linear;
				break;
			case EaseType.spring:
				m_easingFunction = EasingFunction_Spring;
				break;
			case EaseType.easeInBounce:
				m_easingFunction = EasingFunction_EaseInBounce;
				break;
			case EaseType.easeOutBounce:
				m_easingFunction = EasingFunction_EaseOutBounce;
				break;
			case EaseType.easeInOutBounce:
				m_easingFunction = EasingFunction_EaseInOutBounce;
				break;
			case EaseType.easeInBack:
				m_easingFunction = EasingFunction_EaseInBack;
				break;
			case EaseType.easeOutBack:
				m_easingFunction = EasingFunction_EaseOutBack;
				break;
			case EaseType.easeInOutBack:
				m_easingFunction = EasingFunction_EaseInOutBack;
				break;
			case EaseType.easeInElastic:
				m_easingFunction = EasingFunction_EaseInElastic;
				break;
			case EaseType.easeOutElastic:
				m_easingFunction = EasingFunction_EaseOutElastic;
				break;
			case EaseType.easeInOutElastic:
				m_easingFunction = EasingFunction_EaseInOutElastic;
				break;
			case EaseType.punch:
				m_easingFunction = EasingFunction_Punch;
				break;
			}
		}

		// Reset the last realtime value
        m_lastRealTime = Time.realtimeSinceStartup;
	}

	// Called when this object is destroyed
	protected void OnDestroy()
	{
		// Remove ourselves from the tween lists
		ms_tweens.Remove(this);
		ms_tweensPerObject[gameObject].Remove(this);
	}

	// Update the tween
	private void DoTweenUpdate()
	{
		if (!m_isReversed)
		{
			if ((m_percentage < 1.0f) && (m_duration > 0.0f))
				TweenUpdate();
			else
				TweenComplete(false);	
		}
		else
		{
			if ((m_percentage > 0.0f) && (m_duration > 0.0f))
				TweenUpdate();
			else
				TweenComplete(false);	
		}
	}

	// Called each frame to update the component
	protected virtual void Update()
	{
		// Non-physics update
#if ITWEEN_AFFECTS_PHYSICS
		if (m_isRunning && (m_rigidBody == null))
#else
		if (m_isRunning)
#endif
		{
			DoTweenUpdate();
		}
	}

#if ITWEEN_AFFECTS_PHYSICS
	// Called at fixed time intervals to update the component
	protected virtual void FixedUpdate()
	{
		// Physics update
		if (m_isRunning && (m_rigidBody != null))
		{
			DoTweenUpdate();
		}
	}
#endif

	// This is called whenever the component is enabled.
	protected virtual void OnEnable()
	{
		// If the tween was paused
		if (m_isPaused)
		{
			// Unpause, and resume any start delay if it was paused before it finished
			m_isPaused = false;
			if (m_startDelay > 0.0f)
				StartCoroutine("TweenDelay");
		}
		// Otherwise this is the first time the tween is awakened
		else
		{
			// If there is a delay, wait for the delay to finish, and start when it runs out, otherwise start immediately
			if (m_startDelay > 0.0f)
				StartCoroutine("TweenDelay");
			else
				TweenStart();
		}
	}

	// This is called whenever the component is disabled
	protected virtual void OnDisable()
	{
		// If disabling due to not being paused, the gameobject itself has been disabled, and we should
		// look to removing ourselves from the gameobject. However, if we're paused, keep ourselves alive
		if (!this.enabled || m_allowOnDisabled)
		{
			m_isPaused = true;
			return;
		}

		// Remove ourselves from the target, but allow the tween to finish its job first!
		TweenComplete(true);
	}
	
	// Update current percentage of tween based on time
	private void UpdatePercentage()
	{
		// Update the running time. A bit bonkers, but realtime can run backwards at startup, so clamp to positive :(
		m_runningTime += m_useRealTime ? Mathf.Max(0.0f, (Time.realtimeSinceStartup - m_lastRealTime)) : Time.deltaTime;
		m_lastRealTime = Time.realtimeSinceStartup;

		// Update the percentage
		if (m_duration > 0.0f)
			m_percentage = m_isReversed ? (1.0f - m_runningTime / m_duration) : (m_runningTime / m_duration);
		else
			m_percentage = m_isReversed ? 0.0f : 1.0f;
		
		// If the OnPercent callback is present, and it's not a cloned child (which would duplicate the callback), call it
		if (!m_isChild && m_argData.Contains("onpercent"))
			m_argData.Get<Action<float>>("onpercent").Invoke(m_percentage);
	}

	#endregion	
	
	#region Easing Curves
	
	private static float EasingFunction_Linear(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, value);
	}
	
	private static float EasingFunction_Clerp(float start, float end, float value)
	{
		float min = 0.0f;
		float max = 360.0f;
		float half = Mathf.Abs((max - min) * 0.5f);
		float retval = 0.0f;
		float diff = 0.0f;
		if ((end - start) < -half)
		{
			diff = ((max - start) + end) * value;
			retval = start + diff;
		}
		else if ((end - start) > half)
		{
			diff = -((max - end) + start) * value;
			retval = start + diff;
		}
		else
		{
			retval = start + (end - start) * value;
		}
		return retval;
    }

	private static float EasingFunction_Spring(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}

	private static float EasingFunction_EaseInQuad(float start, float end, float value)
	{
		end -= start;
		return end * value * value + start;
	}

	private static float EasingFunction_EaseOutQuad(float start, float end, float value)
	{
		end -= start;
		return -end * value * (value - 2) + start;
	}

	private static float EasingFunction_EaseInOutQuad(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1)
			return end * 0.5f * value * value + start;
		value--;
		return -end * 0.5f * (value * (value - 2) - 1) + start;
	}

	private static float EasingFunction_EaseInCubic(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value + start;
	}

	private static float EasingFunction_EaseOutCubic(float start, float end, float value)
	{
		value--;
		end -= start;
		return end * (value * value * value + 1) + start;
	}

	private static float EasingFunction_EaseInOutCubic(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1)
			return end * 0.5f * value * value * value + start;
		value -= 2;
		return end * 0.5f * (value * value * value + 2) + start;
	}

	private static float EasingFunction_EaseInQuart(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value * value + start;
	}

	private static float EasingFunction_EaseOutQuart(float start, float end, float value)
	{
		value--;
		end -= start;
		return -end * (value * value * value * value - 1) + start;
	}

	private static float EasingFunction_EaseInOutQuart(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1)
			return end * 0.5f * value * value * value * value + start;
		value -= 2;
		return -end * 0.5f * (value * value * value * value - 2) + start;
	}

	private static float EasingFunction_EaseInQuint(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value * value * value + start;
	}

	private static float EasingFunction_EaseOutQuint(float start, float end, float value)
	{
		value--;
		end -= start;
		return end * (value * value * value * value * value + 1) + start;
	}

	private static float EasingFunction_EaseInOutQuint(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1)
			return end * 0.5f * value * value * value * value * value + start;
		value -= 2;
		return end * 0.5f * (value * value * value * value * value + 2) + start;
	}

	private static float EasingFunction_EaseInSine(float start, float end, float value)
	{
		end -= start;
		return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
	}

	private static float EasingFunction_EaseOutSine(float start, float end, float value)
	{
		end -= start;
		return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
	}

	private static float EasingFunction_EaseInOutSine(float start, float end, float value)
	{
		end -= start;
		return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
	}

	private static float EasingFunction_EaseInExpo(float start, float end, float value)
	{
		end -= start;
		return end * Mathf.Pow(2, 10 * (value - 1)) + start;
	}

	private static float EasingFunction_EaseOutExpo(float start, float end, float value)
	{
		end -= start;
		return end * (-Mathf.Pow(2, -10 * value ) + 1) + start;
	}

	private static float EasingFunction_EaseInOutExpo(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1)
			return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
		value--;
		return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
	}

	private static float EasingFunction_EaseInCirc(float start, float end, float value)
	{
		end -= start;
		return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
	}

	private static float EasingFunction_EaseOutCirc(float start, float end, float value)
	{
		value--;
		end -= start;
		return end * Mathf.Sqrt(1 - value * value) + start;
	}

	private static float EasingFunction_EaseInOutCirc(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1)
			return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
		value -= 2;
		return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
	}

	private static float EasingFunction_EaseInBounce(float start, float end, float value)
	{
		end -= start;
		float d = 1f;
		return end - EasingFunction_EaseOutBounce(0, end, d-value) + start;
	}

	private static float EasingFunction_EaseOutBounce(float start, float end, float value)
	{
		value /= 1f;
		end -= start;

		if (value < (1 / 2.75f))
			return end * (7.5625f * value * value) + start;

		if (value < (2 / 2.75f))
		{
			value -= (1.5f / 2.75f);
			return end * (7.5625f * (value) * value + .75f) + start;
		}

		if (value < (2.5 / 2.75))
		{
			value -= (2.25f / 2.75f);
			return end * (7.5625f * (value) * value + .9375f) + start;
		}

		value -= (2.625f / 2.75f);
		return end * (7.5625f * (value) * value + .984375f) + start;
	}

	private static float EasingFunction_EaseInOutBounce(float start, float end, float value)
	{
		end -= start;
		float d = 1f;
		if (value < d* 0.5f)
			return EasingFunction_EaseInBounce(0, end, value*2) * 0.5f + start;
		else
			return EasingFunction_EaseOutBounce(0, end, value*2-d) * 0.5f + end*0.5f + start;
	}

	private static float EasingFunction_EaseInBack(float start, float end, float value)
	{
		end -= start;
		value /= 1;
		float s = 1.70158f;
		return end * (value) * value * ((s + 1) * value - s) + start;
	}

	private static float EasingFunction_EaseOutBack(float start, float end, float value)
	{
		float s = 1.70158f;
		end -= start;
		value = (value) - 1;
		return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
	}

	private static float EasingFunction_EaseInOutBack(float start, float end, float value)
	{
		float s = 1.70158f;
		end -= start;
		value /= .5f;
		if ((value) < 1)
		{
			s *= 1.525f;
			return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
		}
		value -= 2;
		s *= (1.525f);
		return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
	}

	private static float EasingFunction_Punch(float start, float end, float value)
	{
		if ((value == 0) || (value == 1))
			return start;

		float amplitude = end - start;
		float sign = Mathf.Sign(amplitude);
		amplitude = Mathf.Abs(amplitude);
		float period = 1 * 0.3f;
		float s = period / (2 * Mathf.PI) * Mathf.Asin(0);
		float result = sign * (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
		return start + result;
    }
	
	private static float EasingFunction_EaseInElastic(float start, float end, float value)
	{
		if (value == 0)
			return start;
		
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if ((value /= d) == 1)
			return start + end;
		
		if (a == 0f || a < Mathf.Abs(end))
		{
			a = end;
			s = p / 4;
		}
		else
		{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return -(a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
	}		

	private static float EasingFunction_EaseOutElastic(float start, float end, float value)
	{
		if (value == 0)
			return start;
		
		// Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if ((value /= d) == 1)
			return start + end;
		
		if (a == 0f || a < Mathf.Abs(end))
		{
			a = end;
			s = p * 0.25f;
		}
		else
		{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
	}		
	
	private static float EasingFunction_EaseInOutElastic(float start, float end, float value)
	{
		if (value == 0)
			return start;
		
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if ((value /= d*0.5f) == 2)
			return start + end;
		
		if (a == 0f || a < Mathf.Abs(end))
		{
			a = end;
			s = p / 4;
		}
		else
		{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		if (value < 1)
			return -0.5f * (a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
		return a * Mathf.Pow(2, -10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
	}
	
	private static float EasingFunction_ShakeRandom(float speed, float amount, float percent)
	{
		float diminishingControl = 1.0f - percent;
		return UnityEngine.Random.Range(-amount * diminishingControl, amount * diminishingControl);
	}

	private static float EasingFunction_ShakeSine(float speed, float amount, float percent)
	{
		float diminishingControl = 1.0f - percent;
		return Mathf.Sin(speed * percent * Mathf.PI * 2.0f) * amount * diminishingControl;
	}
	
	#endregion

	#region Iterators

	private class IterateAction
	{
		public virtual void DoAction(iTween item)
		{
		}
	}
	
	private static void IterateAllOnGameObject(GameObject target, bool includeChildren, IterateAction action)
	{
		if (action == null)
			return;

		// Use our own cached list, rather than GetComponent
		List<iTween> gameObjectTweens;
		if (ms_tweensPerObject.TryGetValue(target, out gameObjectTweens))
		{
			for (int itemIndex = 0, itemCount = gameObjectTweens.Count; itemIndex < itemCount; ++itemIndex)
				action.DoAction(gameObjectTweens[itemIndex]);
		}

		if (includeChildren)
		{
			int noofChildren = target.transform.childCount;
			for (int childIndex = 0; childIndex < noofChildren; ++childIndex)
			{
				Transform child = target.transform.GetChild(childIndex);
				IterateAllOnGameObject(child.gameObject, true, action);
			}
		}
	}

	private static void IterateAllInScene(IterateAction action)
	{
		if (action == null)
			return;

		for (int i = 0; i < ms_tweens.Count; i++)
			action.DoAction(ms_tweens[i]);
	}
	
	#endregion

	#region Comparisons

	private static bool CompareType(iTween item, string type)
	{
		string targetType = item.m_type + item.m_method;
		int maxLength = Math.Min(type.Length, targetType.Length);
		targetType = targetType.Substring(0, maxLength);
		return (targetType.ToLower() == type.ToLower());
	}

	private static bool CompareTag(iTween item, string tag)
	{
		return (item.m_tagList != null) && item.m_tagList.Contains(tag);
	}

	#endregion
	
	#region Resume

	private class IterateActionResume: IterateAction
	{
		public override void DoAction(iTween item)
		{
			item.enabled = true;
		}
	}

	private class IterateActionResumeType: IterateActionResume
	{
		protected string m_type = null;

		public void SetType(string type)
		{
			m_type = type;
		}
		
		public override void DoAction(iTween item)
		{
			if (CompareType(item, m_type))
				base.DoAction(item);
		}
	}

	private class IterateActionResumeTag: IterateActionResume
	{
		protected string m_tag = null;

		public void SetTag(string tag)
		{
			m_tag = tag;
		}
		
		public override void DoAction(iTween item)
		{
			if (CompareTag(item, m_tag))
				base.DoAction(item);
		}
	}

	private static IterateActionResume ms_resumeItem = new IterateActionResume();
	private static IterateActionResumeType ms_resumeItemType = new IterateActionResumeType();
	private static IterateActionResumeTag ms_resumeItemTag = new IterateActionResumeTag();

	/// <summary>
	/// Resume all iTweens on a GameObject, optionally including its children.
	/// </summary>
	public static void ResumeAll(GameObject target, bool includeChildren)
	{
		IterateAllOnGameObject(target, includeChildren, ms_resumeItem);
	}	
	
	/// <summary>
	/// Resume all iTweens in scene.
	/// </summary>
	public static void ResumeAll()
	{
		IterateAllInScene(ms_resumeItem);
	}	
	
	/// <summary>
	/// Resume all iTweens on a GameObject of a particular type, optionally including its children.
	/// </summar
	/// <param name="type">
	/// A <see cref="System.String"/> name of the type of iTween you would like to resume.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
	/// </param>	
	public static void ResumeByType(GameObject target, string type, bool includeChildren)
	{
		ms_resumeItemType.SetType(type);
		IterateAllOnGameObject(target, includeChildren, ms_resumeItemType);
	}	
	
	/// <summary>
	/// Resume all iTweens in scene of a particular type.
	/// </summary>
	/// <param name="type">
	/// A <see cref="System.String"/> name of the type of iTween you would like to pesume.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
	/// </param> 
	public static void ResumeByType(string type)
	{
		ms_resumeItemType.SetType(type);
		IterateAllInScene(ms_resumeItemType);
	}		

	/// <summary>
	/// Resume all iTweens on a GameObject with a particular tag, optionally including its children.
	/// </summar
	/// <param name="tag">
	/// The <see cref="System.String"/> tag of iTween you would like to resume.
	/// </param>	
	public static void ResumeByTag(GameObject target, string tag, bool includeChildren)
	{
		ms_resumeItemTag.SetTag(tag);
		IterateAllOnGameObject(target, includeChildren, ms_resumeItemTag);
	}

	/// <summary>
	/// Resume all iTweens in current scene with a particular tag.
	/// </summary>
	/// <param name="name">
	/// The <see cref="System.String"/> tag of iTween you would like to resume.
	/// </param> 
	public static void ResumeByTag(string tag)
	{
		ms_resumeItemTag.SetTag(tag);
		IterateAllInScene(ms_resumeItemTag);
	}
	
	#endregion

	#region Pause

	private class IterateActionPause: IterateAction
	{
		public override void DoAction(iTween item)
		{
			if (item.m_startDelay > 0.0f)
			{
				if (item.m_useRealTime)
					item.m_startDelay -= Time.realtimeSinceStartup - item.m_delayStartTime;
				else
					item.m_startDelay -= Time.time - item.m_delayStartTime;
				item.StopCoroutine("TweenDelay");
			}
			item.enabled = false;
		}
	}

	private class IterateActionPauseType: IterateActionPause
	{
		protected string m_type = null;

		public void SetType(string type)
		{
			m_type = type;
		}
		
		public override void DoAction(iTween item)
		{
			if (CompareType(item, m_type))
				base.DoAction(item);
		}
	}

	private class IterateActionPauseTag: IterateActionPause
	{
		protected string m_tag = null;

		public void SetTag(string tag)
		{
			m_tag = tag;
		}
		
		public override void DoAction(iTween item)
		{
			if (CompareTag(item, m_tag))
				base.DoAction(item);
		}
	}

	private static IterateActionPause ms_pauseItem = new IterateActionPause();
	private static IterateActionPauseType ms_pauseItemType = new IterateActionPauseType();
	private static IterateActionPauseTag ms_pauseItemTag = new IterateActionPauseTag();

	/// <summary>
	/// Pause all iTweens on a GameObject, optionally including its children.
	/// </summary>
	public static void PauseAll(GameObject target, bool includeChildren)
	{
		IterateAllOnGameObject(target, includeChildren, ms_pauseItem);
	}	
	
	/// <summary>
	/// Pause all iTweens in scene.
	/// </summary>
	public static void PauseAll()
	{
		IterateAllInScene(ms_pauseItem);
	}	
	
	/// <summary>
	/// Pause all iTweens on a GameObject of a particular type, optionally including its children.
	/// </summar
	/// <param name="type">
	/// A <see cref="System.String"/> name of the type of iTween you would like to pause.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
	/// </param>	
	public static void PauseByType(GameObject target, string type, bool includeChildren)
	{
		ms_pauseItemType.SetType(type);
		IterateAllOnGameObject(target, includeChildren, ms_pauseItemType);
	}	
	
	/// <summary>
	/// Pause all iTweens in scene of a particular type.
	/// </summary>
	/// <param name="type">
	/// A <see cref="System.String"/> name of the type of iTween you would like to pause.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
	/// </param> 
	public static void PauseByType(string type)
	{
		ms_pauseItemType.SetType(type);
		IterateAllInScene(ms_pauseItemType);
	}		

	/// <summary>
	/// Pause all iTweens on a GameObject with a particular tag, optionally including its children.
	/// </summar
	/// <param name="tag">
	/// The <see cref="System.String"/> tag of iTween you would like to pause.
	/// </param>	
	public static void PauseByTag(GameObject target, string tag, bool includeChildren)
	{
		ms_pauseItemTag.SetTag(tag);
		IterateAllOnGameObject(target, includeChildren, ms_pauseItemTag);
	}

	/// <summary>
	/// Pause all iTweens in current scene with a particular tag.
	/// </summary>
	/// <param name="name">
	/// The <see cref="System.String"/> tag of iTween you would like to pause.
	/// </param> 
	public static void PauseByTag(string tag)
	{
		ms_pauseItemTag.SetTag(tag);
		IterateAllInScene(ms_pauseItemTag);
	}

	#endregion
	
	#region Stop

	private class IterateActionStop: IterateAction
	{
		private bool m_finishTween = false;

		public void SetFinish(bool finishTween)
		{
			m_finishTween = finishTween;
		}

		public override void DoAction(iTween item)
		{
			// Remove ourselves from the target, but allow the tween to finish its job first if requested!
			if (m_finishTween)
				item.TweenComplete(true);
			else
				Destroy(item);
		}
	}

	private class IterateActionStopType: IterateActionStop
	{
		protected string m_type = null;

		public void SetType(string type)
		{
			m_type = type;
		}
		
		public override void DoAction(iTween item)
		{
			if (CompareType(item, m_type))
				base.DoAction(item);
		}
	}

	private class IterateActionStopTag: IterateActionStop
	{
		protected string m_tag = null;

		public void SetTag(string tag)
		{
			m_tag = tag;
		}
		
		public override void DoAction(iTween item)
		{
			if (CompareTag(item, m_tag))
				base.DoAction(item);
		}
	}

	private static IterateActionStop ms_stopItem = new IterateActionStop();
	private static IterateActionStopType ms_stopItemType = new IterateActionStopType();
	private static IterateActionStopTag ms_stopItemTag = new IterateActionStopTag();
	
	/// <summary>
	/// Stop and destroy all iTweens on a GameObject, optionally including its children.
	/// </summary>
	/// <param name="target">
	/// The <see cref="GameObject"/> The target to stop tweens on
	/// </param>
	/// <param name="includeChildren">
	/// The <see cref="bool"/> Should tweens be stopped on children too?
	/// </param>
	/// <param name="finishTween">
	/// The <see cref="bool"/> Should the tween be allowed to instantly jump to the end, rather than abort?
	/// </param>
	public static void StopAll(GameObject target, bool includeChildren, bool finishTween)
	{
		ms_stopItem.SetFinish(finishTween);
		IterateAllOnGameObject(target, includeChildren, ms_stopItem);
	}
	
	/// <summary>
	/// Stop and destroy all Tweens in current scene.
	/// </summary>
	/// <param name="finishTween">
	/// The <see cref="bool"/> Should the tween be allowed to instantly jump to the end, rather than abort?
	/// </param>
	public static void StopAll(bool finishTween)
	{
		ms_stopItem.SetFinish(finishTween);
		IterateAllInScene(ms_stopItem);
	}

	/// <summary>
	/// Stop and destroy all iTweens on a GameObject of a particular type, optionally including its children.
	/// </summary>
	/// <param name="target">
	/// The <see cref="GameObject"/> The target to stop tweens on
	/// </param>
	/// <param name="type">
	/// A <see cref="System.String"/> The type of iTween you would like to stop. Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
	/// </param>
	/// <param name="includeChildren">
	/// The <see cref="bool"/> Should tweens be stopped on children too?
	/// </param>
	/// <param name="finishTween">
	/// The <see cref="bool"/> Should the tween be allowed to instantly jump to the end, rather than abort?
	/// </param>
	public static void StopByType(GameObject target, string type, bool includeChildren, bool finishTween)
	{
		ms_stopItemType.SetFinish(finishTween);
		ms_stopItemType.SetType(type);
		IterateAllOnGameObject(target, includeChildren, ms_stopItemType);
	}

	/// <summary>
	/// Stop and destroy all iTweens in current scene of a particular type.
	/// </summary>
	/// <param name="type">
	/// A <see cref="System.String"/> The type of iTween you would like to stop. Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
	/// </param>
	/// <param name="finishTween">
	/// The <see cref="bool"/> Should the tween be allowed to instantly jump to the end, rather than abort?
	/// </param>
	public static void StopByType(string type, bool finishTween)
	{
		ms_stopItemType.SetFinish(finishTween);
		ms_stopItemType.SetType(type);
		IterateAllInScene(ms_stopItemType);
	}

	/// <summary>
	/// Stop and destroy all iTweens on a GameObject with a particular tag, optionally including its children.
	/// </summar
	/// <param name="target">
	/// The <see cref="GameObject"/> The target to stop tweens on
	/// </param>
	/// <param name="tag">
	/// The <see cref="System.String"/> The tag to stop. iTweens will stop if tag is found, even if other tags are present.
	/// </param>
	/// <param name="includeChildren">
	/// The <see cref="bool"/> Should tweens be stopped on children too?
	/// </param>
	/// <param name="finishTween">
	/// The <see cref="bool"/> Should the tween be allowed to instantly jump to the end, rather than abort?
	/// </param>
	public static void StopByTag(GameObject target, string tag, bool includeChildren, bool finishTween)
	{
		ms_stopItemTag.SetFinish(finishTween);
		ms_stopItemTag.SetTag(tag);
		IterateAllOnGameObject(target, includeChildren, ms_stopItemTag);
	}

	/// <summary>
	/// Stop and destroy all iTweens in current scene with a particular tag.
	/// </summary>
	/// <param name="tag">
	/// The <see cref="System.String"/> The tag to stop. iTweens will stop if tag is found, even if other tags are present.
	/// </param>
	/// <param name="finishTween">
	/// The <see cref="bool"/> Should the tween be allowed to instantly jump to the end, rather than abort?
	/// </param>
	public static void StopByTag(string tag, bool finishTween)
	{
		ms_stopItemTag.SetFinish(finishTween);
		ms_stopItemTag.SetTag(tag);
		IterateAllInScene(ms_stopItemTag);
	}
	
	#endregion

}

