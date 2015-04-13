using System.Collections.Generic;

namespace Jump
{
    public static class ExtensionMethods
    {
        #region List<int> Extensions

        public static void BubbleSort(this List<int> list)
        {
            bool hasSwapped;

            do
            {
                hasSwapped = false;
                for (int i = 0; i < list.Count - 1; i++)
                {
                    if (list[i] > list[i + 1])
                    {
                        int temp = list[i + 1];
                        list[i + 1] = list[i];
                        list[i] = temp;

                        hasSwapped = true;
                    }
                }

            } while (hasSwapped);

        }

        public static void SortDescending(this List<int> list)
        {
            list.BubbleSort();
            list.Reverse();
        }

        #endregion
    }
}
