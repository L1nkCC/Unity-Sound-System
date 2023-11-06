using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace CC.Core.InputValidation
{
    public static class InputValidationExtensionMethods
    {
        /// <summary>
        /// List of validators
        /// </summary>
        private static readonly (System.Func<string, bool>, System.Exception)[] validators = new (System.Func<string, bool>, System.Exception)[]
        {
            ((string input) => !string.IsNullOrWhiteSpace(input), new InputValidationException("Inputs cannot be white-space or null!")),
            ((string input) => char.IsLetter(input[0]), new InputValidationException("Inputs must begin with a letter!")),
            ((string input) => input.All(char.IsLetterOrDigit), new InputValidationException("Inputs must contain only alphanumeric characters!")),
        };

        /// <summary>
        /// Highly restrictive validator on strings to assure protection. Throws error if invalid input
        /// </summary>
        /// <param name="input">string to validate</param>
        public static void ValidateInput(this string input)
        {
            //Test inputs against Validators
            foreach ((System.Func<string, bool> validator, System.Exception exception) in validators)
            {
                if (!validator(input)) throw exception;
            }
        }

        /// <summary>
        /// Highly restrictive validator on strings to assure protection. Throws error if invalid input
        /// </summary>
        /// <param name="input">string to validate</param>
        public static void ValidateInput(this ICollection<string> inputs)
        {
            foreach (string input in inputs)
                ValidateInput(input);
        }

    }
    [System.Serializable]
    public class InputValidationException : System.Exception
    {
        public InputValidationException() { }
        public InputValidationException(string message) : base(message) { }
        public InputValidationException(string message, System.Exception inner) : base(message, inner) { }
        protected InputValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
