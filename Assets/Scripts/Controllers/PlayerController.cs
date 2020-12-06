using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using static UnityEngine.ParticleSystem;

public class PlayerController : MonoBehaviour
{
    public float moveForce;
    [Range(0.0f, 180.0F)]
    public float zRotationLimit;
    private Vector3 startRotation;
    [Range(0.0f, 10)]
    public float tiltSensitivity;
    [Range(0F, 1F)]
    public float angleRestrictionHardness;
    [Range(0F, 1F)]
    public float maxAngleRestrictionInterpolation;
    [Range(0F, 10F)]
    public float maxVelocityX;
    public enum MobileControlType { Tilt = 0, Touch = 1 }
    public MobileControlType mobileControls = MobileControlType.Tilt;
    public bool touchingGround;
    public PlatformController pController;
    public bool debugRays;
    public float platformAngle;
    private float touchBeginTime;
    public bool landingParticlesEnabled;
    public bool disableShadowsOnMobile;

    private bool landingEdge;
    public float landingHeight;
    private Vector3 highestJumpPos;
    public GameObject landingParticles;
    public bool disableControls;
    public char activeFocus = ' ';
    public GameObject interactionGO;
    public bool emulateMobile;
    public MobileUIManager mobileUI;
    private bool pullingOpposite;
    private bool interactionRisingEdge;

    public Dictionary<char, GameObject> interactions;

    // Start is called before the first frame update
    void Start()
    {
        startRotation = this.transform.localEulerAngles;

        if (Application.isMobilePlatform)
            GetComponentInChildren<ShadowCaster2D>().enabled = false;

        interactions = new Dictionary<char, GameObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        doInput();

        restrictAngle();
        restrictVelocity();
        removeDeadParticles();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //Platform collidedPlat;
        //if (col.transform.TryGetComponent<Platform>(out collidedPlat)) {
        //    isTouchingGround(col);
        //    collidedPlat.sendPulse(-collidedPlat.transform.up);
        //}

        collisionUpdate(col, true);
    }

    void OnCollisionStay2D(Collision2D col) {
        collisionUpdate(col, false);
    }

    void OnCollisionExit2D(Collision2D col) {
        collisionUpdate(col, false);
    }

    private void collisionUpdate(Collision2D col, bool entering) {
        EdgeCollider2D edge;
        if (col.transform.TryGetComponent<EdgeCollider2D>(out edge))
        {
            if(entering)
                touchingGround = true;
            if (col.contacts.Length > 0)
            {
                Vector2 contactPoint = col.GetContact(0).point;
                Platform plat = getPlatBelow(contactPoint);
                if (plat != null)
                {
                    platformAngle = plat.getAngle();
                    plat.sendPulse(-plat.transform.up, 0.7F, 1F);
                }
            }
            checkLanded();
        }
    }

    private Platform getPlatBelow(Vector2 point) {
        point += ((Vector2)this.transform.position - point) * 0.5F;
        RaycastHit2D hit = Physics2D.Raycast(point, -Vector3.up, pController.tileSize / 2, ~pController.layerMask);
        if (debugRays) {
            Debug.DrawRay(point, -Vector3.up * (pController.tileSize / 2), Color.magenta, 1F, false);
            if (hit.transform != null)
                Debug.Log(hit.transform.name + " | " + LayerMask.LayerToName(hit.transform.gameObject.layer) + "(" + this.transform.name + ")");
            else Debug.Log("None.");
        }

        if (hit.transform != null)
            return hit.transform.GetComponent<Platform>();
        else return null;
    }

    private void restrictVelocity() {
        if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > maxVelocityX) {
            Vector2 v = GetComponent<Rigidbody2D>().velocity;
            GetComponent<Rigidbody2D>().velocity = new Vector2(v.x >= 0 ? maxVelocityX : -maxVelocityX, v.y);
        }
    }

    private void restrictAngle() {
        Vector2 bounds = new Vector2(zRotationLimit + platformAngle, -zRotationLimit + platformAngle);
        //order
        if (bounds.x > bounds.y)
        {
            float temp = bounds.x;
            bounds.x = bounds.y;
            bounds.y = temp;
        }
        //Debug.Log("Angle bounds: " + bounds);

        Vector3 currentRotation = transform.localRotation.eulerAngles;
        float oldRot;
        //Debug.Log(currentRotation.z + " > ");
        float clamped;
        float clampedChange;
        if (currentRotation.z > 180)
        {
            oldRot = currentRotation.z - 360;
            clamped = Mathf.Clamp(currentRotation.z - 360, bounds.x, bounds.y);
            clampedChange = oldRot - clamped;
        }
        else
        {
            oldRot = currentRotation.z;
            clamped = Mathf.Clamp(currentRotation.z, bounds.x, bounds.y);
            clampedChange = oldRot - clamped;
        }
        int sign = (clampedChange >= 0) ? -1 : 1;
        if (Mathf.Abs(clampedChange) > 0)
            GetComponent<Rigidbody2D>().angularVelocity = sign * angleRestrictionHardness * Mathf.Pow(Mathf.Abs(clampedChange), 2);
             
        sitFlat();
    }

    private void sitFlat() {
        Vector3 angles = transform.localEulerAngles;
        if (Mathf.Abs(angles.z) < 0.5) {
            angles.z = 0;
            transform.localEulerAngles = angles;
        }
    }

    [System.Obsolete]
    private void isTouchingGround(Collision2D col) {
        RaycastHit2D hit;
        ContactPoint2D[] contacts = new ContactPoint2D[5];
        int size = col.GetContacts(contacts);
        touchingGround = false;
        for(int c = 0; c < size; c++)
        {
            ContactPoint2D contact = contacts[c];
            float shift = Physics2D.defaultContactOffset * 10;
            float rayLength = shift + Physics2D.defaultContactOffset * 2;

            //Shift the contact point up slightly to make sure it doesn't start inside the platform
            //Also introduces a margin of error, useful because physics and colliders are exact, and we don't want that for this case
            hit = Physics2D.Raycast(contact.point + new Vector2(0, shift), -Vector3.up, rayLength);

            debugRay(hit, contact.point + new Vector2(0, shift), rayLength);

            if (hit.transform != null && hit.collider.TryGetComponent<Platform>(out Platform p))
            {
                touchingGround = true;
                platformAngle = p.getAngle();
            }
        }
        checkLanded();
    }

    private void debugRay(RaycastHit2D hit, Vector2 start, float rayLength){
        if (debugRays)
        {
            Debug.DrawRay(start, -Vector3.up * rayLength, Color.white, 3F, false);
            Debug.Log(start + ": " + (hit.transform != null ? "Hit!" : "No hit."));
        }
    }

    private void jump() {
        if (touchingGround) {
            highestJumpPos = transform.position;
            GetComponent<Rigidbody2D>().AddForce(transform.up * moveForce * 10);
            touchingGround = false;
        }
    }

    private void checkLanded() {
        if (touchingGround && !landingEdge)
        {
            land();
        }
        else if(!touchingGround) {
            if (transform.position.y > highestJumpPos.y)
                highestJumpPos = transform.position;
        }
        landingEdge = touchingGround;
    }

    private void land() {
        float height = highestJumpPos.y - transform.position.y;

        //particles
        if (height > 0.015 && landingParticlesEnabled)
        {
            int particleMultiplier = 750;
            GameObject right = Instantiate(landingParticles, this.transform);
            EmissionModule ems = right.GetComponent<ParticleSystem>().emission;
            ems.rateOverTime = height * particleMultiplier;
            right.transform.SetParent(transform.parent, true);
            right.transform.localScale = new Vector3(1, 1, 1);
            right.transform.localEulerAngles = new Vector3(0, 0, platformAngle);

            GameObject left = Instantiate(landingParticles, this.transform);
            EmissionModule emsL = left.GetComponent<ParticleSystem>().emission;
            emsL.rateOverTime = height * particleMultiplier;
            left.transform.SetParent(transform.parent, true);
            left.transform.localScale = new Vector3(-1, 1, 1);
            left.transform.localEulerAngles = new Vector3(0, 0, platformAngle);
        }

        highestJumpPos = new Vector3();
    }

    private void removeDeadParticles() {
        foreach(GameObject gO in GameObject.FindGameObjectsWithTag("Particles")) {
            ParticleSystem p = gO.GetComponent<ParticleSystem>();
            if (!p.IsAlive(false))
            {
                Destroy(p.gameObject);
            }
        }
    }

    private void doInput() {
        bool remoteEditor = false;
        #if UNITY_EDITOR
            remoteEditor = UnityEditor.EditorApplication.isRemoteConnected;
#endif


        if (!disableControls)
        {
            if (!emulateMobile && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer) && !remoteEditor)
            {
                if (Input.GetKey(KeyCode.D))
                {
                    GetComponent<Rigidbody2D>().AddForce(1 * moveForce * transform.right);
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    GetComponent<Rigidbody2D>().AddForce(-1 * moveForce * transform.right);
                }
                else if (Input.GetKey(KeyCode.W))
                {
                    jump();
                }

                checkFocusKeys();
                
            }
            else if(isMobile())
            {
                if (mobileControls == MobileControlType.Tilt)
                {
                    Vector3 movement = transform.right * Input.acceleration.x;
                    GetComponent<Rigidbody2D>().AddForce(movement * moveForce * tiltSensitivity);
                }
                else if (mobileControls == MobileControlType.Touch)
                {
                    if (activeFocus == ' ')
                    {
                        if (Input.touches.Length > 0)
                        {
                            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved)
                                if (Input.GetTouch(0).position.x >= 5 * (Screen.width / 8))
                                    GetComponent<Rigidbody2D>().AddForce(1 * moveForce * transform.right);
                                else if (Input.GetTouch(0).position.x <= 3 * (Screen.width / 8))
                                    GetComponent<Rigidbody2D>().AddForce(-1 * moveForce * transform.right);
                        }
                        else if (Input.GetMouseButton(0))
                        {
                            if (Input.mousePosition.x >= 5 * (Screen.width / 8))
                                GetComponent<Rigidbody2D>().AddForce(1 * moveForce * transform.right);
                            else if (Input.mousePosition.x <= 3 * (Screen.width / 8))
                                GetComponent<Rigidbody2D>().AddForce(-1 * moveForce * transform.right);
                        }
                    }

                }

                if (Input.touches.Length > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        touchBeginTime = Time.time;
                    }

                    if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        if (Time.time - touchBeginTime < 0.3F)
                            jump();
                    }
                }
            }
        }
    }

    private bool isMobile() {
        bool remoteEditor = false;
        #if UNITY_EDITOR
                remoteEditor = UnityEditor.EditorApplication.isRemoteConnected;
        #endif
        return (emulateMobile || Application.platform == RuntimePlatform.Android || remoteEditor || Application.platform == RuntimePlatform.IPhonePlayer);
    }

    private void checkFocusKeys() {
            IEnumerator ienumerator = interactions.Keys.GetEnumerator();
            while (ienumerator.MoveNext())
            {
                char c = (char)ienumerator.Current;
            if (Input.GetKey((KeyCode)char.ConvertToUtf32(c + "", 0)))
                tryInteract(c);
            else tryStopInteract(c);
            }
    }

    public void addInteraction(char c, GameObject gO) {
        interactions.Add(c, gO);
        if (isMobile()) {
            mobileUI.createButton(gO);
        }
    }

    public void removeInteraction(char c) {
        GameObject focus;
        if (isMobile()) {
            if(interactions.TryGetValue(c, out focus))
                mobileUI.removeButton(focus);
        }
        tryStopInteract(c);
        interactions.Remove(c);
    }

    public void tryInteract(char c) {
        if (interactions.TryGetValue(c, out interactionGO))
        {
            if (activeFocus == ' ' && !interactionRisingEdge)
                interactionRisingEdge = true;
            else interactionRisingEdge = false;

            activeFocus = c;

            KeyPopup.Interactions typeOf = interactionGO.GetComponentInChildren<KeyPopup>().interaction;
            if (typeOf == KeyPopup.Interactions.Focus)
                Camera.main.GetComponent<CameraFollow>().follow = interactionGO;
            else if (typeOf == KeyPopup.Interactions.PullOpposite && interactionRisingEdge)
                pullOpposite(interactionGO.GetComponent<KeyPopup>().objectToPull);
        }
    }

    public void tryStopInteract(char c) {
        if (activeFocus == c) {
            activeFocus = ' ';

            KeyPopup.Interactions typeOf = interactionGO.GetComponentInChildren<KeyPopup>().interaction;
            if (typeOf == KeyPopup.Interactions.Focus)
                Camera.main.GetComponent<CameraFollow>().follow = this.gameObject;
            else if (typeOf == KeyPopup.Interactions.PullOpposite) { }
                //stopPullOpposite(interactionGO.GetComponent<KeyPopup>().objectToPull);
        }
    }

    private void pullOpposite(GameObject objectToPull) {
        if (!pullingOpposite)
        {
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), objectToPull.GetComponent<Collider2D>());
            float dist = Mathf.Abs(this.gameObject.transform.position.x - objectToPull.transform.position.x);
            Vector3 heading = this.transform.position - objectToPull.transform.position;
            heading = Vector3.Normalize(heading);
            heading = new Vector3(heading.x, 0);
            pullingOpposite = true;
            StartCoroutine(PullOpposite(objectToPull, dist, this.gameObject.transform.position.x >= objectToPull.transform.position.x, heading));
            Debug.Log("Starting pull. " + "d: " + dist + ", " + heading);
        }
    }

    private void stopPullOpposite(GameObject interactable)
    {
        Debug.Log("Stopping pull.");
        pullingOpposite = false;
        Rigidbody2D rb;
        if (interactable.TryGetComponent<Rigidbody2D>(out rb))
        {
            rb.velocity = rb.velocity * 0F;
        }
        Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), interactable.GetComponent<Collider2D>(), false);
    }

    IEnumerator PullOpposite(GameObject interactable, float distanceToGoal, bool moveRight, Vector3 heading)
    {
        while (pullingOpposite)
        {
            Rigidbody2D rb;
            if (interactable.TryGetComponent<Rigidbody2D>(out rb))
            {
                Debug.Log("Current dist: " + Mathf.Abs(this.gameObject.transform.position.x - interactable.transform.position.x) + " " + (Mathf.Abs(this.gameObject.transform.position.x - interactable.transform.position.x) >= distanceToGoal));
                if ((Mathf.Abs(this.gameObject.transform.position.x - interactable.transform.position.x) >= distanceToGoal) && (moveRight ? interactable.transform.position.x > this.gameObject.transform.position.x : interactable.transform.position.x <= this.gameObject.transform.position.x))
                {
                    Debug.Log("Stopping A");
                    stopPullOpposite(interactable);
                }
                else {
                    // rb.AddForce(heading * Time.time);
                    rb.velocity = heading;
                }
            }
            else {
                Debug.Log("Stopping B");
                stopPullOpposite(interactable);
            }
            yield return new WaitForSeconds(.1f);
        }
        yield return null;
    }
}
