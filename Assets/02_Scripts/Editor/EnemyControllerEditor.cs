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
        private SerializedProperty _lowGroundCatchDistance;
        private SerializedProperty _highGroundCatchDistance;
        private SerializedProperty _chaseSpeed;
        private SerializedProperty _pullDistance;
        
        // Field Of View variables
        private SerializedProperty _obstructionMask;
        private SerializedProperty _enemyHead;
        private SerializedProperty _obstacleRaycastTransform;
        private SerializedProperty _lookPositionAtSpotted;
        private SerializedProperty _spottedVisionDistance;
        private SerializedProperty _spottedAcousticDistance;
        private SerializedProperty _spottedBar;
        private SerializedProperty _visionSecondsToSpot;
        private SerializedProperty _acousticSecondsToSpot;
        private SerializedProperty _reminderTimeLowGround;
        private SerializedProperty _reminderTimeHighGround;
        private SerializedProperty _spottedTime;
        private SerializedProperty _highGroundViewCone;
        private SerializedProperty _lowGroundViewCone;
        
        // Investigation variables
        private SerializedProperty _firstStageRunSpeed;
        private SerializedProperty _secondStageRunSpeed;
        private SerializedProperty _thirdStageRunSpeed;
        private SerializedProperty _playerSearchSpeed;
        private SerializedProperty _blockAcousticLayerMasks;

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
            _lowGroundCatchDistance = serializedObject.FindProperty("_lowGroundCatchDistance");
            _highGroundCatchDistance = serializedObject.FindProperty("_highGroundCatchDistance");
            _chaseSpeed = serializedObject.FindProperty("_chaseSpeed");
            _pullDistance = serializedObject.FindProperty("_pullDistance");
            
            // Field Of View variables
            _obstructionMask = serializedObject.FindProperty("_obstructionMask");
            _enemyHead = serializedObject.FindProperty("_enemyHead");
            _obstacleRaycastTransform = serializedObject.FindProperty("_obstacleRaycastTransform");
            _lookPositionAtSpotted = serializedObject.FindProperty("_lookPositionAtSpotted");
            _acousticSecondsToSpot = serializedObject.FindProperty("_acousticSecondsToSpot");
            _visionSecondsToSpot = serializedObject.FindProperty("_visionSecondsToSpot");
            _spottedBar = serializedObject.FindProperty("_spottedBar");
            _spottedAcousticDistance = serializedObject.FindProperty("_spottedAcousticDistance");
            _spottedVisionDistance = serializedObject.FindProperty("_spottedVisionDistance");
            _reminderTimeLowGround = serializedObject.FindProperty("_reminderTimeLowGround");
            _reminderTimeHighGround = serializedObject.FindProperty("_reminderTimeHighGround");
            _highGroundViewCone = serializedObject.FindProperty("_highGroundViewCone");
            _lowGroundViewCone = serializedObject.FindProperty("_lowGroundViewCone");
            
            // Investigation variables
            _firstStageRunSpeed = serializedObject.FindProperty("_firstStageRunSpeed");
            _secondStageRunSpeed = serializedObject.FindProperty("_secondStageRunSpeed");
            _thirdStageRunSpeed = serializedObject.FindProperty("_thirdStageRunSpeed");
            _playerSearchSpeed = serializedObject.FindProperty("_playerSearchSpeed");
            _blockAcousticLayerMasks = serializedObject.FindProperty("_blockAcousticLayerMasks");
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
            EditorGUILayout.PropertyField(_lowGroundCatchDistance);
            EditorGUILayout.PropertyField(_highGroundCatchDistance);
            EditorGUILayout.PropertyField(_chaseSpeed);
            EditorGUILayout.PropertyField(_pullDistance);
            
            // Field Of View variables
            EditorGUILayout.PropertyField(_obstructionMask);
            EditorGUILayout.PropertyField(_enemyHead);
            EditorGUILayout.PropertyField(_obstacleRaycastTransform);
            EditorGUILayout.PropertyField(_lookPositionAtSpotted);
            EditorGUILayout.PropertyField(_acousticSecondsToSpot);
            EditorGUILayout.PropertyField(_visionSecondsToSpot);
            EditorGUILayout.PropertyField(_spottedBar);
            EditorGUILayout.PropertyField(_spottedAcousticDistance);
            EditorGUILayout.PropertyField(_spottedVisionDistance);
            EditorGUILayout.PropertyField(_reminderTimeLowGround);
            EditorGUILayout.PropertyField(_reminderTimeHighGround);
            EditorGUILayout.PropertyField(_highGroundViewCone);
            EditorGUILayout.PropertyField(_lowGroundViewCone);

            // Investigation variables
            EditorGUILayout.PropertyField(_firstStageRunSpeed);
            EditorGUILayout.PropertyField(_secondStageRunSpeed);
            EditorGUILayout.PropertyField(_thirdStageRunSpeed);
            EditorGUILayout.PropertyField(_playerSearchSpeed);
            EditorGUILayout.PropertyField(_blockAcousticLayerMasks);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}

