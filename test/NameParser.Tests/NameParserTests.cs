using System;
using Xunit;

namespace NameParser.Tests
{
    public class NameParserTests
    {
        [Fact]
        public void Can_parse_basic_fn_ln_name()
        {
            string name = "Ali Bayat";

            var expectedResult = new Name() { FirstName = "Ali", LastName = "Bayat" };

            var result = name.Parse();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Can_parse_name_with_all_parts()
        {
            string name = "Mr Ali R Von Bayat III";

            var expectedResult = new Name()
            {
                Salutation = "Mr",
                FirstName = "Ali",
                MiddleInitials = "R",
                LastName = "Von Bayat",
                Suffix = "III"
            };

            var result = name.Parse();

            Assert.Equal(expectedResult, result);
        }
    }
}
