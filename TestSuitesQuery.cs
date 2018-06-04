namespace MultiDialogsBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class TestSuitesQuery
    {
        [Prompt("Please select the {&}")]
        public string TestsuiteName { get; set; }

        //[Prompt("Would you like to execute the {&}?")]
        //public string ProjectSuite { get; set; }

        //[Numeric(1, int.MaxValue)]
        //[Prompt("How many {&} do you want to stay?")]
        //public int Nights { get; set; }
    }
}