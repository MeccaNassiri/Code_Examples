//A collection of utility math functions that can be accessed from anywhere in the project

public static class HelperF
{
#region 2D Math
    public static bool IsPointWithinPlaneSegment2D(List<Vector3> planeSegmentEdges, Vector3 pointChecking)
    {
        if (planeSegmentEdges != null && planeSegmentEdges.Count > 0)
        {
            Vector3 largestPoints = planeSegmentEdges[0];
            Vector3 smallestPoints = planeSegmentEdges[0];
            for (int i = 0; i < planeSegmentEdges.Count; i++)
            {
                if (planeSegmentEdges[i].x > largestPoints.x)
                {
                    largestPoints.x = planeSegmentEdges[i].x;
                }
                else if (planeSegmentEdges[i].x < smallestPoints.x)
                {
                    smallestPoints.x = planeSegmentEdges[i].x;
                }
                if (planeSegmentEdges[i].y > largestPoints.y)
                {
                    largestPoints.y = planeSegmentEdges[i].y;
                }
                else if (planeSegmentEdges[i].y < smallestPoints.y)
                {
                    smallestPoints.y = planeSegmentEdges[i].y;
                }
            }
            return IsPointOnLineSegment2D(new Vector3(largestPoints.x, 0, 0), new Vector3(smallestPoints.x, 0, 0), new Vector3(pointChecking.x, 0, 0)) && IsPointOnLineSegment2D(new Vector3(0, largestPoints.y, 0), new Vector3(0, smallestPoints.y, 0), new Vector3(0, pointChecking.y, 0));
        }
        return false;
    }

    public static bool IsPointOnLineSegment2D(Vector3 lineSegmentStart, Vector3 lineSegmentEnd, Vector3 pointChecking)
    {
        return CheckEqualityWithEpsilonMoE(DistanceBetweenTwoPoints2D(lineSegmentEnd, lineSegmentStart), DistanceBetweenTwoPoints2D(lineSegmentStart, pointChecking) + DistanceBetweenTwoPoints2D(lineSegmentEnd, pointChecking));
    }

    /// <summary>
    /// Will return -1 if the points are closer than the wanted distance, 0 if they are equal to the wanted distance, and 1 if they are farther than the wanted distance
    /// </summary>
    /// <param name="wantedDistance"></param>
    /// <param name="pointOne"></param>
    /// <param name="pointTwo"></param>
    /// <returns></returns>
    public static int CheckRelationBetweenPointsAndWantedDistance2D(float wantedDistance, Vector3 pointOne, Vector3 pointTwo)
    {
        float newRef = SquaredDistanceBetweenTwoPoints2D(pointOne, pointTwo) - (wantedDistance * wantedDistance);
        if (CheckEqualityWithEpsilonMoE(newRef, 0))
        {
            return 0;
        }
        else if (newRef > 0)
        {
            return 1;
        }
        return -1;
    }
    public static bool ArePointsCloserThanThisDistance2D(float wantedDistance, Vector3 pointOne, Vector3 pointTwo)
    {
        return (CheckRelationBetweenPointsAndWantedDistance2D(wantedDistance, pointOne, pointTwo) == -1);
    }
    public static bool ArePointsThisDistanceApart2D(float wantedDistance, Vector3 pointOne, Vector3 pointTwo)
    {
        return (CheckRelationBetweenPointsAndWantedDistance2D(wantedDistance, pointOne, pointTwo) == 0);
    }
    public static bool ArePointsFartherThanThisDistance2D(float wantedDistance, Vector3 pointOne, Vector3 pointTwo)
    {
        return (CheckRelationBetweenPointsAndWantedDistance2D(wantedDistance, pointOne, pointTwo) == 1);
    }

    /// <summary>
    /// This is faster than the normal distance function
    /// </summary>
    /// <param name="pointOne"></param>
    /// <param name="pointTwo"></param>
    /// <returns></returns>
    public static float SquaredDistanceBetweenTwoPoints2D(Vector3 pointOne, Vector3 pointTwo)
    {
        return (((pointTwo.x - pointOne.x) * (pointTwo.x - pointOne.x)) + ((pointTwo.y - pointOne.y) * (pointTwo.y - pointOne.y)));
    }

    public static float DistanceBetweenTwoPoints2D(Vector3 pointOne, Vector3 pointTwo)
    {
        return Mathf.Sqrt(SquaredDistanceBetweenTwoPoints2D(pointOne, pointTwo));
    }
    #endregion

    #region 3D Math
    public static bool IsPointWithinPlaneSegment(List<Vector3> planeSegmentEdges, Vector3 pointChecking)
    {
        if (planeSegmentEdges != null && planeSegmentEdges.Count > 0)
        {
            Vector3 largestPoints = planeSegmentEdges[0];
            Vector3 smallestPoints = planeSegmentEdges[0];
            for (int i = 0; i < planeSegmentEdges.Count; i++)
            {
                if (planeSegmentEdges[i].x > largestPoints.x)
                {
                    largestPoints.x = planeSegmentEdges[i].x;
                }
                else if (planeSegmentEdges[i].x < smallestPoints.x)
                {
                    smallestPoints.x = planeSegmentEdges[i].x;
                }
                if (planeSegmentEdges[i].y > largestPoints.y)
                {
                    largestPoints.y = planeSegmentEdges[i].y;
                }
                else if (planeSegmentEdges[i].y < smallestPoints.y)
                {
                    smallestPoints.y = planeSegmentEdges[i].y;
                }
                if (planeSegmentEdges[i].z > largestPoints.z)
                {
                    largestPoints.z = planeSegmentEdges[i].z;
                }
                else if (planeSegmentEdges[i].z < smallestPoints.z)
                {
                    smallestPoints.z = planeSegmentEdges[i].z;
                }
            }
            return IsPointOnLineSegment(new Vector3(largestPoints.x, 0, 0), new Vector3(smallestPoints.x, 0, 0), new Vector3(pointChecking.x, 0, 0)) && IsPointOnLineSegment(new Vector3(0, largestPoints.y, 0), new Vector3(0, smallestPoints.y, 0), new Vector3(0, pointChecking.y, 0)) && IsPointOnLineSegment(new Vector3(0, 0, largestPoints.z), new Vector3(0, 0, smallestPoints.z), new Vector3(0, 0, pointChecking.z));
        }
        return false;
    }
    
    public static bool IsPointOnLineSegment(Vector3 lineSegmentStart, Vector3 lineSegmentEnd, Vector3 pointChecking)
    {
        return CheckEqualityWithEpsilonMoE(DistanceBetweenTwoPoints(lineSegmentEnd, lineSegmentStart), DistanceBetweenTwoPoints(lineSegmentStart, pointChecking) + DistanceBetweenTwoPoints(lineSegmentEnd, pointChecking));
    }

    /// <summary>
    /// Will return -1 if the points are closer than the wanted distance, 0 if they are equal to the wanted distance, and 1 if they are farther than the wanted distance
    /// </summary>
    /// <param name="wantedDistance"></param>
    /// <param name="pointOne"></param>
    /// <param name="pointTwo"></param>
    /// <returns></returns>
    public static int CheckRelationBetweenPointsAndWantedDistance(float wantedDistance, Vector3 pointOne, Vector3 pointTwo)
    {
        float newRef = SquaredDistanceBetweenTwoPoints(pointOne, pointTwo) - (wantedDistance * wantedDistance);
        if (CheckEqualityWithEpsilonMoE(newRef, 0))
        {
            return 0;
        }
        else if (newRef > 0)
        {
            return 1;
        }
        return -1;
    }
    public static bool ArePointsCloserThanThisDistance(float wantedDistance, Vector3 pointOne, Vector3 pointTwo)
    {
        return (CheckRelationBetweenPointsAndWantedDistance(wantedDistance, pointOne, pointTwo) == -1);
    }
    public static bool ArePointsThisDistanceApart(float wantedDistance, Vector3 pointOne, Vector3 pointTwo)
    {
        return (CheckRelationBetweenPointsAndWantedDistance(wantedDistance, pointOne, pointTwo) == 0);
    }
    public static bool ArePointsFartherThanThisDistance(float wantedDistance, Vector3 pointOne, Vector3 pointTwo)
    {
        return (CheckRelationBetweenPointsAndWantedDistance(wantedDistance, pointOne, pointTwo) == 1);
    }

    /// <summary>
    /// This is faster than the normal distance function
    /// </summary>
    /// <param name="pointOne"></param>
    /// <param name="pointTwo"></param>
    /// <returns></returns>
    public static float SquaredDistanceBetweenTwoPoints(Vector3 pointOne, Vector3 pointTwo)
    {
        return (((pointTwo.x - pointOne.x) * (pointTwo.x - pointOne.x)) + ((pointTwo.y - pointOne.y) * (pointTwo.y - pointOne.y)) + ((pointTwo.z - pointOne.z) * (pointTwo.z - pointOne.z)));
    }

    public static float DistanceBetweenTwoPoints(Vector3 pointOne, Vector3 pointTwo)
    {
        return Mathf.Sqrt(SquaredDistanceBetweenTwoPoints(pointOne, pointTwo));
    }

    /// <summary>
    /// Angles should be passed in using degrees (generally accessed by going to an object's transform and calling the localEulerAngles property)
    /// </summary>
    /// <param name="pointRotating"></param>
    /// <param name="pointRotatingAround"></param>
    /// <param name="anglesRotating"></param>
    /// <returns></returns>
    public static Vector3 RotatePointAroundPivot(Vector3 pointRotating, Vector3 pointRotatingAround, Vector3 anglesRotating)
    {
        return (Quaternion.Euler(anglesRotating) * (pointRotating - pointRotatingAround)) + pointRotatingAround;
    }

    /// <summary>
    /// Angles should be passed in using degrees (generally accessed by going to an object's transform and calling the localEulerAngles property)
    /// </summary>
    /// <param name="pointsRotating"></param>
    /// <param name="pointRotatingAround"></param>
    /// <param name="anglesRotating"></param>
    public static void RotateListOfPointsAroundPivot(List<Vector3> pointsRotating, Vector3 pointRotatingAround, Vector3 anglesRotating)
    {
        if (pointsRotating != null)
        {
            for (int i = 0; i < pointsRotating.Count; i++)
            {
                pointsRotating[i] = RotatePointAroundPivot(pointsRotating[i], pointRotatingAround, anglesRotating);
            }
        }
    }
    #endregion
    
    #region Error Checking
    public static bool CheckEqualityWithEpsilonMoE(float valOne, float valTwo)
    {
        return (Mathf.Abs(valOne - valTwo) <= Mathf.Epsilon);
    }
    #endregion
}
