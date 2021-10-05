//This class is used to simulate a 3D "orbit" of 2D sprites around a 2D point, and is calculated entirely in 2 dimensions (no use of the Z axis, at all)
//It simulates a "3D" rotation with any combination of tilt, angle, size/width, or other rotational adjustments (all of which can be adjusted at runtime)
//It also has a multitude of visual effects built into it (alpha fade in at certain positions along the rotation, color adjustment throughout the rotation, etc.)
//^This Faux-3D rotation is accomplished by calculating where the particle would be in relation to the viewer, based on its orbital path
//Then it is drawn behind or in front of its reference point depending on what part of the orbital path each individual sprite is at

public class OrbitManagerScript : MonoBehaviour
{

    public enum ParticleOrbitDensity
    {
        Low,
        Normal,
        Packed
    }

    public enum OrbitRandomStates
    {
        None,
        SingleRandom,
        AllDifferent,
        ComeInAtMiddle,
        Rainbow,
        RainbowSolid
    }

    public enum OrbitDirection
    {
        Right,
        Left
    }

    public enum OrbitExtraActions
    {
        Nothing,
        TiltChangeOneDirection,
        NormalAngleChangeOneDirection,
        MovingUpOrDown,
        MovingLeftOrRight
    }

    private enum OrbitObjTypes
    {
        SinglePixel,
        String
    }
    
    private float[] tempFloats = new float[7];
    
    private void OrbitAdjustmentUpdateFunct()
    {
        for (int i = orbitPointsHere.Count - 1; i >= 0; i--)
        {
            for (int u = 0; u < orbitExtraAction[i].Count; u++)
            {
                switch (orbitExtraAction[i][u])
                {
                    case OrbitExtraActions.TiltChangeOneDirection:
                        if (shouldDoRandomColors[i] != OrbitRandomStates.ComeInAtMiddle && shouldDoRandomAlphas[i] != OrbitRandomStates.ComeInAtMiddle) //remove this if, if you want
                        {
                            tiltOfOrbit[i] += (orbitExtraActionNums[i][u] * Time.deltaTime);
                            if (Mathf.Approximately(orbitExtraActionMaximums[i][u], 0) == false)
                            {
                                if ((orbitExtraActionNums[i][u] < 0 && tiltOfOrbit[i] < orbitExtraActionMaximums[i][u]) || (orbitExtraActionNums[i][u] > 0 && tiltOfOrbit[i] > orbitExtraActionMaximums[i][u]))
                                {
                                    tiltOfOrbit[i] = orbitExtraActionMaximums[i][u];
                                    if (shouldReverseOrbitExtraAction[i][u])
                                    {
                                        orbitExtraActionMaximums[i][u] = orbitExtraActionMaximums[i][u] + (180.0f * Mathf.Sign(orbitExtraActionNums[i][u]) * -1);
                                        orbitExtraActionMaximums[i][u] = ClampLoopAngleOrTilt(orbitExtraActionMaximums[i][u]);
                                        orbitExtraActionNums[i][u] *= -1;
                                    }
                                }
                            }
                            tiltOfOrbit[i] = ClampLoopAngleOrTilt(tiltOfOrbit[i]);
                        }
                        break;
                    case OrbitExtraActions.NormalAngleChangeOneDirection:
                        if (shouldDoRandomColors[i] != OrbitRandomStates.ComeInAtMiddle && shouldDoRandomAlphas[i] != OrbitRandomStates.ComeInAtMiddle) //remove this if, if you want
                        {
                            angleOfOrbit[i] += (orbitExtraActionNums[i][u] * Time.deltaTime);
                            if (Mathf.Approximately(orbitExtraActionMaximums[i][u], 0) == false)
                            {
                                if ((orbitExtraActionNums[i][u] < 0 && angleOfOrbit[i] < orbitExtraActionMaximums[i][u]) || (orbitExtraActionNums[i][u] > 0 && angleOfOrbit[i] > orbitExtraActionMaximums[i][u]))
                                {
                                    angleOfOrbit[i] = orbitExtraActionMaximums[i][u];
                                    if (shouldReverseOrbitExtraAction[i][u])
                                    {
                                        orbitExtraActionMaximums[i][u] = orbitExtraActionMaximums[i][u] + (180.0f * Mathf.Sign(orbitExtraActionNums[i][u]) * -1);
                                        orbitExtraActionMaximums[i][u] = ClampLoopAngleOrTilt(orbitExtraActionMaximums[i][u]);
                                        orbitExtraActionNums[i][u] *= -1;
                                    }
                                }
                            }
                            angleOfOrbit[i] = ClampLoopAngleOrTilt(angleOfOrbit[i]);
                        }
                        break;
                    case OrbitExtraActions.MovingUpOrDown:
                        if (shouldDoRandomColors[i] != OrbitRandomStates.ComeInAtMiddle && shouldDoRandomAlphas[i] != OrbitRandomStates.ComeInAtMiddle) //remove this if, if you want
                        {
                            currentOrbitPositionAdjustment[i] = new Vector3(currentOrbitPositionAdjustment[i].x, currentOrbitPositionAdjustment[i].y + (orbitExtraActionNums[i][u] * Time.deltaTime), currentOrbitPositionAdjustment[i].z);
                            if (Mathf.Approximately(orbitExtraActionMaximums[i][u], 0) == false)
                            {
                                if ((orbitExtraActionNums[i][u] < 0 && currentOrbitPositionAdjustment[i].y < orbitExtraActionMaximums[i][u]) || (orbitExtraActionNums[i][u] > 0 && currentOrbitPositionAdjustment[i].y > orbitExtraActionMaximums[i][u]))
                                {
                                    currentOrbitPositionAdjustment[i] = new Vector3(currentOrbitPositionAdjustment[i].x, orbitExtraActionMaximums[i][u], currentOrbitPositionAdjustment[i].z);
                                    if (shouldReverseOrbitExtraAction[i][u])
                                    {
                                        orbitExtraActionMaximums[i][u] *= -1;
                                        orbitExtraActionNums[i][u] *= -1;
                                    }
                                }
                            }
                        }
                        break;
                    case OrbitExtraActions.MovingLeftOrRight:
                        if (shouldDoRandomColors[i] != OrbitRandomStates.ComeInAtMiddle && shouldDoRandomAlphas[i] != OrbitRandomStates.ComeInAtMiddle) //remove this if, if you want
                        {
                            currentOrbitPositionAdjustment[i] = new Vector3(currentOrbitPositionAdjustment[i].x + (orbitExtraActionNums[i][u] * Time.deltaTime), currentOrbitPositionAdjustment[i].y, currentOrbitPositionAdjustment[i].z);
                            if (Mathf.Approximately(orbitExtraActionMaximums[i][u], 0) == false)
                            {
                                if ((orbitExtraActionNums[i][u] < 0 && currentOrbitPositionAdjustment[i].x < orbitExtraActionMaximums[i][u]) || (orbitExtraActionNums[i][u] > 0 && currentOrbitPositionAdjustment[i].x > orbitExtraActionMaximums[i][u]))
                                {
                                    currentOrbitPositionAdjustment[i] = new Vector3(orbitExtraActionMaximums[i][u], currentOrbitPositionAdjustment[i].y, currentOrbitPositionAdjustment[i].z);
                                    if (shouldReverseOrbitExtraAction[i][u])
                                    {
                                        orbitExtraActionMaximums[i][u] *= -1;
                                        orbitExtraActionNums[i][u] *= -1;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            if (objectLockedOnTo[i] != null)
            {
                startingMiddleOrbitPointForEach[i] = objectLockedOnTo[i].transform.position + objectLockedOnToDisplacement[i];
                sortingOrderAndDistanceBetweenBackAndFront[i][0] = objectLockedOnTo[i].sortingOrder + objectLockedOntoSortingOrderDiff[i];
            }
            relativePositionForFirstParticle[i] += (((orbitDirectionForEach[i] == OrbitDirection.Left) ? -1 : 1) * speedOfOrbit[i] * Time.deltaTime);
            if (relativePositionForFirstParticle[i] < 0)
            {
                relativePositionForFirstParticle[i] = 2 + relativePositionForFirstParticle[i];
            }
            else
            {
                relativePositionForFirstParticle[i] %= 2.0f;
            }
            SetPositionOfParticles(i);
            for (int y = 0; y < orbitPointsHere[i].Count; y++)
            {
                if (orbitPointsHere[i][y] != null)
                {
                    tempFloats[6] = GetRelativePositionOfParticle(i, y) % 2;
                    if (shouldDoRandomColors[i] == OrbitRandomStates.ComeInAtMiddle)
                    {
                        if (((orbitDirectionForEach[i] == OrbitDirection.Right) ? tempFloats[6] >= 1.5f && tempFloats[6] - (speedOfOrbit[i] * Time.deltaTime) < 1.5f : tempFloats[6] <= 1.5f && tempFloats[6] + (speedOfOrbit[i] * Time.deltaTime) > 1.5f) && (Mathf.Approximately(potentialActivatedColors[i].r, orbitPointsHere[i][y].color.r) == false || Mathf.Approximately(potentialActivatedColors[i].g, orbitPointsHere[i][y].color.g) == false || Mathf.Approximately(potentialActivatedColors[i].b, orbitPointsHere[i][y].color.b) == false))
                        {
                            orbitPointsHere[i][y].color = new Color(potentialActivatedColors[i].r, potentialActivatedColors[i].g, potentialActivatedColors[i].b, orbitPointsHere[i][y].color.a);
                            bool gucciGucci = true;
                            int counter = 0;
                            while (counter < orbitPointsHere[i].Count)
                            {
                                if (Mathf.Approximately(orbitPointsHere[i][counter].color.r, potentialActivatedColors[i].r) == false || Mathf.Approximately(orbitPointsHere[i][counter].color.g, potentialActivatedColors[i].g) == false || Mathf.Approximately(orbitPointsHere[i][counter].color.b, potentialActivatedColors[i].b) == false)
                                {
                                    gucciGucci = false;
                                    counter = orbitPointsHere[i].Count;
                                }
                                counter++;
                            }
                            if (gucciGucci)
                            {
                                shouldDoRandomColors[i] = OrbitRandomStates.None;
                            }
                        }
                    }
                    else if (shouldDoRandomColors[i] != OrbitRandomStates.None)
                    {
                        if (shouldDoRandomColors[i] == OrbitRandomStates.SingleRandom)
                        {
                            if (y == 0)
                            {
                                if (Mathf.Approximately(potentialActivatedColors[i].r, rainbowGradientCurrentGoal[i].r) && Mathf.Approximately(potentialActivatedColors[i].g, rainbowGradientCurrentGoal[i].g) && Mathf.Approximately(potentialActivatedColors[i].b, rainbowGradientCurrentGoal[i].b))
                                {
                                    GetNewColor(i);
                                }
                                potentialActivatedColors[i] = new Color(potentialActivatedColors[i].r + ((Mathf.Approximately(rainbowGradientCurrentGoal[i].r, potentialActivatedColors[i].r)) ? 0 : ((Mathf.Approximately(Mathf.Sign(rainbowGradientCurrentGoal[i].r - potentialActivatedColors[i].r), Mathf.Sign(rainbowGradientCurrentGoal[i].r - (potentialActivatedColors[i].r + 0.2f * Time.deltaTime * Mathf.Sign(rainbowGradientCurrentGoal[i].r - potentialActivatedColors[i].r)))) == false) ? rainbowGradientCurrentGoal[i].r - potentialActivatedColors[i].r : 0.2f * Time.deltaTime * Mathf.Sign(rainbowGradientCurrentGoal[i].r - potentialActivatedColors[i].r))), potentialActivatedColors[i].g + ((Mathf.Approximately(rainbowGradientCurrentGoal[i].g, potentialActivatedColors[i].g)) ? 0 : ((Mathf.Approximately(Mathf.Sign(rainbowGradientCurrentGoal[i].g - potentialActivatedColors[i].g), Mathf.Sign(rainbowGradientCurrentGoal[i].g - (potentialActivatedColors[i].g + 0.2f * Time.deltaTime * Mathf.Sign(rainbowGradientCurrentGoal[i].g - potentialActivatedColors[i].g)))) == false) ? rainbowGradientCurrentGoal[i].g - potentialActivatedColors[i].g : 0.2f * Time.deltaTime * Mathf.Sign(rainbowGradientCurrentGoal[i].g - potentialActivatedColors[i].g))), potentialActivatedColors[i].b + ((Mathf.Approximately(rainbowGradientCurrentGoal[i].b, potentialActivatedColors[i].b)) ? 0 : ((Mathf.Approximately(Mathf.Sign(rainbowGradientCurrentGoal[i].b - potentialActivatedColors[i].b), Mathf.Sign(rainbowGradientCurrentGoal[i].b - (potentialActivatedColors[i].b + 0.2f * Time.deltaTime * Mathf.Sign(rainbowGradientCurrentGoal[i].b - potentialActivatedColors[i].b)))) == false) ? rainbowGradientCurrentGoal[i].b - potentialActivatedColors[i].b : 0.2f * Time.deltaTime * Mathf.Sign(rainbowGradientCurrentGoal[i].b - potentialActivatedColors[i].b))));
                            }
                            orbitPointsHere[i][y].color = new Color(potentialActivatedColors[i].r, potentialActivatedColors[i].g, potentialActivatedColors[i].b, orbitPointsHere[i][y].color.a);
                        }
                        else if (shouldDoRandomColors[i] == OrbitRandomStates.AllDifferent)
                        {
                            orbitPointsHere[i][y].color = new Color(Mathf.Clamp(Mathf.Abs(Mathf.Sin(Time.time * y * Mathf.PI)), 0, 1), Mathf.Clamp(Mathf.Abs(Mathf.Sin(Time.time * tempFloats[6] * Mathf.PI)), 0, 1), Mathf.Clamp(Mathf.Sin(Time.time * ((!string.IsNullOrEmpty(tagsForOrbits[i])) ? tagsForOrbits[i].Length : 2) * Mathf.PI), 0, 1), orbitPointsHere[i][y].color.a);
                        }
                        else if (shouldDoRandomColors[i] == OrbitRandomStates.Rainbow)
                        {
                            if (y == 0 || (Mathf.Abs(rainbowGradientCurrentGoal[i].r - orbitPointsHere[i][y - 1].color.r) <= 0.75f && Mathf.Abs(rainbowGradientCurrentGoal[i].g - orbitPointsHere[i][y - 1].color.g) <= 0.75f && Mathf.Abs(rainbowGradientCurrentGoal[i].b - orbitPointsHere[i][y - 1].color.b) <= 0.75f))
                            {
                                orbitPointsHere[i][y].color = new Color(Mathf.Clamp(orbitPointsHere[i][y].color.r + ((rainbowGradientCurrentGoal[i].r - potentialActivatedColors[i].r) * Time.deltaTime * 2), 0, 1), Mathf.Clamp(orbitPointsHere[i][y].color.g + ((rainbowGradientCurrentGoal[i].g - potentialActivatedColors[i].g) * Time.deltaTime * 2), 0, 1), Mathf.Clamp(orbitPointsHere[i][y].color.b + ((rainbowGradientCurrentGoal[i].b - potentialActivatedColors[i].b) * Time.deltaTime * 2), 0, 1), orbitPointsHere[i][y].color.a);
                            }
                            if (y == orbitPointsHere[i].Count - 1 && Mathf.Approximately(orbitPointsHere[i][y].color.r, rainbowGradientCurrentGoal[i].r) && Mathf.Approximately(orbitPointsHere[i][y].color.g, rainbowGradientCurrentGoal[i].g) && Mathf.Approximately(orbitPointsHere[i][y].color.b, rainbowGradientCurrentGoal[i].b))
                            {
                                potentialActivatedColors[i] = rainbowGradientCurrentGoal[i];
                                GetNewColor(i);
                            }
                        }
                        else if (shouldDoRandomColors[i] == OrbitRandomStates.RainbowSolid)
                        {
                            if (y == 0)
                            {
                                if (Mathf.Approximately(potentialActivatedColors[i].r, rainbowGradientCurrentGoal[i].r) && Mathf.Approximately(potentialActivatedColors[i].g, rainbowGradientCurrentGoal[i].g) && Mathf.Approximately(potentialActivatedColors[i].b, rainbowGradientCurrentGoal[i].b))
                                {
                                    GetNewColor(i);
                                }
                                potentialActivatedColors[i] = new Color(Mathf.Clamp(potentialActivatedColors[i].r + ((Mathf.Approximately(potentialActivatedColors[i].r, rainbowGradientCurrentGoal[i].r)) ? 0 : (Mathf.Sign(rainbowGradientCurrentGoal[i].r - potentialActivatedColors[i].r) * 0.2f * Time.deltaTime)), 0, 1), Mathf.Clamp(potentialActivatedColors[i].g + ((Mathf.Approximately(potentialActivatedColors[i].g, rainbowGradientCurrentGoal[i].g)) ? 0 : (Mathf.Sign(rainbowGradientCurrentGoal[i].g - potentialActivatedColors[i].g) * 0.2f * Time.deltaTime)), 0, 1), Mathf.Clamp(potentialActivatedColors[i].b + ((Mathf.Approximately(potentialActivatedColors[i].b, rainbowGradientCurrentGoal[i].b)) ? 0 : (Mathf.Sign(rainbowGradientCurrentGoal[i].b - potentialActivatedColors[i].b) * 0.2f * Time.deltaTime)), 0, 1));
                            }
                            orbitPointsHere[i][y].color = new Color(potentialActivatedColors[i].r, potentialActivatedColors[i].g, potentialActivatedColors[i].b, orbitPointsHere[i][y].color.a);
                        }
                    }
                    if (shouldDoRandomAlphas[i] == OrbitRandomStates.ComeInAtMiddle)
                    {
                        if (((orbitDirectionForEach[i] == OrbitDirection.Right) ? tempFloats[6] >= 1.5f && tempFloats[6] - (speedOfOrbit[i] * Time.deltaTime) < 1.5f : tempFloats[6] <= 1.5f && tempFloats[6] + (speedOfOrbit[i] * Time.deltaTime) > 1.5f) && Mathf.Approximately(orbitPointsHere[i][y].color.a, 0))
                        {
                            orbitPointsHere[i][y].color = new Color(orbitPointsHere[i][y].color.r, orbitPointsHere[i][y].color.g, orbitPointsHere[i][y].color.b, normalAlphaForOrbitPoints[i]);
                            if (orbitPointsHere[i][y].transform.childCount > 0)
                            {
                                SpriteRenderer holder = null;
                                for (int q = 0; q < orbitPointsHere[i][y].transform.childCount; q++)
                                {
                                    holder = orbitPointsHere[i][y].transform.GetChild(q).GetComponent<SpriteRenderer>();
                                    holder.color = new Color(holder.color.r, holder.color.g, holder.color.b, normalAlphaForOrbitPoints[i]);
                                }
                            }
                            bool gucciGucci = true;
                            int counter = 0;
                            while (counter < orbitPointsHere[i].Count)
                            {
                                if (Mathf.Approximately(orbitPointsHere[i][counter].color.a, 0) == true)
                                {
                                    gucciGucci = false;
                                    counter = orbitPointsHere[i].Count;
                                }
                                counter++;
                            }
                            if (gucciGucci)
                            {
                                shouldDoRandomAlphas[i] = OrbitRandomStates.None;
                            }
                        }
                    }
                    else if (shouldDoRandomAlphas[i] != OrbitRandomStates.None)
                    {
                        orbitPointsHere[i][y].color = new Color(orbitPointsHere[i][y].color.r, orbitPointsHere[i][y].color.g, orbitPointsHere[i][y].color.b, Mathf.Clamp((shouldDoRandomAlphas[i] == OrbitRandomStates.Rainbow) ? potentialMaxAlphas[i] * Mathf.Sin((y * 1.0f / orbitPointsHere[i].Count) * 0.5f * Mathf.PI) : potentialMaxAlphas[i] * 0.5f + (potentialMaxAlphas[i] * 0.5f * Mathf.Sin(Time.time + i + ((shouldDoRandomAlphas[i] == OrbitRandomStates.AllDifferent) ? y + (startingMiddleOrbitPointForEach[i].x + currentOrbitPositionAdjustment[i].x - orbitPointsHere[i][y].transform.position.x) + (startingMiddleOrbitPointForEach[i].y + currentOrbitPositionAdjustment[i].y - orbitPointsHere[i][y].transform.position.y) : 0))), 0, potentialMaxAlphas[i]));
                        if (orbitPointsHere[i][y].transform.childCount > 0)
                        {
                            SpriteRenderer holder = null;
                            for (int q = 0; q < orbitPointsHere[i][y].transform.childCount; q++)
                            {
                                holder = orbitPointsHere[i][y].transform.GetChild(q).GetComponent<SpriteRenderer>();
                                holder.color = new Color(holder.color.r, holder.color.g, holder.color.b, orbitPointsHere[i][y].color.a);
                            }
                        }
                    }
                }
            }
            if (orbitTimers[i] > 0.05f)
            {
                orbitCurrentTimers[i] -= Time.deltaTime;
                if (orbitCurrentTimers[i] <= 0)
                {
                    shouldDoRandomAlphas[i] = OrbitRandomStates.None;
                    bool readyToBeRemoved = true;
                    int counter = 0;
                    while (counter < orbitPointsHere[i].Count)
                    {
                        if (Mathf.Approximately(orbitPointsHere[i][counter].color.a, 0) == false)
                        {
                            readyToBeRemoved = false;
                            counter = orbitPointsHere[i].Count;
                        }
                        counter++;
                    }
                    if (readyToBeRemoved)
                    {
                        RemoveOrbitHere(i);
                    }
                    else
                    {
                        counter = 0;
                        while (counter < orbitPointsHere[i].Count)
                        {
                            orbitPointsHere[i][counter].color = new Color(orbitPointsHere[i][counter].color.r, orbitPointsHere[i][counter].color.g, orbitPointsHere[i][counter].color.b, Mathf.Clamp(orbitPointsHere[i][counter].color.a - Time.deltaTime, 0, 1));
                            if (orbitPointsHere[i][counter].transform.childCount > 0)
                            {
                                SpriteRenderer holder = null;
                                for (int q = 0; q < orbitPointsHere[i][counter].transform.childCount; q++)
                                {
                                    holder = orbitPointsHere[i][counter].transform.GetChild(q).GetComponent<SpriteRenderer>();
                                    holder.color = new Color(holder.color.r, holder.color.g, holder.color.b, orbitPointsHere[i][counter].color.a);
                                }
                            }
                            counter++;
                        }
                    }
                }
            }
        }
    }
    
    private float ClampMinMaxOrbitSpeed(float speedWanted)
    {
        return Mathf.Clamp(Mathf.Abs(speedWanted), 0.05f, 3f);
    }

    private float ClampOrbitWidth(float orbitWanted)
    {
        return Mathf.Clamp(Mathf.Abs(orbitWanted), 0.25f, 40);
    }

    private float ClampLoopAngleOrTilt(float angleOrTiltWanted)
    {
        return ((Mathf.Abs(angleOrTiltWanted) % 360.0f) * Mathf.Sign(angleOrTiltWanted));
    }

    private void GetNewColor(int whichOrbitIndex)
    {
        if (whichOrbitIndex >= 0 && whichOrbitIndex < shouldDoRandomColors.Count)
        {
            switch (shouldDoRandomColors[whichOrbitIndex])
            {
                case OrbitRandomStates.SingleRandom:
                    rainbowGradientCurrentGoal[whichOrbitIndex] = new Color(Random.Range(0.25f, 1.0f), Random.Range(0.25f, 1.0f), Random.Range(0.25f, 1.0f));
                    break;
                case OrbitRandomStates.AllDifferent:
                    rainbowGradientCurrentGoal[whichOrbitIndex] = new Color(Random.Range(40, 61) * 0.01f, Random.Range(40, 61) * 0.01f, Random.Range(40, 61) * 0.01f);
                    break;
                case OrbitRandomStates.Rainbow:
                    rainbowGradientCurrentGoal[whichOrbitIndex] = GetNextRainbowColor(rainbowGradientCurrentGoal[whichOrbitIndex]);
                    break;
                case OrbitRandomStates.RainbowSolid:
                    rainbowGradientCurrentGoal[whichOrbitIndex] = GetNextRainbowColor(rainbowGradientCurrentGoal[whichOrbitIndex]);
                    break;
            }
        }
    }

    private Color GetNextRainbowColor(Color currentCol)
    {
        Color tingReturning = new Color(1, 0, 0);
        if (Mathf.Approximately(currentCol.r, 1))
        {
            if (Mathf.Approximately(currentCol.g, 1))
            {
                tingReturning = new Color(1, 0, 0);
            }
            else if (Mathf.Approximately(currentCol.b, 1))
            {
                tingReturning = new Color(0, 0, 1);
            }
            else
            {
                tingReturning = new Color(1, 0, 1);
            }
        }
        else if (Mathf.Approximately(currentCol.g, 1))
        {
            if (Mathf.Approximately(currentCol.r, 1))
            {
                tingReturning = new Color(1, 0, 0);
            }
            else if (Mathf.Approximately(currentCol.b, 1))
            {
                tingReturning = new Color(0, 1, 0);
            }
            else
            {
                tingReturning = new Color(1, 1, 0);
            }
        }
        else if (Mathf.Approximately(currentCol.b, 1))
        {
            if (Mathf.Approximately(currentCol.r, 1))
            {
                tingReturning = new Color(0, 0, 1);
            }
            else if (Mathf.Approximately(currentCol.g, 1))
            {
                tingReturning = new Color(0, 1, 0);
            }
            else
            {
                tingReturning = new Color(0, 1, 1);
            }
        }
        return tingReturning;
    }

    private void SetPositionOfParticles(int whichOrbitSystem)
    {
        if (whichOrbitSystem >= 0 && whichOrbitSystem < orbitPointsHere.Count)
        {
            bool hasTilt = Mathf.Approximately(tiltOfOrbit[whichOrbitSystem], 0) == false;
            tempFloats[0] = relativePositionForFirstParticle[whichOrbitSystem];
            Vector3 positionHere = new Vector3(0, 0, 0);
            tempFloats[1] = Mathf.Sin(Mathf.Deg2Rad * angleOfOrbit[whichOrbitSystem]);
            tempFloats[2] = Mathf.Cos(Mathf.Deg2Rad * angleOfOrbit[whichOrbitSystem]);
            tempFloats[3] = 0;
            tempFloats[4] = (hasTilt) ? Mathf.Sin(Mathf.Deg2Rad * tiltOfOrbit[whichOrbitSystem]) : 0;
            tempFloats[5] = (hasTilt) ? Mathf.Cos(Mathf.Deg2Rad * tiltOfOrbit[whichOrbitSystem]) : 0;
            for (int i = 0; i < orbitPointsHere[whichOrbitSystem].Count; i++)
            {
                if (orbitPointsHere[whichOrbitSystem][i] != null)
                {
                    if (i > 0)
                    {
                        tempFloats[0] = GetRelativePositionOfParticle(whichOrbitSystem, i);
                        tempFloats[0] %= 2;
                    }
                    tempFloats[3] = Mathf.Cos(tempFloats[0] * Mathf.PI);
                    orbitPointsHere[whichOrbitSystem][i].sortingOrder = sortingOrderAndDistanceBetweenBackAndFront[whichOrbitSystem][0] + Mathf.RoundToInt(((tempFloats[0] >= 1.0f) ? 1 : -1) * Mathf.Sign(tempFloats[5]) * sortingOrderAndDistanceBetweenBackAndFront[whichOrbitSystem][1] * 0.5f);
                    positionHere.x = startingMiddleOrbitPointForEach[whichOrbitSystem].x + currentOrbitPositionAdjustment[whichOrbitSystem].x + (tempFloats[3] * maxWidthOfOrbit[whichOrbitSystem] * 0.5f * tempFloats[2]) + ((hasTilt) ? tempFloats[4] * Mathf.Sin(tempFloats[0] * Mathf.PI) * maxWidthOfOrbit[whichOrbitSystem] * 0.5f * tempFloats[1] : 0);
                    positionHere.y = startingMiddleOrbitPointForEach[whichOrbitSystem].y + currentOrbitPositionAdjustment[whichOrbitSystem].y + (tempFloats[3] * maxWidthOfOrbit[whichOrbitSystem] * 0.5f * tempFloats[1]) + ((hasTilt) ? tempFloats[4] * Mathf.Sin(tempFloats[0] * Mathf.PI * -1) * maxWidthOfOrbit[whichOrbitSystem] * 0.5f * tempFloats[2] : 0);
                    orbitPointsHere[whichOrbitSystem][i].transform.position = positionHere;
                    if (realisticRotationHere[whichOrbitSystem])
                    {
                        float yAxisRotation = (tempFloats[0] + 0.5f) * Mathf.PI * Mathf.Rad2Deg;
                        orbitPointsHere[whichOrbitSystem][i].transform.rotation = Quaternion.Euler(0, 0, angleOfOrbit[whichOrbitSystem]);
                        orbitPointsHere[whichOrbitSystem][i].transform.Rotate(0, yAxisRotation, 0, Space.Self);
                        orbitPointsHere[whichOrbitSystem][i].transform.Rotate(tiltOfOrbit[whichOrbitSystem] * -1, 0, 0, Space.Self);
                        if (orbitPointsHere[whichOrbitSystem][i].transform.childCount > 0)
                        {
                            SpriteRenderer thing = null;
                            for (int y = 0; y < orbitPointsHere[whichOrbitSystem][i].transform.childCount; y++)
                            {
                                thing = orbitPointsHere[whichOrbitSystem][i].transform.GetChild(y).GetComponent<SpriteRenderer>();
                                thing.sortingOrder = orbitPointsHere[whichOrbitSystem][i].sortingOrder + ((Mathf.Abs(180 - Mathf.Abs(yAxisRotation)) <= 89.9f) ? 1 : -1);
                            }
                        }
                    }
                    else if (orbitPointsHere[whichOrbitSystem][i].transform.childCount > 0)
                    {
                        SpriteRenderer thing = null;
                        for (int y = 0; y < orbitPointsHere[whichOrbitSystem][i].transform.childCount; y++)
                        {
                            thing = orbitPointsHere[whichOrbitSystem][i].transform.GetChild(y).GetComponent<SpriteRenderer>();
                            thing.sortingOrder = orbitPointsHere[whichOrbitSystem][i].sortingOrder - 1;
                        }
                    }
                }
            }
        }
    }

    private float GetRelativePositionOfParticle(int whichIndexOfSpriteLoop, int whichPointInLoop)
    {
        return (whichIndexOfSpriteLoop >= 0 && whichIndexOfSpriteLoop < orbitPointsHere.Count && whichPointInLoop >= 0 && whichPointInLoop < orbitPointsHere[whichIndexOfSpriteLoop].Count && orbitPointsHere[whichIndexOfSpriteLoop][whichPointInLoop] != null) ? relativePositionForFirstParticle[whichIndexOfSpriteLoop] + ((2.0f * whichPointInLoop) / orbitPointsHere[whichIndexOfSpriteLoop].Count) : 0;
    }
}
