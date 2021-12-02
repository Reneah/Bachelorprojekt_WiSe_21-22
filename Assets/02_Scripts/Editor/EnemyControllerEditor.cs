using UnityEditor;
using Enemy.Controller;

namespace Enemy.CustomInspector
{
        [CustomEditor(typeof(EnemyController))]
    public class EnemyControllerEditor : Editor
    {
        // behaviour variables
        private SerializedProperty _patrolling;
        private SerializedProperty _guarding;
        
        // guard variables
        private SerializedProperty _lookingRoute;
        private SerializedProperty _switchLookTime;
        private SerializedProperty _guardPoint;
        private SerializedProperty _lookSwitchSpeed;
        private SerializedProperty _stopGuardpointDistance;
        private SerializedProperty _desiredBodyRotation;
        private SerializedProperty _currentLookPosition;
        private SerializedProperty _smoothBodyRotation;

        // patrol variables
        private SerializedProperty _patrollingRoute;
        private SerializedProperty _dwellingTimer;
        private SerializedProperty _stopDistance;
        private SerializedProperty _patrolSpeed;
        
        // chase variables
        private SerializedProperty _catchDistance;
        private SerializedProperty _chaseSpeed;
        
        // Field Of View variables
        private SerializedProperty _obstructionMask;
        private SerializedProperty _enemyHead;
        private SerializedProperty _obstacleRaycastTransform;
        private SerializedProperty _lookPositionAtSpotted;
        private SerializedProperty _secondsToSpott;
        private SerializedProperty _spottedBar;
        private SerializedProperty _spottedDistance;
        private SerializedProperty _lastChanceTime;
        private SerializedProperty _spottedTime;
        
        // Investigation variables
        private SerializedProperty _firstStageRunSpeed;
        private SerializedProperty _secondStageRunSpeed;
        private SerializedProperty _thirdStageRunSpeed;
        private SerializedProperty _searchSpeed;
        private SerializedProperty _waypointCounter;

        private void OnEnable()
        {
            // behaviour variables
            _patrolling = serializedObject.FindProperty("_patrolling");
            _guarding = serializedObject.FindProperty("_guarding");
            
            // guard variables
            _lookingRoute = serializedObject.FindProperty("_lookingRoute");
            _switchLookTime = serializedObject.FindProperty("_switchLookTime");
            _lookSwitchSpeed = serializedObject.FindProperty("_lookSwitchSpeed");
            _guardPoint = serializedObject.FindProperty("_guardPoint");
            _stopGuardpointDistance = serializedObject.FindProperty("_stopGuardpointDistance");
            _desiredBodyRotation = serializedObject.FindProperty("_desiredBodyRotation");
            _currentLookPosition = serializedObject.FindProperty("_currentLookPosition");
            _smoothBodyRotation = serializedObject.FindProperty("_smoothBodyRotation");
            
            // patrol variables
            _patrollingRoute = serializedObject.FindProperty("_patrollingRoute");
            _dwellingTimer = serializedObject.FindProperty("_dwellingTimer");
            _stopDistance = serializedObject.FindProperty("_stopDistance");
            _patrolSpeed = serializedObject.FindProperty("_patrolSpeed");
            
            // chase variables
            _catchDistance = serializedObject.FindProperty("_catchDistance");
            _chaseSpeed = serializedObject.FindProperty("_chaseSpeed");
            
            // Field Of View variables
            _obstructionMask = serializedObject.FindProperty("_obstructionMask");
            _enemyHead = serializedObject.FindProperty("_enemyHead");
            _obstacleRaycastTransform = serializedObject.FindProperty("_obstacleRaycastTransform");
            _lookPositionAtSpotted = serializedObject.FindProperty("_lookPositionAtSpotted");
            _secondsToSpott = serializedObject.FindProperty("_secondsToSpott");
            _spottedBar = serializedObject.FindProperty("_spottedBar");
            _spottedDistance = serializedObject.FindProperty("_spottedDistance");
            _lastChanceTime = serializedObject.FindProperty("_lastChanceTime");
            
            // Investigation variables
            _firstStageRunSpeed = serializedObject.FindProperty("_firstStageRunSpeed");
            _secondStageRunSpeed = serializedObject.FindProperty("_secondStageRunSpeed");
            _thirdStageRunSpeed = serializedObject.FindProperty("_thirdStageRunSpeed");
            _searchSpeed = serializedObject.FindProperty("_searchSpeed");
            _waypointCounter = serializedObject.FindProperty("_waypointCounter");
        }

        override public void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var enemyController = target as EnemyController;

            using (new EditorGUI.DisabledScope(enemyController.Patrolling))
            {
                // behaviour variable
                EditorGUILayout.PropertyField(_guarding);
            }
            
            using (new EditorGUI.DisabledScope(enemyController.Guarding))
            {
                // behaviour variable
                EditorGUILayout.PropertyField(_patrolling);
                
                // patrol variables
                EditorGUILayout.PropertyField(_patrollingRoute);
                EditorGUILayout.PropertyField(_dwellingTimer);
                EditorGUILayout.PropertyField(_stopDistance);
                EditorGUILayout.PropertyField(_patrolSpeed);
            }
            
            using (new EditorGUI.DisabledScope(enemyController.Patrolling))
            {
                // guard variables
                EditorGUILayout.PropertyField(_switchLookTime);
                EditorGUILayout.PropertyField(_desiredBodyRotation);
                EditorGUILayout.PropertyField(_currentLookPosition);
                EditorGUILayout.PropertyField(_stopGuardpointDistance);
                EditorGUILayout.PropertyField(_smoothBodyRotation);
                EditorGUILayout.PropertyField(_lookingRoute);
                EditorGUILayout.PropertyField(_lookSwitchSpeed);
                EditorGUILayout.PropertyField(_guardPoint);
            }
            
            // chase variables
            EditorGUILayout.PropertyField(_catchDistance);
            EditorGUILayout.PropertyField(_chaseSpeed);
            
            // Field Of View variables
            EditorGUILayout.PropertyField(_obstructionMask);
            EditorGUILayout.PropertyField(_enemyHead);
            EditorGUILayout.PropertyField(_obstacleRaycastTransform);
            EditorGUILayout.PropertyField(_lookPositionAtSpotted);
            EditorGUILayout.PropertyField(_secondsToSpott);
            EditorGUILayout.PropertyField(_spottedBar);
            EditorGUILayout.PropertyField(_spottedDistance);
            EditorGUILayout.PropertyField(_lastChanceTime);
            
            // Investigation variables
            EditorGUILayout.PropertyField(_firstStageRunSpeed);
            EditorGUILayout.PropertyField(_secondStageRunSpeed);
            EditorGUILayout.PropertyField(_thirdStageRunSpeed);
            EditorGUILayout.PropertyField(_searchSpeed);
            EditorGUILayout.PropertyField(_waypointCounter);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}

