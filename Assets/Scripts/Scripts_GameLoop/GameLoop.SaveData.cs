using System;

namespace GameLoop
{
    [Serializable]
    public class SaveData
    {

        public int NightIndex;

        public SaveData(int nightIndex)
        {
            NightIndex = nightIndex;
        }

    }
}