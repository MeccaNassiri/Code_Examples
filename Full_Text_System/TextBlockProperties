using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBlockProperties : MonoBehaviour {
    
    //IN THE PROJECT, THIS FUNCTIONALITY IS PART OF THE SAME CLASS AS THE OTHER TEXT FUNCTIONALITY
    //HOWEVER, I HAVE MOVED IT FOR THE SAKE OF READABILITY
    //IF THERE IS ANY VARIABLE/FUNCTION CALLED HERE THAT ISN'T PRESENT, IT IS BECAUSE IT IS MOST LIKELY IN THE FULL TEXT CLASS
    //WHICH HASN'T BEEN WRITTEN HERE
    
    /// <summary>
    /// Use -1 or 0 for whichLineWidth if you just want the longest line in the block, put anything > 0 if you want a specific line (it won't return an error if that block doesn't have that many lines)
    /// Example: Line 1 would be 1, line 7 would be 7
    /// </summary>
    public float GetWidthOfTextBlock(string tagOfBlockToUse, int whichLineWidth)
    {
        float widthToReturn = 0;
        if (tagOfBlockToUse != null && tagOfBlockToUse != string.Empty)
        {
            int counter = 0;
            while (counter < textBlocksActive.Count)
            {
                //first we check if the tag is valid
                //then we check to see if a block exists with that tag
                if (tagForThisBlock[counter] == tagOfBlockToUse)
                {
                    int innerCounter = 1;
                    int lineOnCurrent = 1;
                    SpriteRenderer leftFirst = lettersActive[counter][0][0];
                    SpriteRenderer rightFirst = lettersActive[counter][0][lettersActive[counter][0].Count - 1];
                    SpriteRenderer tempLeftFirst = leftFirst;
                    SpriteRenderer tempRightFirst = rightFirst;
                    //what we do here is get the farthest left and right letters in the first row
                    //then we create temp variables for comparison
                    //then we loop through the rows, and check the farthest left and right variables to see if the distance
                    //between them is longer than the ones we have stored
                    //if it is, we set the new ones to leftFirst and rightFirst, and move on to the next row
                    //we then return the difference between the x positions of these two objects at the end of the function
                    while (innerCounter < lettersActive[counter].Count)
                    {
                        if (lettersActive[counter][innerCounter][0].transform.position.x <= tempRightFirst.transform.position.x)//this was the old version to the right (didn't account for wave and shake)//y < tempLeftFirst.transform.position.y - (tempLeftFirst.bounds.size.y * 0.8f))
                        {
                            lineOnCurrent++;
                            if (whichLineWidth <= 0 || (whichLineWidth > 0 && lineOnCurrent <= whichLineWidth))
                            {
                                if ((tempRightFirst.transform.position.x + tempRightFirst.bounds.extents.x) - (tempLeftFirst.transform.position.x - tempLeftFirst.bounds.extents.x) > (rightFirst.transform.position.x + rightFirst.bounds.extents.x) - (leftFirst.transform.position.x - leftFirst.bounds.extents.x))
                                {
                                    leftFirst = tempLeftFirst;
                                    rightFirst = tempRightFirst;
                                }
                                tempLeftFirst = lettersActive[counter][innerCounter][0];
                                tempRightFirst = lettersActive[counter][innerCounter][lettersActive[counter][innerCounter].Count - 1];
                            }
                            else
                            {
                                innerCounter = lettersActive[counter].Count;
                            }
                        }
                        else
                        {
                            tempRightFirst = lettersActive[counter][innerCounter][lettersActive[counter][innerCounter].Count - 1];
                        }
                        innerCounter++;
                    }
                    if ((tempRightFirst.transform.position.x + tempRightFirst.bounds.extents.x) - (tempLeftFirst.transform.position.x - tempLeftFirst.bounds.extents.x) > (rightFirst.transform.position.x + rightFirst.bounds.extents.x) - (leftFirst.transform.position.x - leftFirst.bounds.extents.x))
                    {
                        leftFirst = tempLeftFirst;
                        rightFirst = tempRightFirst;
                    }
                    widthToReturn = (rightFirst.transform.position.x + rightFirst.bounds.extents.x) - (leftFirst.transform.position.x - leftFirst.bounds.extents.x);
                    counter = textBlocksActive.Count;
                }
                counter++;
            }
        }
        return widthToReturn;
    }

    //what we do in this function is to grab the first and last letters in the word (so farthest left in top row, and farthest right in bottom row)
    //and from there, we simply subtract the y position of the lowest point on the last letter
    //from the y position of the highest point on the first letter
    public float GetHeightOfTextBox(string tagOfBlockToUse)
    {
        float heightToReturn = 1;
        if (tagOfBlockToUse != null && tagOfBlockToUse != string.Empty)
        {
            int counter = 0;
            while (counter < textBlocksActive.Count)
            {
                if (tagForThisBlock[counter] == tagOfBlockToUse)
                {
                    SpriteRenderer firstLetter = lettersActive[counter][0][0];
                    SpriteRenderer lastLetter = lettersActive[counter][lettersActive[counter].Count - 1][lettersActive[counter][lettersActive[counter].Count - 1].Count - 1];
                    heightToReturn = (firstLetter.transform.position.y + firstLetter.bounds.extents.y) - (lastLetter.transform.position.y - lastLetter.bounds.extents.y);
                    counter = textBlocksActive.Count;
                }
                counter++;
            }
        }
        return heightToReturn;
    }
}
