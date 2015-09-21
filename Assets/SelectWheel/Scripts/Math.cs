

namespace Rodger
{
    public class Math
    {
        // Range : [0,max]
        static public int Cyclic_Summation(int max, int focus, int delta)
        {
            int sum = focus + delta;

            if (sum > max)
                sum = sum - (max + 1);
            else if (sum < 0)
                sum = sum + 1 + max;

            return sum;
        }
        static public float Cyclic_Summation(float max, float focus, float delta)
        {
            float sum = focus + delta;

            if (sum > max)
                sum = sum - (max);
            else if (sum < 0)
                sum = sum + max;

            return sum;
        }
    }
}