using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public WallJump wj;

    public void changeOrientation()
    {
        wj.orientation.forward = Vector3.Slerp(wj.orientation.forward, wj.lastWallNormal, Time.deltaTime * 300f);
    }
}
