using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictAngles : MonoBehaviour
{
    public float zRotationLimit;
    [Range(0F, 1F)]
    public float angleRestrictionHardness;
    [Range(0F, 1F)]
    public float maxAngleRestrictionInterpolation;
    public bool touchingGround;
    public float platformAngle;
    public bool debugRays;
    public PlatformController pController;
    public bool landingEdge;
    public Vector2 highestJumpPos;
    public LandingScript landingScript;
    public Color color;

    public void collisionUpdate(Collision2D col)
    {
        EdgeCollider2D edge;
        if (col.transform.TryGetComponent<EdgeCollider2D>(out edge))
        {
            touchingGround = true;
            if (col.contacts.Length > 0)
            {
                Vector2 contactPoint = col.GetContact(0).point;
                Platform plat = getPlatBelow(contactPoint);
                if (plat != null)
                {
                    platformAngle = plat.getAngle();
                    Color.RGBToHSV(color, out float h, out float s, out float v);
                    plat.sendPulse(-plat.transform.up, h, s);
                }
            }
            checkLanded();
        }
    }

    private void checkLanded()
    {
        if (touchingGround && !landingEdge)
        {
            landingScript.land();
        }
        else if (!touchingGround)
        {
            if (transform.position.y > highestJumpPos.y)
                highestJumpPos = transform.position;
        }
        landingEdge = touchingGround;
    }

    public void leaveGround() {
        touchingGround = false;
        landingEdge = touchingGround;
    }


    private Platform getPlatBelow(Vector2 point)
    {
        point += ((Vector2)this.transform.position - point) * 0.5F;
        RaycastHit2D hit = Physics2D.Raycast(point, -Vector3.up, pController.tileSize / 2, ~pController.layerMask);
        if (debugRays)
        {
            Debug.DrawRay(point, -Vector3.up * (pController.tileSize / 2), Color.white, 1F, false);
            if (hit.transform != null)
                Debug.Log(hit.transform.name + " | " + LayerMask.LayerToName(hit.transform.gameObject.layer) + "(" + this.transform.name + ", " + this.transform.position + ")");
            else Debug.Log("None.");
        }

        if (hit.transform != null)
            return hit.transform.GetComponent<Platform>();
        else return null;
    }

    public void restrictAngle(float platformAngle)
    {
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
        //Debug.Log(currentRotation.z + ". (clamped to " + clamped + ")");
        //transform.localEulerAngles = currentRotation;
        int sign = (clampedChange >= 0) ? -1 : 1;
        if (Mathf.Abs(clampedChange) > 0)
            GetComponent<Rigidbody2D>().angularVelocity = sign * angleRestrictionHardness * Mathf.Pow(Mathf.Abs(clampedChange), 2);

        sitFlat(platformAngle);
    }

    private void sitFlat(float platformAngle) {
        Vector3 angles = transform.localEulerAngles;
        if (Mathf.Abs(angles.z) < 0.5)
        {
            angles.z = 0;
            transform.localEulerAngles = angles;
        }
    }
}
