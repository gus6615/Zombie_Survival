using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Percent : MonoBehaviour {

    static public bool GetRandFlag(float percent)
    {
        double data = Mathf.Round((float)((double)percent * 1000000f)) / 1000000f;

        if (data >= 1f)
            return true;
        else if (data <= 0f)
            return false;

        int count = 0;

        while(data != (int)data) // data가 소수인가?
        {
            data *= 10f;
            count++;
        }

        float x = (int)data;
        float y = Mathf.Pow(10, count);
        bool findCommons = true;

        while (findCommons)
        {
            List<int> commons = new List<int>();
            int i = 1;

            for (i = 1; i <= Mathf.Sqrt(x); i++)
            {
                if ((x / i == (int)(x / i))) // i가 x의 공약수일 경우
                {
                    commons.Add(i);
                    commons.Add((int)x / i);
                }
            }

            for (int j = 1; j < commons.Count; j++)
            {
                if (y / commons[j] == (int)(y / commons[j])) // i가 y의 공약수일 경우
                {
                    x /= commons[j];
                    y /= commons[j];
                    break;
                }

                if (j + 1 == commons.Count)
                    findCommons = false;
            }

            if (i == (int)Mathf.Sqrt(x) + 1 || x == 1)
                break;
        }

        int criticalX = (int)x;
        int criticalY = (int)y;

        int rand = Random.Range(1, criticalY + 1);
        if (1 <= rand && rand <= criticalX)
            return true;
        else
            return false;
    }
}
