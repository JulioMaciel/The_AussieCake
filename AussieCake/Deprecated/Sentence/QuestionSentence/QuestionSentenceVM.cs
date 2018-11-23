namespace AussieCake.Sentence
{
    public class QuestSenVM_Deprecated
    {
        public int Id { get; private set; }
        public int IdQuest { get; private set; }
        public int IdSen { get; private set; }

        public Model Type { get; private set; }

        public QuestSenVM_Deprecated(int id, int idQuest, int idSen, Model type)
            : this(idQuest, idSen, type)
        {
            Id = id;
        }

        public QuestSenVM_Deprecated(int idQuest, int idSen, Model type)
        {
            IdQuest = idQuest;
            IdSen = idSen;
            Type = Type;
        }

        public QuestSenVM_Deprecated()
        {
        }
    }
}
