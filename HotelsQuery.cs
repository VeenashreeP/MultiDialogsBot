namespace MultiDialogsBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class HotelsQuery
    {
        [Prompt("Please select the test suite BVT,P1,Regression {&}")]
        public string TestsuiteName { get; set; }

        [Prompt("Would you like to {&}?Say Yes or No")]
        public string Execute { get; set; }

        //[Numeric(1, int.MaxValue)]
        //[Prompt("How many {&} do you want to stay?")]
        //public int Nights { get; set; }
    }
}