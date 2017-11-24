using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static  class TransformtExtension
{
	//path
	public static string GetPath (this Transform trans)
	{
		return trans.gameObject.GetPath();
	}

	//order
	public static int GetZOrder (this Transform trans)
	{
		return trans.GetSiblingIndex ();
	}
	
	public static void SetZOrder (this Transform trans, int index)
	{
		trans.gameObject.SetZOrder(index);
	}

	public static void IncZOrder (this Transform trans)
	{
		trans.gameObject.IncZOrder();
	}

	public static void DecZOrder (this Transform trans)
	{
		trans.gameObject.DecZOrder();
	}
	
	//Child
	public static bool HasChild (this Transform trans, GameObject child)
	{
		return trans.gameObject.HasChild (child);
	}
	
	public static void SetParent (this Transform trans, GameObject parent, bool worldPositionStays= true)
	{
		trans.gameObject.SetParent (parent, worldPositionStays);
	}
	
	public static void AddChild (this Transform trans, GameObject child, bool worldPositionStays= true)
	{
		trans.gameObject.AddChild (child, worldPositionStays);
	}

	public static void AddChild (this Transform trans, Transform child, bool worldPositionStays= true)
	{
		trans.gameObject.AddChild (child, worldPositionStays);
	}
	
	public static int RemoveAllChildren (this Transform trans)
	{
		return trans.gameObject.RemoveAllChildren ();
	}
	
	
	public static void ResetTransform (this Transform trans)
	{
		trans.gameObject.ResetTransform ();
	}
	
	//Componet
	public static bool HasComponent<T> (this Transform trans) where T : Component
	{
		return trans.gameObject.HasComponent<T> ();
	}
	
	public static bool RemoveComponent<T> (this Transform trans) where T : Component
	{
		return trans.gameObject.RemoveComponent<T> ();
	}
}
