using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayText : MonoBehaviour {
    
    //THIS IS A FUNCTION FOR DISPLAYING TEXT IN THE GAME
    //IT RENDERS TEXT IN WORLD-SPACE AS A LIST OF OBJECTS, AND TAKES IN NUMEROUS ARGUMENTS TO GIVE
    //EACH WORD/CHARACTER DIFFERENT QUALITIES
    //ALSO ALLOWS YOU TO GROUP TEXT BLOCKS BY TAGS/GROUPS FOR EASY REFERENCE LATER
    
    //THERE IS A FUNCTION BELOW THAT USES THIS FUNCTION, BUT ALLOWS YOU TO WRITE IN THE COLOR/TEXT EFFECTS
    //USING SHORTHAND
    //IT'S CALLED ParseAndDisplayCertainText
    
    /// <summary>
    /// (1) "goldy" is for the glowing golden color to be displayed on certain letters, "bluey" is for the glowing blue talent color to be displayed on certain letters (they are used to modulate color in a different script)
    /// (2) in extraColorToDisplay, put whatever you want, putting {2} at start of string will apply your chosen color to the second instance of that string if present, {3} is the 3rd, etc.
    /// (3) same thing for wavey ("bruuuh" for both just sets all the string to true for whatever)
    /// (4) optionalTagsForSectionToClearLater is for clearing entire sections like "Stats", or "Selections"
    /// (5) optionalGroupsForSectionToClearLater is for grouping things, so that it allows you to use tags as smaller subsections in larger, malleable groups like "Unit Menu Screen 1"
    /// </summary>
    public void DisplayText(string thingToDisplay, float sizeOfText, Vector3 absolutePosition, float widthBoi, AlignmentForText alignment, LayerNames layName, int layerOrder, List<TextColors> extraColorToDisplay, List<string> partOfStringToShowColor, List<bool> trueForRSWaveFalseForAngry, List<string> stringForRSWaveOrAngry, string optionalTagForSectionsToClearLater, string optionalGroupForSectionsToClearLater)
    {
        //first we check if string is valid (not empty)
        //A NOTE: all letters have a child object slightly offset from them and colored black
        //this gives them a shadow effect to simulate depth
        //any change done to the letter object, is also done to the child object (except for changing color values other than alpha)
        if (thingToDisplay != string.Empty && thingToDisplay.Length > 0)
        {
            int counter = 0;
            string[] ting = thingToDisplay.Split(" "[0]); //split the words up
            string layerName = layName.ToString();
            Dictionary<SpriteRenderer, Vector3> dicto = new Dictionary<SpriteRenderer, Vector3>();
            //now we grab a text block and set its position
            //the text block will be the parent object by which we can easily move all the child letters later
            GameObject textBlockHere = GetTextBlockFromStack();
            textBlockHere.transform.position = absolutePosition;
            List<List<SpriteRenderer>> lettersHere = new List<List<SpriteRenderer>>(); //the actual letters (each index is a word, with the inner list there being a list of all the letters)
            List<float> widthsOfWordsHere = new List<float>(); //width of each world in world-space units
            Sprite letterToWorkWith = null;
            for (int i = 0; i < ting.Length; i++)
            {
                lettersHere.Add(new List<SpriteRenderer>());
                widthsOfWordsHere.Add(0);
                while (counter < ting[i].Length)
                {
                    if (counter > 0)
                    {
                        widthsOfWordsHere[widthsOfWordsHere.Count - 1] += (0.0625f * sizeOfText);
                    }
                    if (textDictionary.TryGetValue(ting[i][counter], out letterToWorkWith) == true)
                    {
                        //after checking to make sure the letter sprite is in our SpriteSheet of letters
                        //we set the spriteRenderer object to the correct sprite/size/position, etc.
                        SpriteRenderer leto = GetTextObjFromStack();
                        SpriteRenderer child = leto.transform.GetChild(0).GetComponent<SpriteRenderer>();
                        leto.sprite = letterToWorkWith;
                        leto.sortingOrder = layerOrder;
                        leto.sortingLayerName = layerName;
                        leto.transform.localScale = new Vector3(sizeOfText, sizeOfText, 1);
                        child.transform.localPosition = new Vector3(0.03125f, -0.03125f, 0);
                        child.sprite = letterToWorkWith;
                        child.sortingOrder = layerOrder - 1;
                        child.sortingLayerName = layerName;
                        leto.transform.parent = textBlockHere.transform;
                        leto.transform.localPosition = new Vector3(0, 0, 0);
                        lettersHere[lettersHere.Count - 1].Add(leto);
                        widthsOfWordsHere[widthsOfWordsHere.Count - 1] += (leto.bounds.size.x);
                    }
                    counter++;
                }
                counter = 0;
            }
            //START OF ALIGNMENT
            //this is where we take the AlignmentForText argument to orient the text to the left, center, or right
            float xTotal = 0;
            Vector3 posToUse = new Vector3(widthBoi * -0.5f, 0, 0);
            List<int> wordsInRows = new List<int>();
            //align the actual text blocks to the left as a default (makes it easy to move the text to middle or right later)
            for (int i = 0; i < widthsOfWordsHere.Count; i++)
            {
                //add the bounds of the current letter to the current position, then move on to the next letter/line if necessary
                xTotal += (widthsOfWordsHere[i] + lettersHere[i][0].bounds.extents.x);
                if (i > 0)
                {
                    posToUse.x += (0.25f * sizeOfText);
                    xTotal += (0.25f * sizeOfText);
                }
                if (xTotal > widthBoi)
                {
                    xTotal = widthsOfWordsHere[i];
                    if (i > 0)
                    {
                        posToUse.y -= sizeOfText;
                        posToUse.x = widthBoi * -0.5f;
                        wordsInRows.Add(i);
                    }
                }
                for (int u = 0; u < lettersHere[i].Count; u++)
                {
                    posToUse.x += (lettersHere[i][u].bounds.extents.x);
                    lettersHere[i][u].transform.localPosition = posToUse;
                    posToUse.x += (lettersHere[i][u].bounds.extents.x + (0.0625f * sizeOfText));
                }
            }
            if (alignment == AlignmentForText.Middle)
            {
                int startingWord = 0;
                int counterino = 0;
                if (wordsInRows.Count > 0)
                {
                    //if there are multiple rows, we iterate through each row and shift the words to the center
                    //based on the position difference between the first and last letter in the row
                    while (counterino < wordsInRows.Count)
                    {
                        Vector3 farRightPos = lettersHere[wordsInRows[counterino] - 1][lettersHere[wordsInRows[counterino] - 1].Count - 1].transform.localPosition;
                        Vector3 farLeftPos = lettersHere[startingWord][0].transform.localPosition;
                        Vector3 mover = new Vector3((Mathf.Abs((farRightPos.x - farLeftPos.x) * 0.5f) * -1) - farLeftPos.x, 0, 0);
                        for (int i = startingWord; i < wordsInRows[counterino]; i++)
                        {
                            for (int r = 0; r < lettersHere[i].Count; r++)
                            {
                                lettersHere[i][r].transform.Translate(mover);
                            }
                        }
                        startingWord = wordsInRows[counterino];
                        if (counterino == wordsInRows.Count - 1)
                        {
                            farRightPos = lettersHere[lettersHere.Count - 1][lettersHere[lettersHere.Count - 1].Count - 1].transform.localPosition;
                            farLeftPos = lettersHere[startingWord][0].transform.localPosition;
                            mover = new Vector3((Mathf.Abs((farRightPos.x - farLeftPos.x) * 0.5f) * -1) - farLeftPos.x, 0, 0);
                            for (int y = startingWord; y < lettersHere.Count; y++)
                            {
                                for (int r = 0; r < lettersHere[y].Count; r++)
                                {
                                    lettersHere[y][r].transform.Translate(mover);
                                }
                            }
                        }
                        counterino++;
                    }
                }
                else
                {
                    //otherwise, we can just do the same thing but for the singular row that we have
                    Vector3 farRightPos = lettersHere[lettersHere.Count - 1][lettersHere[lettersHere.Count - 1].Count - 1].transform.localPosition;
                    Vector3 farLeftPos = lettersHere[startingWord][0].transform.localPosition;
                    Vector3 mover = new Vector3((Mathf.Abs((farRightPos.x - farLeftPos.x) * 0.5f) * -1) - farLeftPos.x, 0, 0);
                    for (int y = startingWord; y < lettersHere.Count; y++)
                    {
                        for (int r = 0; r < lettersHere[y].Count; r++)
                        {
                            lettersHere[y][r].transform.Translate(mover);
                        }
                    }
                }
            }
            else if (alignment == AlignmentForText.Right)
            {
                int startingWord = 0;
                int counterino = 0;
                if (wordsInRows.Count > 0)
                {
                    //for aligning to the right, it is similar to aligning to the middle, but we instead push everything over
                    //in order for the farthest right object to be on the far right edge
                    while (counterino < wordsInRows.Count)
                    {
                        SpriteRenderer tingBoi = lettersHere[wordsInRows[counterino] - 1][lettersHere[wordsInRows[counterino] - 1].Count - 1];
                        Vector3 farRightPos = tingBoi.transform.localPosition;
                        Vector3 mover = new Vector3(((widthBoi * 0.5f) - tingBoi.bounds.extents.x) - farRightPos.x, 0, 0);
                        for (int i = startingWord; i < wordsInRows[counterino]; i++)
                        {
                            for (int r = 0; r < lettersHere[i].Count; r++)
                            {
                                lettersHere[i][r].transform.Translate(mover);
                            }
                        }
                        startingWord = wordsInRows[counterino];
                        if (counterino == wordsInRows.Count - 1)
                        {
                            tingBoi = lettersHere[lettersHere.Count - 1][lettersHere[lettersHere.Count - 1].Count - 1];
                            farRightPos = tingBoi.transform.localPosition;
                            mover = new Vector3(((widthBoi * 0.5f) - tingBoi.bounds.extents.x) - farRightPos.x, 0, 0);
                            for (int y = startingWord; y < lettersHere.Count; y++)
                            {
                                for (int r = 0; r < lettersHere[y].Count; r++)
                                {
                                    lettersHere[y][r].transform.Translate(mover);
                                }
                            }
                        }
                        counterino++;
                    }
                }
                else
                {
                    //same thing as above (when there is only one row we do the same, just without iterating through rows)
                    SpriteRenderer tingBoi = lettersHere[lettersHere.Count - 1][lettersHere[lettersHere.Count - 1].Count - 1];
                    Vector3 farRightPos = tingBoi.transform.localPosition;
                    Vector3 mover = new Vector3(((widthBoi * 0.5f) - tingBoi.bounds.extents.x) - farRightPos.x, 0, 0);
                    for (int y = startingWord; y < lettersHere.Count; y++)
                    {
                        for (int r = 0; r < lettersHere[y].Count; r++)
                        {
                            lettersHere[y][r].transform.Translate(mover);
                        }
                    }
                }
            }
            //end of alignment
            //start of coloring words/letters
            List<int> wordsToColor = new List<int>();
            if (extraColorToDisplay != null && extraColorToDisplay.Count > 0 && partOfStringToShowColor != null && partOfStringToShowColor.Count > 0)
            {
                int indexerino = -1;
                //this is just saying that the whole string is going to be this color
                if (partOfStringToShowColor.Contains("bruuuh") == true)
                {
                    indexerino = partOfStringToShowColor.IndexOf("bruuuh");
                    //this function returns the color corresponding to the string argument
                    //for the special colors bluey and goldy, it returns clear and black, respectively
                    //otherwise, it return a color variable with 4 floats from 0.0f to 1.0f, corresponding to r, g, b, and alpha
                    Color colorToSwitch = ReturnTextColor(extraColorToDisplay[indexerino]);
                    if (colorToSwitch != Color.white) //white is the default color for normal text
                    {
                        if (colorToSwitch == Color.clear) //bluey
                        {
                            wordsToColor.Add(-50000); //this is the stuff that is modulated in the larger script
                            wordsToColor.Add(0);
                            colorToSwitch = GetBlueGlowyColor(); //addition
                        }
                        else if (colorToSwitch == Color.black) //goldy
                        {
                            wordsToColor.Add(50000); //see above
                            wordsToColor.Add(0);
                            colorToSwitch = GetYellowGlowyColor(); //addition
                        }
                        //this part used to be inside an else right above here
                        counter = 0;
                        //this is where the colors actually get changed
                        for (int u = 0; u < lettersHere.Count; u++)
                        {
                            while (counter < lettersHere[u].Count)
                            {
                                lettersHere[u][counter].color = colorToSwitch;
                                if (colorToSwitch.a < 0.9f) //changing alpha of letter child if necessary
                                {
                                    lettersHere[u][counter].transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, colorToSwitch.a);
                                }
                                counter++;
                            }
                            counter = 0;
                        }
                        //end of former else part
                    }
                }
                else
                {
                    Dictionary<string, int> insiders = new Dictionary<string, int>();
                    int numerToUse = 1;
                    for (int i = 0; i < partOfStringToShowColor.Count; i++)
                    {
                        //this part checks to see if there are any small substrings that need to be colored
                        if (partOfStringToShowColor[i].Contains("{") && partOfStringToShowColor[i].Contains("}"))
                        {
                            int leftIndex = partOfStringToShowColor[i].IndexOf("{");
                            int rightIndex = partOfStringToShowColor[i].IndexOf("}");
                            numerToUse = int.Parse(partOfStringToShowColor[i].Substring(leftIndex + 1, rightIndex - (leftIndex + 1)));
                            partOfStringToShowColor[i] = partOfStringToShowColor[i].Substring(rightIndex + 1, partOfStringToShowColor[i].Length - (rightIndex - leftIndex + 1));
                            if (insiders.ContainsKey(partOfStringToShowColor[i]) == true)
                            {
                                insiders[partOfStringToShowColor[i]] = numerToUse;
                            }
                            else
                            {
                                insiders.Add(partOfStringToShowColor[i], numerToUse);
                            }
                        }
                        else
                        {
                            if (insiders.ContainsKey(partOfStringToShowColor[i]) == true)
                            {
                                insiders[partOfStringToShowColor[i]]++;
                                numerToUse = insiders[partOfStringToShowColor[i]];
                            }
                            else
                            {
                                insiders.Add(partOfStringToShowColor[i], numerToUse);
                            }
                        }
                        int t = 0;
                        int modulAdder = 0;
                        int numerator = 0;
                        while (t >= 0 && t + modulAdder < thingToDisplay.Length)
                        {
                            //now we loop through the word and change colors/add colors to bluey/goldy lists
                            t = (thingToDisplay.Substring(modulAdder, thingToDisplay.Length - modulAdder)).IndexOf(partOfStringToShowColor[i]);
                            if (t >= 0 && t + modulAdder < thingToDisplay.Length)
                            {
                                numerator++;
                                if (numerator == numerToUse)
                                {
                                    Color colorToSwitchTo = ReturnTextColor(extraColorToDisplay[i]);
                                    if (colorToSwitchTo == Color.clear)
                                    {
                                        wordsToColor.Add(((modulAdder + t) + 10) * -1);
                                        wordsToColor.Add(partOfStringToShowColor[i].Length);
                                        colorToSwitchTo = GetBlueGlowyColor(); //addition
                                    }
                                    else if (colorToSwitchTo == Color.black)
                                    {
                                        wordsToColor.Add(((modulAdder + t) + 10));
                                        wordsToColor.Add(partOfStringToShowColor[i].Length);
                                        colorToSwitchTo = GetYellowGlowyColor(); //addition
                                    }
                                    //this part used to be in an else right above here
                                    int smallerCounter = 0;
                                    int counteyBoi = 0;
                                    while (counteyBoi < lettersHere.Count && smallerCounter < (modulAdder + t + partOfStringToShowColor[i].Length))
                                    {
                                        for (int e = 0; e < lettersHere[counteyBoi].Count; e++)
                                        {
                                            if (smallerCounter >= modulAdder + t && smallerCounter < (modulAdder + t + partOfStringToShowColor[i].Length))
                                            {
                                                lettersHere[counteyBoi][e].color = colorToSwitchTo;
                                                if (colorToSwitchTo.a < 0.9f)
                                                {
                                                    lettersHere[counteyBoi][e].transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, colorToSwitchTo.a);
                                                }
                                            }
                                            smallerCounter++;
                                        }
                                        smallerCounter++;
                                        counteyBoi++;
                                    }
                                    //end of former else part
                                    t = -5;
                                }
                                modulAdder += (t + partOfStringToShowColor[i].Length);
                            }
                        }
                        numerToUse = 1;
                    }
                }
            }
            //start of making words wave or shake
            //these are 2 text effects in the game to get certain emotions across in dialogue
            //the specific letters/words that we input to shake/wave are stored in a larger class and used to modulate words positions
            //over time based on their starting positions and the effect we put on them
            //you can even combine effects in the same word (i.e. hello where the "he" is shaking and the "llo" is waving)
            
            //the setup and implementation is nearly identical to the colors section above, however
            //we also add the starting positions, relative to their parent block, of each word to a list
            //this way, we have a reference point for where they started/their resting place when we do any operations
            
            List<int> wordsToWaveOrAnger = new List<int>();
            if (trueForRSWaveFalseForAngry != null && trueForRSWaveFalseForAngry.Count > 0 && stringForRSWaveOrAngry != null && stringForRSWaveOrAngry.Count > 0)
            {
                int indexerino = -1;
                if (stringForRSWaveOrAngry.Contains("bruuuh") == true)
                {
                    indexerino = stringForRSWaveOrAngry.IndexOf("bruuuh");
                    if (trueForRSWaveFalseForAngry[indexerino] == true)
                    {
                        wordsToWaveOrAnger.Add(-50000);
                        wordsToWaveOrAnger.Add(0);
                    }
                    else
                    {
                        wordsToWaveOrAnger.Add(50000);
                        wordsToWaveOrAnger.Add(0);
                    }
                    for (int i = 0; i < lettersHere.Count; i++) //adding all the local positions to a dictionary for later
                    {
                        for (int u = 0; u < lettersHere[i].Count; u++)
                        {
                            dicto.Add(lettersHere[i][u], lettersHere[i][u].transform.localPosition);
                        }
                    }
                }
                else
                {
                    Dictionary<string, int> insiders = new Dictionary<string, int>();
                    int numerToUse = 1;
                    for (int i = 0; i < stringForRSWaveOrAngry.Count; i++)
                    {
                        if (stringForRSWaveOrAngry[i].Contains("{") && stringForRSWaveOrAngry[i].Contains("}"))
                        {
                            int leftIndex = stringForRSWaveOrAngry[i].IndexOf("{");
                            int rightIndex = stringForRSWaveOrAngry[i].IndexOf("}");
                            numerToUse = int.Parse(stringForRSWaveOrAngry[i].Substring(leftIndex + 1, rightIndex - (leftIndex + 1)));
                            stringForRSWaveOrAngry[i] = stringForRSWaveOrAngry[i].Substring(rightIndex + 1, stringForRSWaveOrAngry[i].Length - (rightIndex - leftIndex + 1));
                            if (insiders.ContainsKey(stringForRSWaveOrAngry[i]) == true)
                            {
                                insiders[stringForRSWaveOrAngry[i]] = numerToUse;
                            }
                            else
                            {
                                insiders.Add(stringForRSWaveOrAngry[i], numerToUse);
                            }
                        }
                        else
                        {
                            if (insiders.ContainsKey(stringForRSWaveOrAngry[i]) == true)
                            {
                                insiders[stringForRSWaveOrAngry[i]]++;
                                numerToUse = insiders[stringForRSWaveOrAngry[i]];
                            }
                            else
                            {
                                insiders.Add(stringForRSWaveOrAngry[i], numerToUse);
                            }
                        }
                        int t = 0;
                        int modulAdder = 0;
                        int numerator = 0;
                        while (t >= 0 && t + modulAdder < thingToDisplay.Length)
                        {
                            t = (thingToDisplay.Substring(modulAdder, thingToDisplay.Length - modulAdder)).IndexOf(stringForRSWaveOrAngry[i]);
                            if (t >= 0 && t + modulAdder < thingToDisplay.Length)
                            {
                                numerator++;
                                if (numerator == numerToUse)
                                {
                                    if (trueForRSWaveFalseForAngry[i] == true)
                                    {
                                        wordsToWaveOrAnger.Add(((modulAdder + t) + 10) * -1);
                                        wordsToWaveOrAnger.Add(stringForRSWaveOrAngry[i].Length);
                                    }
                                    else
                                    {
                                        wordsToWaveOrAnger.Add(((modulAdder + t) + 10));
                                        wordsToWaveOrAnger.Add(stringForRSWaveOrAngry[i].Length);
                                    }
                                    t = -5;
                                }
                                modulAdder += (t + stringForRSWaveOrAngry[i].Length);
                            }
                        }
                        numerToUse = 1;
                    }
                    if (wordsToWaveOrAnger.Count > 0) //adding positions to dictionary!
                    {
                        int bigTing = 0;
                        int littleTing = 0;
                        int actualIndex = 0;
                        bool done = false;
                        int realStart = 0;
                        for (int i = 0; i < wordsToWaveOrAnger.Count; i+=2)
                        {
                            realStart = Mathf.Abs(wordsToWaveOrAnger[i]) - 10;
                            while (bigTing < lettersHere.Count)
                            {
                                while (littleTing < lettersHere[bigTing].Count)
                                {
                                    if (actualIndex >= realStart)
                                    {
                                        if (actualIndex < realStart + wordsToWaveOrAnger[i + 1])
                                        {
                                            dicto.Add(lettersHere[bigTing][littleTing], lettersHere[bigTing][littleTing].transform.localPosition);
                                        }
                                        else
                                        {
                                            done = true;
                                            littleTing = lettersHere[bigTing].Count;
                                        }
                                    }
                                    actualIndex++;
                                    littleTing++;
                                }
                                if (done == true)
                                {
                                    bigTing = lettersHere.Count;
                                }
                                littleTing = 0;
                                actualIndex++;
                                bigTing++;
                            }
                            done = false;
                            actualIndex = 0;
                            bigTing = 0;
                            littleTing = 0;
                        }
                    }
                }
            }
            //here is where we add all the objects and data to lists/dictionaries in another script
            //that manages them/modulates them/destroys them when necessary
            actualStringsShowing.Add(thingToDisplay);
            lettersActive.Add(lettersHere);
            textBlocksActive.Add(textBlockHere);
            textBlocksChildrenAndTheirStartPositions.Add(dicto);
            //add the tag into a larger list up top
            indexesToChangeBlueyOrGoldey.Add(wordsToColor);
            indexesToDoWaveOrAngry.Add(wordsToWaveOrAnger);
            if (optionalTagForSectionsToClearLater != null)
            {
                tagForThisBlock.Add(optionalTagForSectionsToClearLater);
                //this is an edit for displaying dialogue text
                //if text has the dialogue tag, it automatically starts as totally clear
                //so that I can show each character individually on a timer to have type-writer type dialogue
                //that functionality is in another script, but having this edit allows me to have the dialogue orient itself
                //correctly and automatically become invisible when it is created
                if (optionalTagForSectionsToClearLater == GetTagForDialogueText()) //edit for dialogue setting to clear automatically
                {
                    for (int i = 0; i < lettersHere.Count; i++)
                    {
                        for (int y = 0; y < lettersHere[i].Count; y++)
                        {
                            if (lettersHere[i][y] != null)
                            {
                                lettersHere[i][y].color = new Color(lettersHere[i][y].color.r, lettersHere[i][y].color.g, lettersHere[i][y].color.b, 0);
                                SpriteRenderer childToEdit = lettersHere[i][y].transform.GetChild(0).GetComponent<SpriteRenderer>();
                                childToEdit.color = new Color(childToEdit.color.r, childToEdit.color.g, childToEdit.color.b, 0);
                            }
                        }
                    }
                } //end of dialogue edit
            }
            else
            {
                tagForThisBlock.Add(string.Empty);
            }
            if (optionalGroupForSectionsToClearLater != null)
            {
                groupForThisBlock.Add(optionalGroupForSectionsToClearLater);
            }
            else
            {
                groupForThisBlock.Add(string.Empty);
            }
        }
    }
    
    /// <summary>
    /// (1) Put colors in carats like this: <(bluey)>chocolate<>
    /// (2) If you put a number (starting at 1) inside the first bracket, it will look for that number of the word in the string (only works with single digit numbers rn) (has to be before the parentheses for colors/before the w or s for effects)
    /// (3) Put colors inside parentheses inside brackets (can combine with shake/wave), so <(bluey)wave>
    /// (4) Same deal as colors for wave and shake, just put 'wave' or 'shake'
    /// (5) List of colors to use: semiclear, red, blue, green, orange, pink, purple, silver, gray, yellow, brown, bluey, goldy (type them in correctly, or it wont work)
    /// </summary>
    public void ParseAndDisplayCertainText(string thingToParse, float textSiz, Vector3 absoPos, float widdy, AlignmentForText alignmenty, LayerNames layNameO, int layerOrdo, string tagerino, string grouperino)
    {
        //checking if string is valid
        if (thingToParse != null && thingToParse != string.Empty)
        {
            //creating the variables we are going to pass in to the DisplayTextFunction below
            string actualThingToShow = string.Empty;
            List<TextColors> coleys = null;
            List<string> coleyStrings = null;
            List<bool> waveys = null;
            List<string> waveyStrings = null;
            bool currentlyColoring = false;
            bool currentlyWaving = false;
            bool checkingInside = false;
            int numToUse = -1;
            for (int i = 0; i < thingToParse.Length; i++)
            {
                if (checkingInside == false && (currentlyColoring == true || currentlyWaving == true))
                {
                    if (thingToParse[i] == '<') //ending our search for color/waving information
                    {
                        currentlyColoring = false;
                        currentlyWaving = false;
                        i += 2;
                        if (i >= thingToParse.Length)
                        {
                            break;
                        }
                    }
                }
                if (checkingInside == false && currentlyColoring == true) //adding to our colors list
                {
                    coleyStrings[coleyStrings.Count - 1] += thingToParse[i];
                }
                if (checkingInside == false && currentlyWaving == true) //adding to our waving/shaking list
                {
                    waveyStrings[waveyStrings.Count - 1] += thingToParse[i];
                }
                if (checkingInside == false)
                {
                    if (thingToParse[i] == '<') //start our checking inside information
                    {
                        checkingInside = true;
                    }
                    else //adding characters to the string we will actually display on-screen (so that it doesn't show the shorthand in the final string)
                    {
                        actualThingToShow += thingToParse[i];
                    }
                }
                else
                {
                    if (thingToParse[i] == '>') //end of checking for information
                    {
                        checkingInside = false;
                        numToUse = -1;
                    }
                    else if (thingToParse[i] == '(')
                    {
                        //now we start parsing and looking for color information
                        int counter = i + 1;
                        string colorToUse = string.Empty;
                        while (counter < thingToParse.Length)
                        {
                            if (thingToParse[counter] == ')')
                            {
                                i += (counter - i);
                                counter = thingToParse.Length;
                            }
                            else
                            {
                                colorToUse += thingToParse[counter];
                            }
                            counter++;
                        }
                        if (coleys == null)
                        {
                            coleys = new List<TextColors>();
                            coleyStrings = new List<string>();
                        }
                        counter = 0;
                        while (counter <= (int)(TextColors.Bluey))
                        {
                            if ((((TextColors)(counter)).ToString()).ToLower() == colorToUse.ToLower())
                            {
                                coleys.Add((TextColors)(counter));
                                counter = (int)(TextColors.Bluey) + 10;
                            }
                            counter++;
                        }
                        if (coleys.Count > coleyStrings.Count)
                        {
                            currentlyColoring = true;
                            coleyStrings.Add(string.Empty);
                            if (numToUse > 0)
                            {
                                coleyStrings[coleyStrings.Count - 1] += "{" + numToUse.ToString() + "}";
                            }
                        }
                    }
                    else
                    {
                        //now we start parsing and looking for waving/shaking information
                        string parset = (thingToParse[i].ToString()).ToLower();
                        if (parset == "w" || parset == "s")
                        {
                            if (waveys == null)
                            {
                                waveys = new List<bool>();
                                waveyStrings = new List<string>();
                            }
                            if (parset == "w")
                            {
                                waveys.Add(true);
                                i += 3;
                            }
                            else if (parset == "s")
                            {
                                waveys.Add(false);
                                i += 4;
                            }
                            if (waveys.Count > waveyStrings.Count)
                            {
                                waveyStrings.Add(string.Empty);
                                currentlyWaving = true;
                                if (numToUse > 0)
                                {
                                    waveyStrings[waveyStrings.Count - 1] += "{" + numToUse.ToString() + "}";
                                }
                            }
                        }
                        else
                        {
                            //if the char is not 'w' or 's', that means it is an index of a string
                            //so we should parse that index and store that information
                            switch(thingToParse[i])
                            {
                                case '1':
                                    numToUse = 1;
                                    break;
                                case '2':
                                    numToUse = 2;
                                    break;
                                case '3':
                                    numToUse = 3;
                                    break;
                                case '4':
                                    numToUse = 4;
                                    break;
                                case '5':
                                    numToUse = 5;
                                    break;
                                case '6':
                                    numToUse = 6;
                                    break;
                                case '7':
                                    numToUse = 7;
                                    break;
                                case '8':
                                    numToUse = 8;
                                    break;
                                case '9':
                                    numToUse = 9;
                                    break;
                            }
                        }
                    }
                }
            }
            //now that we parsed the string and created the relevant information from the shorthand
            //we can now call the actual DisplayText function to do all the dirty work
            DisplayText(actualThingToShow, textSiz, absoPos, widdy, alignmenty, layNameO, layerOrdo, coleys, coleyStrings, waveys, waveyStrings, tagerino, grouperino);
        }
    }
    
}
