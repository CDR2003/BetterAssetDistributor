using UnityEditor;
using UnityEngine;

namespace RocketPunch.Bad
{
    [CustomPropertyDrawer( typeof( BadVersion ))]
    public class BadVersionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            EditorGUI.BeginProperty( position, label, property );
            
            var rect = EditorGUI.PrefixLabel( position, label );
            var major = property.FindPropertyRelative( "major" );
            var minor = property.FindPropertyRelative( "minor" );
            var autoIncrement = property.FindPropertyRelative( "autoIncrement" );
            var majorRect = new Rect( rect.x, rect.y, rect.width / 3, rect.height );
            var minorRect = new Rect( rect.x + rect.width / 3, rect.y, rect.width / 3, rect.height );
            var autoIncrementRect = new Rect( rect.x + rect.width / 3 * 2, rect.y, rect.width / 3, rect.height );
            
            EditorGUI.PropertyField( majorRect, major, GUIContent.none );
            EditorGUI.PropertyField( minorRect, minor, GUIContent.none );

            GUI.enabled = false;
            EditorGUI.PropertyField( autoIncrementRect, autoIncrement, GUIContent.none );
            GUI.enabled = true;
            
            EditorGUI.EndProperty();
        }
    }
}