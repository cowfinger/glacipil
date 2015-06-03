using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinDataCrawler
{
    class ArgumentsParser
    {
        public enum ArgumentToken
        {
            date_begin,
            date_end,
            directory,
            symbol,
            symbol_file,
        }

        public bool Success
        {
            get;
            private set;
        }

        public IEnumerable<string> Arguments
        {
            get;
            private set;
        }

        public IDictionary<ArgumentToken, string> ArgumentsTable
        {
            get;
            private set;
        }

        public ArgumentsParser(IEnumerable<string> args)
        {
            this.Arguments = args;
            this.Success = this.Parse();
        }

        public bool TryGetArgument<T>(ArgumentToken token, out T value)
        {
            try
            {
                var rawValue = this.ArgumentsTable[token];
                value = (T)Convert.ChangeType(rawValue, typeof(T));
                return true;
            }
            catch (Exception)
            {
                value = (T)new object();
                return false;
            }
        }

        private bool Parse()
        {
            int status = 0;
            ArgumentToken token = ArgumentToken.symbol;
            foreach (var argument in this.Arguments)
            {
                switch(status)
                {
                    case 0:
                    if (argument.StartsWith("-") && Enum.TryParse(argument.Substring(1), out token))
                    {
                        status = 1;
                    }
                    else
                    {
                        return false;
                    }
                    break;

                    case 1:
                    this.ArgumentsTable.Add(token, argument);
                    break;
                }
            }
            return true;
        }
    }
}
