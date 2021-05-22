//using UnityEngine;
//using UnityEditor;

//[InitializeOnLoad]
//public class InputManagerUpdater : Editor {

//    static InputManagerUpdater() {
//        //AddAxis(new InputAxis() { name = "myMouseX", sensitivity = 1f, type = AxisType.MouseMovement, axis = 1 });
//        SetSinglePlayerControls();
//    }

//    private static SerializedProperty GetChildProperty(SerializedProperty parent, string name) {
//        SerializedProperty child = parent.Copy();
//        child.Next(true);
//        do {
//            if (child.name == name) return child;
//        }
//        while (child.Next(false));
//        return null;
//    }

//    private static SerializedProperty GetAxis(string axisName) {
//        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
//        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

//        axesProperty.Next(true);
//        axesProperty.Next(true);
//        while (axesProperty.Next(false)) {
//            SerializedProperty axis = axesProperty.Copy();
//            axis.Next(true);
//            if (axis.stringValue == axisName) return axis;
//        }
//        return null;
//    }

//    private static void UpdateAxis(string axisName, string newKey) {
//        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
//        SerializedProperty axisProperty = GetAxis(axisName);
//        if (axisProperty != null) {
//            Debug.Log("Old " + axisName + ".altPositiveButton  property: " + GetChildProperty(axisProperty, "altPositiveButton").stringValue);
//            GetChildProperty(axisProperty, "altPositiveButton").stringValue = newKey;
//            serializedObject.ApplyModifiedProperties();

//            SerializedProperty test = GetAxis(axisName);
//            Debug.Log("New: " + GetChildProperty(axisProperty, "altPositiveButton").stringValue);
//        }
//        else {
//            Debug.LogWarning("Axis '" + axisName + "' doesn't exists.");
//        }
//    }

//    public static void SetSinglePlayerControls() {
//        UpdateAxis("FireUp", "w");
//        UpdateAxis("FireDown", "x");
//        UpdateAxis("FireLeft", "c");
//        UpdateAxis("FireRight", "v");
//        //UpdateAxis("FireUp", "up");
//        //UpdateAxis("FireDown", "down");
//        //UpdateAxis("FireLeft", "left");
//        //UpdateAxis("FireRight", "right");
//    }

//    public static void SetMultiPlayerControls() {
//        UpdateAxis("FireUp", "");
//        UpdateAxis("FireDown", "");
//        UpdateAxis("FireLeft", "");
//        UpdateAxis("FireRight", "");
//    }



//    private static bool AxisDefined(string axisName) {
//        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
//        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

//        axesProperty.Next(true);
//        axesProperty.Next(true);
//        while (axesProperty.Next(false)) {
//            SerializedProperty axis = axesProperty.Copy();
//            axis.Next(true);
//            if (axis.stringValue == axisName) return true;
//        }
//        return false;
//    }

//    public enum AxisType {
//        KeyOrMouseButton = 0,
//        MouseMovement = 1,
//        JoystickAxis = 2
//    };

//    public class InputAxis {
//        public string name;
//        public string descriptiveName;
//        public string descriptiveNegativeName;
//        public string negativeButton;
//        public string positiveButton;
//        public string altNegativeButton;
//        public string altPositiveButton;

//        public float gravity;
//        public float dead;
//        public float sensitivity;

//        public bool snap = false;
//        public bool invert = false;

//        public AxisType type;

//        public int axis;
//        public int joyNum;
//    }

//    private static void AddAxis(InputAxis axis) {
//        if (AxisDefined(axis.name)) return;

//        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
//        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

//        axesProperty.arraySize++;
//        serializedObject.ApplyModifiedProperties();

//        SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

//        GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
//        GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
//        GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
//        GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
//        GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
//        GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
//        GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
//        GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
//        GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
//        GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
//        GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
//        GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
//        GetChildProperty(axisProperty, "type").intValue = (int) axis.type;
//        GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
//        GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

//        serializedObject.ApplyModifiedProperties();
//    }
//}

////public class InputManagerEditor : MonoBehaviour {

////    private static SerializedProperty GetChildProperty(SerializedProperty parent, string name) {
////        SerializedProperty child = parent.Copy();
////        child.Next(true);
////        do {
////            if (child.name == name) return child;
////        }
////        while (child.Next(false));
////        return null;
////    }

////    private static bool AxisDefined(string axisName) {
////        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
////        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

////        axesProperty.Next(true);
////        axesProperty.Next(true);
////        while (axesProperty.Next(false)) {
////            SerializedProperty axis = axesProperty.Copy();
////            axis.Next(true);
////            if (axis.stringValue == axisName) return true;
////        }
////        return false;
////    }

////    private static SerializedProperty GetAxis(string axisName) {
////        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
////        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

////        axesProperty.Next(true);
////        axesProperty.Next(true);
////        while (axesProperty.Next(false)) {
////            SerializedProperty axis = axesProperty.Copy();
////            axis.Next(true);
////            if (axis.stringValue == axisName) return axis;
////        }
////        return null;
////    }

////    public enum AxisType {
////        KeyOrMouseButton = 0,
////        MouseMovement = 1,
////        JoystickAxis = 2
////    };

////    public class InputAxis {
////        public string name;
////        public string descriptiveName;
////        public string descriptiveNegativeName;
////        public string negativeButton;
////        public string positiveButton;
////        public string altNegativeButton;
////        public string altPositiveButton;

////        public float gravity;
////        public float dead;
////        public float sensitivity;

////        public bool snap = false;
////        public bool invert = false;

////        public AxisType type;

////        public int axis;
////        public int joyNum;
////    }

////    private static void AddAxis(InputAxis axis) {
////        if (AxisDefined(axis.name)) return;

////        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
////        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

////        axesProperty.arraySize++;
////        serializedObject.ApplyModifiedProperties();

////        SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

////        GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
////        GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
////        GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
////        GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
////        GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
////        GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
////        GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
////        GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
////        GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
////        GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
////        GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
////        GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
////        GetChildProperty(axisProperty, "type").intValue = (int) axis.type;
////        GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
////        GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

////        serializedObject.ApplyModifiedProperties();
////    }

////    public static void SetSoloAxis() {
////        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
////        SerializedProperty axisProperty = GetAxis("Up");
////        if (axisProperty != null) {
////            Debug.Log("!= NULL");
////        }
////        else {
////            Debug.Log("== NULL");
////        }
////    }

////    //public static void SetupInputManager() {
////    //    // Add mouse definitions
////    //    AddAxis(new InputAxis() { name = "myMouseX", sensitivity = 1f, type = AxisType.MouseMovement, axis = 1 });
////    //    AddAxis(new InputAxis() { name = "myMouseY", sensitivity = 1f, type = AxisType.MouseMovement, axis = 2 });
////    //    AddAxis(new InputAxis() { name = "myScrollWheel", sensitivity = 1f, type = AxisType.MouseMovement, axis = 3 });

////    //    // Add gamepad definitions
////    //    int i = 1;
////    //    //for (int i = 1; i <= (int)InputBind.Gamepad.Gamepad4; i++)
////    //    //{
////    //    for (int j = 0; j <= (int) InputBind.GamepadAxis.Axis10; j++) {
////    //        AddAxis(new InputAxis() {
////    //            name = "myPad" + i + "A" + (j + 1).ToString(),
////    //            dead = 0.2f,
////    //            sensitivity = 1f,
////    //            type = AxisType.JoystickAxis,
////    //            axis = (j + 1),
////    //            joyNum = i,
////    //        });
////    //    }
////    //    //}
////    //}
////}
