﻿using AussieCake.Question;
using AussieCake.Util;
using AussieCake.Verb;
using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AussieCake.Sentence
{
    public static class AutoGetSentences_Deprecated
    {
        public async static Task<List<string>> GetSentencesFromString(string source)
        {
            Footer.Log("The auto insert sentences has just started. It may take a time to finish.");
            var watcher = new Stopwatch();
            watcher.Start();

            var sentences = new List<string>();
            //QuestControl.LoadCrossData(Model.Voc);

            //Task tasks = Task.Run(() => sentences = GetSentencesFromSource(source));
            //await Task.WhenAll(tasks);

            var filteredSentences = new List<string>();
            var task = sentences.Select(sen => Task.Factory.StartNew(() =>
            {
                if (!Errors.IsNullSmallerOrBigger(sen, SenVM_Deprecated.MinSize, SenVM_Deprecated.MaxSize, false))
                {
                    //if (DoesStartEndProperly(sen))
                    //    filteredSentences.Add(sen);
                }
            }));
            await Task.WhenAll(task);

            var senFromVocs = await GetVocSentenceFromList(filteredSentences);

            var found = new List<string>();
            found.AddRange(senFromVocs);

            return found;
        }

        #region Vocabulary

        private async static Task<List<string>> GetVocSentenceFromList(List<string> sentences)
        {
            var result = new List<string>();

            var watcher = new Stopwatch();
            watcher.Start();

            int actual = 1;
            var tasks = QuestControl.Get(Model.Voc).Select(Voc =>
                Task.Factory.StartNew(() =>
                {
                    foreach (var sen in sentences)
                    {
                        //if (DoesSenContainsVoc((VocVM)Voc, sen) && !result.Contains(sen))
                        //    result.Add(sen);
                    }

                    var log = "Analysing " + sentences.Count + " sentences for Vocabulary " +
                                 actual + " of " + QuestControl.Get(Model.Voc).Count() + ". ";
                    log += result.Count + " suitable sentences found in " + Math.Round(watcher.Elapsed.TotalMinutes, 2) + " minutes. ";

                    if (actual > 10)
                    {
                        var quant_missing = QuestControl.Get(Model.Voc).Count() - actual;
                        var time_to_finish = (watcher.Elapsed.TotalMinutes * quant_missing) / actual;
                        log += "It must finish in " + Math.Round(time_to_finish, 2) + " minutes.";
                    }

                    Footer.Log(log);
                    actual = actual + 1;
                }));
            await Task.WhenAll(tasks);

            return result;
        }

        #endregion
    }
}
