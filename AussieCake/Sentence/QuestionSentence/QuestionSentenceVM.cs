using AussieCake.Question;
using System.Linq;

namespace AussieCake.Sentence
{
    public class QuestSenVM
    {
        public int Id { get; private set; }
        public int IdQuest { get; private set; }
        //public IQuestion Quest { get; private set; }
        public int IdSen { get; private set; }
        //public SenVM Sen { get; private set; }
        public bool IsActive { get; set; }

        public Model Type { get; private set; }

        public QuestSenVM(int id, int idQuest, int idSen, bool isActive, Model type)
            : this(idQuest, idSen, isActive, type)
        {
            Id = id;
        }

        public QuestSenVM(int idQuest, int idSen, bool isActive, Model type)
        {
            IdQuest = idQuest;
            IdSen = idSen;
            IsActive = isActive;
            Type = Type;
        }

        public QuestSenVM()
        {
        }

        //public void LoadCrossData()
        //{
        //    Sen = SenControl.Get().First(x => x.Id == IdSen);
        //    Quest = QuestControl.Get(Type).First(x => x.Id == IdQuest);
        //}

        //public QuestSenVM ToModel()
        //{
        //    return new QuestSenVM(Id, IdQuest, Sen.Id, IsActive, Type);
        //}
    }
}
