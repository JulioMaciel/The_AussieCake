using AussieCake.Models;
using AussieCake.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AussieCake.Controllers
{
  public static class SentenceController
  {
    public static void Insert(SentenceVM sentence)
    {
      if (DBController.Sentences.Any(s => s.Text == sentence.Text))
        return;

      var model = new Sentence(sentence);
      SqLiteHelper.InsertSentence(model);
      DBController.LoadSentencesViewModel();
    }

    public static void Update(SentenceVM sentence)
    {
      var model = new Sentence(sentence);
      SqLiteHelper.UpdateSentence(model);
      var oldVM = DBController.Sentences.FirstOrDefault(x => x.Id == sentence.Id);
      oldVM = sentence;
    }

    public static void Remove(SentenceVM sentence)
    {
      var model = new Sentence(sentence);
      SqLiteHelper.RemoveSentence(model);
      DBController.Sentences.Remove(sentence);
    }
  }
}
