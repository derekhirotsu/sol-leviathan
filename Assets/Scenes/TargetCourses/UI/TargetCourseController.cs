using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ScriptableVariables;

public class TargetCourseController : MonoBehaviour {
    [SerializeField]
    CourseData courseData;
    public CourseData CourseData { 
        get { return courseData; }
    }

    float currentBestTime;
    public float CurrentBestTime {
        get { return currentBestTime; }
    }

    public bool BestTimeBeat {
        get { return courseTimer.elapsedTime < currentBestTime; }
    }

    public float ParTime {
        get { return courseData.ParTime; }
    }

    public bool ParTimeBeat {
        get { return courseTimer.elapsedTime < courseData.ParTime; }
    }

    bool currentClearStatus;
    public bool CurrentClearStatus {
        get { return currentClearStatus; }
    }

    [SerializeField]
    Timer courseTimer;
    public float ElapsedCourseTime {
        get { return courseTimer.elapsedTime ;}
    }
    public string FormattedElapsedTime {
        get { return TimeFormat.FormatSecondsMonospace(courseTimer.elapsedTime, 0.65f); }
    }

    [SerializeField]
    InputActionAsset actionAsset;

    [SerializeField]
    EntityLookup activeTargets;

    [SerializeField]
    EntityLookup activeEnemies;

    [SerializeField]
    BoolVariable CanPauseGame;

    [SerializeField]
    BoolVariable GamePaused;

    MenuController2 menuController;

    Countdown countdown;

    CourseClearSplash courseClearSplash;

    InputActionMap actionMap;

    void Awake() {
        actionMap = actionAsset.FindActionMap("Player");
    }

    void OnEnable() {
        if (actionMap == null) {
            return;
        }

        actionMap["Pause"].performed += OnPause;
    }

    void OnDisable() {
        if (actionMap == null) {
            return;
        }
        
        actionMap["Pause"].performed -= OnPause;
    }

    void Start() {
        menuController = GetComponent<MenuController2>();
        countdown = GetComponentInChildren<Countdown>();
        courseClearSplash = GetComponentInChildren<CourseClearSplash>();

        currentBestTime = (float)courseData.BestTime.Value / 1000;
        currentClearStatus = courseData.CourseComplete.Value;

        menuController.HideAllMenus();
        StartCoroutine(BeginCourse());
    }



    void OnPause(InputAction.CallbackContext context) {
        if (!CanPauseGame.Value || GamePaused.Value) {
            return;
        }

        PauseCourse();
    }

    public void PauseCourse() {
        GamePaused.SetValue(true);
        InputBuffer.ToggleActionMap(InputBuffer.PlayerInputActions.UI);
        menuController.EnableActions();
        menuController.ShowMenu("pause");
        Time.timeScale = 0;
    }

    public void UnpauseCourse() {
        GamePaused.SetValue(false);
        InputBuffer.ToggleActionMap(InputBuffer.PlayerInputActions.Player);
        menuController.ShowMenus("stage_info", "player_hud");
        Time.timeScale = 1;
    }

    public void RestartCourse() {
        Time.timeScale = 1;
        SceneController.ReloadCurrentScene();
    }

    public void ExitCourse(string nextSceneName) {
        Time.timeScale = 1;
        SceneController.LoadSceneByName(nextSceneName);
    }

    IEnumerator BeginCourse() {
        InputBuffer.ToggleActionMap(InputBuffer.PlayerInputActions.UI);
        menuController.ShowMenu("course_start");
        yield return StartCoroutine(countdown.Count());

        InputBuffer.ToggleActionMap(InputBuffer.PlayerInputActions.Player);
        menuController.DisableActions();
        menuController.ShowMenus("stage_info", "player_hud");
        GamePaused.SetValue(false);

        StartCoroutine(TrackCourseProgress());
    }

    IEnumerator TrackCourseProgress() {
        while (activeTargets.ItemCount > 0) {
            yield return null;
        }

        OnCourseCompleted();
    }

    IEnumerator ShowResultsMenus() {
        menuController.ShowMenu("course_complete");
        yield return StartCoroutine(courseClearSplash.DisplayUI());
        menuController.ShowMenu("results");
    }

    void OnCourseCompleted() {
        // Clear all active enemies if any remain
        if (activeEnemies.ItemCount > 0) {
            List<HitDetection.HealthController> enemies = activeEnemies.Items;

            for (int i = activeEnemies.ItemCount-1; i >= 0; --i) {
                Destroy(enemies[i].gameObject);
            }
        }

        InputBuffer.ToggleActionMap(InputBuffer.PlayerInputActions.UI);
        menuController.EnableActions();
        StartCoroutine(ShowResultsMenus());

        int completedTime = Mathf.FloorToInt(courseTimer.elapsedTime * 1000);
        if (!courseData.CourseComplete.Value) {
            courseData.CourseComplete.SetValue(true);
            courseData.BestTime.SetValue(completedTime);
            
            courseData._CourseComplete = true;
            courseData._BestTime = completedTime;

            // PersistentData.TrySaveCourseData(courseData);
        } else if (completedTime < courseData.BestTime.Value) {
            courseData.CourseComplete.SetValue(true);
            courseData.BestTime.SetValue(completedTime);

            courseData._CourseComplete = true;
            courseData._BestTime = completedTime;
            // PersistentData.TrySaveCourseData(courseData);
        }
    }
}
