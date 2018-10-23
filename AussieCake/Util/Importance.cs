using AussieCake.Question;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AussieCake.Util
{
    public enum Importance
    {
        PTE_IELTS_Official = 0,
        Reliable_resource = 1,
        My_own_relevant = 2,
        Random_resource = 3,
        Just_to_be = 4, // apenas para constar
        Any = 5, // filter
    }

    public static class ScoreHelper
    {
        public static int GetScoreFromImportance(Importance imp)
        {
            switch (imp)
            {
                case Importance.PTE_IELTS_Official:
                    return 10;
                case Importance.Reliable_resource:
                    return 7;
                case Importance.My_own_relevant:
                    return 5;
                case Importance.Random_resource:
                    return 3;
                case Importance.Just_to_be:
                    return 1;
            }
            return 0;
        }
    }
}
