using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class saveScript  {
   

    public static void SaveStringArray(string array_name,int array_size,string[] array)  
    {
        PlayerPrefs.SetInt(array_name+"size", array_size);

        for (int i = 0; i < array_size; i++)
        {
            PlayerPrefs.SetString(array_name+i, array[i]);

        }

    }

    public static string[] GetStringArray(string array_name)
    {
        int i = PlayerPrefs.GetInt(array_name+"size");

        string[] retunrn_array=new string[i];

        for(int j = 0; j < i; j++)
        {
            retunrn_array[j] = PlayerPrefs.GetString(array_name+j);
        }

        return retunrn_array;
    }
    
	
}
