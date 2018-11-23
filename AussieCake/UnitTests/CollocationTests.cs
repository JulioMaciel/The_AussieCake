﻿using AussieCake.Challenge;
using AussieCake.Question;
using AussieCake.Sentence;
using AussieCake.Util;
using AussieCake.Verb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AussieCake.UnitTests
{
    [TestClass()]
    public class CollocationTests
    {
        [TestMethod()]
        public void CheckTimeSpentToValidSentenceOnSimpleCollocation()
        {
            var model = new ColModel("", "experience", 1, "", "difficulties", 0, "", "", "", 0, 1);
            var vm = model.ToVM();
            var sen = "Most Parties are likely to experience difficulties in implementing measures.";

            var added = new List<string>();

            var maxSecondsToSpend = 1;
            var watcher = new Stopwatch();
            watcher.Start();

            var result = Sentences.DoesSenContainsCol(vm, sen) && !added.Contains(sen);

            watcher.Stop();

            Assert.IsTrue(result);
            Assert.IsTrue(maxSecondsToSpend >= watcher.Elapsed.TotalSeconds);
        }

        [TestMethod()]
        public void CheckTimeSpentToValidSentenceOnFullCollocation()
        {
            var model = new ColModel("", "take", 1, "up the", "role", 0, "of;as", "", "", 0, 1);
            var vm = model.ToVM();
            var sen = "The commission may also take up the role of the City Council and rules governing land use.";

            var added = new List<string>();

            var maxSecondsToSpend = 1;
            var watcher = new Stopwatch();
            watcher.Start();

            var result = Sentences.DoesSenContainsCol(vm, sen) && !added.Contains(sen);

            watcher.Stop();

            Assert.IsTrue(result);
            Assert.IsTrue(maxSecondsToSpend >= watcher.Elapsed.TotalSeconds);
        }

        [TestMethod()]
        public async Task CheckTimeSpentToValidBulkSentences()
        {
            var watcher = new Stopwatch();
            watcher.Start();

            Debug.WriteLine("CheckTimeSpentToValidBulkSentences has started");

            string source = string.Empty;
            string[] filePaths = Directory.GetFiles(CakePaths.ResourceTxtBooks, "*.txt",
                                                    searchOption: SearchOption.TopDirectoryOnly);

            foreach (var path in filePaths)
                source += File.ReadAllText(path);

            Debug.WriteLine(watcher.Elapsed.TotalSeconds + " sec to get all sources.");
            watcher.Restart();
            //var maxSecondsToSpend = 20 * 60;

            var matchList = Regex.Matches(source, @"[A-Z]+(\w+\,*\;*[ ]{0,1}[\.\?\!]*)+");

            var sentences = matchList.Cast<Match>().Select(match => match.Value).ToList();

            sentences = sentences.Where(s => !Errors.IsNullSmallerOrBigger(s, SenVM_Deprecated.MinSize, SenVM_Deprecated.MaxSize, false) &&
                                              ((s.EndsWith(".") && !s.EndsWith("Dr.") && !s.EndsWith("Mr.") &&
                                              !s.EndsWith("Ms.")) || s.EndsWith("!") || s.EndsWith("?"))).ToList();

            Debug.WriteLine(watcher.Elapsed.TotalSeconds + " sec to filter sources and suck " + sentences.Count +
                                                                                                  " sentences.");
            watcher.Restart();
            var watcherQuest = new Stopwatch();
            watcher.Start();

            var found = new List<string>();

            Task tasks = Task.Run(() => Parallel.ForEach(QuestControl.Get(Model.Col).Take(50), col =>
            {
                Debug.WriteLine("Col started.");
                foreach (var sen in sentences)
                {
                    if (Sentences.DoesSenContainsCol((ColVM)col, sen) && !found.Contains(sen))
                        found.Add(sen);
                }
                Debug.WriteLine(watcher.Elapsed.TotalSeconds + " sec to finish col id " + col.Id);
            }));
            await Task.WhenAll(tasks);

            watcher.Stop();
            Debug.WriteLine(watcher.Elapsed.TotalSeconds + " sec to finish everything. " + found.Count + " sentences found.");

            foreach (var sen in found)
                Debug.WriteLine(sen);

            //Assert.IsTrue(watcher.Elapsed.TotalSeconds > 10);
            //Assert.IsTrue(maxSecondsToSpend >= watcher.Elapsed.TotalSeconds);
        }

        [TestMethod()]
        public void TestCheckCollocationInSentenceMethodIntegraty()
        {
            var model = new ColModel("", "take", 1, "up the", "role", 0, "of;as", "", "", 0, 1);
            var vm = model.ToVM();
            var sen = "The commission may also take up the role of the City Council and rules governing land use.";
            Assert.IsTrue(Sentences.DoesSenContainsCol(vm, sen));

            model = new ColModel("", "experience", 1, "", "difficulties", 0, "", "", "", 0, 1);
            vm = model.ToVM();
            sen = "Most Parties are likely to experience difficulties in implementing measures.";
            Assert.IsTrue(Sentences.DoesSenContainsCol(vm, sen));

            model = new ColModel("", "take", 1, "up the", "role", 0, "of;as", "", "", 0, 1);
            vm = model.ToVM();
            sen = "The commission may also should have taken up the role of the City Council and rules governing land use.";
            Assert.IsTrue(Sentences.DoesSenContainsCol(vm, sen));

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "The fat dog together with the stupid cat made a adorable malicious party.";
            Assert.IsTrue(Sentences.DoesSenContainsCol(vm, sen));

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with; and a", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "Nowadays it is amazing to get a cute dog and a cat at a cute, but malicious home.";
            Assert.IsTrue(Sentences.DoesSenContainsCol(vm, sen));

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "An adorable cow together with a dog and the cat are the most malicious pets.";
            Assert.IsTrue(Sentences.DoesSenContainsCol(vm, sen));

            model = new ColModel("", "experience", 1, "", "difficulties", 0, "", "", "", 0, 1);
            vm = model.ToVM();
            sen = "Most difficulty Parties experienced difficulties in their experiences.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because there're two Component1

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "Adorable dogs together with cats that are fat can be the moment to your realise how cute dogs are.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because there're two Component1

            model = new ColModel("", "experience", 1, "", "difficulties", 0, "", "", "", 0, 1);
            vm = model.ToVM();
            sen = "Most Parties have difficulties to experienced difficulty in their experiences.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because the Component2 requires plural. 
            // Algorithm just change form when it is singular to plural, not the opposite.

            model = new ColModel("", "take", 1, "up the", "role", 0, "of;as", "", "", 0, 1);
            vm = model.ToVM();
            sen = "The commission may also role up the take of the City Council and rules governing land use.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because Componenet2 comes before Component1

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "An adorable cat together with dogs that are fat can be the moment to your realise how cute they are.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because Component2 comes before Component1

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with; and a", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "Nowadays it is amazing to get a cute dog and a stupid cat at a cute home.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because Suffixe comes before Component2

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "A cow together with a dog can be the best pet ever, much better than a malicious cat.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because Link comes before Component1

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "An adorable cow together with a dog can be the best pets ever, and the cat the most stupid one.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because Link comes before Component1

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "The very smart and fat animal that was named as dog, together with the stupid cat made a adorable malicious party.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because Prefix is too far away from Component1

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with;that", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "That cute dog, because of its long corpse, is causing trouble together with the cat again, so that, I need to kill it.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because Component1 is too far away from Component2

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "The fat dog together with the stupid cat made a adorable dance which seemed like a malicious party.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because Suffix is too far away from Component2

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "Adorable dogs together with wolfs are the perfect picture of the example that cats are stupid.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because Link is too far away from Component2

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "The fat dog was stupid and with the stupid cat made a adorable malicious party.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because there's no Link

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "The dog and the rat are the perfect example why you should never have a tremendous stupid cat which is also fat.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because there's no Link

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "Who would like to have a fat dog these days when you can have a fat cat instead?";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because there's no Prefix

            model = new ColModel("cute;adorable;the", "dog", 0, "and the;together with", "cat", 0, "fat;stupid;malicious", "", "", 0, 1);
            vm = model.ToVM();
            sen = "The fat dog together with the stupid cat made a adorable party.";
            Assert.IsFalse(Sentences.DoesSenContainsCol(vm, sen));
            // Fails because there's no Suffix
        }

        [TestMethod()]
        public void TestIsVerbIntegraty()
        {
            foreach (ColVM col in QuestControl.Get(Model.Col))
            {
                if (col.IsComp1Verb && !col.Component1.IsVerb())
                    Debug.WriteLine(col.Component1 + " is set as Verb, but the method said no");

                if (!col.IsComp1Verb && col.Component1.IsVerb())
                    Debug.WriteLine(col.Component1 + " is NOT set as Verb, but the method said yes");

                if (col.IsComp2Verb && !col.Component2.IsVerb())
                    Debug.WriteLine(col.Component2 + " is set as Verb, but the method said no");

                if (!col.IsComp2Verb && col.Component2.IsVerb())
                    Debug.WriteLine(col.Component2 + " is NOT set as Verb, but the method said yes");
            }
        }
    }
}
