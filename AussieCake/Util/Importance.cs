using System.ComponentModel;

namespace AussieCake.Util
{
    public enum Importance
    {
        [Description("PTE IELTS")]
        PTE_IELTS_Official = 0,

        [Description("Reliable")]
        Reliable_resource = 1,

        [Description("Relevant")]
        My_own_relevant = 2,

        [Description("Random")]
        Random_resource = 3,

        [Description("Just to be")]
        Just_to_be = 4, // apenas para constar

        [Description("Any")]
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
