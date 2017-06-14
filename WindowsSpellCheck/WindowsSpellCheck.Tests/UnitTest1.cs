using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace WindowsSpellCheck.Tests
{
    [TestClass]
    public class UnitTestSpellChecker
    {
        static SpellChecker checker = new SpellChecker("en-US");

        [TestMethod]
        public void Base()
        {
            SpellChecker sc = new SpellChecker("en-US");
            List<SpellError> errors = sc.GetErrors("tst");
            List<string> suggestions = sc.GetSuggestions("thign");
        }

        [TestMethod]
        public void Spelling()
        {
            const string test = "this is a tst strnig";
            const int err1start = 10;
            const int err1length = 3;
            const int err2start = 14;
            const int err2length = 6;

            List<SpellError> errors = checker.GetErrors(test);

            Assert.IsTrue(errors.Any(x => x.Start == err1start && x.Length == err1length));
            Assert.IsTrue(errors.Any(x => x.Start == err2start && x.Length == err2length));
        }
        
        [TestMethod]
        public void Suggestion()
        {
            const string test1 = "thign";
            const string test1Answer = "thing";

            const string test2 = "tro";
            const string test2Answer1 = "two";
            const string test2Answer2 = "to";

            List<string> sugg1 = checker.GetSuggestions(test1);
            Assert.IsTrue(sugg1.Contains(test1Answer));

            List<string> sugg2 = checker.GetSuggestions(test2);
            Assert.IsTrue(sugg2.Contains(test2Answer1));
            Assert.IsTrue(sugg2.Contains(test2Answer2));

        }
    }
}
