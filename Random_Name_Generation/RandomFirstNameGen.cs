using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFirstNameGen : MonoBehaviour {
    
    Dictionary<char, float[]> characterProbsDictionary = new Dictionary<char, float[]>();
    
    //THIS SCRIPT IS PART OF A LARGER RANDOM MISSION GENERATING SCRIPT
    //THAT SCRIPT IS WELL OVER 3K LINES, SO I PULLED ONLY THE PERTINENT RANDOM NAME GENERATION CODE
    //HOWEVER, THIS MEANS THAT SOME VARIABLES/FUNCTIONS THAT ARE REFERENCED HERE AREN'T PRESENT IN THIS SCRIPT
    
    //what we do here is initialize the character probability dictionary based on a given list of names
    //the goal is to make a list of probabilities for each letter in the alphabet coming after a given letter
    //then use a random number generator to create random names that generally follow the conventions of the input names
    public void InitializeCharProbsDictionary()
    {
        string alphabetString = "abcdefghijklmnopqrstuvwxyz";
        nameListForMapping = Resources.Load("MainMapStuff/NameExamples") as TextAsset;
        Dictionary<char, float[]> tempHoldingDict = new Dictionary<char, float[]>();
        for (int i = 0; i < alphabetString.Length; i++)
        {
            characterProbsDictionary.Add(alphabetString[i], new float[26]);
            tempHoldingDict.Add(alphabetString[i], new float[26]);
        }
        string assetToParse = nameListForMapping.ToString();
        string[] namesToAnalyzeOG = Regex.Split(assetToParse, "\n");
        List<string> namesToAnalyze = new List<string>();
        for (int i = 0; i < namesToAnalyzeOG.Length; i++)
        {
            if (namesToAnalyze.Contains(namesToAnalyzeOG[i].Trim()) == false)
            {
                namesToAnalyze.Add(namesToAnalyzeOG[i].Trim());
            }
        }
        //now that we have a clean list of strings to work with, we can start parsing characters
        for (int u = 0; u < namesToAnalyze.Count; u++)
        {
            string stringToWorkWith = namesToAnalyze[u].ToLower().Trim();
            for (int i = 0; i < stringToWorkWith.Length - 1; i++)
            {
                char charGettingAddedTo = stringToWorkWith[i];
                int indexToAddTo = 0;
                for (int o = 0; o < alphabetString.Length; o++)
                {
                    if (stringToWorkWith[i + 1] == alphabetString[o])
                    {
                        indexToAddTo = o;
                        break;
                    }
                }
                tempHoldingDict[charGettingAddedTo][indexToAddTo]++;
            }
        }
        for (int i = 0; i < alphabetString.Length; i++)
        {
            int totalNum = 0;
            //what we do here is see how many total characters are in the array
            for (int u = 0; u < tempHoldingDict[alphabetString[i]].Length; u++)
            {
                totalNum += Mathf.RoundToInt(tempHoldingDict[alphabetString[i]][u]);
            }
            //then we use that to create percentage points
            //showing the probability that a given letter comes after the letter we are currently on
            for (int u = 0; u < tempHoldingDict[alphabetString[i]].Length; u++)
            {
                characterProbsDictionary[alphabetString[i]][u] = tempHoldingDict[alphabetString[i]][u] / totalNum;
            }
        }
        Resources.UnloadAsset(nameListForMapping);
        nameListForMapping = null;
    }
    
    //initialize this function with a random letter (usually just alphabetString[Random.Range(0, 26)])
    //however you can also just pass in an empty string, the function can add the first letter for you
    //then call it however many times, depending on how long you want the generated name to be
    //the name length is also usually just a random number in a given range (i.e. length = Random.Range(3, 12))
    public string GetAlphabetLetter(string givenWordAlready)
    {
        string alphabetString = "abcdefghijklmnopqrstuvwxyz";
        string stringToReturn = givenWordAlready;
        string stringToAnalyze = stringToReturn.ToLower();
        if (stringToReturn.Length < 1)
        {
            //adding a random first character to the string if necessary
            //my subset of names has every letter as a possible first letter, but if yours does not
            //this can easily be modified to exclude certain letters as the beginning of a name
            stringToReturn += alphabetString[Random.Range(0, 26)];
            stringToReturn = stringToReturn.ToUpper();
            return stringToReturn;
        }
        else
        {
            //now we create a random float between 0 and 1 to use to decide the next letter
            float randomNum = Random.Range(0, 1.0f);
            float numNow = 0;
            float numAfter = 0;
            //what we do is that we loop through the probability dictionary and add the current float to a variable
            //if that variable is in between the probability we currently have 
            //and the probability of the next letter occuring, or we are at the last index, we add that char to our string
            for (int i = 0; i < characterProbsDictionary[stringToAnalyze[stringToAnalyze.Length - 1]].Length; i++)
            {
                if (characterProbsDictionary[stringToAnalyze[stringToAnalyze.Length - 1]][i] > 0)
                {
                    numNow = numAfter;
                    numAfter += characterProbsDictionary[stringToAnalyze[stringToAnalyze.Length - 1]][i];
                    if ((randomNum > numNow && randomNum < numAfter) || i == characterProbsDictionary[stringToAnalyze[stringToAnalyze.Length - 1]].Length)
                    {
                        stringToReturn += alphabetString[i].ToString();
                        break;
                    }
                }
            }
            return stringToReturn;
        }
    }
    
}
