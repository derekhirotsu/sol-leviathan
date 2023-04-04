using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Default Values:
//      Intensity     - 0.7
//      Flip          - 1.0
public class GlitchController : MonoBehaviour
{
    [SerializeField] protected bool glitchActive = false;

    protected float timeToToggle = 0f;
    protected Vector2 glitchActiveTimeRange = new Vector2(2f, 3f); // How long an active glitch can last (in seconds). Should be relatively short.
    protected Vector2 glitchInactiveTimeRange = new Vector2(12f, 20f); // How long the glitch remains inactive (in seconds). Should be long.
    GlitchEffect glitchEffect;

    IEnumerator glitchCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        glitchEffect = this.GetComponent<GlitchEffect>();
        glitchEffect.enabled = false;

        glitchCoroutine = ContinuousGlitchToggle();
        StartCoroutine(glitchCoroutine);

        
    }

    // Toggles the glitch effect on or off. Default values may be overridden.
    public void ToggleGlitch(bool enable = false, float intensity = 0.7f, float flip = 1.0f) {
        glitchEffect.enabled = enable;

        glitchEffect.intensity = intensity;
        glitchEffect.flipIntensity = flip;
    }

    // Interrupts the continuous glitch coroutine and toggles the effect on for brief moment.
    public void SingleGlitchToggle() {
        // Interrrupt the continuous coroutine.
        if (glitchCoroutine != null) {
            StopCoroutine(glitchCoroutine);
        }

        // Activate a single glitch toggle.
        glitchCoroutine = SingleToggle();
        StartCoroutine(glitchCoroutine);
    }

    protected IEnumerator SingleToggle() {
        
        // Toggle the effect on.
        ToggleGlitch(true);
        timeToToggle = Random.Range(glitchActiveTimeRange.x, glitchActiveTimeRange.y);

        // Wait until the effect is complete.
        while (timeToToggle >= 0) {
            timeToToggle -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        // Restart the continuous coroutine
        glitchCoroutine = ContinuousGlitchToggle();
        StartCoroutine(glitchCoroutine);
        
    }

    protected IEnumerator ContinuousGlitchToggle() {
        // When we first enter the loop, we want to effect to be 'toggled' off.
        timeToToggle = 0f;
        glitchActive = true;

        while (true) {

            // Time has expired.
            // Check active glitch state, and toggle to the opposite using appropriate time range.
            if (timeToToggle <= 0) {

                if (glitchActive) {
                    // CASE : Toggle off.
                    timeToToggle = Random.Range(glitchInactiveTimeRange.x, glitchInactiveTimeRange.y);
                }

                else {
                    // CASE : Toggle on.
                    timeToToggle = Random.Range(glitchActiveTimeRange.x, glitchActiveTimeRange.y);
                }

                glitchActive = !glitchActive;
                ToggleGlitch(glitchActive);

            }


            timeToToggle -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    // An event that resets the glitch timer if any user input is detected
    protected void ResetTimer() {
        // Reset timer
        timeToToggle = Random.Range(glitchInactiveTimeRange.x, glitchInactiveTimeRange.y);

        // If active, set to inactive.
        if (glitchActive) {
            glitchActive = !glitchActive;
            ToggleGlitch(glitchActive);
        }

    }

    
}
