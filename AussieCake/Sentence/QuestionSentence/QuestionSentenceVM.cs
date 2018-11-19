namespace AussieCake.Sentence
{
    public class QuestSenVM
    {
        public int Id { get; private set; }
        public int IdQuest { get; private set; }
        public int IdSen { get; private set; }

        public Model Type { get; private set; }

        public QuestSenVM(int id, int idQuest, int idSen, Model type)
            : this(idQuest, idSen, type)
        {
            Id = id;
        }

        public QuestSenVM(int idQuest, int idSen, Model type)
        {
            IdQuest = idQuest;
            IdSen = idSen;
            Type = Type;
        }

        public QuestSenVM()
        {
        }
    }
}
