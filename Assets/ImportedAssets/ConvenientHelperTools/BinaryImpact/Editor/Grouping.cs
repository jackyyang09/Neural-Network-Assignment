using UnityEditor;
using UnityEngine;

namespace BIG.Utility {

	/// <summary>
	/// Groups all selected objects as childs of a new object
	/// </summary>
	public class Grouping {
		[MenuItem ( "GameObject/Group it %g" )]
		private static void GroupSelected() {
			//no transform selected return
			if ( !Selection.activeTransform ) {
				return;
			}

			GameObject go = new GameObject {
				name = "Group"
			};

			Undo.RegisterCreatedObjectUndo ( go, "Group it" );

			//Set parent to selection parent
			go.transform.SetParent ( Selection.activeTransform.parent, false );

			//iterate over selection to add to new group go
			foreach ( var transform in Selection.transforms ) {
				Undo.SetTransformParent ( transform, go.transform, "Group it" );
			}

			//make the new groupt the selected target
			Selection.activeGameObject = go;
		}
	}

}
