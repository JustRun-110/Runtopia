using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extentions
{
    public static void CrossFade(this Animator animatior, CrossFadeSettings settings)
    {
        animatior.CrossFade(
            settings.stateName,
            settings.transitionDuration,
            settings.layer,
            settings.timeOffset);
    }
    public static void CrossFadeInFixedTime(this Animator animatior, CrossFadeSettings settings)
    {
        animatior.CrossFadeInFixedTime(
            settings.stateName,
            settings.transitionDuration,
            settings.layer,
            settings.timeOffset);
    }
}
