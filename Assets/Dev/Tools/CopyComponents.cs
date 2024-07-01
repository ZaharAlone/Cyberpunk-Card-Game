using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using Sirenix.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace Tools
{
    public class CopyComponents : OdinEditorWindow
    {
        [MenuItem("Tools/Custom/Copy Components")]
        public static void Init()
        {
            GetWindow<CopyComponents>().Show();
        }

        public GameObject SourceGO;
        public GameObject TargetGO;

        [TableList]
        public List<OnComponent> Components = new List<OnComponent>();

        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        public void GetComponents()
        {
            if (SourceGO == null)
            {
                EditorGUILayout.LabelField("No Game Object Select");
                return;
            }

            Components.Clear();
            foreach (var component in SourceGO.GetComponents<Component>())
            {
                var newComponent = new OnComponent();
                newComponent.Component = component;
                newComponent.IsCopy = true;
                
                Components.Add(newComponent);
            }
        }

        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        public void SetComponents()
        {
            if (TargetGO == null)
            {
                EditorGUILayout.LabelField("No Game Object Select");
                return;
            }

            foreach (var component in Components)
            {
                if (component.IsCopy)
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(component.Component);
                    var componentType = component.Component.GetType();

                    var isDouble = false;
                    var doubleComponent = new Component();
                    
                    foreach (var targetComponent in TargetGO.GetComponents<Component>())
                    {
                        var targetComponentType = targetComponent.GetType();
                        if (componentType == targetComponentType)
                        {
                            isDouble = true;
                            doubleComponent = targetComponent;
                            break;
                        }
                    }

                    if (isDouble)
                        UnityEditorInternal.ComponentUtility.PasteComponentValues(doubleComponent);
                    else
                        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(TargetGO);
                }

            }
        }

        [VerticalGroup]
        [Button(ButtonSizes.Large)]
        public void CopyLayerAndTag()
        {
            if (TargetGO == null || SourceGO == null)
            {
                EditorGUILayout.LabelField("No Game Object Select");
                return;
            }

            TargetGO.layer = SourceGO.layer;
            TargetGO.tag = SourceGO.tag;
        }
    }

    [Serializable]
    public struct OnComponent
    {
        public bool IsCopy;
        public Component Component;
    }
}
#endif