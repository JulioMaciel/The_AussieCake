using AussieCake.Util;
using System.Collections.Generic;
using System.Linq;

namespace AussieCake.Question
{
    public class SpellVM : QuestVM
    {
        public SpellVM(int id, string text, Importance importance, bool isActive)
            : base(id, text, importance, isActive, Model.Spell)
        {
        }

        public SpellVM(string text, Importance importance, bool isActive)
            : base(text, importance, isActive, Model.Spell)
        {
        }

        public SpellModel ToModel()
        {
            var isActive_raw = IsActive.ToInt();

            if (IsReal)
                return new SpellModel(Id, Text, (int)Importance, isActive_raw);
            else
                return new SpellModel(Text, (int)Importance, isActive_raw);
        }

        public override void LoadCrossData()
        {
            base.LoadCrossData();
        }
    }
}
