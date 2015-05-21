using UnityEngine;
using UnityEditor;
using System.Collections;

public class SortChildren : ScriptableObject {
	
	[MenuItem ("GameObject/Sort Children")]
	
	static void MenuAddChild() {
		Sort(Selection.activeTransform);
	}
	
	static void Sort(Transform current) {
		foreach (Transform child in current)
			Sort(child);
		current.parent = current.parent;
	}
}